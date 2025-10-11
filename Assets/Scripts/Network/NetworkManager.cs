using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetworkManager
{
    private static NetworkManager _instance;
    public static NetworkManager Instance => _instance ??= new NetworkManager();

    // 내부 네트워크 필드
    private TcpClient client;
    private NetworkStream stream;
    private Thread recvThread;

    // 로그 이벤트 (UIManager나 Console 연결용)
    public event Action<string> OnLog;

    // 외부 생성 금지
    private NetworkManager()
    {
        Log("클라이언트 매니저 생성 완료");
    }

    // 서버 연결 (자동 탐색)
    public void Connect()
    {
        var serverInfo = Udp.FindServer();
        if (serverInfo == null)
        {
            Log("같은 네트워크에서 서버를 찾을 수 없습니다.");
            return ;
        }

        string ip = serverInfo.Value.ip;
        int port = serverInfo.Value.port;

        try
        {
            client = new TcpClient();
            client.Connect(ip, port);
            stream = client.GetStream();

            Log($"서버 연결 성공 ({ip}:{port})");

            SendPacket((int)Define.Protocol.Login, pw => pw.WriteString("지혁"));
            recvThread = new Thread(ReceiveLoop);
            recvThread.Start();
        }
        catch (Exception ex)
        {
            Log($"서버 연결 실패: {ex.Message}");
            return ;
        }

        return;
    }

    // 방 생성
    public void CreateRoom()
    {
        SendPacket((int)Define.Protocol.CreateRoom);
        Log("방 생성 요청 전송");
    }

    // 방 입장
    public void JoinRoom(int roomId = 1)
    {
        SendPacket((int)Define.Protocol.JoinRoom, pw => pw.WriteInt(roomId));
        Log($"방 입장 요청 (RoomId={roomId})");
    }

    // 패킷 전송
    private void SendPacket(int protocol, Action<PacketWriter> body = null)
    {
        if (stream == null)
        {
            Log("서버에 연결되어 있지 않습니다.");
            return;
        }

        using (var pw = new PacketWriter())
        {
            pw.WriteInt(protocol);
            body?.Invoke(pw);
            byte[] data = pw.ToArrayWithLengthPrefix();
            stream.Write(data, 0, data.Length);
        }
    }

    // 수신 루프 (스레드)
    private void ReceiveLoop()
    {
        try
        {
            byte[] buffer = new byte[4096];
            while (true)
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read == 0) break;

                byte[] data = new byte[read];
                Array.Copy(buffer, data, read);

                // Unity 메인스레드로 이벤트 전달
                UnityMainThreadDispatcher.Instance().Enqueue(() => HandlePacket(data));
            }
        }
        catch (Exception ex)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Log($"서버 연결 종료: {ex.Message}");
            });
        }
    }

    // 패킷 처리
    private void HandlePacket(byte[] data)
    {
        using (var pr = new PacketReader(data))
        {
            int protocol = pr.ReadInt();

            switch ((Define.Protocol)protocol)
            {
                case Define.Protocol.CreateRoom:
                    int roomId = pr.ReadInt();
                    Log($"[서버] 방 생성 완료 (RoomId={roomId})");
                    break;

                case Define.Protocol.GetRoomList:
                    int count = pr.ReadInt();
                    Log($"[서버] 방 목록 {count}개 수신");
                    for (int i = 0; i < count; i++)
                    {
                        int id = pr.ReadInt();
                        string host = pr.ReadString();
                        int cur = pr.ReadInt();
                        int max = pr.ReadInt();

                        RoomData roomData = new RoomData(id, host, cur, max);
                        Log(roomData.ToString());
                    }
                    break;

                case Define.Protocol.Event:
                    int roomIdEvent = pr.ReadInt();
                    int eventType = pr.ReadInt();
                    string param = pr.ReadString();
                    Log($"[이벤트] Room {roomIdEvent} → {(Define.ServerEvent)eventType} ({param})");
                    break;

                case Define.Protocol.RoomInfo:
                    int rId = pr.ReadInt();
                    string hostName = pr.ReadString();
                    int maxPlayers = pr.ReadInt();
                    int curPlayers = pr.ReadInt();
                    Log($"[Room {rId}] Host: {hostName} ({curPlayers}/{maxPlayers})");

                    for (int i = 0; i < curPlayers; i++)
                        Log($" - {pr.ReadString()}");
                    break;

                default:
                    Log($"알 수 없는 프로토콜 ({protocol}) 수신");
                    break;
            }
        }
    }

    // 로그 출력 (UI 연결용)
    private void Log(string msg)
    {
        Debug.Log(msg);
        OnLog?.Invoke(msg); // UIManager가 구독해서 표시 가능
    }

    // 종료 처리
    public void Close()
    {
        try
        {
            recvThread?.Abort();
            stream?.Close();
            client?.Close();
            Log("연결 종료됨");
        }
        catch (Exception e)
        {
            Log($"종료 중 오류: {e.Message}");
        }
    }
}
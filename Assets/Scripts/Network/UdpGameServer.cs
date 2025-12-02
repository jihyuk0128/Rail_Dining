using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UdpGameServer
{
    private UdpClient _udp;
    private IPEndPoint _serverEP;
    private bool _running = false;

    public void Start(string serverIp, int port)
    {
        try
        {
            _udp = new UdpClient();
            _udp.Client.ReceiveTimeout = 3000;

            _serverEP = new IPEndPoint(IPAddress.Parse(serverIp), port);
            _running = true;

            Debug.Log($"[UDP] 연결 시도 → {_serverEP}");
            SendTestPacket();

            // 비동기 수신 시작
            _udp.BeginReceive(OnReceive, null);
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDP] 시작 실패: {e.Message}");
        }
    }

    private void SendTestPacket()
    {
        string msg = "HELLO_UDP_SERVER";
        byte[] data = Encoding.UTF8.GetBytes(msg);
        _udp.Send(data, data.Length, _serverEP);
        Debug.Log("[UDP] 테스트 패킷 전송 완료");
    }

    private void OnReceive(IAsyncResult ar)
    {
        try
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = _udp.EndReceive(ar, ref remoteEP);

            // 먼저 문자열인지 체크
            string text = Encoding.UTF8.GetString(data);
            if (text == "WELCOME_UDP_CLIENT")
            {
                Debug.Log("[UDP] 서버 응답 수신: WELCOME_UDP_CLIENT");
                _udp.BeginReceive(OnReceive, null);
                return;
            }

            using (var reader = new PacketReader(data))
            {
                int pid = reader.ReadInt();

                if (pid == (int)Define.UdpPacket.PLAYER_MOVE_BROADCAST)
                {
                    string playerName = reader.ReadString();
                    float x = reader.ReadFloat();
                    float y = reader.ReadFloat();
                    float z = reader.ReadFloat();
                    float mx = reader.ReadFloat();
                    float my = reader.ReadFloat();
                    bool isRunning = reader.ReadBool();
                    bool isFalling = reader.ReadBool();

                    if (playerName == Managers.Network.player.Username)
                        return;

                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        Vector2 move = new Vector2(mx, my);
                        var manager = UnityEngine.Object.FindFirstObjectByType<PlayerManager>();
                        if (manager != null)
                            manager.UpdatePlayer(playerName, pos, move, isRunning, isFalling);
                    });
                }
            }

            // 계속 수신 대기
            _udp.BeginReceive(OnReceive, null);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[UDP] 수신 오류: {e.Message}");
            if (_running)
                _udp.BeginReceive(OnReceive, null);
        }
    }

    public void Stop()
    {
        _running = false;
        _udp?.Close();
        Debug.Log("[UDP] 종료됨");
    }

    /// <summary>
    ///  플레이어 이동정보전송
    /// </summary>
    
    public void SendPlayerMove(string name, Vector3 pos, Vector2 move, bool isRunning, bool isFalling)
    {
        if (!_running || _udp == null) return;

        try
        {
            using (var pw = new PacketWriter())
            {
                pw.WriteInt((int)Define.UdpPacket.PLAYER_MOVE);
                pw.WriteString(name);
                pw.WriteFloat(pos.x);
                pw.WriteFloat(pos.y);
                pw.WriteFloat(pos.z);
                pw.WriteFloat(move.x);
                pw.WriteFloat(move.y);
                pw.WriteBool(isRunning);
                pw.WriteBool(isFalling);

                byte[] data = pw.ToArray();
                _udp.Send(data, data.Length, _serverEP);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[UDP] 이동 패킷 전송 실패: {e.Message}");
        }
    }

    private GameObject FindRemotePlayer(string name)
    {
        // 임시함수
        var obj = GameObject.Find(name);
        if (obj != null) return obj;

        // 없으면 새로 생성 (테스트용)
        GameObject newPlayer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        newPlayer.name = name;
        return newPlayer;
    }


}

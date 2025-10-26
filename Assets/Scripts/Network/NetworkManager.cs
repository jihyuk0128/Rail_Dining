using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkManager
{
    private TcpClient _tcp;
    private NetworkStream _stream;
    private byte[] _recvBuffer = new byte[4096];
    public bool IsConnected => _tcp != null && _tcp.Connected;

    public string ConnectedIp { get; private set; }
    public ClientPlayer player { get; private set; }
    public RoomData roomData { get; private set; }

    // udp 
    private UdpGameServer _udpGame;
    public UdpGameServer UdpGame => _udpGame;

    public Vector3? pendingSpawnPos = null;

    public void InitUdp(string ip)
    {
        _udpGame = new UdpGameServer();
        _udpGame.Start(ip, Define.UDP_GAME_PORT);
    }

    // 이벤트 (다른 매니저가 구독 가능)
    public event Action<string> OnLoginSuccess;
    public event Action<int> OnRoomCreate;
    public event Action<int> OnRoomJoin;
    public event Action<string> OnRoomLeave;
    public event Action<string> OnHostAssigned;
    public event Action<string> OnChatReceived;
    public event Action OnGameStart;
    public event Action<string> OnError;
    public event Action<int,int,float> OnCustomerSpawn;
    public event Action<int> OnGameStartDay;
    public event Action<int,int,float> OnOrderMenu;
    public event Action<int> OnCustomerLeave;



    // ===================================================
    // 로그인: 연결 + 패킷 전송 + 내부 상태처리 한 번에
    // ===================================================
    public async void Login(string name)
    {
        try
        {
            if (!IsConnected)
            {
                var server = Udp.FindServer();
                if (server == null)
                {
                    OnError?.Invoke("[UDP] 서버를 찾을 수 없습니다.");
                    return;
                }

                ConnectedIp = server.Value.ip;
                _tcp = new TcpClient();
                await _tcp.ConnectAsync(server.Value.ip, server.Value.port);
                _stream = _tcp.GetStream();
                Debug.Log($"[Network] 서버 연결 성공: {ConnectedIp}:{server.Value.port}");
                StartReceive();
            }

            player = new ClientPlayer(name);
            Debug.Log($"[Network] 로그인 시도: {player.Username}");

            Send(pw =>
            {
                pw.WriteInt((int)Define.CtoS.LOGIN);
                pw.WriteString(player.Username);
            });
        }
        catch (Exception e)
        {
            OnError?.Invoke($"로그인 실패: {e.Message}");
        }
    }

    // ===================================================
    // 방 관련 요청 (상태 확인 + 패킷 전송)
    // ===================================================
    public void CreateRoom()
    {
        if (!IsConnected || roomData != null)
        {
            Debug.LogWarning("[Network] 서버와 연결되지 않거나 이미 방이있음");
            return;
        }

        Debug.Log("[Network] 방 생성 요청");
        Send(pw =>
        {
            pw.WriteInt((int)Define.CtoS.CREATE_ROOM);
            pw.WriteString(player.Username);
        });
    }

    public void JoinRoom(int roomId)
    {
        if (!IsConnected || roomData != null)
        {
            Debug.LogWarning("[Network] 서버와 연결되지 않거나 이미 방이있음");
            return;
        }

        Debug.Log($"[Network] 방 접속 요청: {roomId}");
        Send(pw =>
        {
            pw.WriteInt((int)Define.CtoS.JOIN_ROOM);
            pw.WriteInt(roomId);
        });
    }

    public void LeaveRoom()
    {
        if (!IsConnected || roomData != null)
        {
            Debug.LogWarning("[Network] 서버와 연결되지 않음");
            return;
        }

        Debug.Log("[Network] 방 나가기 요청");
        Send(pw => pw.WriteInt((int)Define.CtoS.LEAVE_ROOM));
    }

    public void StartGame()
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[Network] 서버와 연결되지 않음");
            return;
        }

        Debug.Log("[Network] 게임 시작 요청");
        Send(pw =>
        {
            pw.WriteInt((int)Define.CtoS.START_GAME);
        });
    }

    public void TutorialEnd()
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[Network] 서버와 연결되지 않음");
            return;
        }

        Debug.Log("[Network] 튜토리얼 종료 메세지 보냄");
        Send(pw =>
        {
            pw.WriteInt((int)Define.CtoS.TUTORIAL_END);
        });
    }

    public void TakeOrder(int customerid)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[Network] 서버와 연결되지 않음");
            return;
        }

        Debug.Log($"[Network] customerid : {customerid} 에게 레시피 뽑아줘");
        Send(pw =>
        {
            pw.WriteInt((int)Define.CtoS.TAKE_ORDER);
            pw.WriteInt(customerid);
        });
    }

    public void OrderSuccess(int customerid)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("[Network] 서버와 연결되지 않음");
            return;
        }

        Debug.Log($"[Network] customerid : {customerid} 주문 완료!");
        Send(pw =>
        {
            pw.WriteInt((int)Define.CtoS.ORDER_SUCCESS);
            pw.WriteInt(customerid);
        });
    }

    // ===================================================
    // 내부 공통 전송 로직
    // ===================================================
    public void Send(Action<PacketWriter> build)
    {
        try
        {
            using (var pw = new PacketWriter())
            {
                build(pw);
                byte[] data = pw.ToArrayWithLengthPrefix();
                _stream.Write(data, 0, data.Length);
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke($"패킷 전송 실패: {e.Message}");
        }
    }

    // ===================================================
    // 수신 처리
    // ===================================================
    private void StartReceive()
    {
        _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, OnReceive, null);
    }

    private void OnReceive(IAsyncResult ar)
    {
        try
        {
            int bytes = _stream.EndRead(ar);
            if (bytes <= 0) return;

            byte[] data = new byte[bytes];
            Array.Copy(_recvBuffer, data, bytes);

            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                int len = reader.ReadInt32();
                byte[] body = reader.ReadBytes(len);

                using (var pr = new PacketReader(body))
                {
                    int id = pr.ReadInt();
                    HandlePacket(id, pr);
                }
            }

            StartReceive();
        }
        catch (Exception e)
        {
            OnError?.Invoke($"데이터 수신 실패: {e.Message}");
        }
    }

    private void HandlePacket(int id, PacketReader reader)
    {
        // ------------------------------
        // [1] 서버 → 클라 응답(Response)
        // ------------------------------
        if (Enum.IsDefined(typeof(Define.StoC_Response), id))
        {
            switch ((Define.StoC_Response)id)
            {
                case Define.StoC_Response.LOGIN_OK:
                    player.Username = reader.ReadString();
                    OnLoginSuccess?.Invoke(player.Username);
                    Debug.Log("[Network] Login 성공!");
                    return;

                case Define.StoC_Response.ROOM_CEATE_OK:
                    {
                        int roomId = reader.ReadInt();
                        OnRoomCreate?.Invoke(roomId);
                        Debug.Log($"[Network] Room 생성 완료 ({roomId})");
                        return;
                    }

                case Define.StoC_Response.ROOM_JOIN_OK:
                    {
                        player.CurrentRoomId = reader.ReadInt();
                        OnRoomJoin?.Invoke(player.CurrentRoomId);
                        roomData = new RoomData(player.CurrentRoomId, false);
                        Debug.Log($"[Network] Room 참가 성공: {player.CurrentRoomId}");
                        return;
                    }
                case Define.StoC_Response.ROOM_LEAVE_OK:
                    {
                        roomData = null; // roomdata삭제
                        player.CurrentRoomId = -1;
                        Debug.Log("[Network] 방 나가기 성공");
                        return;
                    }
                case Define.StoC_Response.HOST_ASSIGNED:
                    string msg = reader.ReadString();
                    roomData.IsHost = true;
                    Debug.Log($"[Network] {msg}");
                    OnHostAssigned?.Invoke(msg);
                    break;

                case Define.StoC_Response.ACTION_DENIED:
                    string reason = reader.ReadString();
                    OnError?.Invoke(reason);
                    Debug.LogWarning($"[Network] 응답 : {reason}");
                    return;

                case Define.StoC_Response.ORDER_MENU:
                    {
                        int customerid = reader.ReadInt();
                        int ordermenu = reader.ReadInt();
                        float time = reader.ReadFloat();
                        Debug.Log($"[SERVER] 레시피 불러오기 시작");
                        OnOrderMenu?.Invoke(customerid,ordermenu, time);
                        return;
                    }
            }
        }

        // ------------------------------
        // [2] 서버 → 클라 이벤트(Event)
        // ------------------------------
        if (Enum.IsDefined(typeof(Define.StoC_Event), id))
        {
            switch ((Define.StoC_Event)id)
            {
                case Define.StoC_Event.BROADCAST_CHAT:
                    string chat = reader.ReadString();
                    OnChatReceived?.Invoke(chat);
                    Debug.Log($"[SERVER] {chat}");
                    break;

                case Define.StoC_Event.PLAYER_JOINED:
                    string joinedName = reader.ReadString();
                    roomData?.AddPlayer(joinedName);
                    Debug.Log($"[SERVER] 플레이어 입장: {joinedName}");
                    Debug.Log($"현재방상태 {roomData.CurrentPlayers}명 , {roomData.Players[0]}");


                    //UnityMainThreadDispatcher.Instance?.Enqueue(() =>
                    //{
                    //    // UI에 인원 갱신 요청
                    //    Managers.UI.UpdateRoomPlayerList(roomData.Players);
                    //});
                    break;

                case Define.StoC_Event.PLAYER_LEFT:
                    string leftName = reader.ReadString();
                    roomData?.RemovePlayer(leftName);
                    Debug.Log($"[SERVER] 플레이어 퇴장: {leftName}");

                    //UnityMainThreadDispatcher.Instance?.Enqueue(() =>
                    //{
                    //    Managers.UI.UpdateRoomPlayerList(roomData.Players);
                    //});
                    break;
                 

                case Define.StoC_Event.GAME_START:
                    {

                        Debug.Log("[SERVER] 게임 시작 신호 수신"); 

                        float x = reader.ReadFloat();
                        float y = reader.ReadFloat();
                        float z = reader.ReadFloat();
                        // --- 스폰 위치 적용 ---
                        Managers.Network.pendingSpawnPos = new Vector3(x, y, z);

                        // UDP 연결 (UDP 게임 서버 시작)
                        InitUdp(ConnectedIp);


                        OnGameStart?.Invoke();
                        break;
                    }

                case Define.StoC_Event.CUSTOMER_SPAWN:    
                    {
                        int gender = reader.ReadInt();
                        int CustomerId = reader.ReadInt();
                        float searchTime = reader.ReadFloat();
                        Debug.Log($"[SERVER] 손님 스폰 수신 (gender: {gender}  , cusmterid {CustomerId} , seardchtime {searchTime})");

                        OnCustomerSpawn?.Invoke(gender, CustomerId, searchTime);
                        break;
                    }
                case Define.StoC_Event.GAME_START_DAY:
                    {
                        int day = reader.ReadInt();
                        Debug.Log($"[SERVER] {day} 일차 시작!");
                        OnGameStartDay?.Invoke(day);

                        break;
                    }
                case Define.StoC_Event.CUSTOMER_LEAVE:
                    {
                        int customerId = reader.ReadInt();
                        Debug.Log($"[SERVER] {customerId}번 손님 떠남!");
                        OnCustomerLeave?.Invoke(customerId);
                        break;
                    }
            }

        }
    }
}
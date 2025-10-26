using UnityEngine;

public class Define
{
    public enum UIEvent
    {
        Click,
        Drag,
        BeginDrag,
        EndDrag
    }

    public const int UDP_PORT = 8888;
    public const int TCP_PORT = 7777;
    public const int MAX_ROOM_PLAYER = 2;
    public const int UDP_GAME_PORT = 7778;

    // ================================
    //  클라이언트 → 서버
    // ================================
    public enum CtoS
    {
        LOGIN = 1,
        CREATE_ROOM,
        JOIN_ROOM,
        LEAVE_ROOM,
        CHAT,
        START_GAME,
        TUTORIAL_END,
        TAKE_ORDER,
        ORDER_SUCCESS,
    }

    // ================================
    //  서버 → 클라이언트 응답
    // ================================
    public enum StoC_Response
    {
        LOGIN_OK = 100,
        ROOM_CEATE_OK,
        ROOM_JOIN_OK,
        ROOM_JOIN_FAIL,
        ROOM_LEAVE_OK,
        HOST_ASSIGNED,
        ACTION_DENIED,
        ORDER_MENU,

    }

    // ================================
    //  서버 → 클라이언트 이벤트
    // ================================
    public enum StoC_Event
    {
        BROADCAST_CHAT = 200,
        PLAYER_JOINED,
        PLAYER_LEFT,
        GAME_START,
        GAME_START_DAY,
        DAY_END,

        CUSTOMER_SPAWN, // 새로 추가
        CUSTOMER_LEAVE,

    }
    // ================================
    //  UDP 이동 관련 패킷
    // ================================
    public enum UdpPacket
    {
        PLAYER_MOVE = 500,
        PLAYER_MOVE_BROADCAST = 501,
        PLAYER_STATE = 510,
        PLAYER_STATE_BROADCAST = 511,
    }

}

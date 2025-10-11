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

    public enum Protocol
    {
        None = 0,
        Login = 1,
        CreateRoom = 2,
        JoinRoom = 3,
        Chat = 4,
        GetRoomList = 5,
        LeaveRoom = 6,
        StartGame = 7,
        Event = 8,
        RoomInfo = 9,
    }

    public enum ServerEvent
    {
        None = 0,
        PlayerJoined = 1,
        PlayerLeft = 2,
        HostLeft = 3,
        RoomClosed = 4,
        GameStarted = 5,
    }
}

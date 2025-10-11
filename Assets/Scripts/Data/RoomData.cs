using UnityEngine;

public class RoomData
{
    public int RoomId;
    public string HostName;
    public int CurrentPlayers;
    public int MaxPlayers;

    public RoomData(int roomId, string hostName, int current, int max)
    {
        RoomId = roomId;
        HostName = hostName;
        CurrentPlayers = current;
        MaxPlayers = max;
    }

    public override string ToString()
    {
        return $"Room {RoomId} | Host: {HostName} | Players: {CurrentPlayers}/{MaxPlayers}";
    }
}

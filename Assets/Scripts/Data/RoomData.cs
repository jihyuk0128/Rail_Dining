using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public int RoomId;
    public bool IsHost;
    public List<string> Players { get; private set; } = new();
    public int CurrentPlayers = 0;
    public int MaxPlayers;

    public RoomData(int roomId, bool isHost)
    {
        RoomId = roomId;
        IsHost = isHost;
        CurrentPlayers++;
        MaxPlayers = 2;
    }

    public void AddPlayer(string name)
    {
        if (!Players.Contains(name))
            Players.Add(name);
        CurrentPlayers++;
    }

    public void RemovePlayer(string name)
    {
        Players.Remove(name);
    }

    public void Clear()
    {
        Players.Clear();
    }
}

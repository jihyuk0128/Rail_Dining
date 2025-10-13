using UnityEngine;

public class ClientPlayer
{
    public string Username { get; set; }
    public int CurrentRoomId { get; set; } = -1;
    public bool IsHost { get; set; }

    public ClientPlayer(string name)
    {
        Username = name;
    }
}
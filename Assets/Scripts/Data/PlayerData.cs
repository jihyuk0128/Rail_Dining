using UnityEngine;

public class PlayerData 
{
    public string Username { get; private set; }
    public int Gender { get ; private set; }

    public PlayerData(string username, int gender)
    {
        Username = username;
        Gender = gender;

    }
}

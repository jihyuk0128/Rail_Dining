using System.Linq;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public static SeatManager Instance { get; private set; }

    private Seat[] seats;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        seats = FindObjectsOfType<Seat>();
    }

    public Seat GetEmptySeat()
    {
        return seats.FirstOrDefault(seat => seat.IsSeating == false);
    }
}

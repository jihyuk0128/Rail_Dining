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

        // SeatId 기준으로 정렬해서 항상 일정한 순서 보장
        seats = FindObjectsOfType<Seat>()
            .OrderBy(seat => seat.SeatId)
            .ToArray();
    }

    public Seat GetEmptySeat()
    {
        return seats.FirstOrDefault(seat => seat.IsSeating == false);
    }
}

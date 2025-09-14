using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField] private int seatId;        // 좌석 고유 번호
    [SerializeField] private Transform seatPoint; // 앉는 정확한 위치

    public bool IsSeating { get; set; } = false;

    void Awake()
    {
        if (seatPoint == null)
            seatPoint = this.transform; // 기본값 자기 자신
    }

    public int SeatId => seatId;
    public Vector3 GetSeatPosition() => seatPoint != null ? seatPoint.position : transform.position;
}

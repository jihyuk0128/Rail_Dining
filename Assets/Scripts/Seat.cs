using UnityEngine;

public enum SeatDirection
{
    FrontLeft,
    FrontRight,
    BehindLeft,
    BehindRight
}

public class Seat : MonoBehaviour
{
    [SerializeField] private int seatId;  // 좌석 고유 번호

    [Header("경로 설정")]
    public Transform[] entryRoutePoints;  // 입장 경로
    public Transform[] exitRoutePoints;   // 퇴장 경로 입장 경로의 역순

    [Header("앉는 방향")]
    public SeatDirection seatDirection;

    [Header("레이어 정렬")]
    public SeatYSort seatYSort;  // 좌석/소파 Sprite 정렬 담당

    public bool IsSeating { get; set; } = false;

    public int SeatId => seatId;
    public Vector3 GetSeatPosition() => transform.position;

    private void Awake()
    {
        if (entryRoutePoints != null && entryRoutePoints.Length > 0)
        {
            // entryRoutePoints 역순으로 exitRoutePoints 생성
            exitRoutePoints = new Transform[entryRoutePoints.Length];
            for (int i = 0; i < entryRoutePoints.Length; i++)
            {
                exitRoutePoints[i] = entryRoutePoints[entryRoutePoints.Length - 1 - i];
            }
        }

        // Seat 자체에 없으면 부모에서 SeatYSort 찾기
        if (seatYSort == null)
            seatYSort = GetComponent<SeatYSort>() ?? GetComponentInParent<SeatYSort>();
    }
}

using UnityEngine;

public class SeatYSort : YSort
{
    public enum SeatDirection { Front, Back }

    public SeatDirection direction = SeatDirection.Front;

    public int GetSortingOrder() { return rend.sortingOrder; }
}

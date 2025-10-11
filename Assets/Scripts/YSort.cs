using Spine.Unity;
using UnityEngine;

public class YSort : MonoBehaviour
{
    [Tooltip("Y 정렬 기준점 (없으면 transform.position 사용)")]
    public Transform pivot;

    protected Renderer rend;
    private SkeletonAnimation spineAnim;
    public bool isSeat = false;
    public SeatYSort seatYSort;

    void Awake()
    {
        spineAnim = GetComponent<SkeletonAnimation>();

        if (spineAnim != null)
            rend = spineAnim.GetComponent<MeshRenderer>();
        else
            rend = GetComponent<Renderer>();
    }

    void LateUpdate()
    {
        if (rend == null) return;
        if (isSeat && seatYSort != null) 
        {
            if (seatYSort.direction == SeatYSort.SeatDirection.Front)
                rend.sortingOrder = seatYSort.GetSortingOrder() + 1;
            else
                rend.sortingOrder = seatYSort.GetSortingOrder() - 1;
            return;
        }
        // 기준점이 있으면 그걸 사용, 없으면 자기 position
        float yPos = pivot ? pivot.position.y : transform.position.y;

        rend.sortingOrder = Mathf.RoundToInt(-yPos * 100);
    }
}

// sortingLayer 에서 배경과 오브젝트들의 레이어를 다르게 
using UnityEngine;
using Spine.Unity;

public class PlayerSpineController : MonoBehaviour
{
    [Header("Spine Objects")]
    public SkeletonAnimation frontSpine; // 앞모습 Spine
    public SkeletonAnimation backSpine;  // 뒤모습 Spine

    [Header("Pivot Object")]
    public Transform spinePivot; // Flip 기준 Empty GameObject

    [Header("스파인 위치 설정")]
    public Vector3 spineOffset = Vector3.zero; // 발밑 기준 보정

    private SkeletonAnimation activeSpine; // 현재 활성화 Spine
    private bool facingRight = true;

    private void Awake()
    {
        activeSpine = frontSpine;
        activeSpine.gameObject.SetActive(true);
    }

    public void UpdateSpine(Vector2 inputVector, bool isRunning)
    {
        if (inputVector == Vector2.zero)
        {
            // 입력 없으면 마지막 Spine 활성화, 애니메이션 정지
            if (activeSpine != null)
            {
                // Spine은 활성화 유지
                activeSpine.timeScale = 0f; // 애니메이션 일시정지
            }
            return;
        }

        // 이동 방향 판단 (앞/뒤)
        if (inputVector.y > 0) // 위쪽 이동 → 뒤모습 Spine
        {
            SetActiveSpine(backSpine, isRunning);
        }
        else // 아래쪽 이동 → 앞모습 Spine
        {
            SetActiveSpine(frontSpine, isRunning);
        }

        // 좌우 Flip 처리 (Pivot만 Flip)
        if (inputVector.x > 0) FlipPivot(true);
        else if (inputVector.x < 0) FlipPivot(false);
    }

    void SetActiveSpine(SkeletonAnimation spine, bool isRunning)
    {
        if (activeSpine != spine)
        {
            // 이전 Spine 비활성화
            if (activeSpine != null) activeSpine.gameObject.SetActive(false);

            activeSpine = spine;
            activeSpine.gameObject.SetActive(true);

            // SpineObject 발밑 기준 위치 보정
            activeSpine.transform.localPosition = spineOffset;

            // 새로 Spine이 켜졌으니 애니메이션 재생
            activeSpine.AnimationState.SetAnimation(0, "animation", true);
        }
        else
        {
            // 같은 Spine인데 현재 트랙에 애니메이션이 없는 경우 → 재생
            var currentAnim = activeSpine.AnimationState.GetCurrent(0);
            if (currentAnim == null)
            {
                activeSpine.AnimationState.SetAnimation(0, "animation", true);
            }
        }

        // 속도 조절 (달리기/걷기)
        activeSpine.timeScale = isRunning ? 3f : 1f;
    }

    // 좌우반전
    void FlipPivot(bool faceRight)
    {
        if (facingRight == faceRight) return;

        facingRight = faceRight;
        Vector3 scale = spinePivot.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        spinePivot.localScale = scale;
    }

    public void PlayFallAnimation()
    {
        //activeSpine.AnimationState.SetAnimation();    넘어진 애니메이션 재생  
    }
}

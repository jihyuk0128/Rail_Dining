using UnityEngine;
using Spine.Unity;

enum AnimState
{
    Idle,
    Walking,
    Running,
    Falling
}

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

    private AnimState currentState = AnimState.Idle; // 현재 상태

    private void Awake()
    {
        activeSpine = frontSpine;
        activeSpine.gameObject.SetActive(true);
        ChangeAnimation(AnimState.Idle);
    }

    public void UpdateSpine(Vector2 inputVector, bool isRunning)
    {
        // --- Idle 처리 ---
        if (inputVector == Vector2.zero)
        {
            ChangeAnimation(AnimState.Idle);
            return;
        }

        // --- 앞/뒤 Spine 전환 ---
        if (inputVector.y > 0)
        {
            SetActiveSpine(backSpine);
        }
        else
        {
            SetActiveSpine(frontSpine);
        }

        // --- 달리기/걷기 상태 전환 ---
        if (isRunning)
            ChangeAnimation(AnimState.Running);
        else
            ChangeAnimation(AnimState.Walking);

        // --- 좌우 Flip ---
        if (inputVector.x > 0) FlipPivot(true);
        else if (inputVector.x < 0) FlipPivot(false);
    }

    private void SetActiveSpine(SkeletonAnimation spine)
    {
        if (activeSpine == spine) return;

        if (activeSpine != null) activeSpine.gameObject.SetActive(false);

        activeSpine = spine;
        activeSpine.gameObject.SetActive(true);
        activeSpine.transform.localPosition = spineOffset;
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

    // 상태 기반 애니메이션 변경
    private void ChangeAnimation(AnimState newState)
    {
        // 같은 상태라도 앞 뒤 변경시에 애니메이션이 없으면 재생
        if (currentState == newState)
        {
            var currentAnim = activeSpine.AnimationState.GetCurrent(0);
            if (currentAnim == null || currentAnim.Animation.Name != StateToAnimName(newState))
            {
                string animNameRetry = StateToAnimName(newState);
                activeSpine.AnimationState.SetAnimation(0, animNameRetry, true);
            }
            return;
        }

        currentState = newState;
        string animName = StateToAnimName(newState);

        if (currentState == AnimState.Falling)
        {
            SetActiveSpine(frontSpine);
        }

        activeSpine.AnimationState.SetAnimation(0, animName, true);

        // 러닝일 경우 속도 증가
        activeSpine.timeScale = (newState == AnimState.Running) ? 3f : 1f;
    }

    // 상태와 Spine 애니메이션 이름 매핑
    private string StateToAnimName(AnimState state)
    {
        switch (state)
        {
            case AnimState.Idle: return "idle";
            case AnimState.Walking: return "walking";
            case AnimState.Running: return "walking"; // 러닝도 걷기 애니메이션 기반
            case AnimState.Falling: return "falling_down";
            default: return "idle";
        }
    }

    // 넘어짐 상태 외부 호출용
    public void PlayFallAnimation()
    {
        ChangeAnimation(AnimState.Falling);
    }
}

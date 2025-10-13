using UnityEngine;
using Spine.Unity;

enum PlayerAnimState
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

    private PlayerAnimState currentState = PlayerAnimState.Idle; // 현재 상태

    private void Awake()
    {
        activeSpine = frontSpine;
        activeSpine.gameObject.SetActive(true);
        ChangeAnimation(PlayerAnimState.Idle);
    }

    public void UpdateSpine(Vector2 inputVector, bool isRunning)
    {
        // --- Idle 처리 ---
        if (inputVector == Vector2.zero)
        {
            ChangeAnimation(PlayerAnimState.Idle);
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
            ChangeAnimation(PlayerAnimState.Running);
        else
            ChangeAnimation(PlayerAnimState.Walking);

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
        Vector3 scale = spinePivot.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        spinePivot.localScale = scale;
    }

    // 상태 기반 애니메이션 변경
    private void ChangeAnimation(PlayerAnimState newState)
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

        if (currentState == PlayerAnimState.Falling)
        {
            SetActiveSpine(frontSpine);
        }

        activeSpine.AnimationState.SetAnimation(0, animName, true);

        // 러닝일 경우 속도 증가
        activeSpine.timeScale = (newState == PlayerAnimState.Running) ? 3f : 1f;
    }

    // 상태와 Spine 애니메이션 이름 매핑
    private string StateToAnimName(PlayerAnimState state)
    {
        switch (state)
        {
            case PlayerAnimState.Idle: return "idle";
            case PlayerAnimState.Walking: return "walking";
            case PlayerAnimState.Running: return "walking"; // 러닝도 걷기 애니메이션 기반
            case PlayerAnimState.Falling: return "falling_down";
            default: return "idle";
        }
    }

    // 넘어짐 상태 외부 호출용
    public void PlayFallAnimation()
    {
        ChangeAnimation(PlayerAnimState.Falling);
    }

}

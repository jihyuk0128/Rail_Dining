using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


enum CustomerAnimState
{
    Idle,
    Walking,
    Siting,
    Siting_Idle,
    Happy,
}

public class CustomerSpineController : MonoBehaviour
{
    [Header("Spine Objects")]
    public SkeletonAnimation frontSpine; // 앞모습 Spine
    public SkeletonAnimation backSpine;  // 뒤모습 Spine

    [Header("Pivot Object")]
    public Transform spinePivot; // Flip 기준 Empty GameObject

    [Header("스파인 위치 설정")]
    public Vector3 spineOffset = Vector3.zero; // 발밑 기준 보정
    public Vector3 sitingOffset = Vector3.zero; // 앉을 때 보정

    private SkeletonAnimation activeSpine; // 현재 활성화 Spine
    private bool facingRight = true;
    public bool isSiting = false;
    public bool isSuccess = false;
    public SeatDirection seatDirection;

    private CustomerAnimState currentState = CustomerAnimState.Idle; // 현재 상태

    private void Awake()
    {
        frontSpine.gameObject.SetActive(false);
        backSpine.gameObject.SetActive(false);

        activeSpine = backSpine;
        activeSpine.gameObject.SetActive(true);
        ChangeAnimation(CustomerAnimState.Idle);
    }

    public void UpdateSpine(Vector2 inputVector)
    {
        // --- Idle 처리 ---
        if (inputVector == Vector2.zero && !isSiting)
        {
            ChangeAnimation(CustomerAnimState.Idle);
            return;
        }

        if (inputVector == Vector2.zero && isSuccess)
        {
            ChangeAnimation(CustomerAnimState.Happy);
            return;
        }

        if (inputVector == Vector2.zero && isSiting)
        {
            ChangeAnimation(CustomerAnimState.Siting);
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

        ChangeAnimation(CustomerAnimState.Walking);

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
    private void ChangeAnimation(CustomerAnimState newState)
    {
        if(newState != CustomerAnimState.Happy)
            activeSpine.transform.localPosition = spineOffset;
        else
            activeSpine.transform.localPosition = sitingOffset;
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

        if (currentState == CustomerAnimState.Siting)
        {
            StartCoroutine(SitingRoutine());
            return;
        }

        activeSpine.AnimationState.SetAnimation(0, animName, true);
        activeSpine.timeScale = 1f;
        if (currentState == CustomerAnimState.Idle)
            activeSpine.timeScale = 0f;
    }

    // 상태와 Spine 애니메이션 이름 매핑
    private string StateToAnimName(CustomerAnimState state)
    {
        switch (state)
        {
            case CustomerAnimState.Idle: return "walking";
            case CustomerAnimState.Walking: return "walking";
            case CustomerAnimState.Siting: return "siting";
            case CustomerAnimState.Siting_Idle: return "siting_idle";
            case CustomerAnimState.Happy: return "happy";
            default: return "walking";
        }
    }

    // Siting 루틴
    private IEnumerator SitingRoutine()
    {
        switch (seatDirection)
        {
            case SeatDirection.FrontRight:
                SetActiveSpine(frontSpine);
                FlipPivot(true);
                break;
            case SeatDirection.FrontLeft:
                SetActiveSpine(frontSpine);
                FlipPivot(false);
                break;
            case SeatDirection.BehindRight:
                SetActiveSpine(backSpine);
                FlipPivot(true);
                break;
            case SeatDirection.BehindLeft:
                SetActiveSpine(backSpine);
                FlipPivot(false);
                break;
        }
        string animName = StateToAnimName(currentState);

        activeSpine.transform.localPosition = sitingOffset;
        activeSpine.AnimationState.SetAnimation(0, animName, true);

        yield return new WaitForSeconds(0.5f);

        currentState = CustomerAnimState.Siting_Idle;
        animName = StateToAnimName(currentState);
        activeSpine.AnimationState.SetAnimation(0, animName, true);
        activeSpine.timeScale = 1f;
    }
}

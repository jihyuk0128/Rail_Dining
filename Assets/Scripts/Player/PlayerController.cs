using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동속도설정")]
    public float moveSpeed = 5f; // 기본 속도
    public float runSpeed = 8f; // 달리기 속도

    [Header("Spine Controller")]
    public PlayerSpineController spineController; // Spine 컨트롤러 참조

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private bool IsRunning;        // 달리기 여부 체크
    public bool IsFall { get; private set; } = false; // 넘어짐 체크 
    public bool isEventActive { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsFall || isEventActive) return;

        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveInput = Vector2.zero;
        }

        // 입력이 들어올 때마다 Spine 업데이트
        if (spineController != null)
        {
            spineController.UpdateSpine(moveInput, IsRunning);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (IsFall || isEventActive) return;

        if (context.performed)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
        // 입력이 들어올 때마다 Spine 업데이트
        if (spineController != null)
        {
            spineController.UpdateSpine(moveInput, IsRunning);
        }
    }

    private void FixedUpdate()
    {
        if (IsFall || isEventActive) return;

        float currentSpeed = IsRunning ? runSpeed : moveSpeed;
        Vector2 movement = moveInput.normalized * currentSpeed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime); 
    }

    public void SetFall(float duration)
    {
        if (!IsFall)
        {
            StartCoroutine(FallRoutine(duration));
        }
    }

    public void SetEvent(bool isEvent)
    {
        isEventActive = isEvent;
        if (spineController != null && isEventActive)
        {
            moveInput = Vector2.zero;
            IsRunning = false;
            spineController.UpdateSpine(moveInput, IsRunning);
        }
    }

    private IEnumerator FallRoutine(float duration)
    {
        IsFall = true;
        moveInput = Vector2.zero; // 입력 초기화

        Managers.Inventory.ClearInventory();

        if (spineController != null)
        {
            spineController.PlayFallAnimation(); // 넘어짐 애니메이션 재생 
        }

        yield return new WaitForSeconds(duration);

        IsFall = false;

        if (spineController != null)
        {
            spineController.UpdateSpine(Vector2.zero, false); // 애니메이션 idle로 복귀
        }
    }

    // TrainEvent로 정지중일때 손님에게 충돌이 끝나면 멈추게
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isEventActive && collision.collider.CompareTag("Customer"))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}

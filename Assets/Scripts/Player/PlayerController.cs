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

    // udp
    private float udpSendTimer = 0f;
    private float udpSendInterval = 0.01f; // 20Hz 전송
    private Vector3 _lastSentPos;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (Managers.Network.pendingSpawnPos.HasValue)
        {
            transform.position = Managers.Network.pendingSpawnPos.Value;
            Debug.Log($"[PlayerSpawn] 대기 중이던 스폰 위치 적용: {transform.position}");
            Managers.Network.pendingSpawnPos = null; // 한 번 적용 후 초기화
        }
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
        if(IsFall || isEventActive) return;

        float currentSpeed = IsRunning ? runSpeed : moveSpeed;
        Vector2 movement;

        // 상하좌우는 그대로
        if (moveInput.x == 0 || moveInput.y == 0) 
        {
            movement = moveInput.normalized * currentSpeed;
        }
        else
        {   // 대각선이동은 아이소메트릭 방향으로
            Vector2 isoDir = new Vector2(moveInput.x, (moveInput.y) / 2);
            movement = isoDir.normalized * currentSpeed;
        }

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);


        // === UDP 이동 패킷 전송 (주기 제어) ===
        if (Managers.Network.UdpGame == null)
            return;


        udpSendTimer += Time.fixedDeltaTime;
        if (udpSendTimer >= udpSendInterval) // 0.05초마다 한 번만
        {
            udpSendTimer = 0f;

            Vector2 dir = movement;
            if (dir.sqrMagnitude > 0.001f)
                dir.Normalize();

            float dist = Vector3.Distance(transform.position, _lastSentPos);
            if (dist > 0.0001f) // 최소 이동 거리 조건
            {
                Managers.Network.UdpGame.SendPlayerMove(
                    Managers.Network.player.Username,
                    new Vector3(transform.position.x, transform.position.y, 0f),
                    dir,
                   IsRunning,
                   IsFall
                );

                _lastSentPos = transform.position;
            }
        }
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

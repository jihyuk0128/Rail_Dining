using UnityEngine;

public class PlayerInteractionCollider : MonoBehaviour
{
    public PlayerController player;               // 플레이어 Transform
    public Transform interactionCollider;  // 상호작용 콜라이더 Transform
    public float offset = 1f;              // 플레이어 전방으로 떨어지는 거리
    public float holdDiagonalTime = 0.05f; // 마지막 대각선 유지 시간 (초)

    private Vector2 inputDirection;
    private float diagonalHoldTimer = 0f;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        interactionCollider = transform;
    }

    void Update()
    {
        if (Managers.UI.IsPopupOpen() || !GameManager.Instance.IsPlaying() || player.IsFall || player.isEventActive) return;

        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (inputDirection != Vector2.zero && !player.isEventActive)
        {
            if(Mathf.Abs(inputDirection.x) > 0 && Mathf.Abs(inputDirection.y) > 0)
            {
                diagonalHoldTimer = holdDiagonalTime;
                MoveCollider(inputDirection);
            }
            else
            {
                diagonalHoldTimer -= Time.deltaTime;
                if(diagonalHoldTimer <= 0f)
                {
                    diagonalHoldTimer = 0f;
                    MoveCollider(inputDirection);
                }
            }   
        }
    }

    void MoveCollider(Vector2 dir)
    {
        dir.Normalize(); // 방향 벡터 단위화

        // 8방향으로 제한
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector2 newPos = Vector2.zero;

        if (angle >= -22.5f && angle < 22.5f) newPos = Vector2.right;       // →
        else if (angle >= 22.5f && angle < 67.5f) newPos = new Vector2(1, 1);   // ↗
        else if (angle >= 67.5f && angle < 112.5f) newPos = Vector2.up;          // ↑
        else if (angle >= 112.5f && angle < 157.5f) newPos = new Vector2(-1, 1);  // ↖
        else if (angle >= 157.5f || angle < -157.5f) newPos = Vector2.left;       // ←
        else if (angle >= -157.5f && angle < -112.5f) newPos = new Vector2(-1, -1);// ↙
        else if (angle >= -112.5f && angle < -67.5f) newPos = Vector2.down;      // ↓
        else if (angle >= -67.5f && angle < -22.5f) newPos = new Vector2(1, -1);  // ↘
        
        if ( newPos == Vector2.left || newPos == Vector2.right)
            interactionCollider.localPosition = newPos.normalized * (offset * 1.5f);
        else 
            interactionCollider.localPosition = newPos.normalized * offset;

    }
}

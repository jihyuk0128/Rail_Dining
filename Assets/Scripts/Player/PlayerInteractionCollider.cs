using UnityEngine;

public class PlayerInteractionCollider : MonoBehaviour
{
    public PlayerController player;               // 플레이어 Transform
    public Transform interactionCollider;  // 상호작용 콜라이더 Transform
    public float offset = 1f;              // 플레이어 전방으로 떨어지는 거리

    private Vector2 inputDirection;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        interactionCollider = transform;
    }

    void Update()
    {
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (inputDirection != Vector2.zero && !player.isEventActive)
        {
            MoveCollider(inputDirection);
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
        
        interactionCollider.localPosition = newPos.normalized * offset;
    }
}

using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float speed = 2f;
    public Vector2 moveDir = new Vector2(1f, 0.5f); // 아이소메트릭 방향
    public Vector2 spriteSize = new Vector2(0f,0f); // 배경 1장의 크기 (X, Y)

    private Transform[] backgrounds;

    void Start()
    {
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        Vector3 dir = (Vector3)moveDir.normalized;
        Vector3 move = dir * speed * Time.deltaTime;

        foreach (Transform bg in backgrounds)
        {
            bg.position += move;

            bool xPassed = moveDir.x < 0
                ? bg.position.x < -spriteSize.x      // 왼쪽 이동
                : bg.position.x > spriteSize.x;      // 오른쪽 이동

            bool yPassed = moveDir.y < 0
                ? bg.position.y < -spriteSize.y * 0.5f // 아래 이동
                : bg.position.y > spriteSize.y * 0.5f; // 위 이동

            if (xPassed && yPassed)
            {
                Vector3 offset = new Vector3(
                    spriteSize.x * backgrounds.Length * Mathf.Sign(-moveDir.x),
                    spriteSize.y * backgrounds.Length * 0.5f * Mathf.Sign(-moveDir.y),
                    0f
                );

                bg.position += offset;
            }
        }
    }
}



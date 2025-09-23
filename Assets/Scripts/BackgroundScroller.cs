using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float speed = 2f;
    public Vector2 moveDir = new Vector2(-1f, -0.5f); // ↙ 아이소메트릭 방향
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
        Vector3 move = (Vector3)(moveDir.normalized * speed * Time.deltaTime);

        foreach (Transform bg in backgrounds)
        {
            bg.position += move;

            // X, Y 좌표가 모두 spriteSize만큼 벗어나면 뒤로 보냄
            if (bg.position.x < -spriteSize.x && bg.position.y < -spriteSize.y * 0.5f)
            {
                Vector3 offset = new Vector3(spriteSize.x * backgrounds.Length, spriteSize.y * backgrounds.Length * 0.5f, 0f);
                bg.position += offset;
            }
        }
    }
}



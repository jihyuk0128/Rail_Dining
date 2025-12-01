using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float speed = 2f;
    public Vector2 moveDir = new Vector2(1f, 0.5f);

    [Header("세트별 타일 크기")]
    public Vector2 tileSizeA;
    public Vector2 tileSizeB;

    [Header("각 세트 오브젝트 (Tile1, Tile2 포함)")]
    public Transform setA;
    public Transform setB;

    private bool usingA = true;

    private float switchInterval = 10f;
    private float timer = 0f;

    private void Start()
    {
        usingA = true;
        setA.gameObject.SetActive(true);
        setB.gameObject.SetActive(false);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            timer = 0f;
            SwitchBackgroundSet();
        }

        ScrollCurrentSet();
    }

    private void ScrollCurrentSet()
    {
        Transform currentSet = usingA ? setA : setB;
        Vector2 tileSize = usingA ? tileSizeA : tileSizeB;

        Vector3 dir = moveDir.normalized;
        Vector3 move = dir * speed * Time.deltaTime;

        for (int i = 0; i < currentSet.childCount; i++)
        {
            Transform tile = currentSet.GetChild(i);
            tile.position += move;

            // 타일 범위 판정
            bool xPassed = moveDir.x > 0
                ? tile.position.x > tileSize.x
                : tile.position.x < -tileSize.x;

            bool yPassed = moveDir.y > 0
                ? tile.position.y > tileSize.y * 0.5f
                : tile.position.y < -tileSize.y * 0.5f;

            if (xPassed && yPassed)
            {
                Vector3 offset = new Vector3(
                    tileSize.x * currentSet.childCount * -Mathf.Sign(moveDir.x),
                    tileSize.y * currentSet.childCount * -Mathf.Sign(moveDir.y) * 0.5f,
                    0f
                );

                tile.position += offset;
            }
        }
    }

    private void SwitchBackgroundSet()
    {
        usingA = !usingA;

        setA.gameObject.SetActive(usingA);
        setB.gameObject.SetActive(!usingA);
    }
}
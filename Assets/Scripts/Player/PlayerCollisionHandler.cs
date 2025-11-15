using UnityEngine;
using System.Collections.Generic;

public class PlayerCollisionHandler : MonoBehaviour
{
    private Dictionary<Collider2D, float> collisionTimers = new Dictionary<Collider2D, float>();
    private HashSet<Collider2D> ignoredColliders = new HashSet<Collider2D>();
    private HashSet<Collider2D> overlappingColliders = new HashSet<Collider2D>();

    private Collider2D myCollider;

    [SerializeField] private float ignoreDelay = 3f;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private bool IsPlayerOrRemote(Collider2D col)
    {
        return col.CompareTag("Player") || col.CompareTag("Remote");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D col = collision.collider;
        if (!IsPlayerOrRemote(col)) return;

        if (ignoredColliders.Contains(col)) return;

        if (!collisionTimers.ContainsKey(col))
            collisionTimers.Add(col, 0f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D col = collision.collider;
        if (!IsPlayerOrRemote(col)) return;

        if (ignoredColliders.Contains(col))
        {
            // 계속 겹치고 있으므로 기록
            overlappingColliders.Add(col);
            return;
        }

        collisionTimers[col] += Time.deltaTime;

        if (collisionTimers[col] >= ignoreDelay)
        {
            Debug.Log($"[Collision2D] 3초 유지 → 충돌 무시: {col.name}");

            Physics2D.IgnoreCollision(myCollider, col, true);
            ignoredColliders.Add(col);

            collisionTimers.Remove(col);
            overlappingColliders.Add(col); // 겹친 상태로 기록
        }
    }

    private void Update()
    {
        // === 자동 충돌 복구 체크 ===
        List<Collider2D> toRestore = new List<Collider2D>();

        foreach (var col in ignoredColliders)
        {
            if (col == null) continue;

            var dist = myCollider.Distance(col);

            if (!dist.isOverlapped) // 겹침이 끝난 순간
            {
                toRestore.Add(col);
            }
        }

        foreach (var col in toRestore)
        {
            Debug.Log($"자동 충돌 복구 (겹침에서 벗어남): {col.name}");

            Physics2D.IgnoreCollision(myCollider, col, false);
            ignoredColliders.Remove(col);
            overlappingColliders.Remove(col);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Collider2D col = collision.collider;
        if (!IsPlayerOrRemote(col)) return;

        if (ignoredColliders.Contains(col)) return;

        if (collisionTimers.ContainsKey(col))
            collisionTimers.Remove(col);
    }
}
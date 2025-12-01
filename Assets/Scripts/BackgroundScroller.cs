using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundTheme
{
    public GameObject[] tilePrefabs;   // 이 테마를 구성하는 타일(prefab)들
}

public class BackgroundScroller : MonoBehaviour
{
    public float speed = 2f;
    public Vector2 moveDir = new Vector2(1f, 0.5f);
    public float tileDistance = 20f;

    [Header("Themes")]
    public List<BackgroundTheme> themes;     // 여러 테마들
    private int currentThemeIndex = 0;

    private BackgroundTheme currentTheme;
    private BackgroundTheme nextTheme;

    private List<Transform> activeGroups = new List<Transform>();
    private bool isTransitioning = false;
    private int transitionCount = 0;
    private int nextPrefabIndex = 0;

    [Header("Auto Theme Change")]
    public float themeChangeInterval = 60f; // 1분(60초)
    private float themeTimer = 0f;

    void Start()
    {
        if (themes == null || themes.Count == 0)
        {
            Debug.LogError("[BackgroundScroller] Themes가 비어있습니다!");
            return;
        }

        currentThemeIndex = 0;
        currentTheme = themes[currentThemeIndex];
        nextTheme = currentTheme;

        SpawnInitialGroups();
    }

    void Update()
    {
        ScrollBackground();
        UpdateThemeTimer();
    }

    // ===========================
    //   배경 자동 교체 타이머
    // ===========================
    private void UpdateThemeTimer()
    {
        themeTimer += Time.deltaTime;

        if (themeTimer >= themeChangeInterval)
        {
            themeTimer = 0f;

            // 다음 테마 적용
            currentThemeIndex = (currentThemeIndex + 1) % themes.Count;
            nextTheme = themes[currentThemeIndex];

            Debug.Log($"[BackgroundScroller] 테마 전환 시작 → {nextTheme}");

            StartThemeTransition(nextTheme);
        }
    }

    // ===========================
    //   배경 스크롤 처리
    // ===========================
    private void ScrollBackground()
    {
        Vector3 move = (Vector3)moveDir.normalized * speed * Time.deltaTime;

        for (int i = 0; i < activeGroups.Count; i++)
        {
            Transform group = activeGroups[i];
            group.position += move;

            if (IsPassed(group))
            {
                ReplaceGroup(group);
            }
        }
    }

    // 초기 2개 그룹 생성
    void SpawnInitialGroups()
    {
        var g1 = Instantiate(currentTheme.tilePrefabs[0], transform);
        var g2 = Instantiate(currentTheme.tilePrefabs[1], transform);

        g1.transform.localPosition = Vector3.zero;
        g2.transform.localPosition = (Vector3)moveDir.normalized * -tileDistance;

        activeGroups.Add(g1.transform);
        activeGroups.Add(g2.transform);
    }

    bool IsPassed(Transform group)
    {
        return Vector3.Dot(group.localPosition, moveDir) > tileDistance;
    }

    void ReplaceGroup(Transform oldGroup)
    {
        oldGroup.localPosition -= (Vector3)moveDir.normalized * tileDistance * 2f;

        if (isTransitioning)
        {
            Transform newGroup = SwapGroupPrefab(oldGroup);
            transitionCount++;

            if (transitionCount >= 2)
            {
                isTransitioning = false;
                transitionCount = 0;
                currentTheme = nextTheme;
            }
        }
    }

    Transform SwapGroupPrefab(Transform oldGroup)
    {
        int idx = nextPrefabIndex;
        nextPrefabIndex = (nextPrefabIndex + 1) % nextTheme.tilePrefabs.Length;

        Vector3 pos = oldGroup.position;
        Destroy(oldGroup.gameObject);

        GameObject newObj = Instantiate(nextTheme.tilePrefabs[idx], transform);
        newObj.transform.position = pos;

        int listIndex = activeGroups.IndexOf(oldGroup);
        activeGroups[listIndex] = newObj.transform;

        return newObj.transform;
    }

    public void StartThemeTransition(BackgroundTheme newTheme)
    {
        nextTheme = newTheme;
        isTransitioning = true;
        nextPrefabIndex = 0;
    }
}
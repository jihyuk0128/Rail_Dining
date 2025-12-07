using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundTheme
{
    public GameObject[] tilePrefabs;
}

public class BackgroundScroller : MonoBehaviour
{
    public float speed = 2f;
    public Vector2 moveDir = new Vector2(1f, 0.5f);
    public float tileDistance = 20f;

    [Header("Themes")]
    public List<BackgroundTheme> themes;
    private int currentThemeIndex = 0;

    private BackgroundTheme currentTheme;
    private BackgroundTheme nextTheme;

    private List<Transform> activeGroups = new List<Transform>();

    private bool isTransitioning = false;
    private int nextPrefabIndex = 0;
    private int tilesToReplace = 0;

    [Header("Auto Theme Change")]
    public float themeChangeInterval = 60f;
    private float themeTimer = 0f;

    // --- Station spawn ---
    private float[] smallStationTimes = new float[] { 5f, 20f };
    private HashSet<float> usedSmallTimes = new HashSet<float>();
    private bool usedLargeStation = false;

    public GameObject smallStationPrefab;
    public GameObject largeStationPrefab;

    public Transform smallStationSpawnPoint;
    public Transform largeStationSpawnPoint;

    private List<GameObject> activeStations = new List<GameObject>();

    public bool stationActive = false;
    private bool currentStationIsLarge = false;
    private TileBackground firstBlockedTile = null;
    private TileBackground secondBlockedTile = null;


    List<GameObject> oldTilesToDestroy = new List<GameObject>();

    private HashSet<Transform> tilesPendingTransition = new HashSet<Transform>();

    // ★ 추가
    private int transitionCount = 0;

    void Start()
    {
        currentTheme = themes[currentThemeIndex];
        nextTheme = currentTheme;
        SpawnInitialGroups();
    }

    void Update()
    {
        ScrollBackground();
        ScrollStations();
        UpdateThemeTimer();
    }

    private void UpdateThemeTimer()
    {
        themeTimer += Time.deltaTime;

        if (themeTimer >= themeChangeInterval)
        {
            themeTimer = 0f;

            currentThemeIndex = (currentThemeIndex + 1) % themes.Count;
            nextTheme = themes[currentThemeIndex];
            isTransitioning = true;
            nextPrefabIndex = 0;

            transitionCount = 0;

            // ★ 여기에서 두 타일 모두 교체 예약
            tilesPendingTransition.Clear();
            foreach (var t in activeGroups)
                tilesPendingTransition.Add(t);

            ResetStationSpawnPlan();

            Debug.Log("테마 전환 시작 → " + currentThemeIndex);
        }
    }

    void ScrollBackground()
    {
        Vector3 move = (Vector3)moveDir.normalized * speed * Time.deltaTime;

        for (int i = 0; i < activeGroups.Count; i++)
        {
            Transform group = activeGroups[i];
            if (group == null) continue;

            group.position += move;

            if (IsPassed(group))
                ReplaceGroup(group);
        }
    }

    bool IsPassed(Transform group)
    {
        return Vector3.Dot(group.localPosition, moveDir) > tileDistance;
    }


    // ----------------------------
    //        ReplaceGroup
    // ----------------------------
    void ReplaceGroup(Transform oldGroup)
    {
        if (oldGroup == null)
            return;

        TrySpawnStation();

        TileBackground bg = oldGroup.GetComponent<TileBackground>();

        // --- 역 처리 ---
        // --- 작은역이면 첫 타일에서 extraTreeGroup만 끈다 ---
        if (stationActive && !currentStationIsLarge && firstBlockedTile == null)
        {
            firstBlockedTile = bg;
            bg.blockedTile = true;

            // ★ 작은역 전용 그룹 끄기
            bg.SetExtraTreeVisible(false);

            MoveTile(oldGroup);
            return;
        }

        // --- 큰역이면 기존 로직(원래 나무그룹 끄기) ---
        if (stationActive && currentStationIsLarge && firstBlockedTile == null)
        {
            firstBlockedTile = bg;
            bg.blockedTile = true;
            bg.SetTreeVisible(false);

            MoveTile(oldGroup);
            return;
        }

        if (stationActive && currentStationIsLarge && secondBlockedTile == null && bg != firstBlockedTile)
        {
            secondBlockedTile = bg;
            bg.blockedTile = true;
            bg.SetTreeVisible(false);

            MoveTile(oldGroup);
            return;
        }

        // --- 작은역 복구 ---
        if (!currentStationIsLarge && bg != null && bg.blockedTile)
        {
            bg.blockedTile = false;
            bg.SetExtraTreeVisible(true);
        }

        // --- 큰역 복구 ---
        if (currentStationIsLarge && bg != null && bg.blockedTile)
        {
            bg.blockedTile = false;
            bg.SetTreeVisible(true);
        }

        // ??? 테마 전환 중이면서, 이 타일이 교체 예약 상태라면 바로 교체 ???
        if (isTransitioning && tilesPendingTransition.Contains(oldGroup))
        {
            tilesPendingTransition.Remove(oldGroup);
            SwapGroupPrefab(oldGroup);
            transitionCount++;

            if (transitionCount >= 2)
            {
                isTransitioning = false;
                currentTheme = nextTheme;
                StartCoroutine(CleanupOldTiles());
            }

            MoveTile(oldGroup);
            return;
        }

        // --- 기본 이동 ---
        MoveTile(oldGroup);
    }

    // ----------------------------
    //     Swap Prefab Tile
    // ----------------------------
    Transform SwapGroupPrefab(Transform oldGroup)
    {
        int index = activeGroups.IndexOf(oldGroup);
        if (index == -1)
            return oldGroup;

        int prefabIndex = nextPrefabIndex % nextTheme.tilePrefabs.Length;
        nextPrefabIndex++;

        Vector3 pos = oldGroup.position;

        // ?? oldGroup을 activeGroups 리스트에서 제거
        activeGroups.RemoveAt(index);

        // old tile 비활성화 → 나중에 삭제
        oldGroup.gameObject.SetActive(false);
        oldTilesToDestroy.Add(oldGroup.gameObject);

        // 새 tile 생성
        GameObject newObj = Instantiate(nextTheme.tilePrefabs[prefabIndex], pos, Quaternion.identity, transform);

        // ?? activeGroups의 동일 인덱스에 새 tile 삽입
        activeGroups.Insert(index, newObj.transform);

        return newObj.transform;
    }


    // ----------------------------
    //   초기 타일 생성
    // ----------------------------
    void SpawnInitialGroups()
    {
        var g1 = Instantiate(currentTheme.tilePrefabs[0], transform);
        var g2 = Instantiate(currentTheme.tilePrefabs[1], transform);

        g1.transform.localPosition = Vector3.zero;
        g2.transform.localPosition = moveDir.normalized * -tileDistance;

        activeGroups.Add(g1.transform);
        activeGroups.Add(g2.transform);
    }

    // ----------------------------
    //     Station Logic
    // ----------------------------
    void ResetStationSpawnPlan()
    {
        themeTimer = 0f;

        usedSmallTimes.Clear();
        smallStationTimes = new float[] { 0f, 20f };

        usedLargeStation = false;
    }

    void TrySpawnStation()
    {
        // ?? 가을 테마에서는 역 금지
        if (currentThemeIndex != 0)
            return;

        foreach (float t in smallStationTimes)
        {
            if (!usedSmallTimes.Contains(t) && themeTimer >= t)
            {
                usedSmallTimes.Add(t);
                SpawnStation(smallStationPrefab, false);
                return;
            }
        }

        if (!usedLargeStation && themeTimer >= 40f)
        {
            usedLargeStation = true;
            SpawnStation(largeStationPrefab, true);
        }
    }

    void SpawnStation(GameObject prefab, bool isLarge)
    {
        Vector3 pos = isLarge ? largeStationSpawnPoint.position : smallStationSpawnPoint.position;

        GameObject st = Instantiate(prefab, pos, Quaternion.identity);
        activeStations.Add(st);

        stationActive = true;
        currentStationIsLarge = isLarge;   // ★ 현재 역 종류 저장

        firstBlockedTile = null;
        secondBlockedTile = null;
    }


    // ----------------------------
    //       Utility
    // ----------------------------
    void MoveTile(Transform tile)
    {
        tile.localPosition -= (Vector3)moveDir.normalized * tileDistance * 2f;
    }

    IEnumerator SafeDestroy(GameObject obj)
    {
        yield return null;
        if (obj != null)
            Destroy(obj);
    }

    IEnumerator CleanupOldTiles()
    {
        yield return null;

        foreach (var tile in oldTilesToDestroy)
        {
            if (tile != null)
                Destroy(tile);
        }

        oldTilesToDestroy.Clear();
    }

    void ScrollStations()
    {
        Vector3 move = (Vector3)moveDir.normalized * speed * Time.deltaTime;

        for (int i = activeStations.Count - 1; i >= 0; i--)
        {
            GameObject st = activeStations[i];
            if (st == null)
            {
                activeStations.RemoveAt(i);
                continue;
            }

            st.transform.position += move;

            if (Vector3.Dot(st.transform.localPosition, moveDir) > tileDistance * 3f)
            {
                Destroy(st);
                activeStations.RemoveAt(i);
            }
        }
    }
}
using Spine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("게임 설정")]
    [Tooltip("영업 플레이 시간 (초 단위)")]
    public float playTime = 60f; // 에디터에서 자유롭게 수정 가능

    [Header("매니저 참조")]
    public CustomerSpawner customerSpawner;
    public MoneyManager moneyManager;

    public int currentDay { get; private set; } = 1;
    private bool isPlaying = false;
    private int quota = 1;
    private bool resultClosed = false;
    private bool isMiniPlaying = false;

    public Transform playerSpawnPoint;

    [SerializeField] private GameObject playerPrefabM;
    [SerializeField] private GameObject playerPrefabF;

    // 임시 랜덤 테이블
    //public List<int> availableItemIDs = new() { 103, 104, 106, 108, 112, 113, 114, 116, 119, 120, 304 };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SpawnPlayer(false);
        StartCoroutine(GameLoop());
        Managers.Network.OnGameRestart += OnGameRestart;
        Managers.Network.OnGameOver += EndDay;
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            yield return StartCoroutine(StartDay());
            yield return StartCoroutine(PlayDay());


            yield return new WaitUntil(() => resultClosed);
            resultClosed = false;
        }
    }

    // 영업 시작
    private IEnumerator StartDay()
    {
        SoundManager.Instance?.StopAllBGM();
        var popup = Managers.UI.ShowPopupUI<UI_CountDown>();

        // 3, 2, 1, 0 (START)
        for (int i = 3; i >= 0; i--)
        {
            popup.ShowCount(i);
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"=== Day {currentDay} Start ===");
        isPlaying = true;

        // bgm 시작과 같이
        SoundManager.Instance?.PlayBGM("4min_BGM", 0);
        SoundManager.Instance?.PlayBGM("Train_BGM", 1);

        // 시작 사운드 사운드
        SoundManager.Instance?.PlaySFX("WorkStart_SFX");

        // 머니 초기화
        moneyManager?.ResetMoney();

        // 손님 스폰 시작
        customerSpawner?.StartSpawning();

        yield return null;
    }

    // 영업 진행
    private IEnumerator PlayDay()
    {
        float timer = 360f;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // 손님 스폰 종료
        customerSpawner?.StopSpawning();
        isPlaying = false;
        var popup = Managers.UI.ShowPopupUI<UI_CountDown>();
        popup.ShowCount(-1);
        yield return new WaitForSeconds(1f);
    }

    // 영업 종료
    public void EndDay(string name)
    {
        Managers.MainThread.Enqueue(() =>
        {
            Debug.Log($"=== Day {currentDay} End ===");
            /*
            // 결과 데이터
            int totalOrders = customerSpawner?.spawnedCount ?? 0;
            int successOrders = customerSpawner?.SuccessCount ?? 0;
            int earnedMoney = moneyManager?.GetMoney() ?? 0;

            // Managers.UI 로 결과창 표시
            UI_ResultSummary resultUI = Managers.UI.ShowPopupUI<UI_ResultSummary>();
            resultUI.SetResultData(currentDay, "1플레이어", "2플레이어", totalOrders, successOrders, earnedMoney, earnedMoney, quota, onClose: OnResultClosed);
            */
            var resultUI = Managers.UI.ShowPopupUI<UI_GameResult>();
            resultUI.OnRetryClicked += () =>
            {
                GameManager.Instance.RestartGame();
            };


            if (name == Managers.Network.player.Username)
            {
                if (Managers.Network.player.IsHost == true)
                {
                    resultUI.ShowResult(true);
                }
                resultUI.ShowResult(false);
            }
            else
            {
                if (Managers.Network.player.IsHost == true)
                {
                    resultUI.ShowResult(false);
                }
                resultUI.ShowResult(true);
            }
        });
    }

    private void OnResultClosed()
    {
        Debug.Log("결과창 닫힘 → 다음 날 시작");
        //currentDay++;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        RestartGame();
        resultClosed = true;
    }

    public void RestartGame()
    {
        Managers.Network.RestartGame();
    }

    private IEnumerator RestartRoutine()
    {
        // 씬이 완전히 로드된 후 다시 루프 시작
        yield return new WaitForSeconds(0.1f);
        if (customerSpawner == null)
            customerSpawner = GameObject.Find("CustomerSpawner").GetComponent<CustomerSpawner>();
        customerSpawner.ReStart(); // 스포너 카운트 리셋
        StartCoroutine(GameLoop());
    }

    public bool IsPlaying() => isPlaying;

    public bool IsMiniPlaying() => isMiniPlaying;
    public void SetMiniPlaying(bool set) => isMiniPlaying = set;

    public ItemData Getorder(int recipeId)
    {
        // 레시피가 존재하는지 확인
        if (!Managers.Data.RecipeDict.TryGetValue(recipeId, out var recipe))
        {
            Debug.LogWarning($"레시피 ID {recipeId}를 찾을 수 없습니다!");
            return null;
        }

        // 결과 아이템 ID
        int resultId = recipe.resultId;

        // 아이템 데이터 찾기
        if (Managers.Data.ItemDict.TryGetValue(resultId, out var item))
            return item;

        Debug.LogWarning($"결과 아이템 ID {resultId}를 ItemDict에서 찾을 수 없습니다!");
        return null;
    }

    public void OnGameRestart(string msg)
    {
        Managers.MainThread.Enqueue(() =>
        {
            Debug.Log("게임 재시작!");

            StopAllCoroutines();      // 기존 루프 중단
            resultClosed = false;
            isPlaying = false;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            StartCoroutine(RestartRoutine());
        });
    }

    private void OnDestroy()
    {
        Managers.Network.OnGameRestart -= OnGameRestart;
    }

    public void SpawnPlayer(bool gender)
    {
        // gender == false 남자, gender == true 여자
        GameObject prefab = gender ? playerPrefabF : playerPrefabM;

        if (prefab == null)
        {
            Debug.LogError("플레이어 프리팹이 설정되지 않았습니다!");
            return;
        }

        // 생성 위치는 playerSpawnPoint 기준
        Vector3 spawnPos = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;

        GameObject player = Instantiate(prefab, spawnPos, Quaternion.identity, playerSpawnPoint);

        TrainEventManager.Instance.player = player.GetComponent<PlayerController>();
        TrainEventManager.Instance.SetIndicationUI();

        Debug.Log($"플레이어 스폰 완료! gender={gender}");

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("게임 설정")]
    [Tooltip("영업 플레이 시간 (초 단위)")]
    public float ReadyTime = 30f;
    public float playTime = 60f; // 에디터에서 자유롭게 수정 가능

    [Header("매니저 참조")]
    public CustomerSpawner customerSpawner;
    public MoneyManager moneyManager;

    public int currentDay { get; private set; } = 1;
    private bool isPlaying = false;
    private int quota = 1;
    private bool resultClosed = false;

    // 임시 랜덤 테이블
    public List<int> availableItemIDs = new() { 103, 104, 106, 108, 112, 113, 114, 116, 119, 120, 304 };

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
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            yield return StartCoroutine(StartDay());
            yield return StartCoroutine(PlayDay());
            yield return StartCoroutine(EndDay());

            yield return new WaitUntil(() => resultClosed);
            resultClosed = false;
        }
    }

    // 영업 시작
    private IEnumerator StartDay()
    {
        Debug.Log($"=== Day {currentDay} Start ===");
        isPlaying = true;

        // bgm 시작과 같이
        SoundManager.Instance?.PlayBGM("BackGround_BGM");

        // 영업 준비 시간
        float timer = ReadyTime;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

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
        float timer = playTime;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // 손님 스폰 종료
        customerSpawner?.StopSpawning();
        isPlaying = false;
    }

    // 영업 종료
    private IEnumerator EndDay()
    {
        Debug.Log($"=== Day {currentDay} End ===");

        // 결과 데이터
        int totalOrders = customerSpawner?.spawnedCount ?? 0;
        int successOrders = customerSpawner?.SuccessCount ?? 0;
        int earnedMoney = moneyManager?.GetMoney() ?? 0;

        // Managers.UI 로 결과창 표시
        UI_ResultSummary resultUI = Managers.UI.ShowPopupUI<UI_ResultSummary>();
        resultUI.SetResultData(currentDay, "1플레이어", "2플레이어", totalOrders, successOrders, earnedMoney, earnedMoney, quota, onClose: OnResultClosed);

        yield break;
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
        Debug.Log("게임 재시작!");

        StopAllCoroutines();      // 기존 루프 중단
        resultClosed = false;
        isPlaying = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(RestartRoutine());
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

    public ItemData Getorder(int randId)
    {
        if (availableItemIDs.Count == 0)
        {
            Debug.LogWarning("선택 가능한 메뉴 ID가 없습니다!");
            return null;
        }

        int itemId = availableItemIDs[randId];

        if (Managers.Data.ItemDict.TryGetValue(itemId, out var item))
            return item;

        Debug.LogWarning($"Item ID {itemId}를 찾을 수 없습니다!");
        return null;
    }
  
}
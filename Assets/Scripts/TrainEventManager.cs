using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class TrainEventManager : MonoBehaviour
{
    public static TrainEventManager Instance { get; private set; }

    [Header("Event Settings")]
    public float eventInterval = 10f; // 10초마다 덜컹거림 체크
    public float fallTime = 1.5f;     // 넘어지는 시간

    [Header("References")]
    public PlayerController player;   // 플레이어 컨트롤러
    public TrainShaker trainShaker;   // 덜컹거림 효과 실행용

    private bool isEventActive = false;
    private UI_ShakeTrainEvent shakeUI = null;
    private GameObject indicationUI;

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
        StartCoroutine(EventRoutine());
    }

    private IEnumerator EventRoutine()
    {
        // UI 미리 생성 (씬 로딩 시 함께 존재)
        shakeUI = Managers.UI.ShowSceneUI<UI_ShakeTrainEvent>();
        shakeUI.gameObject.SetActive(false);

        while (true)
        {
            yield return new WaitForSeconds(eventInterval);

            // 덜컹 효과 실행
            if (trainShaker != null)
                trainShaker.TrainShake();

            // 50% 확률로 이벤트 발생
            if (Random.value < 0.5f && !Managers.UI.IsPopupOpen())
            {
                if (!isEventActive)
                    TriggerShakeEvent();
            }
        }
    }

    private void TriggerShakeEvent()
    {
        Debug.Log("[TrainEvent] 덜컹거림 이벤트 발생!");
        isEventActive = true;

        // UI 활성화 및 미니게임 시작
        if (indicationUI != null)
            StartCoroutine(ShowIndication());

        shakeUI.gameObject.SetActive(true);
        shakeUI.StartMiniGame();

        // 결과 콜백 등록
        shakeUI.OnMiniGameEnd = (result) =>
        {
            isEventActive = false;
            HandleResult(result);
        };
    }

    private void HandleResult(string result)
    {
        switch (result)
        {
            case "Perfect":
                Debug.Log("[TrainEvent] 완벽하게 균형 유지!");
                break;

            case "Success":
                Debug.Log("[TrainEvent] 덜컹거림 버팀 성공!");
                break;

            case "Fail":
                Debug.Log("[TrainEvent] 실패! 플레이어 넘어짐!");
                if (player != null)
                {
                    player.SetFall(fallTime);
                    player.SetEvent(false);
                }
                break;
        }
        // UI 자동 비활성화 (UI_ShakeTrainEvent 내부에서 처리됨)
    }

    private IEnumerator ShowIndication()
    {
        indicationUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        indicationUI.SetActive(false);
    }

    public bool IsEventActive() => isEventActive;

    public void SetIndicationUI()
    {
        GameObject go = GameObject.FindWithTag("Player");
        player = go?.GetComponent<PlayerController>();

        Transform ui = go.transform.Find("UI");
        if (ui != null)
        {
            indicationUI = ui.Find("indication")?.gameObject;
        }

        if (indicationUI == null)
            Debug.LogWarning("[TrainEventManager] indication UI를 찾을 수 없습니다!");

        else
            indicationUI.SetActive(false);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class TrainEventManager : MonoBehaviour
{
    [Header("Event Timing")]
    public float eventInterval = 10f; // 10초마다 체크
    public float keyTimeLimit = 3f;   // 각 키를 눌러야 하는 시간(초)
    public float fallTime = 1.5f;  // 넘어져 있는 시간

    [Header("References")]
    public PlayerController player; // 플레이어 레퍼런스 
    public TrainShaker trainShaker; // 매니저에서 덜컹거림 실행용

    private bool isEventActive = false;

    private UI_ShakeTrainEvent shakeUI = null;

    private void Start()
    {
        StartCoroutine(EventRoutine());
    }

    private IEnumerator EventRoutine()
    {
        // UI 생성
        shakeUI = Managers.UI.ShowSceneUI<UI_ShakeTrainEvent>();
        while (true)
        { 
            yield return new WaitForSeconds(eventInterval);

            if(trainShaker != null)
            {
                trainShaker.TrainShake();
            }
            // 50% 확률로 이벤트 발생
            if (Random.value < 0.5f && !Managers.UI.IsPopupOpen())
            {
                // 중첩 방지
                if (!isEventActive)
                    TriggerEvent();
            }
        }
    }

    private void TriggerEvent()
    {
        float r = Random.value;
        if (r < 0.8f)
        {
            //player.SetEvent(true);
            StartCoroutine(WASDChallenge());
        }
        else
        {
            //player.SetEvent(true);
            StartCoroutine(SpacebarChallenge());
        }
    }

    // WASD 2키 이벤트 (순차적) 
    private IEnumerator WASDChallenge()
    {
        isEventActive = true;
        Debug.Log("[Event] WASD 2-key challenge START");

        KeyCode key1 = GetRandomWASD();
        KeyCode key2 = GetRandomWASDExcluding(key1);

        // 여기서 UI로 key1/key2를 보여주기
        Debug.Log(key1+", "+key2);
        shakeUI.ShowUI();
        if (shakeUI != null)
        {
            shakeUI.OnCountdownFinished = () =>
            {
                StartCoroutine(WaitForKeySequence(key1, key2));
            };
            shakeUI.ShowKeyEvent(key1, key2);
        }

        yield return null;
    }

    // WASD 이벤트 
    private IEnumerator WaitForKeySequence(KeyCode key1, KeyCode key2)
    {
        Debug.Log($"첫 번째 키 입력 대기: {key1}");
        bool result1 = false;
        yield return WaitForKeyPress(key1, keyTimeLimit, ok => result1 = ok);

        if (!result1)
        {
            EventFailed();
            isEventActive = false;
            shakeUI.HideUI();
            yield break;
        }

        Debug.Log($"두 번째 키 입력 대기: {key2}");
        bool result2 = false;
        yield return WaitForKeyPress(key2, keyTimeLimit, ok => result2 = ok);

        if (!result2)
        {
            EventFailed();
            isEventActive = false;
            shakeUI.HideUI();
            yield break;
        }

        Debug.Log("[Event] WASD challenge SUCCESS");
        isEventActive = false;
        player.SetEvent(false);
        shakeUI.HideUI();
    }

    // Space 이벤트 
    private IEnumerator SpacebarChallenge()
    {
        isEventActive = true;
        Debug.Log("[Event] Spacebar challenge START");

        // 스페이스바 UI 들어갈 곳
        shakeUI.ShowUI();
        if (shakeUI != null)
        {
            shakeUI.OnCountdownFinished = () =>
            {
                StartCoroutine(WaitForKeyPressSequence(KeyCode.Space));
            };
            shakeUI.ShowSpaceEvent();
        }

        yield return null;
    }

    private IEnumerator WaitForKeyPressSequence(KeyCode key)
    {
        Debug.Log($"스페이스바 키 입력 대기");
        bool result = false;
        yield return WaitForKeyPress(key, keyTimeLimit, ok => result = ok);

        if (!result)
        {
            EventFailed();
            isEventActive = false;
            shakeUI.HideUI();
            yield break;
        }

        Debug.Log("[Event] Spacebar challenge SUCCESS");
        isEventActive = false;
        player.SetEvent(false);
        shakeUI.HideUI();
    }

    //키 입력 대기 (콜백으로 성공/실패 통보) 
    private IEnumerator WaitForKeyPress(KeyCode key, float timeLimit, System.Action<bool> callback)
    {
        float t = 0f;
        while (t < timeLimit)
        {
            if (Input.GetKeyDown(key))
            {
                callback?.Invoke(true);
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        // 시간 초과
        callback?.Invoke(false);
    }

    // 실패 처리
    private void EventFailed()
    {
        Debug.Log("[Event] 실패! 플레이어 넘어짐 상태로 전환");
        if (player != null)
        {
            player.SetFall(fallTime); // 플레이어 스크립트에 구현 필요
            player.SetEvent(false);
        }
    }

    // 무작위 WASD 키
    private KeyCode GetRandomWASD()
    {
        KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        return keys[Random.Range(0, keys.Length)];
    }

    // key1과 다른 키 반환
    private KeyCode GetRandomWASDExcluding(KeyCode exclude)
    {
        KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        KeyCode picked;
        do
        {
            picked = keys[Random.Range(0, keys.Length)];
        } while (picked == exclude);
        return picked;
    }
}
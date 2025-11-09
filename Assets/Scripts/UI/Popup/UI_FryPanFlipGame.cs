using System;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;

public class UI_FryPanFlipGame : UI_Popup
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform cursor;
    [SerializeField] private RectTransform successZone;
    [SerializeField] private RectTransform flipBar;
    [SerializeField] private SkeletonGraphic fryPanSpine;
    [SerializeField] private Button startButton;
    [SerializeField] private Image _spaceBarIcon;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 300f;
    [SerializeField] private int maxStage = 3;
    [SerializeField] private float speedUpRate = 2f;
    [SerializeField] private float slowDownRate = 0.8f;
    [SerializeField] private float zonePadding = 50f;

    [Header("Space Bar Sprites")]
    [SerializeField] private Sprite[] spaceBarFrames; // 여러 프레임 이미지
    [SerializeField] private float frameInterval = 0.2f; // 프레임 전환 간격(초)

    // 애니메이션 관련 변수
    private float frameTimer = 0f;
    private int currentFrame = 0;

    public bool isPlaying { get; private set; } = false;
    private bool movingRight = true;
    private int currentStage = 1;
    private int successCount = 0;
    private float minX, maxX;
    private bool waitToStop = false;

    public Action<string> OnMiniGameEnd;
    public bool isSuccess { get; private set; } = false;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        if (startButton != null)
            startButton.onClick.AddListener(StartMiniGame);

        float halfWidth = flipBar.rect.width / 2f;
        minX = -halfWidth;
        maxX = halfWidth;

        ResetCursor();
        ResetSuccessZone();

        // Spine 세팅
        if (fryPanSpine != null)
        {
            fryPanSpine.timeScale = 0f; // 처음엔 정지 상태
            fryPanSpine.AnimationState.Complete += OnSpineComplete;
        }

        _spaceBarIcon.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying) return;

        // === [SpaceBar 아이콘 애니메이션 처리] ===
        if (spaceBarFrames != null && spaceBarFrames.Length > 0 && _spaceBarIcon != null)
        {
            frameTimer += Time.deltaTime;              // 프레임 간 시간 누적
            if (frameTimer >= frameInterval)           // 설정된 간격마다 프레임 전환
            {
                frameTimer = 0f;                       // 타이머 리셋
                currentFrame = (currentFrame + 1) % spaceBarFrames.Length; // 다음 프레임으로
                _spaceBarIcon.sprite = spaceBarFrames[currentFrame];       // 이미지 교체
            }
        }

        MoveCursor();

        if (Input.GetKeyDown(KeyCode.Space))
            CheckSuccess();
    }

    private void MoveCursor()
    {
        float delta = moveSpeed * Time.deltaTime;
        float newX = cursor.anchoredPosition.x + (movingRight ? delta : -delta);

        if (newX >= maxX)
        {
            newX = maxX;
            movingRight = false;
        }
        else if (newX <= minX)
        {
            newX = minX;
            movingRight = true;
        }

        cursor.anchoredPosition = new Vector2(newX, cursor.anchoredPosition.y);
    }

    private void CheckSuccess()
    {
        float cursorX = cursor.anchoredPosition.x;
        float zoneMin = successZone.anchoredPosition.x - successZone.rect.width / 2f;
        float zoneMax = successZone.anchoredPosition.x + successZone.rect.width / 2f;

        if (cursorX >= zoneMin && cursorX <= zoneMax)
            OnFlipSuccess();
        else
            OnFlipFail();
    }

    private void OnFlipSuccess()
    {
        successCount++;
        currentStage++;
        moveSpeed *= speedUpRate;

        if (successCount >= maxStage)
        {
            EndGame(true);
        }
        else
        {
            ResetSuccessZone();
        }
    }

    private void OnFlipFail()
    {
        moveSpeed *= slowDownRate;
        ResetStage();
    }

    private void ResetStage()
    {
        ResetCursor();
        movingRight = true;
        ResetSuccessZone();
    }

    private void ResetCursor()
    {
        cursor.anchoredPosition = new Vector2(minX, cursor.anchoredPosition.y);
    }

    private void ResetSuccessZone()
    {
        float zoneHalfWidth = successZone.rect.width / 2f;
        float rangeMin = minX + zoneHalfWidth;
        float rangeMax = maxX - zoneHalfWidth;

        float randomX = UnityEngine.Random.Range(rangeMin, rangeMax);
        successZone.anchoredPosition = new Vector2(randomX, successZone.anchoredPosition.y);
    }

    public void StartMiniGame()
    {
        if (startButton != null)
            startButton.gameObject.SetActive(false);

        isPlaying = true;
        currentStage = 1;
        successCount = 0;
        moveSpeed = 300f;

        ResetCursor();
        ResetSuccessZone();

        PlaySpine(); // 웍질 애니메이션 시작

        _spaceBarIcon.gameObject.SetActive(true);
    }

    private void PlaySpine()
    {
        if (fryPanSpine == null) return;

        fryPanSpine.timeScale = 1f;
        fryPanSpine.AnimationState.SetAnimation(0, "animation", true);
    }

    private void StopSpineSmoothly()
    {
        // 현재 재생 중인 루프 애니메이션이 끝날 때까지만 돌리고,
        // Complete 이벤트에서 timeScale = 0으로 멈추게 함
        waitToStop = true;
    }

    private void EndGame(bool success)
    {
        isSuccess = success;
        isPlaying = false;
        //OnMiniGameEnd?.Invoke(success ? "Success" : "Fail");
        StopSpineSmoothly();
    }

    private void OnSpineComplete(TrackEntry entry)
    {
        // Complete은 루프 시에도 매 사이클마다 호출됨
        if (!waitToStop) return;

        // 루프 사이클 하나 끝났을 때 멈춤
        fryPanSpine.timeScale = 0f;
        waitToStop = false;

        // 약간의 텀을 두고 닫기
        //Invoke(nameof(RequestClosePopup), 0.5f);
    }

    private void RequestClosePopup()
    {
        Debug.Log("닫기");
        Managers.UI.ClosePopupUI(); // UIManager를 통해 정상 종료
    }
}
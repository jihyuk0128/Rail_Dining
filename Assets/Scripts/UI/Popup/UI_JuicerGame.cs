using System;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;

public class UI_JuicerGame : UI_Popup
{
    [Header("UI Elements")]
    [SerializeField] private SkeletonGraphic juicerSpine; // Spine 그래픽 (Juicer 애니메이션)
    [SerializeField] private Button startButton;          // 시작 버튼
    [SerializeField] private Image progressBar;           // 완성도 표시 (fillAmount로 조절)
    [SerializeField] private Image _spaceBarIcon;

    [Header("Settings")]
    [SerializeField] private float addPerPress = 5f;      // 스페이스바 입력 시 완성도 증가량 (%)
    [SerializeField] private float animationSpeed = 1f;   // Spine 애니메이션 속도

    [Header("Space Bar Sprites")]
    [SerializeField] private Sprite[] spaceBarFrames; // 여러 프레임 이미지
    [SerializeField] private float frameInterval = 0.2f; // 프레임 전환 간격(초)

    // 애니메이션 관련 변수
    private float frameTimer = 0f;
    private int currentFrame = 0;

    private float progress = 0f;
    private bool isPlaying = false;
    private bool isWaitingStop = false;

    public Action<string> OnMiniGameEnd; // "Success" 전달

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        if (startButton != null)
            startButton.onClick.AddListener(StartMiniGame);

        if (progressBar != null)
            progressBar.fillAmount = 0f;

        // Spine 초기화
        if (juicerSpine != null)
        {
            juicerSpine.timeScale = 0f; // 정지된 상태로 시작
            juicerSpine.AnimationState.Complete += OnSpineAnimationComplete;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddProgress();
        }
    }

    private void AddProgress()
    {
        progress += addPerPress;
        progress = Mathf.Min(progress, 100f);

        if (progressBar != null)
            progressBar.fillAmount = progress / 100f;

        Debug.Log($"{progress}% ");

        if (progress >= 100f)
            EndGame();
    }

    private void PlayJuicerAnimation()
    {
        if (juicerSpine == null) return;

        // Spine의 "animation" 애니메이션을 한 번 재생
        juicerSpine.timeScale = animationSpeed;
        juicerSpine.AnimationState.SetAnimation(0, "animation", false);
    }

    public void StartMiniGame()
    {
        if (startButton != null)
            startButton.gameObject.SetActive(false);

        isPlaying = true;
        progress = 0f;

        if (progressBar != null)
            progressBar.fillAmount = 0f;
        _spaceBarIcon.gameObject.SetActive(true);
    }

    private void EndGame()
    {
        isPlaying = false;
        isWaitingStop = true;

        PlayJuicerAnimation();

        // 현재 애니메이션이 한 사이클 끝날 때까지 기다렸다가 멈춤
        if (juicerSpine != null)
            juicerSpine.AnimationState.SetAnimation(0, "animation", false);

        OnMiniGameEnd?.Invoke("Success");
    }

    private void OnSpineAnimationComplete(TrackEntry entry)
    {
        if (!isWaitingStop) return;

        juicerSpine.timeScale = 0f;
        isWaitingStop = false;

        // 살짝 텀을 두고 UI 닫기
        Invoke(nameof(RequestClosePopup), 0.5f);
    }

    private void RequestClosePopup()
    {
        Debug.Log("닫기");
        Managers.UI.ClosePopupUI(); // UIManager를 통해 정상 종료
    }
}
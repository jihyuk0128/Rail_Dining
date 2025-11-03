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

    [Header("Settings")]
    [SerializeField] private float addPerPress = 5f;      // 스페이스바 입력 시 완성도 증가량 (%)
    [SerializeField] private float animationSpeed = 1f;   // Spine 애니메이션 속도

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
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddProgress();
            PlayJuicerAnimation();
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

        // Spine 애니메이션 재생 시작 (루프)
        if (juicerSpine != null)
        {
            juicerSpine.timeScale = animationSpeed;
            juicerSpine.AnimationState.SetAnimation(0, "animation", true);
        }
    }

    private void EndGame()
    {
        isPlaying = false;
        isWaitingStop = true;

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
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_ShakeTrainEvent : UI_Scene
{
    private enum GameObjects
    {
        CursorRoot,
    }

    private enum Images
    {
        GaugeBar,
        SuccessZone,
        PerfectZone,
        SpaceBarIcon,
    }

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 1f;    // 커서 이동 속도
    [SerializeField] private int maxBounces = 3;      // 왕복 횟수 제한
    [SerializeField] private bool autoStart = false;  // 자동 시작 여부

    [Header("Space Bar Sprites")]
    [SerializeField] private Sprite[] spaceBarFrames; // 여러 프레임 이미지
    [SerializeField] private float frameInterval = 0.2f; // 프레임 전환 간격(초)

    public Action<string> OnMiniGameEnd; // 결과: "Perfect", "Success", "Fail"

    private RectTransform _gaugeBar;
    private RectTransform _successZone;
    private RectTransform _perfectZone;
    private RectTransform _cursorRoot;
    private Image _spaceBarIcon;

    private bool isPlaying = false;
    private bool movingRight = true;
    private float progress = 0f;
    private int bounceCount = 0;

    private float leftEdge;
    private float rightEdge;

    // 애니메이션 관련 변수
    private float frameTimer = 0f;
    private int currentFrame = 0;

    private void Awake()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        _gaugeBar = GetImage((int)Images.GaugeBar).GetComponent<RectTransform>();
        _successZone = GetImage((int)Images.SuccessZone).GetComponent<RectTransform>();
        _perfectZone = GetImage((int)Images.PerfectZone).GetComponent<RectTransform>();
        _spaceBarIcon = GetImage((int)Images.SpaceBarIcon);

        _cursorRoot = GetObject((int)GameObjects.CursorRoot)?.GetComponent<RectTransform>();
        if (_cursorRoot == null)
            Debug.LogWarning("[UI_ShakeTrainEvent] CursorRoot 찾기 실패!");

        // 게이지 이동 범위 계산
        leftEdge = -_gaugeBar.rect.width / 2f;
        rightEdge = _gaugeBar.rect.width / 2f;

        if (autoStart)
            StartMiniGame();
    }

    private void Update()
    {
        if (!isPlaying) return;

        // 스페이스바 아이콘 애니메이션 처리
        if (spaceBarFrames != null && spaceBarFrames.Length > 0 && _spaceBarIcon != null)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameInterval)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % spaceBarFrames.Length;
                _spaceBarIcon.sprite = spaceBarFrames[currentFrame];
            }
        }

        // 커서 왕복 이동
        progress += (movingRight ? 1 : -1) * moveSpeed * Time.deltaTime;

        if (progress >= 1f)
        {
            progress = 1f;
            movingRight = false;
            bounceCount++;
        }
        else if (progress <= 0f)
        {
            progress = 0f;
            movingRight = true;
            bounceCount++;
        }

        // 커서 위치 갱신
        if (_cursorRoot != null)
        {
            float newX = Mathf.Lerp(leftEdge, rightEdge, progress);
            _cursorRoot.anchoredPosition = new Vector2(newX, _cursorRoot.anchoredPosition.y);
        }

        // 실패 조건: 왕복 횟수 초과
        if (bounceCount >= maxBounces)
        {
            Fail();
            return;
        }

        // Space 입력 처리 (판정만 남기고 아이콘 변경은 제거)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSuccess();
        }
    }

    private void CheckSuccess()
    {
        if (_cursorRoot == null) return;

        float cursorX = _cursorRoot.anchoredPosition.x;

        float successMin = _successZone.anchoredPosition.x - _successZone.rect.width / 2f;
        float successMax = _successZone.anchoredPosition.x + _successZone.rect.width / 2f;

        float perfectMin = _perfectZone.anchoredPosition.x - _perfectZone.rect.width / 2f;
        float perfectMax = _perfectZone.anchoredPosition.x + _perfectZone.rect.width / 2f;

        string result;

        if (cursorX >= perfectMin && cursorX <= perfectMax)
        {
            result = "Perfect";
            Debug.Log("대성공!");
        }
        else if (cursorX >= successMin && cursorX <= successMax)
        {
            result = "Success";
            Debug.Log("성공!");
        }
        else
        {
            result = "Fail";
            Debug.Log("실패!");
        }

        if (result == "Fail")
        {
            Fail();
        }
        else
        {
            isPlaying = false;
            OnMiniGameEnd?.Invoke(result);
            gameObject.SetActive(false);
        }
    }

    private void Fail()
    {
        Debug.Log("덜컹거림 실패!");
        isPlaying = false;
        OnMiniGameEnd?.Invoke("Fail");
        gameObject.SetActive(false);
    }

    public void StartMiniGame()
    {
        progress = 0f;
        movingRight = true;
        bounceCount = 0;
        isPlaying = true;

        // 아이콘 애니메이션 초기화
        frameTimer = 0f;
        currentFrame = 0;
        if (spaceBarFrames != null && spaceBarFrames.Length > 0)
            _spaceBarIcon.sprite = spaceBarFrames[0];
    }

    public void StopMiniGame()
    {
        isPlaying = false;
        gameObject.SetActive(false);
    }
}
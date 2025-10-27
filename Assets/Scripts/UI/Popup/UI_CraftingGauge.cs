using System;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class UI_CraftingGauge : UI_Popup
{
    private enum Images
    {
        GaugeBar,
        SuccessZone,
        PerfectZone,
        Cursor,
        SpaceBarIcon,
    }

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 80f; // 커서 회전 속도 (deg/s)
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 180f;
    [SerializeField] private float radius = 240f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float yOffset = -40f;
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private int maxBounces = 3;      // 왕복 횟수 제한

    [Header("Space Bar Sprites")]
    [SerializeField] private Sprite[] spaceBarFrames; // 여러 장의 이미지
    [SerializeField] private float frameInterval = 0.2f; // 프레임 전환 속도

    [Header("Space Bar Sprites")]
    public SkeletonGraphic craftingSpine;

    private bool isAnimActive = false;

    public Action<string> OnMiniGameEnd; // "Perfect", "Success", "Fail"

    private RectTransform _cursor;
    private RectTransform _successZone;
    private RectTransform _perfectZone;
    private Image _spaceBarIcon;

    private bool isPlaying = false;
    private bool movingRight = true;
    private float currentAngle;
    private int bounceCount = 0;

    // prefab 기준 각도 판정용
    private float successMinAngle, successMaxAngle;
    private float perfectMinAngle, perfectMaxAngle;

    private float iconTimer = 0f;
    private int iconFrameIndex = 0;
    private float startDelayTimer = 0f;

    // Start에서 Init 호출
    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));

        _cursor = GetImage((int)Images.Cursor).GetComponent<RectTransform>();
        _successZone = GetImage((int)Images.SuccessZone).GetComponent<RectTransform>();
        _perfectZone = GetImage((int)Images.PerfectZone).GetComponent<RectTransform>();
        _spaceBarIcon = GetImage((int)Images.SpaceBarIcon);

        // Prefab 기준 각도 계산
        ReadZoneAngles();

        if (autoStart)
            StartMiniGame();
    }

    void Update()
    {
        // 애니메이션 아이콘 자동 교체
        if (spaceBarFrames != null && spaceBarFrames.Length > 0)
        {
            iconTimer += Time.deltaTime;
            if (iconTimer >= frameInterval)
            {
                iconTimer = 0f;
                iconFrameIndex = (iconFrameIndex + 1) % spaceBarFrames.Length;
                _spaceBarIcon.sprite = spaceBarFrames[iconFrameIndex];
            }
        }

        // 시작 대기 시간
        if (isPlaying && startDelayTimer < startDelay)
        {
            startDelayTimer += Time.deltaTime;
            return; // 아직 커서 움직이지 않음
        }

        if (!isPlaying) return;

        if (!isAnimActive)
        {
            craftingSpine.AnimationState.SetAnimation(0, "1", true);
            craftingSpine.timeScale = 1f;
            isAnimActive = true;
        }

        // 0~180 왕복 이동
        float delta = moveSpeed * Time.deltaTime;
        currentAngle += movingRight ? delta : -delta;

        if (currentAngle >= maxAngle)
        {
            currentAngle = maxAngle;
            bounceCount++;
            movingRight = false;
        }
        else if (currentAngle <= minAngle)
        {
            currentAngle = minAngle;
            bounceCount++;
            movingRight = true;
        }

        // 실패 조건: 왕복 횟수 초과
        if (bounceCount >= maxBounces)
        {
            Fail();
            return;
        }

        // 커서 위치 계산
        Vector2 pos = new Vector2(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * radius;

        pos.y += yOffset;
        _cursor.anchoredPosition = pos;
        _cursor.localRotation = Quaternion.Euler(0, 0, currentAngle - 90f);

        // Space 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
            CheckSuccess();
    }

    private void ReadZoneAngles()
    {
        // Prefab에 설정된 성공/대성공 각도를 기준으로 판정 범위 계산
        float successRot = NormalizeAngle(_successZone.localEulerAngles.z + 90f);
        float perfectRot = NormalizeAngle(_perfectZone.localEulerAngles.z + 90f);

        // RectTransform의 실제 폭 읽기
        float successPixelWidth = _successZone.rect.width;  
        float perfectPixelWidth = _perfectZone.rect.width;

        // 반지름을 기준으로 폭(px) → 각도(°)로 환산
        float successWidth = (successPixelWidth / radius) * Mathf.Rad2Deg;
        float perfectWidth = (perfectPixelWidth / radius) * Mathf.Rad2Deg;

        successMinAngle = successRot - successWidth / 2f;
        successMaxAngle = successRot + successWidth / 2f;

        perfectMinAngle = perfectRot - perfectWidth / 2f;
        perfectMaxAngle = perfectRot + perfectWidth / 2f;

        Debug.Log($"[UI_CraftingGauge] 성공({successMinAngle:F1}°~{successMaxAngle:F1}°), 대성공({perfectMinAngle:F1}°~{perfectMaxAngle:F1}°)");
    }

    private void CheckSuccess()
    {
        string result;
        craftingSpine.timeScale = 0f;
        if (currentAngle >= perfectMinAngle && currentAngle <= perfectMaxAngle)
        {
            result = "Perfect";
            Debug.Log("대성공!");
        }
        else if (currentAngle >= successMinAngle && currentAngle <= successMaxAngle)
        {
            result = "Success";
            Debug.Log("성공!");
        }
        else
        {
            result = "Fail";
            Debug.Log("실패!");
        }

        isPlaying = false;
        OnMiniGameEnd?.Invoke(result);
    }

    public void StartMiniGame()
    {
        currentAngle = minAngle;
        movingRight = true;
        isPlaying = true;
        startDelayTimer = 0f; // 대기시간 초기화

        // 아이콘 애니메이션 초기화
        iconFrameIndex = 0;
        iconTimer = 0f;
        if (spaceBarFrames != null && spaceBarFrames.Length > 0)
            _spaceBarIcon.sprite = spaceBarFrames[0];
        craftingSpine.gameObject.SetActive(true);
    }

    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
        isPlaying = false;
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private void Fail()
    {
        Debug.Log("제작 실패!");
        isPlaying = false;
        craftingSpine.timeScale = 0f;
        OnMiniGameEnd?.Invoke("Fail");
        gameObject.SetActive(false);
    }
}
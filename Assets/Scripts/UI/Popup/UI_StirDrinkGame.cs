using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StirDrinkGame : UI_Popup, IHasMiniGameEnd
{
    private enum Images
    {
        Cup,
        Spoon,
        ProgressBar,
        MouseIcon,
    }

    private Image _cup;
    private RectTransform _spoon;
    private Image _progressBar;
    private Image _mouseIcon;

    [Header("Settings")]
    [SerializeField] private float stirProgress = 0.05f; // 한 번 왕복할 때 게이지 증가량 (10%)
    [SerializeField] private float dragSensitivity = 1.0f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float leftX = 200f;     // 왼쪽 끝
    [SerializeField] private float rightX = 300f;     // 오른쪽 끝
    [SerializeField] private float speed = 150f;

    public bool isPlaying { get; private set; } = false;
    private bool isDragging = false;

    private Vector2 dragStartPos;
    private float cupLeftLimit;
    private float cupRightLimit;

    private float progress = 0f;
    private float prevX;
    private int stirStep = 0; // 0: 가운데, 1: 오른쪽 도달, 2: 왼쪽 도달
    private int stirDirection = 0; // -1: 왼쪽, +1: 오른쪽
    [SerializeField] private bool reachedRight = false;
    [SerializeField] private bool reachedLeft = false;

    public event Action<bool> OnMiniGameEnd;

    public bool isSuccess { get; private set; } = false;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));

        _cup = GetImage((int)Images.Cup);
        _spoon = GetImage((int)Images.Spoon).GetComponent<RectTransform>();
        _progressBar = GetImage((int)Images.ProgressBar);
        _mouseIcon = GetImage((int)Images.MouseIcon);

        _progressBar.fillAmount = 0f;
        _spoon.gameObject.SetActive(false);
        _mouseIcon.gameObject.SetActive(false);

        // 드래그 이벤트
        BindEvent(_spoon.gameObject, OnBeginDragSpoon, Define.UIEvent.BeginDrag);
        BindEvent(_spoon.gameObject, OnDragSpoon, Define.UIEvent.Drag);
        BindEvent(_spoon.gameObject, OnEndDragSpoon, Define.UIEvent.EndDrag);

        StartMiniGame();
    }

    private void StartMiniGame()
    {
        _spoon.gameObject.SetActive(true);
        _mouseIcon.gameObject.SetActive(true);
        _progressBar.fillAmount = 0f;
        progress = 0f;
        isPlaying = true;
        GameManager.Instance.SetMiniPlaying(isPlaying);

        StartCoroutine(MoveLoop());

        RectTransform cupRect = _cup.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        cupRect.GetLocalCorners(corners); // 로컬 좌표로 변경

        // 부모 기준으로 좌우 경계 설정
        Vector3 cupPos = cupRect.localPosition;
        cupLeftLimit = cupPos.x + corners[0].x;
        cupRightLimit = cupPos.x + corners[3].x;

        // 컵 좌우 보정
        cupLeftLimit += 45f;
        cupRightLimit -= 45f;

        // 중앙 정렬
        Vector3 spoonPos = _spoon.localPosition;
        spoonPos.x = (cupLeftLimit + cupRightLimit) / 2f;
        _spoon.localPosition = spoonPos;

        prevX = _spoon.localPosition.x;

        Debug.Log($"[StirGame] Local Cup range: {cupLeftLimit:F1} ~ {cupRightLimit:F1}");
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (!isDragging)
        {
            Vector3 pos = _spoon.localPosition;
            pos.x = Mathf.Lerp(pos.x, (cupLeftLimit + cupRightLimit) / 2f, Time.deltaTime * returnSpeed);
            _spoon.localPosition = pos;
        }

        if (progress >= 1f)
            StartCoroutine(FinishGame());
    }

    private void OnBeginDragSpoon(PointerEventData evt)
    {
        if (!isPlaying) return;
        isDragging = true;
        dragStartPos = evt.position;
    }

    private void OnDragSpoon(PointerEventData evt)
    {
        if (!isPlaying) return;

        float deltaX = (evt.position.x - dragStartPos.x) * dragSensitivity;
        dragStartPos = evt.position;

        // localPosition으로 변경 (로컬 좌표계 일치)
        Vector3 pos = _spoon.localPosition;
        pos.x = Mathf.Clamp(pos.x + deltaX, cupLeftLimit, cupRightLimit);
        _spoon.localPosition = pos;

        float x = pos.x;

        // 오른쪽 끝 도달 시
        if (x >= cupRightLimit - 10f)
        {
            if (stirDirection != 1) // 이전이 왼쪽 → 지금 오른쪽
            {
                stirDirection = 1;
                AddStirProgress();
                Debug.Log("?? 오른쪽 도달: 게이지 상승");
            }
        }
        // 왼쪽 끝 도달 시
        else if (x <= cupLeftLimit + 10f)
        {
            if (stirDirection != -1) // 이전이 오른쪽 → 지금 왼쪽
            {
                stirDirection = -1;
                AddStirProgress();
                Debug.Log("?? 왼쪽 도달: 게이지 상승");
            }
        }
    }

    private void OnEndDragSpoon(PointerEventData evt)
    {
        isDragging = false;
    }

    private void AddStirProgress()
    {
        progress = Mathf.Clamp01(progress + stirProgress);
        _progressBar.fillAmount = progress;

        SoundManager.Instance.PlaySFX("Mixglass_SFX");

        // 디버그 확인용
        Debug.Log($"[Stir] Progress = {progress * 100f:F1}%");
    }

    private IEnumerator FinishGame()
    {
        isSuccess = true;
        isPlaying = false;
        _progressBar.fillAmount = 1f;
        yield return new WaitForSeconds(0.3f);
        OnMiniGameEnd?.Invoke(isSuccess);
        GameManager.Instance.SetMiniPlaying(isPlaying);
        // 살짝 텀을 두고 UI 닫기
        //Invoke(nameof(RequestClosePopup), 0.5f);
    }

    private IEnumerator MoveLoop()
    {
        bool movingRight = true;
        var rect = _mouseIcon.GetComponent<RectTransform>();
        while (true)
        {
            Vector2 pos = rect.anchoredPosition;

            // 방향에 따라 이동
            float dir = movingRight ? 1f : -1f;
            pos.x += dir * speed * Time.deltaTime;
            rect.anchoredPosition = pos;

            // 왼쪽/오른쪽 끝에 닿으면 방향 반전
            if (pos.x >= rightX)
                movingRight = false;

            if (pos.x <= leftX)
                movingRight = true;

            yield return null;
        }
    }
    private void RequestClosePopup()
    {
        Debug.Log("닫기");
        Managers.UI.ClosePopupUI(); // UIManager를 통해 정상 종료
    }
}
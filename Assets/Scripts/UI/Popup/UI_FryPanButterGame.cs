using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_FryPanButterGame : UI_Popup
{
    private enum Images
    {
        FryPan_Empty,
        ButterIcon,
        ProgressBar,
    }

    private enum Buttons
    {
        StartButton,
    }

    private RawImage _maskImage;
    private Image _butterIcon;
    private Image _progressBar;
    private Button _startButton;

    [Header("Settings")]
    [SerializeField] private float brushSize = 48f;         // 버터 브러시 크기
    [SerializeField] private Texture2D brushTexture;        // 버터 브러시 (원형, 반투명 텍스처)
    [SerializeField] private float moveSmooth = 10f;        // 마우스 따라다니기 속도
    [SerializeField] private float fillThreshold = 0.85f;   // 85% 이상 채우면 성공

    private RenderTexture _maskRT;
    private Material _blendMat;
    private bool isDragging = false;
    public bool isPlaying { get; private set; } = false;
    private float progress = 0f;
    private Material _drawMat;
    private Texture2D butterAlphaMask;

    public Action<string> OnMiniGameEnd;

    public bool isSuccess { get; private set; } = false;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 바인드
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        // 참조 캐싱
        _maskImage = GetImage((int)Images.FryPan_Empty).GetComponentInChildren<RawImage>();
        _butterIcon = GetImage((int)Images.ButterIcon);
        _progressBar = GetImage((int)Images.ProgressBar);
        _startButton = GetButton((int)Buttons.StartButton);

        // 이벤트 등록
        _startButton.onClick.AddListener(StartMiniGame);

        BindEvent(_butterIcon.gameObject, OnButterBeginDrag, Define.UIEvent.BeginDrag);
        BindEvent(_butterIcon.gameObject, OnButterDrag, Define.UIEvent.Drag);
        BindEvent(_butterIcon.gameObject, OnButterEndDrag, Define.UIEvent.EndDrag);

        if (brushTexture == null)
            brushTexture = Texture2D.whiteTexture;

        // 초기 상태
        _progressBar.fillAmount = 0f;
        _butterIcon.gameObject.SetActive(false);

        CreateRenderTexture();
    }


    private void CreateRenderTexture()
    {

        RectTransform rect = _maskImage.rectTransform;
        int width = Mathf.Max(256, (int)rect.rect.width);
        int height = Mathf.Max(256, (int)rect.rect.height);

        _maskRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        _maskRT.Create();

        Shader maskShader = Shader.Find("UI/ButterMask");
        Texture2D butteredPanTex = Resources.Load<Texture2D>("Art/UI/CraftGimmick/Fryingpan/FryPan_Butter");
        _blendMat = new Material(maskShader);
        _blendMat.SetTexture("_MainTex", butteredPanTex);
        _blendMat.SetTexture("_MaskTex", _maskRT);

        // 프라이팬 알파를 CPU용 Texture2D로 복사
        butterAlphaMask = CopyReadableAlpha(butteredPanTex, width, height);

        Shader drawShader = Shader.Find("Sprites/Default");
        _drawMat = new Material(drawShader);
        _drawMat.color = new Color(1, 1, 1, 1);

        _maskImage.material = _blendMat;
        _maskImage.texture = null;

        RenderTexture.active = _maskRT;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;

        Debug.Log($"[RenderTexture Ready] {width}x{height}, Mask ready={butterAlphaMask != null}");
    }

    // GPU 텍스처를 CPU에서 읽을 수 있게 복사
    private Texture2D CopyReadableAlpha(Texture2D source, int width, int height)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(source, tmp);

        Texture2D readable = new Texture2D(width, height, TextureFormat.RGBA32, false);
        RenderTexture.active = tmp;
        readable.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        readable.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tmp);

        return readable;
    }

    private void StartMiniGame()
    {
        if (_maskRT == null)
        {
            Debug.LogWarning("[UI_FryPanButterGame] RenderTexture가 아직 생성되지 않았습니다!");
            return;
        }

        // 버튼 숨기고 버터 활성화
        _startButton.gameObject.SetActive(false);
        _butterIcon.gameObject.SetActive(true);

        // 진행 상태 초기화
        isPlaying = true;
        progress = 0f;
        _progressBar.fillAmount = 0f;

        // 버터칠 초기화
        RenderTexture.active = _maskRT;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;

        Debug.Log($"MaskImage: {_maskImage}, RenderTexture: {_maskRT}, Material: {_blendMat}, Brush: {brushTexture}");
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (isDragging)
        {
            Vector3 targetPos = Input.mousePosition;
            _butterIcon.transform.position = Vector3.Lerp(
                _butterIcon.transform.position,
                targetPos,
                Time.deltaTime * moveSmooth
            );

            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _maskImage.rectTransform, Input.mousePosition, null, out localPos))
            {
                DrawBrush(localPos);
            }
        }

        UpdateProgress();
    }

    private void DrawBrush(Vector2 localPos)
    {
        if (_maskRT == null) return;

        Rect rect = _maskImage.rectTransform.rect;
        float u = Mathf.InverseLerp(rect.xMin, rect.xMax, localPos.x);
        float v = Mathf.InverseLerp(rect.yMin, rect.yMax, localPos.y);
        int x = Mathf.RoundToInt(u * _maskRT.width - brushSize / 2);
        int y = Mathf.RoundToInt(v * _maskRT.height - brushSize / 2);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = _maskRT;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, _maskRT.width, _maskRT.height, 0);

           // 브러시 텍스처를 불투명하게 덮기 (드로잉용 재질 사용)
        Graphics.DrawTexture(
        new Rect(x, _maskRT.height - y - brushSize, brushSize, brushSize),
        brushTexture,
        _drawMat
           );

        GL.PopMatrix();
        RenderTexture.active = prev;
    }

    private void UpdateProgress()
    {
        if (_maskRT == null || butterAlphaMask == null) return;

        // RenderTexture → Texture2D 복사
        Texture2D maskTex = new Texture2D(_maskRT.width, _maskRT.height, TextureFormat.RGBA32, false);
        RenderTexture.active = _maskRT;
        maskTex.ReadPixels(new Rect(0, 0, _maskRT.width, _maskRT.height), 0, 0);
        maskTex.Apply();
        RenderTexture.active = null;

        Color32[] maskPixels = maskTex.GetPixels32();
        Color32[] butterPixels = butterAlphaMask.GetPixels32();

        int validArea = 0;
        int filledArea = 0;

        for (int i = 0; i < maskPixels.Length; i++)
        {
            if (butterPixels[i].a > 50) // 프라이팬 내부만
            {
                validArea++;
                if (maskPixels[i].a > 20)
                    filledArea++;
            }
        }

        progress = validArea > 0 ? (float)filledArea / validArea : 0f;
        _progressBar.fillAmount = Mathf.Lerp(_progressBar.fillAmount, progress, 0.2f);

        if (progress >= fillThreshold)
            EndGame();

        Destroy(maskTex);
    }

    private void OnButterBeginDrag(PointerEventData evt)
    {
        if (!isPlaying) return;
        isDragging = true;
    }

    private void OnButterDrag(PointerEventData evt) { }

    private void OnButterEndDrag(PointerEventData evt)
    {
        isDragging = false;
    }

    private void EndGame()
    {
        Debug.Log("EndGame");
        isSuccess = true;
        if (!isPlaying) return;
        isPlaying = false;

        // 게이지를 100%로 채우는 보정 
        StartCoroutine(FillGaugeToFull());
    }

    private IEnumerator FillGaugeToFull()
    {
        float start = _progressBar.fillAmount;
        float time = 0f;
        float duration = 0.4f; // 게이지가 자연스럽게 차는 속도

        while (time < duration)
        {
            time += Time.deltaTime;
            _progressBar.fillAmount = Mathf.Lerp(start, 1f, time / duration);
            yield return null;
        }

        _progressBar.fillAmount = 1f; // 100%로 고정

        // 게임 종료 이벤트 트리거
       // OnMiniGameEnd?.Invoke("Success");

        // 살짝 텀을 두고 UI 닫기
        //Invoke(nameof(RequestClosePopup), 0.5f);
    }

    private void RequestClosePopup()
    {
        Managers.UI.ClosePopupUI();
    }
}
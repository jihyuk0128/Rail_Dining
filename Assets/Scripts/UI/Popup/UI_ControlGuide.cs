using UnityEngine;
using UnityEngine.UI;

public class UI_ControlGuide : UI_Popup
{
    private enum Images
    {
        GuideImage,
    }

    private enum Buttons
    {
        CloseButton,
        LeftArrow,
        RightArrow,
    }

    [Header("Guide Sprites")]
    [SerializeField] private Sprite[] guideSprites;   // 5장 이미지 등록 (Inspector)

    private Image _guideImage;

    private Button _closeButton;
    private Button _leftButton;
    private Button _rightButton;

    private int currentIndex = 0;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        _guideImage = GetImage((int)Images.GuideImage);

        _closeButton = GetButton((int)Buttons.CloseButton);
        _leftButton = GetButton((int)Buttons.LeftArrow);
        _rightButton = GetButton((int)Buttons.RightArrow);

        // 이벤트 등록
        _closeButton.onClick.AddListener(ClosePopupUI);
        _leftButton.onClick.AddListener(OnLeftClicked);
        _rightButton.onClick.AddListener(OnRightClicked);

        // 첫 이미지 표시
        ShowImage(0);
    }

    private void Update()
    {
        // ESC 로 닫기
        if (Input.GetKeyDown(KeyCode.Escape))
            ClosePopupUI();
    }

    private void ShowImage(int index)
    {
        if (guideSprites == null || guideSprites.Length == 0)
        {
            Debug.LogError("[UI_ControlGuide] Guide Sprites not assigned!");
            return;
        }

        currentIndex = Mathf.Clamp(index, 0, guideSprites.Length - 1);
        _guideImage.sprite = guideSprites[currentIndex];

        // 양 끝일 때 버튼 비활성화
        _leftButton.interactable = currentIndex > 0;
        _rightButton.interactable = currentIndex < guideSprites.Length - 1;
    }

    private void OnLeftClicked()
    {
        ShowImage(currentIndex - 1);
    }

    private void OnRightClicked()
    {
        ShowImage(currentIndex + 1);
    }
}

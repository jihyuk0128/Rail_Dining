using UnityEngine;
using UnityEngine.UI;

public class UI_CountDown : UI_Popup
{
    private enum Images
    {
        NumberImage,
    }

    private Image _numberImage;

    [Header("Sprites")]
    public Sprite sprite3;
    public Sprite sprite2;
    public Sprite sprite1;
    public Sprite spriteStart;
    public Sprite spriteStop;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        _numberImage = GetImage((int)Images.NumberImage);
    }

    /// <summary>
    /// 외부에서 호출하는 카운트다운 출력 함수
    /// </summary>
    public void ShowCount(int num)
    {
        switch (num)
        {
            case 3:
                _numberImage.sprite = sprite3;
                break;
            case 2:
                _numberImage.sprite = sprite2;
                break;
            case 1:
                _numberImage.sprite = sprite1;
                break;
            case 0:
                _numberImage.sprite = spriteStart;
                break;
            case -1:
                _numberImage.sprite = spriteStop;
                break;
        }

        // START가 표시되면 1초 후 자동으로 닫기
        if (num == 0 || num == -1)
            Invoke(nameof(ClosePopup), 1f);
    }

    private void ClosePopup()
    {
        Managers.UI.ClosePopupUI();
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spine.Unity;

public class UI_GameResult : UI_Popup
{
    private enum Images
    {
        Win,
        Lose,
    }
    
    private enum Buttons
    {
        RestartButton,
        TitleButton,
    }

    private enum SpineGraphics
    {
        WinnerSpine,
        LoserSpine,
    }

    private Image _winImage;
    private Image _loseImage;
    private Button _restartButton;
    private Button _titleButton;
    private SkeletonGraphic _winnerSpine;
    private SkeletonGraphic _loserSpine;

    [Header("Sprites")]
    [SerializeField] private Sprite winSprite;  // 승리 이미지
    [SerializeField] private Sprite loseSprite; // 패배 이미지

    public Action OnRetryClicked;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<SkeletonGraphic>(typeof(SpineGraphics));

        _winImage = GetImage((int)Images.Win);
        _loseImage = GetImage((int)Images.Lose);
        _restartButton = GetButton((int)Buttons.RestartButton);
        _titleButton = GetButton((int)Buttons.TitleButton);
        _winnerSpine = Get<SkeletonGraphic>((int)SpineGraphics.WinnerSpine);
        _loserSpine = Get<SkeletonGraphic>((int)SpineGraphics.LoserSpine);

        _restartButton.onClick.AddListener(OnClickRestart);
        _titleButton.onClick.AddListener(OnClickTitle);

        // 기본적으로 숨겨둠
        _winImage.gameObject.SetActive(false);
        _loseImage.gameObject.SetActive(false);

      
    }

    public void ShowResult(bool isWin)
    {
        if (isWin)
        {
            // 승리한 플레이어 모션
            _winnerSpine.AnimationState.SetAnimation(0, "win", true);
            _loserSpine.AnimationState.SetAnimation(0, "lose", true);

            // 승리/패배 이미지 표시
            _winImage.sprite = winSprite;
            _loseImage.sprite = loseSprite;
        }
        else
        {
            // 반대로 설정 (상대가 승리)
            _winnerSpine.AnimationState.SetAnimation(0, "lose", true);
            _loserSpine.AnimationState.SetAnimation(0, "win", true);

            _winImage.sprite = loseSprite;
            _loseImage.sprite = winSprite;
        }
        
        _winImage.gameObject.SetActive(true);
        _loseImage.gameObject.SetActive(true);

        SoundManager.Instance.PlaySFX("WorkStart_SFX");
    }

    private void OnClickRestart()
    {
        Debug.Log("[UI_GameResult] Restart Clicked");
        OnRetryClicked?.Invoke(); // 외부에서 재시작 로직 처리
        Managers.UI.ClosePopupUI();
    }

    private void OnClickTitle()
    {
        Debug.Log("[UI_GameResult] Title Clicked");
        SoundManager.Instance.StopBGM();
        Managers.UI.ClosePopupUI();
        SceneManager.LoadScene("CinematicScene");
        Managers.Network.LeaveRoom();
    }
}
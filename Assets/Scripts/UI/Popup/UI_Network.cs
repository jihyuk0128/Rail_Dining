using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Network : UI_Popup
{
    [Header("Player Images")]
    [SerializeField] private Image player1Image;
    [SerializeField] private Image player2Image;

    [Header("Character Sprites")]
    [SerializeField] private Sprite maleSprite;
    [SerializeField] private Sprite femaleSprite;

    enum Buttons
    {
        TutorialButton,
        Player1Ready,
        Player2Ready,
        ExitButton
    }

    enum TextMeshProUGUIS
    {
        Player1Name,
        Player2Name,
    }

    enum Images
    {
        Player1Img, 
        Player2Img,
    }


    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIS));
        Bind<Image>(typeof(Images));

        // 초기 이미지 설정 (예: Player1 = 남, Player2 = 여)
        //player1Image.sprite = maleSprite;
        //player2Image.sprite = femaleSprite;

        GetButton((int)Buttons.TutorialButton).gameObject.BindEvent(OnStartTutorial);
        GetButton((int)Buttons.Player1Ready).gameObject.BindEvent(OnStartGame);
        GetButton((int)Buttons.Player2Ready).gameObject.BindEvent(OnStartGame);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClose);


        // 네트워크 이벤트 등록
        Managers.Network.OnGameStart += OnStartGameNetwork;
    }

    private void OnStartTutorial(PointerEventData data)
    {

    }
    // 업무시작버튼
    private void OnStartGame(PointerEventData data)
    {
        Managers.Network.StartGame();
    }

    private void OnStartGameNetwork()
    {
        Managers.MainThread.Enqueue(() =>
        {
            SceneManager.LoadScene("TestSceneNew");
        });
    }
    void OnClose(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }


    private void OnDestroy()
    {
        Managers.Network.LeaveRoom();
        Managers.Network.OnGameStart -= OnStartGameNetwork;
    }
}

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

    private string _pendingPlayer1Name;
    private string _pendingPlayer2Name;
    private bool _isInitialized = false;

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
        Managers.Network.OnRoomJoinEvent += OnRoomJoinEvent;

        _isInitialized = true;

        // Init 전에 InitName이 호출되었다면 여기서 UI에 적용
        if (!string.IsNullOrEmpty(_pendingPlayer1Name) ||
            !string.IsNullOrEmpty(_pendingPlayer2Name))
        {
            ApplyNames(_pendingPlayer1Name, _pendingPlayer2Name);
        }
    }

    private void ApplyNames(string player1name, string player2name)
    {
        var p1 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player1Name);
        var p2 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player2Name);

        if (p1 != null)
            p1.text = player1name;

        if (p2 != null)
            p2.text = player2name;
    }

    public void InitName(string player1name, string player2name)
    {
        // 값 저장
        _pendingPlayer1Name = player1name;
        _pendingPlayer2Name = player2name;

        // 이미 Init이 끝난 상태면 바로 UI 반영
        if (_isInitialized)
        {
            ApplyNames(player1name, player2name);
        }
    }

    private void OnStartTutorial(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_ControlGuide>();
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
        Managers.Network.OnRoomJoinEvent -= OnRoomJoinEvent;
    }

    private void OnRoomJoinEvent(string name)
    {
        Managers.MainThread.Enqueue(() =>
        {
            GetTextMeshProUGUI((int)TextMeshProUGUIS.Player2Name);
        });
    }
}

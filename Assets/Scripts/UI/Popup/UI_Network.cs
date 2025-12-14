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
        RoomCode,
    }

    enum Images
    {
        Player1Img, 
        Player2Img,
        Player1Check, 
        Player2Check
    }

    private string _pendingPlayer1Name;
    private string _pendingPlayer2Name;
    private bool _isInitialized = false;

    private bool _player1Ready = false;
    private bool myready = false;

    private int _roomnumber;

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
        //Managers.Network.OnHostAssigned += OnHostAssignedEvent;
        Managers.Network.OnReadyUpdate += OnReadyEvent;
        Managers.Network.OnPlayerLeft += OnPlayerLeft;

        _isInitialized = true;
        myready = false;

        // Init 전에 InitName이 호출되었다면 여기서 UI에 적용
        if (!string.IsNullOrEmpty(_pendingPlayer1Name) ||
            !string.IsNullOrEmpty(_pendingPlayer2Name))
        {
            ApplyNames(_pendingPlayer1Name, _pendingPlayer2Name,_player1Ready);
        }

        _roomnumber = Managers.Network.roomData.RoomId;
        GetTextMeshProUGUI((int)TextMeshProUGUIS.RoomCode).text = "서버코드: " + _roomnumber.ToString();
    }

    private void ApplyNames(string player1name, string player2name, bool player1Ready)
    {
        var p1 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player1Name);
        var p2 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player2Name);

        if (p1 != null)
            p1.text = player1name;

        if (p2 != null)
            p2.text = player2name;

        var btnP1 = GetButton((int)Buttons.Player1Ready).gameObject;
        var btnP2 = GetButton((int)Buttons.Player2Ready).gameObject;

        Debug.LogWarning($"나호스트야? {Managers.Network.player.IsHost}");

        if (Managers.Network.player.IsHost == true)
        {
            btnP1.SetActive(true);
            btnP2.SetActive(false);
        }
        else
        {
            btnP1.SetActive(false);
            btnP2.SetActive(true);
        }

        Debug.LogWarning($"1번 레드 됫어?  {player1Ready}");
        var check1 = GetImage((int)Images.Player1Check);

        if (player1Ready == true)
            check1.gameObject.SetActive(true);
        else check1.gameObject.SetActive(false);

        Debug.LogWarning($"{_pendingPlayer1Name},{_pendingPlayer2Name}");
    }

    public void InitName(string player1name, string player2name , bool player1ready)
    {
        // 값 저장

        _player1Ready = player1ready;
        _pendingPlayer1Name = player1name; 
        _pendingPlayer2Name = player2name;


        // 이미 Init이 끝난 상태면 바로 UI 반영
        if (_isInitialized)
        {
            ApplyNames(player1name, player2name, player1ready);
        }
    }

    private void OnStartTutorial(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_ControlGuide>();
    }

    private void OnStartGame(PointerEventData data)
    {
        myready = true;
        Managers.Network.ReadyGame();
    }

    private void OnStartGameNetwork()
    {
        Managers.MainThread.Enqueue(() =>
        {
            Managers.UI.ClosePopupUI();
            Managers.UI.PopupInit();
            SceneManager.LoadScene("TestSceneNew");
        });
    }
    void OnClose(PointerEventData data)
    {
        Managers.Network.LeaveRoom();
        Managers.UI.ClosePopupUI();
    }


    private void OnDestroy()
    {
        Managers.Network.OnGameStart -= OnStartGameNetwork;
        Managers.Network.OnRoomJoinEvent -= OnRoomJoinEvent;
        //Managers.Network.OnHostAssigned -= OnHostAssignedEvent;
        Managers.Network.OnReadyUpdate -= OnReadyEvent;
    }

    private void OnRoomJoinEvent(string name)
    {
        Managers.MainThread.Enqueue(() =>
        {
            var p2 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player2Name);

            if (p2 != null)
                p2.text = name;

            Debug.LogWarning($"{_pendingPlayer1Name},{_pendingPlayer2Name}");
        });
    }

    //private void OnHostAssignedEvent(string msg)
    //{
    //    Managers.MainThread.Enqueue(() =>
    //    {
    //        _pendingPlayer1Name = _pendingPlayer2Name;
    //        _pendingPlayer2Name = "";
    //
    //        ApplyNames(_pendingPlayer1Name, _pendingPlayer2Name);
    //    });
    //}
    private void OnPlayerLeft(string name)
    {
        Managers.MainThread.Enqueue(() =>
        {
            var p1 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player1Name);
            var p2 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player2Name);

            var check2 = GetImage((int)Images.Player2Check);

            if (p1 != null && p1.text == name)
            {
                _pendingPlayer1Name = _pendingPlayer2Name;
                _pendingPlayer2Name = "";
                _player1Ready = myready;
                ApplyNames(_pendingPlayer1Name, _pendingPlayer2Name , _player1Ready);
                check2.gameObject.SetActive(false);
            }
            else if (p2 != null && p2.text == name) 
            {
                _pendingPlayer2Name = "";
                _player1Ready = myready;
                ApplyNames(_pendingPlayer1Name, _pendingPlayer2Name, _player1Ready);
                check2.gameObject.SetActive(false);
            }
        });
    }

    private void OnReadyEvent(string name)
    {
        Managers.MainThread.Enqueue(() =>
        {
            var p1 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player1Name);
            var p2 = GetTextMeshProUGUI((int)TextMeshProUGUIS.Player2Name);

            var check1 = GetImage((int)Images.Player1Check);
            var check2 = GetImage((int)Images.Player2Check);

            if (p1 != null && p1.text == name)
            {
                check1.gameObject.SetActive(true);
            }
            else if (p2 != null && p2.text == name)
            {
                check2.gameObject.SetActive(true);
            }
        });
    }

}

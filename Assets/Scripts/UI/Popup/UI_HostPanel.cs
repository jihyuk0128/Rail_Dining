using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_HostPanel: UI_Popup
{
    enum Buttons
    {
        ExitButton
    }

    enum TextMeshProUGUIS
    {
        RoomCode,
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

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClose);
        Managers.Network.CreateRoom();

        Managers.Network.OnRoomCreate += OnHostNetwork;
        Managers.Network.OnHostAssigned += OnHostAssigned;
    }

    private void OnHostNetwork(int roomid)
    {
        // 생성한방 입장하는코드.
        Managers.Network.JoinRoom(roomid);
    }

    private void OnHostAssigned(string msg)
    {
        // 생성한방 입장성공.
        Managers.MainThread.Enqueue(() =>
        {
            Managers.UI.ClosePopupUI();
            var popup = Managers.UI.ShowPopupUI<UI_Network>();
            popup.InitName(Managers.Network.player.Username, " ");
        });
    }

    void OnSubmitCode(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("코드를 입력하세요!");
            return;
        }

        Debug.Log($"입력된 코드: {text}");

        // 서버 방생성요청(추후코드번호 설정할수있게끔할예정)
        Managers.Network.CreateRoom();
    }

    void OnClose(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }

    private void OnDestroy()
    {
        Managers.Network.OnRoomCreate -= OnHostNetwork;
        Managers.Network.OnHostAssigned -= OnHostAssigned;
    }
}

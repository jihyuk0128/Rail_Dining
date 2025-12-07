using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_HostPanel: UI_Popup
{
    enum Buttons
    {
    }

    enum TextMeshProUGUIS
    {
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
            popup.InitName(Managers.Network.player.Username, " ", false);
        });
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

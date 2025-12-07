using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GuestPanel : UI_Popup
{
    enum Buttons { ExitButton, CheckButton }
    enum InputFields { RoomCodeInput }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClose);
        GetButton((int)Buttons.CheckButton).gameObject.BindEvent(OnCheck);

        TMP_InputField input = Get<TMP_InputField>((int)InputFields.RoomCodeInput);
        input.onSubmit.AddListener(OnSubmitCode);

        Managers.Network.OnRoomList += OnRoomlist;
    }

    private void TryJoinRoom()
    {
        TMP_InputField input = Get<TMP_InputField>((int)InputFields.RoomCodeInput);
        string text = input.text;

        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("코드를 입력하세요!");
            return;
        }

        Managers.Network.JoinRoom(int.Parse(text));
    }

    private void OnSubmitCode(string text)
    {
        TryJoinRoom();
    }

    void OnCheck(PointerEventData data)
    {
        TryJoinRoom();
    }

    void OnClose(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }

    private void OnRoomlist(string name , bool ready)
    {
        Managers.MainThread.Enqueue(() =>
        {
            Managers.UI.ClosePopupUI();

            Debug.LogWarning($"ready : {ready}");

            var popup = Managers.UI.ShowPopupUI<UI_Network>();
            popup.InitName(name,Managers.Network.player.Username, ready);
        });
    }

    private void OnDestroy()
    {
        Managers.Network.OnRoomList -= OnRoomlist;
    }

}
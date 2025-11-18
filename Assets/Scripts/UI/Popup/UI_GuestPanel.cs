using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_GuestPanel : UI_Popup
{
    enum Buttons
    {
        ExitButton
    }

    enum InputFields
    {
        RoomCodeInput,
    }

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
        TMP_InputField inputCode = Get<TMP_InputField>((int)InputFields.RoomCodeInput);
        inputCode.onSubmit.AddListener(OnSubmitCode);

        Managers.Network.OnRoomJoin += OnRoomJoin;
    }

    private void OnRoomJoin(int roomid)
    {
        // 방 입장성공.
        Managers.MainThread.Enqueue(() =>
        {
            Managers.UI.ClosePopupUI();
            Managers.UI.ShowPopupUI<UI_Network>();
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

        // 서버 방참가요청(추후이름설정예정)
        Managers.Network.JoinRoom(int.Parse(text));
    }

    void OnClose(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }
}

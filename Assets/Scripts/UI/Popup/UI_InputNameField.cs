using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InputNameField : UI_Popup
{
    private string Name = null;
    private bool Isnewgame = true;

    enum Buttons
    {
        ExitButton,
        CheckButton
    }

    enum InputFields
    {
        LoginInput,
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
        GetButton((int)Buttons.CheckButton).gameObject.BindEvent(OnCheck);

        TMP_InputField inputName = Get<TMP_InputField>((int)InputFields.LoginInput);
        inputName.onSubmit.AddListener(OnSubmitName);

        Managers.Network.OnLoginSuccess += OnLoginNetwork;
    }

    public void IsNewGame(bool game)
    {
        Isnewgame = game;
    }

    /// <summary>
    /// Enter / Check 버튼 둘 다 여기로 오게 만드는 공통 로그인 로직
    /// </summary>
    private void TryLogin()
    {
        TMP_InputField inputName = Get<TMP_InputField>((int)InputFields.LoginInput);
        string text = inputName.text;

        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("이름을 입력하세요!");
            return;
        }

        Debug.Log($"입력된 이름: {text}");

        // 서버 로그인 요청
        Managers.Network.Login(text);
    }

    void OnSubmitName(string text)
    {
        // Enter 눌렀을 때 실행
        TryLogin();
    }

    void OnCheck(PointerEventData data)
    {
        // Check 버튼 눌렀을 때 실행
        TryLogin();
    }

    private void OnLoginNetwork(string username)
    {
        // 메인 스레드에서 실행되도록 던지기
        Managers.MainThread.Enqueue(() =>
        {
            Managers.UI.ClosePopupUI();
            Name = username;

            if (Isnewgame)
                Managers.UI.ShowPopupUI<UI_HostPanel>();
            else
                Managers.UI.ShowPopupUI<UI_GuestPanel>();
        });
    }

    void OnClose(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }
}
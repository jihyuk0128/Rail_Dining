using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_TitleScene : UI_Scene
{
    enum Buttons
    {
        NewGame,
        Continue,
        SettingButton,
        ExitButton,
    }

    void Start()
    {
        Init();
        SoundManager.Instance.StopAllBGM();
        SoundManager.Instance?.PlayBGM("BackGround_BGM"); // bgm 재생
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.NewGame).gameObject.BindEvent(OnStartGame);
        GetButton((int)Buttons.Continue).gameObject.BindEvent(OnButtonContinue);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(OnButtonSettingButton);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnQuitGame);
    }

    public void OnStartGame(PointerEventData data)
    {
        if (Managers.Network.IsLoggedIn)
        {
            Debug.Log("[Network] 이미 로그인되어 있어 Login 요청을 무시합니다.");

            Managers.UI.ShowPopupUI<UI_HostPanel>();

            return;
        }

        var popup = Managers.UI.ShowPopupUI<UI_InputNameField>();
        popup.IsNewGame(true);
    }

    public void OnButtonContinue(PointerEventData data)
    {
        if (Managers.Network.IsLoggedIn)
        {
            Debug.Log("[Network] 이미 로그인되어 있어 Login 요청을 무시합니다.");

            Managers.UI.ShowPopupUI<UI_GuestPanel>();

            return;
        }

        var popup = Managers.UI.ShowPopupUI<UI_InputNameField>();
        popup.IsNewGame(false);
    }

    public void OnButtonSettingButton(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_SoundSetting>();
    }


    public void OnQuitGame(PointerEventData data)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
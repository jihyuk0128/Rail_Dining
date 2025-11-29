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
        SoundManager.Instance?.PlayBGM("BackGround_BGM"); // bgm Àç»ý
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
        var popup = Managers.UI.ShowPopupUI<UI_InputNameField>();
        popup.IsNewGame(true);
    }

    public void OnButtonContinue(PointerEventData data)
    {
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
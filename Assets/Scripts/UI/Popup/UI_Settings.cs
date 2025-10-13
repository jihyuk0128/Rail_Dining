using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UI_Settings : UI_Popup
{
    public enum Buttons
    {
        CloseButton,
        SoundButton,
        RestartButton,
        MainButton,
        SoundBackButton
    }

    public enum Sliders
    {
        BGMSlider,
        SFXSlider
    }

    private GameObject settingPanel;
    private GameObject soundSettingPanel;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();   
        // 버튼 & 슬라이더 바인드
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));

        // 패널 참조
        settingPanel = transform.Find("SettingPanel")?.gameObject;
        soundSettingPanel = transform.Find("SoundSettingPanel")?.gameObject;

        // 기본 활성 상태 설정
        if (settingPanel != null) settingPanel.SetActive(true);
        if (soundSettingPanel != null) soundSettingPanel.SetActive(false);

        // 버튼 리스너 등록
        GetButton((int)Buttons.SoundButton).onClick.AddListener(OnSound);
        GetButton((int)Buttons.RestartButton).onClick.AddListener(OnRestart);
        GetButton((int)Buttons.MainButton).onClick.AddListener(OnMain);
        GetButton((int)Buttons.CloseButton).onClick.AddListener(OnClose);
        GetButton((int)Buttons.SoundBackButton).onClick.AddListener(BackToSettings);

        // 슬라이더 리스너 등록
        GetSlider((int)Sliders.BGMSlider).onValueChanged.AddListener(OnBGMVolumeChanged);
        GetSlider((int)Sliders.SFXSlider).onValueChanged.AddListener(OnSFXVolumeChanged);

        // 초기 슬라이더값 로드
        GetSlider((int)Sliders.BGMSlider).value = SoundManager.Instance.GetBGMVolume();
        GetSlider((int)Sliders.SFXSlider).value = SoundManager.Instance.GetSFXVolume();
    }

    // ===== 버튼 이벤트 =====
    private void OnSound()
    {
        settingPanel?.SetActive(false);
        soundSettingPanel?.SetActive(true);
    }

    private void OnRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    private void OnClose()
    {
        Managers.UI.ClosePopupUI();
        Time.timeScale = 1f;
    }

    public void BackToSettings()
    {
        settingPanel?.SetActive(true);
        soundSettingPanel?.SetActive(false);
    }

    // ===== 슬라이더 이벤트 =====
    private void OnBGMVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBGMVolume(value);

    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(value);
    }
}
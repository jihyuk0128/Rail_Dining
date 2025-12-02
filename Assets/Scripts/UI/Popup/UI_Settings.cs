using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;

public class UI_Settings : UI_Popup
{
    public enum Buttons
    {
        CloseButton,
        SoundButton,
        //RestartButton,
        MainButton,
        SoundSettingClose
    }

    public enum Sliders
    {
        BGMSlider,
        SFXSlider
    }

    public enum Objects
    {
        BGMOn,
        BGMOff,
        SFXOn,
        SFXOff
    }

    private GameObject settingPanel;
    private GameObject soundSettingPanel;

    private GameObject BGMOn;
    private GameObject BGMOff;
    private GameObject SFXOn;
    private GameObject SFXOff;

    private bool isBGM = true;
    private bool isSFX = true;
    private bool isAnimating = false;

    [SerializeField] private float bounceHeight = 20f;  // 튕김 높이
    [SerializeField] private float bounceDuration = 0.3f; // 튕김 시간

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
        Bind<GameObject>(typeof(Objects));

        // 패널 참조
        settingPanel = transform.Find("SettingPanel")?.gameObject;
        soundSettingPanel = transform.Find("SoundSettingPanel")?.gameObject;

        // 기본 활성 상태 설정
        if (settingPanel != null) settingPanel.SetActive(true);
        if (soundSettingPanel != null) soundSettingPanel.SetActive(false);

        // 버튼 리스너 등록
        GetButton((int)Buttons.SoundButton).onClick.AddListener(OnSound);
        //GetButton((int)Buttons.RestartButton).onClick.AddListener(OnRestart);
        GetButton((int)Buttons.MainButton).onClick.AddListener(OnMain);
        GetButton((int)Buttons.CloseButton).onClick.AddListener(OnClose);
        GetButton((int)Buttons.SoundSettingClose).onClick.AddListener(BackToSettings);

        // 슬라이더 리스너 등록
        GetSlider((int)Sliders.BGMSlider).onValueChanged.AddListener(OnBGMVolumeChanged);
        GetSlider((int)Sliders.SFXSlider).onValueChanged.AddListener(OnSFXVolumeChanged);

        // 초기 슬라이더값 로드
        GetSlider((int)Sliders.BGMSlider).value = SoundManager.Instance.GetBGMVolume();
        GetSlider((int)Sliders.SFXSlider).value = SoundManager.Instance.GetSFXVolume();

        // 이미지
        BGMOn = GetObject((int)Objects.BGMOn);
        BGMOff = GetObject((int)Objects.BGMOff);
        SFXOn = GetObject((int)Objects.SFXOn);
        SFXOff = GetObject((int)Objects.SFXOff);
    }

    // ===== 버튼 이벤트 =====
    private void OnSound()
    {
        settingPanel?.SetActive(false);
        soundSettingPanel?.SetActive(true);
    }

    private void OnRestart()
    {
        Managers.UI.ClosePopupUI();
        Time.timeScale = 1f;
        GameManager.Instance.RestartGame();
    }

    private void OnMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("NewTitleScene");
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
        if (isAnimating) return;
        if (isBGM && value <= 0.001f)
        {
            isBGM = false;
            StartCoroutine(AnimateIcon(BGMOff));
        }
        else if(!isBGM && value > 0.001f)
        {
            isBGM = true;
            StartCoroutine(AnimateIcon(BGMOn));
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(value);
        if (isAnimating) return;
        if (isSFX && value <= 0.001f)
        {
            isSFX = false;
            StartCoroutine(AnimateIcon(SFXOff));
        }
        else if (!isSFX && value > 0.001f)
        {
            isSFX = true;
            StartCoroutine(AnimateIcon(SFXOn));
        }
    }

    private IEnumerator AnimateIcon(GameObject fromIcon)
    {
        isAnimating = true;
        RectTransform fromRect = fromIcon.GetComponent<RectTransform>();

        Vector2 startPos = fromRect.anchoredPosition;
        Vector2 upPos = startPos + Vector2.up * bounceHeight;

        float t = 0f;

        // 위로 이동
        while (t < bounceDuration / 2f)
        {
            t += Time.deltaTime;
            float progress = t / (bounceDuration / 2f);
            fromRect.anchoredPosition = Vector2.Lerp(startPos, upPos, Mathf.SmoothStep(0, 1, progress));
            yield return null;
        }

        // 아래로 복귀
        t = 0f;
        while (t < bounceDuration / 2f)
        {
            t += Time.deltaTime;
            float progress = t / (bounceDuration / 2f);
            fromRect.anchoredPosition = Vector2.Lerp(upPos, startPos, Mathf.SmoothStep(0, 1, progress));
            yield return null;
        }

        fromRect.anchoredPosition = startPos;
        isAnimating = false;
    }
}
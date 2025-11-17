using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_SoundSetting : UI_Popup
{
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

    public enum Buttons
    {
        SoundSettingClose
    }

    private Slider bgmSlider;
    private Slider sfxSlider;

    private GameObject BGMOn;
    private GameObject BGMOff;
    private GameObject SFXOn;
    private GameObject SFXOff;

    private bool isBGM = true;
    private bool isSFX = true;
    private bool isAnimating = false;

    [SerializeField] private float bounceHeight = 20f;
    [SerializeField] private float bounceDuration = 0.3f;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Slider>(typeof(Sliders));
        Bind<GameObject>(typeof(Objects));
        Bind<Button>(typeof(Buttons));

        // 슬라이더 가져오기
        bgmSlider = GetSlider((int)Sliders.BGMSlider);
        sfxSlider = GetSlider((int)Sliders.SFXSlider);

        // 아이콘
        BGMOn = GetObject((int)Objects.BGMOn);
        BGMOff = GetObject((int)Objects.BGMOff);
        SFXOn = GetObject((int)Objects.SFXOn);
        SFXOff = GetObject((int)Objects.SFXOff);

        // 슬라이더 초기값 세팅
        bgmSlider.value = SoundManager.Instance.GetBGMVolume();
        sfxSlider.value = SoundManager.Instance.GetSFXVolume();

        // 리스너 연결
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // 닫기 버튼
        GetButton((int)Buttons.SoundSettingClose).onClick.AddListener(() =>
        {
            Managers.UI.ClosePopupUI();
        });
    }


    // ===== Volume Sliders =====
    private void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);

        if (isAnimating) return;

        if (isBGM && value <= 0.001f)
        {
            isBGM = false;
            StartCoroutine(AnimateIcon(BGMOff));
        }
        else if (!isBGM && value > 0.001f)
        {
            isBGM = true;
            StartCoroutine(AnimateIcon(BGMOn));
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
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

    // ===== Icon Animation =====
    private IEnumerator AnimateIcon(GameObject icon)
    {
        isAnimating = true;

        RectTransform rect = icon.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 upPos = startPos + Vector2.up * bounceHeight;

        float t = 0f;

        // 위로 이동
        while (t < bounceDuration / 2f)
        {
            t += Time.deltaTime;
            rect.anchoredPosition =
                Vector2.Lerp(startPos, upPos, Mathf.SmoothStep(0, 1, t / (bounceDuration / 2f)));
            yield return null;
        }

        // 내려오기
        t = 0;
        while (t < bounceDuration / 2f)
        {
            t += Time.deltaTime;
            rect.anchoredPosition =
                Vector2.Lerp(upPos, startPos, Mathf.SmoothStep(0, 1, t / (bounceDuration / 2f)));
            yield return null;
        }

        rect.anchoredPosition = startPos;
        isAnimating = false;
    }
}
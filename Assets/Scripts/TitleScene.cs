using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TitleScene : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject NewGamePanel;
    [SerializeField] private GameObject ContinuePanel;
    [SerializeField] private GameObject InvitePanel;

    [Header("Audio")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    public TMP_InputField inviteInputField;

    private void Start()
    {
        // 입력창 이벤트 등록
        inviteInputField.onSubmit.AddListener(OnCodeSubmitted);

        // 사운드 슬라이더 이벤트 등록
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // 현재 사운드 매니저의 볼륨값 반영
        bgmSlider.value = SoundManager.Instance.GetBGMVolume();
        sfxSlider.value = SoundManager.Instance.GetSFXVolume();

        SoundManager.Instance?.PlayBGM("BackGround_BGM");
    }

    private void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    // UI 버튼 이벤트들
    public void OnStartGame() => NewGamePanel.SetActive(true);
    public void OnContinue() => ContinuePanel.SetActive(true);
    public void OnOpenSettings() => settingsPanel.SetActive(true);

    public void OnHost() => SceneManager.LoadScene("NetworkScene");

    public void OnGuest()
    {
        NewGamePanel.SetActive(false);
        ContinuePanel.SetActive(false);
        InvitePanel.SetActive(true);
    }

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClose()
    {
        settingsPanel.SetActive(false);
    }

    private void ClosePanel()
    {
        NewGamePanel.SetActive(false);
        ContinuePanel.SetActive(false);
        settingsPanel.SetActive(false);
        InvitePanel.SetActive(false);
    }

    // 초대코드 입력 처리
    private void OnCodeSubmitted(string code)
    {
        Debug.Log($"입력된 초대 코드: {code}");
        ProcessInviteCode(code);
        inviteInputField.text = "";
    }

    private void ProcessInviteCode(string code)
    {
        if (code.Length == 8)
        {
            Debug.Log("방 참가 시도");
            // SceneManager.LoadScene("NetworkScene");
        }
        else
        {
            Debug.LogWarning("잘못된 초대 코드 형식입니다!");
        }
    }


    private void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }
}
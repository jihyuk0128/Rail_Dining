using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class TitleScene : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject NewGamePanel;
    [SerializeField] private GameObject ContinuePanel;
    [SerializeField] private GameObject InvitePanel;
    [SerializeField] private GameObject loginPanel;

    [Header("Audio")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject BGMOn;
    [SerializeField] private GameObject BGMOff;
    [SerializeField] private GameObject SFXOn;
    [SerializeField] private GameObject SFXOff;
    [SerializeField] private Button SoundSettingClose;

    [Header("Input")]
    public TMP_InputField inviteInputField;
    public TMP_InputField LoginInputField;

    private bool isBGM = true;
    private bool isSFX = true;
    private bool isAnimating = false;

    [SerializeField] private float bounceHeight = 20f;  // 튕김 높이
    [SerializeField] private float bounceDuration = 0.3f; // 튕김 시간

    public string playerName = null;
    private bool isNewGame = true;

    private void Start()
    {
        // 입력창 이벤트 등록
        inviteInputField.onSubmit.AddListener(OnCodeSubmitted);
        LoginInputField.onSubmit.AddListener(OnLoginSubmitted);

        // 사운드 슬라이더 이벤트 등록
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // 현재 사운드 매니저의 볼륨값 반영
        bgmSlider.value = SoundManager.Instance.GetBGMVolume();
        sfxSlider.value = SoundManager.Instance.GetSFXVolume();

        SoundManager.Instance?.PlayBGM("BackGround_BGM");

        // 네트워크용 이벤트 키 받아오기
        Managers.Network.OnLoginSuccess += OnLoginNetwork;
        Managers.Network.OnRoomCreate += OnHostNetwork;
        Managers.Network.OnRoomJoin += OnRoomJoin;

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
    public void OnStartGame()
    {
        loginPanel.SetActive(true);
        isNewGame = true;
    }
    public void OnContinue()
    {
        loginPanel.SetActive(true);
        isNewGame = false;
    }
    public void OnOpenSettings() => settingsPanel.SetActive(true);

    public void OnHost() { Managers.Network.CreateRoom(); }
    
    private void OnHostNetwork(int roomid) 
    { 
        Managers.Network.JoinRoom(roomid);
    }

    private void OnRoomJoin(int roomid) 
    {
        Managers.MainThread.Enqueue(() =>
        {
            SceneManager.LoadScene("NetworkScene");
        });
    }
    
    public void OnGuest()
    {
        // 방참가. 
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
        loginPanel.SetActive(false);
    }

    public void CloseSoundPanel()
    {
        settingsPanel.SetActive(false);
    }

    // 초대코드 입력 처리 
    private void OnCodeSubmitted(string code)
    {
        Debug.Log($"입력된 초대 코드: {code}");
        Managers.Network.JoinRoom(int.Parse(code));
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

    // 로그인 이름 입력받으면 호출되는 함수
    private void OnLoginSubmitted(string code)
    {
        playerName = code;
        Managers.Network.Login(code);
        LoginInputField.text = "";
    }

    // 로그인 되었는지 확인하는함수 되었다면 다음화면진행
    private void OnLoginNetwork(string username)
    {
        // 메인 스레드에서 실행되도록 던지기
        Managers.MainThread.Enqueue(() =>
        {
            playerName = username;
            loginPanel.SetActive(false);

            if (isNewGame)
                NewGamePanel.SetActive(true);
            else
                ContinuePanel.SetActive(true);
        });
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
        else if (!isBGM && value > 0.001f)
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
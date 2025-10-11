using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject NewGamePanel;
    [SerializeField] private GameObject ContinuePanel;
    [SerializeField] private GameObject InvitePanel;
    public TMP_InputField inviteInputField;

    private void Start()
    {
        // 입력창 이벤트 등록
        inviteInputField.onSubmit.AddListener(OnCodeSubmitted);
    }

    private void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    // 게임 시작 버튼
    public void OnStartGame()
    {
        NewGamePanel.SetActive(true);
    }

    // 컨티뉴 버튼
    public void OnContinue()
    {
        ContinuePanel.SetActive(true);
    }

    // 설정 열기 버튼
    public void OnOpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    // 호스트 버튼
    public void OnHost()
    {
        SceneManager.LoadScene("NetworkScene");
    }
    // 게스트 버튼
    public void OnGuest()
    {
        NewGamePanel.SetActive(false);
        ContinuePanel.SetActive(false);
        InvitePanel.SetActive(true);
    }
    // 게임 종료 버튼
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

    private void OnCodeSubmitted(string code)
    {
        Debug.Log($"입력된 초대 코드: {code}");

        // 임시 처리
        ProcessInviteCode(code);

        // 입력창 초기화
        inviteInputField.text = "";
    }

    private void ProcessInviteCode(string code)
    {
        // 대충 테스트용
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
}

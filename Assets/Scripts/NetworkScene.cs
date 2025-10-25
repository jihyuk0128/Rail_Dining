using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkScene : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button swapButton;

    [Header("Player Images")]
    [SerializeField] private Image player1Image;
    [SerializeField] private Image player2Image;

    [Header("Character Sprites")]
    [SerializeField] private Sprite maleSprite;
    [SerializeField] private Sprite femaleSprite;

    private bool isMale = true;

    

    private void Start()
    {
        // 버튼 이벤트 등록
        startButton.onClick.AddListener(OnStartGame);
        exitButton.onClick.AddListener(OnExit);
        swapButton.onClick.AddListener(OnSwapCharacters);

        // 초기 이미지 설정 (예: Player1 = 남, Player2 = 여)
        player1Image.sprite = maleSprite;
        player2Image.sprite = femaleSprite;

        // 네트워크 이벤트 등록
        Managers.Network.OnGameStart += OnStartGameNetwork;
    }

    // 업무시작버튼
    private void OnStartGame()
    {
        Managers.Network.StartGame();
    }

    private void OnStartGameNetwork()
    {
        Managers.MainThread.Enqueue(() =>
        {
            SceneManager.LoadScene("TutorialScene");
        });
    }

    //나가기버튼
    private void OnExit()
    {
        Managers.Network.LeaveRoom();
        SceneManager.LoadScene("TitleScene");
    }
    // 캐릭터 교체버튼
    private void OnSwapCharacters()
    {
        if (isMale)
        {
            // 1P <-> 2P 캐릭터 교환
            player1Image.sprite = femaleSprite;
            player2Image.sprite = maleSprite;
            isMale = false;
        }
        else
        {
            player1Image.sprite = maleSprite;
            player2Image.sprite = femaleSprite;
            isMale = true;
        }
    }

    private void OnDestroy()
    {
        Managers.Network.OnGameStart -= OnStartGameNetwork;
    }
}

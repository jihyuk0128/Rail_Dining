using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MoneyGaugeEnemy : UI_Base
{
    private enum Images
    {
        PlayerImage,
        ImportGaugeBack,
        ImportGaugeBar,
        ImportGauge,
    }

    private enum Texts
    {
        MoneyText1,
        MoneyText2,
        MoneyText3,
        MoneyText4,
    }

    private Image _playerImage;
    private Image _importGauge;

    private Sprite playerSprite1;
    private Sprite playerSprite2;

    private TextMeshProUGUI _txt25;
    private TextMeshProUGUI _txt50;
    private TextMeshProUGUI _txt75;
    private TextMeshProUGUI _txt100;

    private int _goalMoney = 0;
    private int _currentMoney = 0;

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _playerImage = GetImage((int)Images.PlayerImage);
        _importGauge = GetImage((int)Images.ImportGauge);

        _txt25 = GetTextMeshProUGUI((int)Texts.MoneyText1);
        _txt50 = GetTextMeshProUGUI((int)Texts.MoneyText2);
        _txt75 = GetTextMeshProUGUI((int)Texts.MoneyText3);
        _txt100 = GetTextMeshProUGUI((int)Texts.MoneyText4);

        playerSprite1 = Resources.Load<Sprite>("Art/UI/import gauge/player_gauge_man");
        playerSprite2 = Resources.Load<Sprite>("Art/UI/import gauge/player_gauge_Woman");

        _importGauge.fillAmount = 0f;
        Managers.Network.OnMoneyUpdate += OnMoneyUpdate;

    }

    private void Start()
    {
        Init();
        SetGoalMoney(200); // 목표 수익 정하기
    }

    // 수익 추가하기
    public void SetGoalMoney(int goalMoney)
    {
        _goalMoney = goalMoney;

        _txt25.text = (goalMoney * 0.25f).ToString("0");
        _txt50.text = (goalMoney * 0.50f).ToString("0");
        _txt75.text = (goalMoney * 0.75f).ToString("0");
        _txt100.text = goalMoney.ToString();
    }

    // 수익 추가되면 호출해서 게이지 채우기
    public void UpdateMoney(int newMoney)
    {
        _currentMoney = newMoney;
        _importGauge.fillAmount = Mathf.Clamp01((float)_currentMoney / _goalMoney);
    }


    public void SetPlayerSprite(Sprite sprite)
    {
        if (_playerImage != null)
            _playerImage.sprite = sprite;
    }

    public void OnMoneyUpdate(int player1Money, int player2Money)
    {
        Managers.MainThread.Enqueue(() =>
        {
            Debug.LogWarning($"2번 {player2Money} 얼마 가졌음");
            UpdateMoney(player2Money);
        });
    }

    private void OnDestroy()
    {
        Managers.Network.OnMoneyUpdate -= OnMoneyUpdate;
    }
}

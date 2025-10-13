using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_ResultSummary : UI_Popup
{
    public enum Texts
    {
        DayText,
        PlayerNamesText,   // "플레이어1 / 플레이어2"
        OrderResultText,   // "성공 / 전체 주문"
        OrderRewardText1,   // "주문 수익"
        OrderRewardText2,   // "주문 수익"
        TotalRewardText,   // "총 수익"
        QuotaText          // "할당량"
    }
    public enum Images
    {
        ClearImage,
        FailImage
    }

    public enum Buttons
    {
        NextButton
    }

    private System.Action onCloseCallback;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.NextButton).onClick.AddListener(OnConfirm);
    }

    public void SetResultData(
        int day,
        string player1Name,
        string player2Name,
        int totalOrders,
        int totalSuccess,
        int orderReward,
        int totalReward,
        int quota,
        System.Action onClose = null)
    {
        Get<TextMeshProUGUI>((int)Texts.DayText).text = $"{day}일차";
        Get<TextMeshProUGUI>((int)Texts.PlayerNamesText).text = $"{player1Name} / {player2Name}";
        Get<TextMeshProUGUI>((int)Texts.OrderResultText).text = $"[{totalSuccess} / {totalOrders}]";
        Get<TextMeshProUGUI>((int)Texts.OrderRewardText1).text = $"[{orderReward}$]";
        Get<TextMeshProUGUI>((int)Texts.OrderRewardText2).text = $"일반 주문: {orderReward}$";
        Get<TextMeshProUGUI>((int)Texts.TotalRewardText).text = $"수익 합계: {totalReward}$";
        Get<TextMeshProUGUI>((int)Texts.QuotaText).text = $"금일 할당량: {quota}$";

        Image clearImg = Get<Image>((int)Images.ClearImage);
        Image failImg = Get<Image>((int)Images.FailImage);

        bool isClear = totalReward >= quota;

        if (clearImg != null) clearImg.gameObject.SetActive(isClear);
        if (failImg != null) failImg.gameObject.SetActive(!isClear);

        onCloseCallback = onClose;
    }

    private void OnConfirm()
    {
        onCloseCallback?.Invoke();
        Managers.UI.ClosePopupUI();
    }
}
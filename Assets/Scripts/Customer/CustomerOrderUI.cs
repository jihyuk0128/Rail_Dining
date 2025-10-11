using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerOrderUI : MonoBehaviour
{
    [Header("Order Elements")]
    public GameObject questionPanel;   // 물음표 아이콘
    public GameObject menuPanel;       // 메뉴 패널
    public Image menuIcon;             // 아이콘 이미지
    public TextMeshProUGUI countText;  // 개수 텍스트
    public GameObject interactionIcon; // E 아이콘
    public GameObject interactionIcon2; // E 아이콘

    private void Awake()
    {
        questionPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    // 주문 대기 상태 (물음표만 띄움)
    public void ShowWaiting()
    {
        questionPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    // 주문 표시 (메뉴 아이콘 + 카운트)
    public void ShowOrder(ItemData item, int current, int required)
    {
        questionPanel.SetActive(false);
        menuPanel.SetActive(true);

        // 아이콘 로딩
        Sprite icon = Resources.Load<Sprite>(item.iconPath);
        if (icon != null)
            menuIcon.sprite = icon;
        else
            Debug.LogWarning($"[OrderUI] 아이콘을 찾을 수 없음: {item.iconPath}");

        countText.text = $"{current}/{required}";
    }

    // 주문 진행 중, 개수 갱신
    public void UpdateProgress(int current, int required)
    {
        if (menuPanel.activeSelf)
        {
            countText.text = $"{current}/{required}";
        }
    }

    // 주문 완료 → UI 숨김
    public void HideOrder()
    {
        questionPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void SetInteractionVisible(bool visible)
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(visible);
        if (interactionIcon2 != null)
            interactionIcon2.SetActive(visible);
    }
}

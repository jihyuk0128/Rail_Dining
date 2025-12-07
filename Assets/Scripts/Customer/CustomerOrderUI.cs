using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CustomerOrderUI : MonoBehaviour
{
    [Header("Order Elements")]
    public GameObject questionPanel;   // 물음표 아이콘
    public GameObject menuPanel;       // 메뉴 패널
    public Image menuIcon;             // 아이콘 이미지
    public GameObject interactionIcon; // E 아이콘
    public GameObject interactionIcon2; // E 아이콘

    [Header("Wait Gauge")]
    public Slider waitSlider;                // 대기 게이지

    [Header("Coin Effect")]
    public Image coinIcon;                // 코인 아이콘
    public float coinJumpHeight = 40f;    // 점프 높이
    public float coinDuration = 1.5f;

    private float waitTime;       // 전체 대기 시간
    private float remainingTime;  // 남은 시간
    private bool timerActive = false;

    private void Awake()
    {
        questionPanel.SetActive(false);
        menuPanel.SetActive(false);

        if (waitSlider != null)
            waitSlider.gameObject.SetActive(false);
        if (coinIcon != null)
            coinIcon.gameObject.SetActive(false);
    }

    // 주문 대기 상태
    public void ShowWaiting()
    {
        questionPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    // 주문 표시 (메뉴 아이콘 + 카운트)
    public void ShowOrder(ItemData item, int current, int required, float totalWaitTime = 40f)
    {
        questionPanel.SetActive(false);
        menuPanel.SetActive(true);
        SetInteractionVisible(false);

        // 아이콘 로드 테두리 있는 아이콘
        Sprite icon = Managers.Resource.Load<Sprite>(item.iconPath+" 1");
        if (icon != null)
            menuIcon.sprite = icon;
        else
            Debug.LogWarning($"[OrderUI] 아이콘을 찾을 수 없음: {item.iconPath}");

        // 대기시간 게이지 초기화
        waitTime = totalWaitTime;
        remainingTime = totalWaitTime;
        if (waitSlider != null)
        {
            waitSlider.gameObject.SetActive(true);
            waitSlider.value = 1f;
        }
        timerActive = true;
    }

    private void Update()
    {
        if (timerActive)
        {
            remainingTime -= Time.deltaTime;
            float ratio = Mathf.Clamp01(remainingTime / waitTime);

            if (waitSlider != null)
            {
                waitSlider.value = ratio;
            }

            // 시간 초과 시
            if (remainingTime <= 0f)
            {
                timerActive = false;
            }
        }
    }

    // 남은 시간 갱신 (외부에서 직접 조절 가능)
    public void SetRemainingTime(float current, float max)
    {
        waitTime = max;
        remainingTime = Mathf.Clamp(current, 0f, max);

        float ratio = Mathf.Clamp01(remainingTime / waitTime);
        if (waitSlider != null)
        {
            waitSlider.value = ratio;
        }
    }

    // 주문 완료 → UI 숨김
    [ContextMenu("테스트")]
    public void HideOrder()
    {
        questionPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void ShowCoinEffect()
    {
        if (coinIcon == null) return;
            StartCoroutine(CoinEffect());
    }

    private IEnumerator CoinEffect()
    {
        coinIcon.gameObject.SetActive(true);

        RectTransform rect = coinIcon.rectTransform;
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * coinJumpHeight;

        float t = 0f;
        float duration = coinDuration;

        Color startColor = coinIcon.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            // 위치 이동
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, lerp));

            // 알파 감소
            coinIcon.color = Color.Lerp(startColor, endColor, lerp);

            yield return null;
        }

        coinIcon.gameObject.SetActive(false);
        coinIcon.color = startColor;
        rect.anchoredPosition = startPos;
    }


    public void SetInteractionVisible(bool visible)
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(visible);
        if (interactionIcon2 != null)
            interactionIcon2.SetActive(visible);
    }
}

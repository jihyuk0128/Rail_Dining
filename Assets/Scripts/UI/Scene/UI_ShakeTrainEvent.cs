using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_ShakeTrainEvent : UI_Scene
{
    public enum GameObjects
    {
        LoadingButton3,
        LoadingButton2,
        LoadingButton1,
        PressButton1,
        PressButton2,
        SpaceBarButton
    }

    public Action OnCountdownFinished; // 카운트다운 완료 콜백함수 실행용

    private Sprite OnButtonSprite;
    private Sprite OffButtonSprite;

    private void Awake()
    {
        Init();
        HideUI();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(GameObjects));

        // 버튼 상태 이미지 로드 (Resources 폴더 아래 경로)
        OnButtonSprite = Managers.Resource.Load<Sprite>("Art/UI/Loading_Click_on");
        OffButtonSprite = Managers.Resource.Load<Sprite>("Art/UI/Loading_Click_off");
    }

    // 키 두 개 이벤트 UI 표시
    public void ShowKeyEvent(KeyCode key1, KeyCode key2)
    {
        // 스페이스바 버튼 숨기기
        GetObject((int)GameObjects.SpaceBarButton).SetActive(false);

        // 키 버튼 표시
        GetObject((int)GameObjects.PressButton1).SetActive(true);
        GetObject((int)GameObjects.PressButton2).SetActive(true);

        Sprite sprite1 = GetKeySprite(key1);
        Sprite sprite2 = GetKeySprite(key2);

        if (sprite1 != null)
            GetImage((int)GameObjects.PressButton1).sprite = sprite1;
        if (sprite2 != null)
            GetImage((int)GameObjects.PressButton2).sprite = sprite2;

        // 버튼 투명도 초기화 (반투명 상태)
        SetButtonAlpha(GetImage((int)GameObjects.PressButton1), 0.4f);
        SetButtonAlpha(GetImage((int)GameObjects.PressButton2), 0.4f);

        // 카운트다운 시작
        StartCoroutine(StartCountdown());
    }

    // 스페이스바 이벤트 UI 표시
    public void ShowSpaceEvent()
    {
        // 키 버튼 숨기기
        GetObject((int)GameObjects.PressButton1).SetActive(false);
        GetObject((int)GameObjects.PressButton2).SetActive(false);

        // 스페이스바 버튼 표시
        GetObject((int)GameObjects.SpaceBarButton).SetActive(true);

        SetButtonAlpha(GetImage((int)GameObjects.SpaceBarButton), 0.4f);

        StartCoroutine(StartCountdown());
    }

    // 카운트다운 처리
    private IEnumerator StartCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            SoundManager.Instance.PlaySFX("TrainShake_SFX");
            UpdateLoadingButtons(i);
            yield return new WaitForSeconds(1f);
        }
        GetImage((int)GameObjects.LoadingButton1).sprite = OffButtonSprite;

        // 카운트다운 완료 콜백 호출
        OnCountdownFinished?.Invoke();

        // 3초 끝난 후 버튼 투명도 변경
        if (GetObject((int)GameObjects.PressButton1).activeSelf)
        {
            SetButtonAlpha(GetImage((int)GameObjects.PressButton1), 1f);
            SetButtonAlpha(GetImage((int)GameObjects.PressButton2), 1f);
        }

        if (GetObject((int)GameObjects.SpaceBarButton).activeSelf)
        {
            SetButtonAlpha(GetImage((int)GameObjects.SpaceBarButton), 1f);
        }
    }

    // 카운트다운 버튼 이미지 변경
    private void UpdateLoadingButtons(int step)
    {
        GetImage((int)GameObjects.LoadingButton3).sprite = (step == 3) ? OnButtonSprite : OffButtonSprite;
        GetImage((int)GameObjects.LoadingButton2).sprite = (step == 2) ? OnButtonSprite : OffButtonSprite;
        GetImage((int)GameObjects.LoadingButton1).sprite = (step == 1) ? OnButtonSprite : OffButtonSprite;
    }

    // 버튼 투명도 변경
    private void SetButtonAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    // KeyCode → Sprite 매핑
    private Sprite GetKeySprite(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W: return Managers.Resource.Load<Sprite>("Art/UI/button/W_button_on");
            case KeyCode.A: return Managers.Resource.Load<Sprite>("Art/UI/button/A_button_on");
            case KeyCode.S: return Managers.Resource.Load<Sprite>("Art/UI/button/S_button_on");
            case KeyCode.D: return Managers.Resource.Load<Sprite>("Art/UI/button/D_button_on");
            case KeyCode.Space: return Managers.Resource.Load<Sprite>("Art/UI/button/space_button_on");
            default: return null;
        }
    }
    public void HideUI()
    {
        gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
    }
}
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UI_Clock : UI_Base
{
    enum Texts
    {
        ClockText,
    }

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Debug.Log($"UI_Clock Init 실행 대상: {gameObject.name}");
        Bind<TextMeshProUGUI>(typeof(Texts));

        GetTextMeshProUGUI((int)Texts.ClockText).text = FormatTime(Managers.Network.roomData.MaxTimer);
        Managers.Network.OnTimerUpdate += UpdateTime;
    }

    void UpdateTime(float time)
    {
        Managers.MainThread.Enqueue(() =>
        {
            var clockText = GetTextMeshProUGUI((int)Texts.ClockText);
            if (clockText == null)
            {
                Debug.Log("못찾았어");
                return;  // 아직 못 찾으면 그냥 리턴
            }

            clockText.text = FormatTime(Managers.Network.roomData.MaxTimer);
        });

    }

    public static string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        return $"{minutes:00}:{seconds:00}";
    }

    private void OnDestroy()
    {
        Managers.Network.OnTimerUpdate -= UpdateTime;
    }
}

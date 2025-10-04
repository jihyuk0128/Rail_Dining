using UnityEngine;
using TMPro;
using System;

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
        UpdateTime();
    }

    void UpdateTime()
    {
        var clockText = GetTextMeshProUGUI((int)Texts.ClockText);
        if (clockText == null)
        {
        Debug.Log("못찾았어");
        return;  // 아직 못 찾으면 그냥 리턴
        } 

        clockText.text = DateTime.Now.ToString("HH:mm");
    }

    void Update()
    {
        // 매 프레임 현재 시간 갱신
        UpdateTime();
    }
}

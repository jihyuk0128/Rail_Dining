using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

using TMPro;                
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine.EventSystems;
public class UI_Button : UI_Popup
{
    // EventSystem 사용해서 버튼부분 누르지않게하기
    // ex) if (EventSystem.current.IsPointerOverGameObject()) return;

    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        PointText,
    }

    enum TextMeshProUGUIS
    {
        ScoreText,
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        ItemIcon,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIS));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));


        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);
    }

    int _score = 0;


    public void OnButtonClicked(PointerEventData data)
    {
        Debug.Log("ButtonClicked");

        _score++;
        GetTextMeshProUGUI((int)TextMeshProUGUIS.ScoreText).text = $"score : {_score}";
        Managers.Network.Connect();
    }
}

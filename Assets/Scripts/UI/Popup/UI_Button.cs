using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

using TMPro;                
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class UI_Button : UI_Popup
{
    // EventSystem 사용해서 버튼부분 누르지않게하기
    // ex) if (EventSystem.current.IsPointerOverGameObject()) return;

    enum Buttons
    {
        PointButton,
        PointButton2,
        PointButton3,
        PointButton4,
        PointButton5,
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
        Managers.Network.OnRoomCreate += test;
        Managers.Network.OnHostAssigned += test2;
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
  
        Bind<Text>(typeof(Texts));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIS));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));


        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked1);
        GetButton((int)Buttons.PointButton2).gameObject.BindEvent(OnButtonClicked2);
        GetButton((int)Buttons.PointButton3).gameObject.BindEvent(OnButtonClicked3);
        GetButton((int)Buttons.PointButton4).gameObject.BindEvent((PointerEventData data) => { Managers.Network.LeaveRoom(); });
        GetButton((int)Buttons.PointButton5).gameObject.BindEvent((PointerEventData data) => { Managers.Network.JoinRoom(1); });


    }

    int _score = 0;


    public void OnButtonClicked1(PointerEventData data)
    {
        Debug.Log("ButtonClicked");

        Managers.Network.Login("h231");
    }

    public void test2 (string msg)
    {
        Debug.Log(msg);
    }
    public void test (int roomid)
    {
        Managers.Network.JoinRoom(roomid);
        
    }

    public void OnButtonClicked2(PointerEventData data)
    {
        Debug.Log("ButtonClicked2");

        Managers.Network.CreateRoom();

        //if (Managers.Network.IsConnected)
        //{
        //    Managers.Network.CreateRoom();// 룸생성
        //}
    }

    public void OnButtonClicked3(PointerEventData data)
    {
        Debug.Log("ButtonClicked3");

        Managers.Network.StartGame();

        //if (Managers.Network.IsConnected)
        //{
        //    Managers.Network.CreateRoom();// 룸생성
        //}
    }

}

using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : UI_Popup
{
    // EventSystem 사용해서 버튼부분 누르지않게하기
    // ex) if (EventSystem.current.IsPointerOverGameObject()) return;

    enum Buttons
    {
        TestButton,
        TestButton2
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));


        GetButton((int)Buttons.TestButton).gameObject.BindEvent(OnButtonClicked1);
        GetButton((int)Buttons.TestButton2).gameObject.BindEvent(OnButtonClicked2);


    }


    public void OnButtonClicked1(PointerEventData data)
    {
        Managers.Network.Login("TEST");
        Managers.Network.CreateRoom();
        Managers.Network.JoinRoom(1);
        Managers.Network.StartGame();

   
    }

    public void OnButtonClicked2(PointerEventData data)
    {
       
    }
}

using UnityEngine;
using static UI_ShakeTrainEvent;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Test : UI_Popup
{
    enum Buttons
    {
        Button1,
        //Button2,
        //Button3,
        //Button4,
    }

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        

       //GetButton((int)Buttons.Button1).gameObject.BindEvent((PointerEventData data) => { Managers.Network.CreateRoom(); });
    }

}

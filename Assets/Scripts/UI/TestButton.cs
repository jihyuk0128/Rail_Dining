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
        TestButton
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


    }


    public void OnButtonClicked1(PointerEventData data)
    {
        Managers.UI.ShowPopupUI<UI_Chest>();  // 작업대 UI
        Managers.Inventory.AddItemToChest(1);
    }

}

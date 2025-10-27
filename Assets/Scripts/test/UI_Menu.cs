using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Menu : UI_Popup
{
    enum Buttons { CloseButton, BeverageButton, CulsineButton, JuiceButton }
    enum GameObjects { Grid }

    private Transform _gridParent;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClose);
        GetButton((int)Buttons.BeverageButton).gameObject.BindEvent(OnBeverageButton);
        GetButton((int)Buttons.CulsineButton).gameObject.BindEvent(OnCulsineButton);
        GetButton((int)Buttons.JuiceButton).gameObject.BindEvent(OnJuiceButton);

        _gridParent = GetObject((int)GameObjects.Grid).transform;

        Managers.Crafting.CreateCraftingMenu(_gridParent, 1);
        Debug.Log("[UI_CraftingMenu] 제작 메뉴판 초기화 완료");
    }
    void OnClose(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }

    void OnBeverageButton(PointerEventData data)
    {
        Managers.Crafting.CreateCraftingMenu(_gridParent, 3);
        Debug.Log("[UI_Menu] 음료 메뉴 표시");
    }

    void OnCulsineButton(PointerEventData data)
    {
        Managers.Crafting.CreateCraftingMenu(_gridParent, 4);
        Debug.Log("[UI_Menu] 음료 메뉴 표시");
    }

    void OnJuiceButton(PointerEventData data)
    {
        Managers.Crafting.CreateCraftingMenu(_gridParent, 2);
        Debug.Log("[UI_Menu] 주스 메뉴 표시");
    }
}

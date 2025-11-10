using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Menu : UI_Popup
{
    enum Buttons { CloseButton }
    enum GameObjects { Grid }

    private Transform _gridParent;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClose);

        _gridParent = GetObject((int)GameObjects.Grid).transform;

        Managers.Crafting.CreateCraftingMenu(_gridParent, 1);
        Debug.Log("[UI_CraftingMenu] 제작 메뉴판 초기화 완료");
    }

    public void MenuInit(int type)
    {
        Managers.Crafting.CreateCraftingMenu(_gridParent, type);
        Debug.Log("[UI_Menu] 메뉴 표시완료!");
    }
    void OnClose(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }
}

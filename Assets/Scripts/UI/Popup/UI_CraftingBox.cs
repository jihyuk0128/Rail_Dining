using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CraftingBox : UI_Popup
{
    enum Buttons { CloseButton, CraftButton }
    enum GameObjects { InventoryGrid, CraftingGrid, ResultSlot }

    List<UI_Slot> _inventorySlots = new();
    List<UI_Slot> _craftingSlots = new();
    UI_Slot _resultSlot;

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Managers.Inventory.RegisterCraftingBoxUI(this);

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClose);
        GetButton((int)Buttons.CraftButton).gameObject.BindEvent(OnCraft);

        CreateSlots(GetObject((int)GameObjects.InventoryGrid), 8, SlotType.Inventory, _inventorySlots);
        CreateSlots(GetObject((int)GameObjects.CraftingGrid), 4, SlotType.Crafting, _craftingSlots);

        var resultParent = GetObject((int)GameObjects.ResultSlot).transform;
        var go = Managers.Resource.Instantiate("UI/Slot", resultParent);
        _resultSlot = go.GetComponent<UI_Slot>();
        _resultSlot.SlotType = SlotType.Result;
        _resultSlot.Index = 0;
        _resultSlot.Init();

        RefreshUI();
    }

    private void OnCraft(PointerEventData data)
    {
        Managers.Inventory.Craft();
        SoundManager.Instance.PlaySFX("MakingCocktails_SFX");
    }

    void CreateSlots(GameObject parent, int count, SlotType type, List<UI_Slot> list)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Slot", parent.transform);
            var slot = go.GetComponent<UI_Slot>();
            slot.SlotType = type;
            slot.Index = i;
            slot.Init();
            list.Add(slot);
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < _inventorySlots.Count; i++)
            _inventorySlots[i].SetData(Managers.Inventory.InventorySlots[i]);

        for (int i = 0; i < _craftingSlots.Count; i++)
            _craftingSlots[i].SetData(Managers.Inventory.CraftingSlots[i]);

        _resultSlot.SetData(Managers.Inventory.ResultSlot);
    }

    void OnClose(PointerEventData data)
    {
        //Managers.Inventory.CraftingSlots.Clear();
        Managers.Inventory.UnregisterCraftingBoxUI();
        Managers.UI.ClosePopupUI();
    }

}

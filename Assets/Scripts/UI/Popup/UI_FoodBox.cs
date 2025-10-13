using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_FoodBox : UI_Popup
{
    enum Buttons { CloseButton, CraftButton }
    enum GameObjects { InventoryGrid, CraftingSlot_1, CraftingSlot_2, CraftingSlot_3, ResultSlot }

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

        Managers.Inventory.RegisterFoodBoxUI(this);

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClose);
        GetButton((int)Buttons.CraftButton).gameObject.BindEvent(OnCraft);

        CreateSlots(GetObject((int)GameObjects.InventoryGrid),8, SlotType.Inventory, _inventorySlots);
        CreateSlot(GetObject((int)GameObjects.CraftingSlot_1),0, SlotType.Crafting, _craftingSlots);
        CreateSlot(GetObject((int)GameObjects.CraftingSlot_2),1, SlotType.Crafting, _craftingSlots);
        CreateSlot(GetObject((int)GameObjects.CraftingSlot_3),2, SlotType.Crafting, _craftingSlots);

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
        SoundManager.Instance.PlaySFX("CookingFood_SFX");
    }

    void CreateSlots(GameObject parent, int count, SlotType type, List<UI_Slot> list)
    {
        for (int i = 0; i < count; i++) list.Add(CreateSlot(parent, i, type));
    }

    UI_Slot CreateSlot(GameObject parent, int index, SlotType type)
    {
        GameObject go = Managers.Resource.Instantiate("UI/Slot", parent.transform);
        var slot = go.GetComponent<UI_Slot>();
        slot.SlotType = type;
        slot.Index = index;
        slot.Init();
        return slot;
    }

    void CreateSlot(GameObject parent, int index, SlotType type, List<UI_Slot> list)
    {
        list.Add(CreateSlot(parent, index, type));
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
        Managers.Inventory.CraftingSlots.Clear();
        Managers.Inventory.UnregisterFoodBoxUI();
        Managers.UI.ClosePopupUI();
    }
}

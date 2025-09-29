using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UI_CraftingBox : UI_Popup
{
    enum Buttons { CloseButton, CraftButton }
    enum GameObjects { InventoryGrid, CraftingGrid, ResultSlot }

    private List<UI_Slot> _inventorySlots = new List<UI_Slot>();
    private List<UI_Slot> _craftingSlots = new List<UI_Slot>();
    private UI_Slot _resultSlot;

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        // 버튼 이벤트
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
        GetButton((int)Buttons.CraftButton).gameObject.BindEvent(OnCraftButtonClicked);

        // 매니저에 자기 자신 등록
        Managers.Inventory.RegisterCraftingBoxUI(this);

        // 인벤토리 그리드(공유 뷰)
        var invGrid = GetObject((int)GameObjects.InventoryGrid);
        _inventorySlots.Clear();
        for (int i = 0; i < 8; i++)
        {
            GameObject slotObj = Managers.Resource.Instantiate("UI/Slot", invGrid.transform);
            UI_Slot slot = slotObj.GetComponent<UI_Slot>();
            slot.Configure(SlotType.Inventory, i);
            slot.Init();
            slotObj.name = $"InventorySlot_InCraft_{i + 1}";
            _inventorySlots.Add(slot);
        }

        // 작업대 그리드(2x2 = 4칸)
        var craftGrid = GetObject((int)GameObjects.CraftingGrid);
        _craftingSlots.Clear();
        for (int i = 0; i < 4; i++)
        {
            GameObject slotObj = Managers.Resource.Instantiate("UI/Slot", craftGrid.transform);
            UI_Slot slot = slotObj.GetComponent<UI_Slot>();
            slot.Configure(SlotType.Crafting, i);
            slot.Init();
            slotObj.name = $"CraftingSlot_{i + 1}";
            _craftingSlots.Add(slot);
        }

        // 결과 슬롯(드래그 대상 아님, 표시 전용)
        _resultSlot = Managers.Resource.Instantiate("UI/Slot", GetObject((int)GameObjects.ResultSlot).transform).GetComponent<UI_Slot>();
        _resultSlot.Init();
        _resultSlot.Configure(SlotType.Result, 0); // 항상 0 고정

        RefreshUI();
    }

    public void RefreshUI()
    {
        // 인벤토리 미러링
        var inv = Managers.Inventory.InventoryItems;
        var amounts = Managers.Inventory.InventoryAmounts;

        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            var item = (i < inv.Count) ? inv[i] : null;
            int amt = (i < amounts.Count) ? amounts[i] : 0;
            _inventorySlots[i].SetItem(item, amt);
        }

        // 작업대 미러링
        var craft = Managers.Inventory.CraftingItems;
        for (int i = 0; i < _craftingSlots.Count; i++)
        {
            var item = (i < craft.Count) ? craft[i] : null;
            int amt = (i < amounts.Count) ? amounts[i] : 0;
            _craftingSlots[i].SetItem(item, amt);
        }

        // 결과
        if (_resultSlot != null)
            _resultSlot.SetItem(Managers.Inventory.ResultItem, Managers.Inventory.ResultAmount);
    }

    void OnCloseButtonClicked(PointerEventData data)
    {
        ReturnCraftingItemsToInventory();
        Managers.Inventory.UnregisterCraftingBoxUI();
        Managers.UI.ClosePopupUI();
    }

    void OnCraftButtonClicked(PointerEventData data)
    {
        Managers.Inventory.Craft();



        // 제작 성공 시 결과 슬롯에 아이템 표시 추가
        if (Managers.Inventory.ResultItem != null)
        {
            _resultSlot.SetItem(Managers.Inventory.ResultItem, Managers.Inventory.ResultAmount); 
        }
    }

    void ReturnCraftingItemsToInventory()
    {
        for (int i = 0; i < Managers.Inventory.CraftingItems.Count; i++)
        {
            var item = Managers.Inventory.CraftingItems[i];
            int amount = Managers.Inventory.CraftingAmounts[i];

            if (item != null && amount > 0)
            {
                // 인벤토리에 아이템 추가
                Managers.Inventory.AddItemToInventory(item.id, amount);

                // 작업대 초기화
                Managers.Inventory.CraftingItems[i] = null;
                Managers.Inventory.CraftingAmounts[i] = 0;
            }
        }

        // UI 새로고침
        Managers.Inventory.RefreshAllUI();
    }
}

using UnityEngine;
using System.Collections.Generic;

public class UI_Inventory : UI_Base
{
    enum GameObjects { Grid }

    private GameObject _grid;
    private List<UI_Slot> _slots = new List<UI_Slot>();

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        _grid = GetObject((int)GameObjects.Grid);

        // 매니저에 자기 자신 등록
        Managers.Inventory.RegisterInventoryUI(this);

        // 인벤토리/작업대 전체 초기화는 한 군데에서만(여기서 한 번만) 수행
        if (Managers.Inventory.InventoryItems.Count == 0 && Managers.Inventory.CraftingItems.Count == 0)
        {
            Managers.Inventory.Init(inventoryCount: 8, craftingCount: 4); // 인벤 8칸, 작업대 2x2
        }

        // 슬롯 생성(8)
        _slots.Clear();
        for (int i = 0; i < 8; i++)
        {
            GameObject slotObj = Managers.Resource.Instantiate("UI/Slot", _grid.transform);
            UI_Slot slot = slotObj.GetComponent<UI_Slot>();
            slot.Configure(SlotType.Inventory, i);
            slot.Init();
            slotObj.name = $"InventorySlot_{i + 1}";
            _slots.Add(slot);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        var items = Managers.Inventory.InventoryItems;
        var amounts = Managers.Inventory.InventoryAmounts;

        for (int i = 0; i < _slots.Count; i++)
        {
            var item = (i < items.Count) ? items[i] : null;
            int amt = (i < amounts.Count) ? amounts[i] : 0;
            _slots[i].SetItem(item, amt);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;


public class UI_Inventory : UI_Base
{
    enum GameObjects { Grid }

    List<UI_Slot> _slots = new();

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Managers.Inventory.RegisterInventoryUI(this);

        var grid = GetObject((int)GameObjects.Grid).transform;

        for (int i = 0; i < 8; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Slot", grid);
            var slot = go.GetComponent<UI_Slot>();
            slot.SlotType = SlotType.Inventory;
            slot.Index = i;
            slot.Init();
            _slots.Add(slot);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetData(Managers.Inventory.InventorySlots[i]);
        }
    }
}

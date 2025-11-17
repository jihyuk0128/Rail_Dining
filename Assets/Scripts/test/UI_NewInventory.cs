using System.Collections.Generic;
using UnityEngine;

public class UI_NewInventory : UI_Base
{
    enum GameObjects { Grid }

    private void Start()
    {
        base.Init();
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        var grid = GetObject((int)GameObjects.Grid).transform;
            
        for (int i = 0; i < 4; i++)
        {
            var go = Managers.Resource.Instantiate("TestPrefabs/UI_NewInventorySlot", grid);
            var slot = go.GetComponent<UI_NewInventorySlot>();
            slot.Init();
            slot.SetIndex(i);
        }

    }

    public void RefreshInventoryUI()
    {
        var slots = GetComponentsInChildren<UI_NewInventorySlot>();

        foreach (var slot in slots)
        {
            slot.SetData(Managers.Slot.InventorySlots[slot.SlotIndex]);
            slot.Refresh();
        }
    }

}

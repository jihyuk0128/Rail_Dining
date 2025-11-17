using System.Collections.Generic;
using UnityEngine;

public class UI_PublicChest : UI_Popup
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

        for (int i = 0; i < 8; i++)
        {
            var go = Managers.Resource.Instantiate("TestPrefabs/UI_NewChestSlot", grid);
            var slot = go.GetComponent<UI_NewChestSlot>();
            slot.Init();
            slot.SetIndex(i);
        }
    }

    public void RefreshChestUI()
    {
        var slots = GetComponentsInChildren<UI_NewChestSlot>();

        Debug.Log("=== [UI_PublicChest] RefreshChestUI È£Ãâ ===");

        foreach (var slot in slots)
        {
            int idx = slot.SlotIndex; // Chest ½½·Ô ÀÎµ¦½º

            var data = Managers.Slot.ChestSlots[idx];

            string itemName = (data != null && data.Item != null) ? data.Item.name : "null";
            int amount = (data != null) ? data.Amount : -1;

            Debug.Log($"[ChestUI] UI½½·Ô {idx} => Item: {itemName}, Amount: {amount}");

            slot.SetData(data);
            slot.Refresh();
        }
    }
}

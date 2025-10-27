using System.Collections.Generic;
using UnityEngine;

public class UI_NewInventory : UI_Base
{
    enum GameObjects { Grid }

    List<UI_NewInventorySlot> _slots = new();

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        Managers.Slot.InitSlots(SLOTTYPE.Inventory, 4);
        Managers.NewInventory.Init(4);
        var grid = GetObject((int)GameObjects.Grid).transform;
            
        for (int i = 0; i < 4; i++)
        {
            var go = Managers.Resource.Instantiate("TestPrefabs/UI_NewInventorySlot", grid);
            var slot = go.GetComponent<UI_NewInventorySlot>();
            slot.Init();
            slot.SetIndex(i);
        }

        // 테스트용 아이템 몇 개 추가
        Managers.NewInventory.AddItem(101);
        Managers.NewInventory.AddItem(201);
    }

}

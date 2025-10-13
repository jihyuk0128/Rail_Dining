using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Chest : UI_Popup
{
    enum Buttons { CloseButton }
    enum GameObjects { InventoryGrid, ChestGrid }

    List<UI_Slot> _inventorySlots = new();
    List<UI_Slot> _chestSlots = new();

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Managers.Inventory.RegisterChestUI(this);


        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClose);

        CreateSlots(GetObject((int)GameObjects.InventoryGrid), 8, SlotType.Inventory, _inventorySlots);

        CreateSlots(GetObject((int)GameObjects.ChestGrid), 12, SlotType.Chest, _chestSlots);
       
        RefreshUI();
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

        for(int i = 0; i < _chestSlots.Count; i++)
            _chestSlots[i].SetData(Managers.Inventory.ChestSlots[i]); 
    }

    void OnClose(PointerEventData data)
    {
        Managers.Inventory.UnRegisterChestUI(this);
        Managers.UI.ClosePopupUI();
    }

}

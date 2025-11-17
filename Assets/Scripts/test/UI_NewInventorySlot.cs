using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NewInventorySlot : UI_SlotDragHandler
{
    public int Index { get; private set; }

    public override void Init()
    {
        base.Init();
    }

    public void SetIndex(int index)
    {
        Debug.Log($"[UI_NewInventorySlot] SetIndex »£√‚µ  °Ê index: {index}");

        Index = index;
        SetSlotInfo(SLOTTYPE.Inventory,index);
        SetData(Managers.Slot.InventorySlots[index]);
    }
}
using UnityEngine;

public class UI_NewChestSlot : UI_SlotDragHandler
{
    public int Index { get; private set; }

    public override void Init()
    {
        base.Init();
    }

    public void SetIndex(int index)
    {
        Debug.Log($"[UI_NewChestSlot] SetIndex »£√‚µ  °Ê index: {index}");
        Debug.Log($"ChestSlots[{index}] = {Managers.Slot.ChestSlots[index]?.Item}");

        Index = index;
        SetSlotInfo(SLOTTYPE.Chest, index);
        SetData(Managers.Slot.ChestSlots[index]);
    }
}
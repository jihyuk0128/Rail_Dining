using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NewChestSlot : UI_SlotDragHandler
{
    public int Index { get; private set; }

    public void SetIndex(int index)
    {
        Index = index;
    }

    protected override void HandleItemTransfer(UI_SlotDragHandler fromSlot)
    {
        base.HandleItemTransfer(fromSlot);

        // 창고 갱신
        Managers.Slot.RefreshAll(SLOTTYPE.Chest);
    }
}

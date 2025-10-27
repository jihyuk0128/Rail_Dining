using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NewInventorySlot : UI_SlotDragHandler
{
    public int Index { get; private set; }

    public override void Init()
    {
        base.Init(); // 부모 Init() 호출해야 Bind 작동

    }

    public void SetIndex(int index)
    {
        Index = index;
        SetData(Managers.Slot.InventorySlots[index]);
    }

    protected override void HandleItemTransfer(UI_SlotDragHandler fromSlot)
    {
        base.HandleItemTransfer(fromSlot);

        // 인벤토리 갱신
        Managers.Slot.RefreshAll(SLOTTYPE.Inventory);
    }

    public override void Refresh()
    {
        base.Refresh();
    }

}

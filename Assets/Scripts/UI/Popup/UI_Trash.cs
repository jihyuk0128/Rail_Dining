using UnityEngine;

public class UI_Trash : UI_SlotDragHandler
{
    public override void Init()
    {
        base.Init();
        // 쓰레기통은 SlotType/SlotIndex를 사용하지 않지만,
        // 다른 처리에는 영향 없으므로 따로 건들 필요 없음.
    }

    protected override void HandleItemTransfer(UI_SlotDragHandler sourceSlot)
    {
        // sourceSlot = 드래그한 슬롯
        Managers.Slot.RemoveItemAt(sourceSlot.SlotType, sourceSlot.SlotIndex);

        // UI 갱신
        Managers.Slot.RefreshAll(sourceSlot.SlotType);

        Debug.Log($"[Trash] 삭제됨: {sourceSlot.SlotType}[{sourceSlot.SlotIndex}]");
    }
}
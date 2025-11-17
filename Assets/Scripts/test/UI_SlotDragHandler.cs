using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_SlotDragHandler : UI_ParentSlot
{
    protected Canvas parentCanvas;

    // slot 타입분활
    public SLOTTYPE SlotType { get; private set; }
    public int SlotIndex { get; private set; }

    public void SetSlotInfo(SLOTTYPE type, int index)
    {
        SlotType = type;
        SlotIndex = index;
    }

    public override void Init()
    {
        base.Init(); 

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
            Debug.LogWarning("[UI_SlotDragHandler] 부모 Canvas를 찾을 수 없음!");

        BindEvent(gameObject, OnBeginDrag, Define.UIEvent.BeginDrag);
        BindEvent(gameObject, OnDrag, Define.UIEvent.Drag);
        BindEvent(gameObject, OnEndDrag, Define.UIEvent.EndDrag);
    }

    // --- 드래그 시작 ---
    void OnBeginDrag(PointerEventData data)
    {
        if (slotData?.Item == null)
            return;

        Managers.UI.ShowDragIcon(slotData.Item.iconPath, parentCanvas);
        Managers.UI.UpdateDragIcon(data.position);

        Managers.Slot.SetDragSource(this);
    }

    // --- 드래그 중 ---
    void OnDrag(PointerEventData data)
    {
        Managers.UI.UpdateDragIcon(data.position);
    }

    // --- 드래그 종료 ---
    void OnEndDrag(PointerEventData data)
    {
        Managers.UI.HideDragIcon();

        // 드롭 위치 찾기
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        UI_SlotDragHandler targetSlot = null;

        foreach (var hit in results)
        {
            targetSlot = hit.gameObject.GetComponent<UI_SlotDragHandler>();
            if (targetSlot != null && targetSlot != this)
                break;
        }

        if (targetSlot != null)
        {
            HandleItemTransfer(targetSlot);
        }

        Managers.Slot.ClearDragSource();
    }

    // --- 실제 이동/스왑 로직 ---
    protected virtual void HandleItemTransfer(UI_SlotDragHandler targetSlot)
    {
        bool moved = Managers.Slot.MoveSlot(
        this.SlotType,
        this.SlotIndex,
        targetSlot.SlotType,
        targetSlot.SlotIndex
        );

        if (moved)
        {
            // 인벤/체스트 둘 다 갱신
            Managers.Slot.RefreshAll(this.SlotType);
            Managers.Slot.RefreshAll(targetSlot.SlotType);
        }

        Debug.Log($"[Drag] 슬롯 이동: {SlotType}[{SlotIndex}] → {targetSlot.SlotType}[{targetSlot.SlotIndex}]");
    }
}

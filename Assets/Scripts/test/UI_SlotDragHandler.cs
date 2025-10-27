using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_SlotDragHandler : UI_ParentSlot
{
    protected Canvas parentCanvas;

    public override void Init()
    {
        base.Init();

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
            Debug.LogWarning("[UI_SlotDragHandler] 부모 Canvas를 찾을 수 없음!");

        // 기본 드래그 이벤트만 등록
        BindEvent(gameObject, OnBeginDrag, Define.UIEvent.BeginDrag);
        BindEvent(gameObject, OnDrag, Define.UIEvent.Drag);
        BindEvent(gameObject, OnEndDrag, Define.UIEvent.EndDrag);
    }

    // ==============================
    // 드래그 시작
    // ==============================
    void OnBeginDrag(PointerEventData data)
    {
        if (slotData?.Item == null)
            return;

        Managers.UI.ShowDragIcon(slotData.Item.iconPath, parentCanvas);
        Managers.UI.UpdateDragIcon(data.position);

        Managers.Slot.SetDragSource(this);
    }

    // ==============================
    // 드래그 중
    // ==============================
    void OnDrag(PointerEventData data)
    {
        Managers.UI.UpdateDragIcon(data.position);
    }

    // ==============================
    // 드래그 종료 시 교환 처리
    // ==============================
    void OnEndDrag(PointerEventData data)
    {
        Managers.UI.HideDragIcon();

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

    // ==============================
    // 아이템 이동 / 교환 로직
    // ==============================
    protected virtual void HandleItemTransfer(UI_SlotDragHandler targetSlot)
    {
        if (targetSlot == null)
            return;

        var fromData = this.slotData;
        var toData = targetSlot.slotData;

        if (fromData == null || fromData.Item == null)
            return;

        // 빈 칸이면 이동
        if (toData == null || toData.Item == null)
        {
            targetSlot.slotData.Item = fromData.Item;
            targetSlot.slotData.Amount = fromData.Amount;
            fromData.Clear();
        }
        else // 이미 아이템 있으면 교체
        {
            ItemData tempItem = toData.Item;
            int tempAmount = toData.Amount;

            toData.Item = fromData.Item;
            toData.Amount = fromData.Amount;

            fromData.Item = tempItem;
            fromData.Amount = tempAmount;
        }

        targetSlot.Refresh();
        Refresh();

        Debug.Log($"[UI_SlotDragHandler] 아이템 교환 완료 ({fromData.Item?.name ?? "빈칸"} ↔ {toData.Item?.name ?? "빈칸"})");
    }
}
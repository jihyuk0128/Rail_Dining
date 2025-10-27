using System.Collections.Generic;
using UnityEngine;

public class NewInventoryManager
{
    private NewSlotManager _slotManager => Managers.Slot;

    // 인벤토리 슬롯 접근 단축 프로퍼티
    private List<ItemSlot> Slots => _slotManager.InventorySlots;

    // 1. 인벤토리 초기화
    public void Init(int slotCount)
    {
        _slotManager.InitSlots(SLOTTYPE.Inventory, slotCount);
        Debug.Log($"[InventoryManager] 인벤토리 초기화 완료 ({slotCount}칸)");
    }

    // 2. 아이템 추가
    public bool AddItem(int id, int amount = 1)
    {
        if (!_slotManager.AddItem(SLOTTYPE.Inventory, id, amount))
        {
            Debug.LogWarning("[InventoryManager] 인벤토리 가득 참!");
            return false;
        }
        RefreshUI();
        return true;
    }

    // 3. 아이템 제거
    public bool RemoveItem(int id)
    {
        bool result = _slotManager.RemoveItem(SLOTTYPE.Inventory, id);
        if (result)
        {
            Debug.Log($"[InventoryManager] 아이템 제거됨 (ID: {id})");
            RefreshUI();
        }
        return result;
    }

    //  4. 아이템 존재 확인
    public bool HasItem(int id)
    {
        return _slotManager.HasItem(SLOTTYPE.Inventory, id);
    }

    //  5. 빈 슬롯 개수 확인
    public int CountEmptySlots()
    {
        int count = 0;
        foreach (var slot in Slots)
        {
            if (slot.Item == null)
                count++;
        }
        return count;
    }

    // 6. 전체 초기화
    public void ClearAll()
    {
        _slotManager.ClearSlots(SLOTTYPE.Inventory);
        RefreshUI();
    }

    // 7. 인벤토리 UI 갱신 (나중에 연결)
    public void RefreshUI()
    {
        // 나중에 UI 연결 시 Managers.UI_Inventory.Refresh() 이런 식으로 연결
        // 지금은 로그로 대체
        Debug.Log("[InventoryManager] 인벤토리 UI 갱신 요청");

        foreach (var slot in Object.FindObjectsOfType<UI_ParentSlot>())
            slot.Refresh();
    }

    // 8. 디버그용 전체 출력
    public void LogInventory()
    {
        Debug.Log("=== [InventoryManager] 현재 인벤토리 ===");
        for (int i = 0; i < Slots.Count; i++)
        {
            var slot = Slots[i];
            string itemName = slot.Item == null ? "(빈 슬롯)" : slot.Item.name;
            Debug.Log($"[{i}] {itemName} x{slot.Amount}");
        }
    }
}

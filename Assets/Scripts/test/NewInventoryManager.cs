using System.Collections.Generic;
using UnityEngine;

public class NewInventoryManager
{
    private NewSlotManager _slotManager => Managers.Slot;

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

    public bool HasItem(int id) => _slotManager.HasItem(SLOTTYPE.Inventory, id);

    public int CountEmptySlots()
    {
        int count = 0;
        foreach (var slot in Slots)
            if (slot.Item == null)
                count++;

        return count;
    }

    public void ClearAll()
    {
        _slotManager.ClearSlots(SLOTTYPE.Inventory);
        RefreshUI();
    }

    // UI 갱신
    public void RefreshUI()
    {
        Debug.Log("[Inventory] UI 전체 갱신");

        var inventoryUI = Object.FindObjectOfType<UI_NewInventory>();
        if (inventoryUI != null)
            inventoryUI.RefreshInventoryUI();
    }

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

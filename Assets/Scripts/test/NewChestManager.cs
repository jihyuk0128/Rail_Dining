using System.Collections.Generic;
using UnityEngine;

public class NewChestManager
{
    private NewSlotManager SlotManager => Managers.Slot;
    private NewInventoryManager Inventory => Managers.NewInventory;

    // 내부 접근 단축
    private List<ItemSlot> ChestSlots => SlotManager.ChestSlots;

    // 1. 창고 초기화
    public void Init(int slotCount)
    {
        SlotManager.InitSlots(SLOTTYPE.Chest, slotCount);
        Debug.Log($"[ChestManager] 창고 초기화 완료 ({slotCount}칸)");
    }

    // 2. 아이템 추가 (직접 넣기)
    public bool AddItem(int id, int amount = 1)
    {
        if (!SlotManager.AddItem(SLOTTYPE.Chest, id, amount))
        {
            Debug.LogWarning("[ChestManager] 창고 가득 참!");
            return false;
        }
        RefreshUI();
        return true;
    }

    // 3. 아이템 제거
    public bool RemoveItem(int id)
    {
        bool result = SlotManager.RemoveItem(SLOTTYPE.Chest, id);
        if (result)
        {
            Debug.Log($"[ChestManager] 아이템 제거됨 (ID: {id})");
            RefreshUI();
        }
        return result;
    }

    // 4. 인벤토리 → 창고로 아이템 이동 (Deposit)
    public bool DepositItem(int itemId)
    {
        // 인벤토리에서 해당 아이템 제거
        if (!Inventory.HasItem(itemId))
        {
            Debug.LogWarning($"[ChestManager] 인벤토리에 아이템(ID:{itemId})이 없습니다.");
            return false;
        }

        // 창고에 추가 시도
        if (!SlotManager.AddItem(SLOTTYPE.Chest, itemId))
        {
            Debug.LogWarning("[ChestManager] 창고에 빈칸이 없습니다.");
            return false;
        }

        // 인벤토리에서 제거
        Inventory.RemoveItem(itemId);

        Debug.Log($"[ChestManager] 인벤 → 창고 이동 완료 (ID:{itemId})");
        RefreshUI();
        return true;
    }

    // 5. 창고 → 인벤토리로 아이템 이동 (Withdraw)
    public bool WithdrawItem(int itemId)
    {
        // 창고에 해당 아이템 있는지 확인
        bool hasItem = SlotManager.HasItem(SLOTTYPE.Chest, itemId);
        if (!hasItem)
        {
            Debug.LogWarning($"[ChestManager] 창고에 아이템(ID:{itemId})이 없습니다.");
            return false;
        }

        // 인벤토리에 추가 시도
        if (!SlotManager.AddItem(SLOTTYPE.Inventory, itemId))
        {
            Debug.LogWarning("[ChestManager] 인벤토리에 빈칸이 없습니다.");
            return false;
        }

        // 창고에서 제거
        SlotManager.RemoveItem(SLOTTYPE.Chest, itemId);

        Debug.Log($"[ChestManager] 창고 → 인벤 이동 완료 (ID:{itemId})");
        RefreshUI();
        return true;
    }

    // 6. 전체 비우기
    public void ClearAll()
    {
        SlotManager.ClearSlots(SLOTTYPE.Chest);
        RefreshUI();
    }

    // 7. UI 갱신 (나중에 연결)
    private void RefreshUI()
    {
        Debug.Log("[ChestManager] 창고 UI 갱신 요청");
    }

    // 8. 디버그용 로그
    public void LogChest()
    {
        Debug.Log("=== [ChestManager] 현재 창고 ===");
        for (int i = 0; i < ChestSlots.Count; i++)
        {
            var slot = ChestSlots[i];
            string itemName = slot.Item == null ? "(빈 슬롯)" : slot.Item.name;
            Debug.Log($"[{i}] {itemName} x{slot.Amount}");
        }
    }
}

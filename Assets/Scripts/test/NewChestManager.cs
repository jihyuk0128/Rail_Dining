using System.Collections.Generic;
using UnityEngine;

public class NewChestManager
{
    private NewSlotManager SlotManager => Managers.Slot;
    private NewInventoryManager Inventory => Managers.NewInventory;

    private List<ItemSlot> ChestSlots => SlotManager.ChestSlots;

    // 초기화
    public void Init(int slotCount)
    {
        SlotManager.InitSlots(SLOTTYPE.Chest, slotCount);
        Debug.Log($"[ChestManager] 창고 초기화 완료 ({slotCount}칸)");
    }

    // 추가
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

    // 제거
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

    // 인벤 → 창고
    public bool DepositItem(int itemId)
    {
        if (!Inventory.HasItem(itemId))
        {
            Debug.LogWarning($"[ChestManager] 인벤토리에 아이템(ID:{itemId})이 없습니다.");
            return false;
        }

        if (!SlotManager.AddItem(SLOTTYPE.Chest, itemId))
        {
            Debug.LogWarning("[ChestManager] 창고에 빈칸이 없습니다.");
            return false;
        }

        Inventory.RemoveItem(itemId);

        Debug.Log($"[ChestManager] 인벤 → 창고 이동 완료 (ID:{itemId})");
        RefreshUI();
        return true;
    }

    // 창고 → 인벤
    public bool WithdrawItem(int itemId)
    {
        if (!SlotManager.HasItem(SLOTTYPE.Chest, itemId))
        {
            Debug.LogWarning($"[ChestManager] 창고에 아이템(ID:{itemId})이 없습니다.");
            return false;
        }

        if (!SlotManager.AddItem(SLOTTYPE.Inventory, itemId))
        {
            Debug.LogWarning("[ChestManager] 인벤토리에 빈칸이 없습니다.");
            return false;
        }

        SlotManager.RemoveItem(SLOTTYPE.Chest, itemId);

        Debug.Log($"[ChestManager] 창고 → 인벤 이동 완료 (ID:{itemId})");
        RefreshUI();
        return true;
    }

    public void ClearAll()
    {
        SlotManager.ClearSlots(SLOTTYPE.Chest);
        RefreshUI();
    }

    public void RefreshUI()
    {
        Debug.Log("[ChestManager] UI 전체 갱신");
        var chest = Object.FindObjectOfType<UI_PublicChest>();
        if (chest != null)
            chest.RefreshChestUI();

    }
}

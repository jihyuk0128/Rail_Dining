using System.Collections.Generic;
using UnityEngine;

public enum SLOTTYPE { Inventory, Chest }

public class NewSlotManager
{
    public List<ItemSlot> InventorySlots = new();
    public List<ItemSlot> ChestSlots = new();

    private UI_SlotDragHandler _dragSourceSlot;

    public void SetDragSource(UI_SlotDragHandler slot) => _dragSourceSlot = slot;
    public UI_SlotDragHandler GetDragSource() => _dragSourceSlot;
    public void ClearDragSource() => _dragSourceSlot = null;


    public int InventorySize => InventorySlots.Count;
    public int ChestSize => ChestSlots.Count;

    // 슬롯 초기화 (타입별)
    public void InitSlots(SLOTTYPE type, int count)
    {
        var list = GetSlotList(type);
        if (list == null)
        {
            Debug.LogWarning($"[NewSlotManager] 잘못된 슬롯 타입: {type}");
            return;
        }

        list.Clear();
        for (int i = 0; i < count; i++)
            list.Add(new ItemSlot());

        Debug.Log($"[NewSlotManager] {type} 슬롯 초기화 완료 (슬롯 수: {count})");
    }

    // 전체 슬롯 비우기
    public void ClearSlots(SLOTTYPE type)
    {
        var list = GetSlotList(type);
        if (list == null)
            return;

        foreach (var slot in list)
            slot.Clear();

        Debug.Log($"[NewSlotManager] {type} 슬롯 전체 초기화");
    }

    // 특정 슬롯 가져오기
    public ItemSlot GetSlot(SLOTTYPE type, int index)
    {
        var list = GetSlotList(type);
        if (list == null || index < 0 || index >= list.Count)
            return null;
        return list[index];
    }

    // 아이템 추가 (빈 슬롯 하나만)
    public bool AddItem(SLOTTYPE type, int id, int amount = 1)
    {
        var list = GetSlotList(type);
        if (list == null)
            return false;

        if (!Managers.Data.ItemDict.ContainsKey(id))
        {
            Debug.LogWarning($"[NewSlotManager] 존재하지 않는 아이템 ID: {id}");
            return false;
        }

        foreach (var slot in list)
        {
            if (slot.Item == null)
            {
                slot.Item = Managers.Data.ItemDict[id];
                slot.Amount = amount;
                Debug.Log($"[NewSlotManager] [{type}] 아이템 추가됨 → {slot.Item.name} x{amount}");
                return true;
            }
        }

        Debug.LogWarning($"[NewSlotManager] [{type}] 빈 슬롯이 없습니다!");
        return false;
    }

    // 아이템 제거 (ID 일치하는 슬롯 하나만)
    public bool RemoveItem(SLOTTYPE type, int id)
    {
        var list = GetSlotList(type);
        if (list == null)
            return false;

        foreach (var slot in list)
        {
            if (slot.Item != null && slot.Item.id == id)
            {
                slot.Clear();
                Debug.Log($"[NewSlotManager] [{type}] 아이템 제거됨 → ID {id}");
                return true;
            }
        }

        Debug.LogWarning($"[NewSlotManager] [{type}] 제거할 아이템(ID {id})을 찾지 못했습니다.");
        return false;
    }

    // 내부 공용 함수: 슬롯 리스트 선택
    private List<ItemSlot> GetSlotList(SLOTTYPE type)
    {
        return type switch
        {
            SLOTTYPE.Inventory => InventorySlots,
            SLOTTYPE.Chest => ChestSlots,
            _ => null
        };
    }

    public bool HasItem(SLOTTYPE type, int id)
    {
        var list = GetSlotList(type);
        foreach (var slot in list)
            if (slot.Item != null && slot.Item.id == id)
                return true;
        return false;
    }

    public int FindEmptySlotIndex(SLOTTYPE type)
    {
        var list = GetSlotList(type);
        for (int i = 0; i < list.Count; i++)
            if (list[i].Item == null)
                return i;
        return -1;
    }

    public int CountItems(SLOTTYPE type)
    {
        var list = GetSlotList(type);
        int count = 0;
        foreach (var slot in list)
            if (slot.Item != null)
                count++;
        return count;
    }

    public void RefreshAll(SLOTTYPE type)
    {
        switch (type)
        {
            case SLOTTYPE.Inventory:
                Managers.NewInventory.RefreshUI();
                break;
            case SLOTTYPE.Chest:
                //Managers.Chest.RefreshUI();
                break;
        }
    }
}

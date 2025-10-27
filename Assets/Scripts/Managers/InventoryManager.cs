using System.Collections.Generic;
using UnityEngine;

public enum SlotType { Inventory, Crafting, Result, Chest }
public enum MoveMode { One,Half,All}

public class InventoryManager
{
    public List<ItemSlot> InventorySlots { get; private set; } = new();
    public List<ItemSlot> CraftingSlots { get; private set; } = new();
    public List<ItemSlot> ChestSlots { get; private set; } = new();
    public ItemSlot ResultSlot { get; private set; } = new();

    public UI_Inventory InventoryUI { get; private set; }
    public UI_CraftingBox CraftingBoxUI { get; private set; }
    public UI_FoodBox FoodBoxUI { get; private set; }
    public UI_Chest ChestUI { get; private set; } 
    public UI_Slot DragSourceSlot { get; set; }

    public void Init(int inventoryCount, int craftingCount)
    {
        Debug.Log($"[Inventory] Init 실행됨 (인벤={inventoryCount}, 크래프팅={craftingCount})");

        InventorySlots.Clear();
        CraftingSlots.Clear();

        for (int i = 0; i < inventoryCount; i++)
        {
            InventorySlots.Add(new ItemSlot());
            Debug.Log($"[Inventory] 인벤 {i} 생성됨");
        }

        for (int i = 0; i < craftingCount; i++)
        {
            CraftingSlots.Add(new ItemSlot());
            Debug.Log($"[Inventory] 작업대 {i} 생성됨");
        }

        ResultSlot = new ItemSlot();
        for (int i = 0; i < 12; i++)
        {
            ChestSlots.Add(new ItemSlot()); //창고 12개 생성
            Debug.Log("창고생성");
        }
    }

    public void RegisterInventoryUI(UI_Inventory ui) => InventoryUI = ui;
    public void RegisterCraftingBoxUI(UI_CraftingBox ui) => CraftingBoxUI = ui;
    public void RegisterFoodBoxUI(UI_FoodBox ui) => FoodBoxUI = ui;
    public void RegisterChestUI(UI_Chest ui) => ChestUI = ui;
      
    public void UnregisterCraftingBoxUI()
    {
        CraftingBoxUI = null;
        //CraftingSlots.Clear();
    }
    public void UnregisterFoodBoxUI()
    {
        FoodBoxUI = null;
        //CraftingSlots.Clear();
    }
    public void UnRegisterChestUI(UI_Chest ui)
    {
        ChestUI = null;
        for (int i = 0; i < 12; i++) ChestSlots[i].Clear();
    }
    public void AddItemToChest(int id)
    {
        if (!Managers.Data.ItemDict.ContainsKey(id))
        {
            Debug.LogWarning($"Item ID {id} 없음");
            return;
        }


        for (int i = 0; i < ChestSlots.Count; i++)
        {
            if (ChestSlots[i].Item == null)
            {
                ChestSlots[i].Item = Managers.Data.ItemDict[id];
                ChestSlots[i].Amount = 10;
                Debug.Log($"[창고] {ChestSlots[i].Item.name} x{10} 추가됨");
                RefreshAllUI();
                return;
            }
        }

    }

    public void AddItemToInventory(int id, int amount = 1)
    {
        if (!Managers.Data.ItemDict.ContainsKey(id))
        {
            Debug.LogWarning($"Item ID {id} 없음");
            return;
        }

        for (int i = 0; i < InventorySlots.Count; i++)
        {
            if (InventorySlots[i].Item == null)
            {
                InventorySlots[i].Item = Managers.Data.ItemDict[id];
                InventorySlots[i].Amount = amount;
                RefreshAllUI();
                return;
            }

            if (InventorySlots[i].Item.id == id && !InventorySlots[i].IsFull)
            {
                InventorySlots[i].Amount++;
                return;
            }
        }
    }

    public void MoveItem(SlotType fromType, int fromIndex, SlotType toType, int toIndex, bool moveAll = false)
    {
        var fromList = GetSlotList(fromType);
        var toList = GetSlotList(toType);
        if (fromList == null || toList == null) return;

        if (fromIndex < 0 || fromIndex >= fromList.Count) return;
        if (toIndex < 0 || toIndex >= toList.Count) return;

        var from = fromList[fromIndex];
        var to = toList[toIndex];
        if (from.Item == null || from.Amount <= 0) return;

        int moveAmount = moveAll ? from.Amount : 1;

        if (to.Item == null)
        {
            to.Item = from.Item;
            to.Amount = 0;
        }

        if (to.Item.id == from.Item.id)
        {
            int space = ItemSlot.MAX_STACK - to.Amount;
            int transfer = Mathf.Min(space, moveAmount);

            to.Amount += transfer;
            from.Amount -= transfer;

            if (from.Amount <= 0)
                from.Clear();
        }

        RefreshAllUI();
    }

    List<ItemSlot> GetSlotList(SlotType type)
    {
        return type switch
        {
            SlotType.Inventory => InventorySlots,
            SlotType.Crafting => CraftingSlots,
            _ => null
        };
    }

    public void RefreshAllUI()
    {
        InventoryUI?.RefreshUI();
        CraftingBoxUI?.RefreshUI();
        FoodBoxUI?.RefreshUI();
        ChestUI?.RefreshUI();
    }

    public void Craft()
    {
        var current = new List<int>();

        foreach (var slot in CraftingSlots)
        {
            if (slot.Item == null)
                current.Add(0);
            else
                current.Add(slot.Item.id);
        }

        RecipeData match = null;
        foreach (var recipe in Managers.Data.RecipeDict.Values)
        {
            if (CheckRecipe(recipe, current))
            {
                match = recipe;
                break;
            }
        }

        if (match == null)
        {
            Debug.Log("레시피 불일치!");
            return;
        }

        ResultSlot.Item = Managers.Data.ItemDict[match.resultId];

        foreach (var slot in CraftingSlots)
            slot.Clear();

        Debug.Log($"[제작 성공] {ResultSlot.Item.name} x{ResultSlot.Amount}");
        RefreshAllUI();
    }

    private bool CheckRecipe(RecipeData recipe, List<int> current)
    {
        if (recipe.ingredients.Count != current.Count) return false;
        // 정렬해서 순서상관없이 값만 비교
        var sortedRecipe = new List<int>(recipe.ingredients);
        var sortedCurrent = new List<int>(current);

        sortedRecipe.Sort();
        sortedCurrent.Sort();

        for (int i = 0; i < sortedRecipe.Count; i++)
        {
            if (sortedRecipe[i] != sortedCurrent[i])
                return false;
        }
        return true;
    }

    // 아이템 비교해서 있으면 true 반환 밑 해당아이템 한 개 제거
    public bool CheckItemToRemove(ItemData orderItem)
    {
        bool hasCorrectItem = false;

        foreach (var slot in Managers.Inventory.InventorySlots)
        {
            if (slot.Item != null && slot.Item.id == orderItem.id)
            {
                // 올바른 아이템 발견 → 한 개 제거
                slot.Amount--;
                if (slot.Amount <= 0)
                    slot.Clear();

                Managers.Inventory.RefreshAllUI();
                hasCorrectItem = true;
                break;
            }
        }
        return hasCorrectItem;
    }

    public void ClearInventory()
    {
        foreach (var slot in InventorySlots)
            slot.Clear();

        RefreshAllUI();
    }
}

using UnityEngine;
using System.Collections.Generic;

public enum SlotType { Inventory, Crafting, Result }
public enum MoveMode { One,Half,All}

public class InventoryManager
{
    public List<ItemSlot> InventorySlots { get; private set; } = new();
    public List<ItemSlot> CraftingSlots { get; private set; } = new();
    public ItemSlot ResultSlot { get; private set; } = new();

    public UI_Inventory InventoryUI { get; private set; }
    public UI_CraftingBox CraftingBoxUI { get; private set; }
    public UI_FoodBox FoodBoxUI { get; private set; }
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
    }

    public void RegisterInventoryUI(UI_Inventory ui) => InventoryUI = ui;
    public void RegisterCraftingBoxUI(UI_CraftingBox ui) => CraftingBoxUI = ui;
    public void RegisterFoodBoxUI(UI_FoodBox ui) => FoodBoxUI = ui;
    public void UnregisterCraftingBoxUI()
    {
        CraftingBoxUI = null;
        CraftingSlots.Clear();
    }
    public void UnregisterFoodBoxUI()
    {
        FoodBoxUI = null;
        CraftingSlots.Clear();
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
                Debug.Log($"[인벤] {InventorySlots[i].Item.name} x{amount} 추가됨");
                RefreshAllUI();
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
    }

    public void Craft()
    {
        var current = new List<int>();

        foreach (var slot in CraftingSlots)
        {
            if (slot.Item == null) return;
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
        ResultSlot.Amount = match.resultAmount;

        foreach (var slot in CraftingSlots)
            slot.Clear();

        Debug.Log($"[제작 성공] {ResultSlot.Item.name} x{ResultSlot.Amount}");
        RefreshAllUI();
    }

    private bool CheckRecipe(RecipeData recipe, List<int> current)
    {
        if (recipe.ingredients.Count != current.Count) return false;
        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            if (recipe.ingredients[i] != current[i]) return false;
        }
        return true;
    }
}

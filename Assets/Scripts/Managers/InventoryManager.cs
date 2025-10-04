using UnityEngine;
using System.Collections.Generic;

public enum SlotType { Inventory, Crafting, Result }

public class InventoryManager
{
    public List<ItemData> InventoryItems { get; private set; } = new List<ItemData>();
    public List<ItemData> CraftingItems { get; private set; } = new List<ItemData>();
    public List<int> InventoryAmounts { get; private set; } = new List<int>();
    public List<int> CraftingAmounts { get; private set; } = new List<int>();
    public ItemData ResultItem { get; private set; }
    public int ResultAmount { get; private set; }

    public UI_Inventory InventoryUI { get; private set; }
    public UI_CraftingBox CraftingBoxUI { get; private set; }

    public void Init(int inventoryCount, int craftingCount)
    {
        InventoryItems.Clear();
        CraftingItems.Clear();
        InventoryAmounts.Clear();
        CraftingAmounts.Clear();

        for (int i = 0; i < inventoryCount; i++) { InventoryItems.Add(null); InventoryAmounts.Add(0); }
        for (int i = 0; i < craftingCount; i++) { CraftingItems.Add(null); CraftingAmounts.Add(0); }

        ResultItem = null;
        ResultAmount = 0;

        RefreshAllUI();
    }

    public void RegisterInventoryUI(UI_Inventory ui) => InventoryUI = ui;
    public void RegisterCraftingBoxUI(UI_CraftingBox ui) => CraftingBoxUI = ui;

    public void AddItemToInventory(int id, int amount = 1)
    {
        if (!Managers.Data.ItemDict.ContainsKey(id))
        {
            Debug.LogWarning($"Item ID {id} 없음");
            return;
        }

        for (int i = 0; i < InventoryItems.Count; i++)
        {
            if (InventoryItems[i] == null)
            {
                InventoryItems[i] = Managers.Data.ItemDict[id];
                InventoryAmounts[i] = amount;
                RefreshAllUI();
                return;
            }
        }
    }

    public void MoveItem(SlotType fromType, int fromIndex, SlotType toType, int toIndex)
    {
        var fromList = GetItemList(fromType);
        var toList = GetItemList(toType);
        var fromAmt = GetAmountList(fromType);
        var toAmt = GetAmountList(toType);


        if (fromList == null || toList == null) return;
        if (fromIndex < 0 || fromIndex >= fromList.Count) return;
        if (toIndex < 0 || toIndex >= toList.Count) return;

        if (fromList[fromIndex] == null || fromAmt[fromIndex] <= 0) return;

        // 대상 슬롯이 비어있으면 아이템 초기화
        if (toList[toIndex] == null)
        {
            toList[toIndex] = fromList[fromIndex];
            toAmt[toIndex] = 0;
        }

        // 아이템이 같으면 한 개씩만 옮기기
        if (toList[toIndex] == fromList[fromIndex])
        {
            fromAmt[fromIndex] -= 1;
            toAmt[toIndex] += 1;

            if (fromAmt[fromIndex] <= 0)
                fromList[fromIndex] = null;
        }

        RefreshAllUI();
    }

    List<ItemData> GetItemList(SlotType type)
    {
        return type switch
        {
            SlotType.Inventory => InventoryItems,
            SlotType.Crafting => CraftingItems,
            _ => null
        };
    }

    List<int> GetAmountList(SlotType type)
    {
        return type switch
        {
            SlotType.Inventory => InventoryAmounts,
            SlotType.Crafting => CraftingAmounts,
            _ => null
        };
    }

    public void RefreshAllUI()
    {
        InventoryUI?.RefreshUI();
        CraftingBoxUI?.RefreshUI();
    }

    public void Craft()
    {
        // 작업대에 있는 아이템 ID 리스트 추출
        List<int> currentIngredients = new List<int>();
        for (int i = 0; i < CraftingItems.Count; i++)
        {
            if (CraftingItems[i] == null)
            {
                Debug.Log("작업대에 빈칸 있음!");
                return;
            }
            currentIngredients.Add(CraftingItems[i].id);
        }

        // 레시피 검사
        RecipeData matchedRecipe = null;
        foreach (var recipe in Managers.Data.RecipeDict.Values)
        {
            if (CheckRecipeMatch(recipe, currentIngredients))
            {
                matchedRecipe = recipe;
                break;
            }
        }

        if (matchedRecipe == null)
        {
            Debug.Log("레시피 불일치!");
            return;
        }

        // 결과 아이템 생성
        ResultItem = Managers.Data.ItemDict[matchedRecipe.resultId];
        ResultAmount = matchedRecipe.resultAmount;

        // 재료 소모
        for (int i = 0; i < CraftingItems.Count; i++)
        {
            CraftingItems[i] = null;
            CraftingAmounts[i] = 0;
        }

        Debug.Log($"제작 성공: {ResultItem.name} x{ResultAmount}");
        RefreshAllUI();
    }

    private bool CheckRecipeMatch(RecipeData recipe, List<int> currentIngredients)
    {
        if (recipe.ingredients.Count != currentIngredients.Count)
            return false;

        // 같은 재료 개수 비교
        Dictionary<int, int> recipeCount = new Dictionary<int, int>();
        Dictionary<int, int> currentCount = new Dictionary<int, int>();

        foreach (var id in recipe.ingredients)
        {
            if (!recipeCount.ContainsKey(id)) recipeCount[id] = 0;
            recipeCount[id]++;
        }

        foreach (var id in currentIngredients)
        {
            if (!currentCount.ContainsKey(id)) currentCount[id] = 0;
            currentCount[id]++;
        }

        if (recipeCount.Count != currentCount.Count) return false;

        foreach (var kvp in recipeCount)
        {
            if (!currentCount.ContainsKey(kvp.Key) || currentCount[kvp.Key] != kvp.Value)
                return false;
        }

        return true;
    }

    public void UnregisterCraftingBoxUI()
    {
        CraftingBoxUI = null;
    }
}



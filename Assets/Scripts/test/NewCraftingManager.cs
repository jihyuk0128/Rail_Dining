using System.Collections.Generic;
using UnityEngine;

public class NewCraftingManager
{
    private NewInventoryManager _inventory => Managers.NewInventory;

    // 카테고리 타입 정의
    private enum RecipeType { Ingredient = 1, Juice = 2, Beverage = 3, Cuisine = 4 }

    // 메뉴 생성 (UI에서 호출)
    public void CreateCraftingMenu(Transform parent, int filterType = 0)
    {
        if (parent == null)
        {
            Debug.LogWarning("[CraftingManager] 부모 Transform이 null입니다. 메뉴 생성 불가.");
            return;
        }

        // 기존 슬롯 제거
        foreach (Transform child in parent)
            Object.Destroy(child.gameObject);

        int count = 0;

        // 전체 레시피 순회
        foreach (var recipe in Managers.Data.RecipeDict.Values)
        {
            if (!Managers.Data.ItemDict.TryGetValue(recipe.resultId, out var resultItem))
                continue;

            // 3자리 type 값에서 앞자리 추출 (예: 301 → 3)
            int category = resultItem.id / 100;

            // 필터가 걸려 있으면 해당 타입만 표시
            if (filterType != 0 && category != filterType)
                continue;

            // 슬롯 생성
            GameObject go = Managers.Resource.Instantiate("TestPrefabs/UI_NewCraftingSlot", parent);
            var slot = go.GetComponent<UI_NewCraftingSlot>();
            slot.Init();
            slot.SetRecipe(recipe.recipeId);
            count++;
        }

        string filterName = filterType switch
        {
            1 => "Ingredient",
            2 => "Juice",
            3 => "Beverage",
            4 => "Cuisine",
            _ => "All"
        };

        Debug.Log($"[CraftingManager] 메뉴 생성 완료 (필터: {filterName}, 생성된 슬롯 수: {count})");
    }

    // 레시피 상세 보기
    public void ShowRecipeDetail(UI_RecipeDetail ui, int recipeId)
    {
        if (!Managers.Data.RecipeDict.TryGetValue(recipeId, out var recipe))
        {
            Debug.LogWarning($"[CraftingManager] 레시피 ID {recipeId} 없음");
            return;
        }

        // 결과 아이콘 설정
        var resultItem = Managers.Data.ItemDict[recipe.resultId];
        //ui.ResultSlot.sprite = Resources.Load<Sprite>(resultItem.iconPath);

        // --- 재료 슬롯 생성 ---
        //var grid = ui.GridParent;

        //foreach (Transform child in grid)
        //    Object.Destroy(child.gameObject);

        //foreach (int ingId in recipe.ingredients)
        //{
        //    if (ingId == 0) continue;
        //
        //    var go = Managers.Resource.Instantiate("TestPrefabs/UI_NewInventorySlot", grid);
        //    var slot = go.GetComponent<UI_ParentSlot>();
        //    slot.Init();
        //    slot.SetData(new ItemSlot { Item = Managers.Data.ItemDict[ingId], Amount = 1 });
        //}

        Debug.Log($"[CraftingManager] 레시피 상세 구성 완료 (ID:{recipeId})");
    }


    // 제작 시도
    public void TryCraft(int recipeId)
    {
        if (!Managers.Data.RecipeDict.TryGetValue(recipeId, out var recipe))
        {
            Debug.LogWarning($"[CraftingManager] 제작 실패 - 레시피 ID {recipeId}를 찾을 수 없음.");
            return;
        }

        // 재료 확인
        foreach (int ingredientId in recipe.ingredients)
        {
            if (!_inventory.HasItem(ingredientId))
            {
                Debug.LogWarning($"[CraftingManager] 재료 부족 (ID:{ingredientId})");
                return;
            }
        }

        // 재료 제거
        foreach (int ingredientId in recipe.ingredients)
            _inventory.RemoveItem(ingredientId);

        // 결과물 추가
        _inventory.AddItem(recipe.resultId);
        Debug.Log($"[CraftingManager] 제작 성공: {recipe.resultId}");

        _inventory.RefreshUI();
    }
}

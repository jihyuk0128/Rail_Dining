using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Experimental.AI;

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
            UnityEngine.Object.Destroy(child.gameObject);

        int count = 0;

        // 전체 레시피 순회
        foreach (var recipe in Managers.Data.RecipeDict.Values)
        {
            Debug.Log($"recipe{recipe.recipeId} 생성");
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

    // 제작 시도
    public bool TryCraft(int recipeId)
    {
        if (!Managers.Data.RecipeDict.TryGetValue(recipeId, out var recipe))
        {
            Debug.LogWarning($"[CraftingManager] 제작 실패 - 레시피 ID {recipeId}를 찾을 수 없음.");
            return false;
        }

        // 재료 확인
        foreach (int ingredientId in recipe.ingredients)
        {
            if (ingredientId == 0)
                continue; // 빈칸은 무시

            if (!_inventory.HasItem(ingredientId))
            {
                Debug.LogWarning($"[CraftingManager] 재료 부족 (ID:{ingredientId})");
                return false;
            }
        }

        return true;
    }

    public void StartMiniGame(int recipeId)
    {
        if (!Managers.Data.RecipeDict.TryGetValue(recipeId, out var recipe))
        {
            Debug.LogWarning($"[CraftingManager] 레시피 ID {recipeId}를 찾을 수 없습니다!");
            return;
        }
        int resultId = recipe.resultId;

        if (_minigameMapping.TryGetValue(resultId, out var gameType))
        {
            Debug.Log($"[CraftingManager] resultId {resultId}: {gameType.Name} 실행!");
            var miniGamePopup = Managers.UI.ShowPopupUI(gameType) as UI_Popup;

            if (miniGamePopup is IHasMiniGameEnd notifier)
            {
                notifier.OnMiniGameEnd += (success) =>
                {
                    Debug.Log($"미니게임 완료! 성공여부: {success}");
                    Managers.UI.ClosePopupUI();
                    var popup = Managers.UI.ShowPopupUI<UI_CreateResult>();
                    popup.TryCraft(true, recipeId);
                };

            }

        }
    }

    public void SuccessCraft(int recipeId)
    {
        if (!Managers.Data.RecipeDict.TryGetValue(recipeId, out var recipe))
        {
            Debug.LogWarning($"[CraftingManager] 제작 실패 - 레시피 ID {recipeId}를 찾을 수 없음.");
            return;
        }

        // 재료 제거
        foreach (int ingredientId in recipe.ingredients)
            _inventory.RemoveItem(ingredientId);

        // 결과물 추가
        _inventory.AddItem(recipe.resultId);
        Debug.Log($"[CraftingManager] 제작 성공: {recipe.resultId}");

        _inventory.RefreshUI();
    }

    public void FailCraft(int recipeId)
    {
        if (!Managers.Data.RecipeDict.TryGetValue(recipeId, out var recipe))
        {
            Debug.LogWarning($"[CraftingManager] 제작 실패 - 레시피 ID {recipeId}를 찾을 수 없음.");
            return;
        }

        // 재료 제거
        foreach (int ingredientId in recipe.ingredients)
            _inventory.RemoveItem(ingredientId);

        _inventory.RefreshUI();
    }

    private Dictionary<int, Type> _minigameMapping = new Dictionary<int, Type>
    {
        // ───────────────────────────────
        // 200대: 주스류 (착즙기)
        // ───────────────────────────────
        { 201, typeof(UI_JuicerGame) }, // Strawberry_juice
        { 202, typeof(UI_JuicerGame) }, // Orange_juice
        { 203, typeof(UI_JuicerGame) }, // Lemon_juice

        // ───────────────────────────────
        // 300대: 커피류 (섞기)
        // ───────────────────────────────
        { 301, typeof(UI_StirDrinkGame) }, // coffee_latte
        { 302, typeof(UI_StirDrinkGame) }, // Coffee
        { 303, typeof(UI_StirDrinkGame) }, // Strawberry_latte

        // ───────────────────────────────
        // 400대: 음식류 (프라이팬/조리)
        // ───────────────────────────────
        { 401, typeof(UI_FryPanButterGame) }, // Egg_toast
        { 402, typeof(UI_FryPanButterGame) }, // Fried_egg
        { 403, typeof(UI_FryPanButterGame) }, // roasted_sausage
        { 404, typeof(UI_FryPanFlipGame) }, // Sausage_egg_toast
        { 405, typeof(UI_FryPanFlipGame) }, // sausage_omelet
        { 406, typeof(UI_FryPanFlipGame) }, // hot_dog
    };
}



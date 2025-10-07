using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Recipe : UI_Base
{
    enum GameObjects 
    {
        Grid
    }

    private List<UI_ImageSlot> _slots = new();


    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        var grid = GetObject((int)GameObjects.Grid).transform;
        for (int i = 0; i < 4; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/UI_ImageSlot", grid);
            var slot = go.GetComponent<UI_ImageSlot>();
            slot.Init();
            _slots.Add(slot);
        }
    }

    public void SetImageUI(RecipeData recipe)
    {
        if (recipe == null)
        {
            Debug.LogWarning("[UI_Recipe] RecipeData가 null임");
            return;
        }

        // 슬롯 초기화 (모두 비활성화)
        for (int i = 0; i < _slots.Count; i++)
            _slots[i].SetImage(null);

        // 레시피 재료 순서대로 아이콘 설정
        for (int i = 0; i < recipe.ingredients.Count && i < _slots.Count; i++)
        {
            int itemId = recipe.ingredients[i];

            // 0이거나 없는 아이템 → 빈칸으로 처리
            if (itemId == 0)
            {
                _slots[i].SetImage(null);
                continue;
            }

            if (Managers.Data.ItemDict.TryGetValue(itemId, out var itemData))
            {
                _slots[i].SetImage(itemData);
            }
            else
            {
                Debug.LogWarning($"[UI_Recipe] Item ID {itemId} 존재하지 않음 → 빈칸 처리");
                _slots[i].SetImage(null);
            }
        }

        Debug.Log($"[UI_Recipe] 레시피 {recipe.recipeId} UI 세팅 완료 (결과: {recipe.resultId})");
    }
  
}

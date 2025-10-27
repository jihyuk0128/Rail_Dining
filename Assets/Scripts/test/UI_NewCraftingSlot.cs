using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NewCraftingSlot : UI_ParentSlot
{
    private int recipeId;

    public override void Init()
    {
        base.Init();
        BindEvent(gameObject, OnClick, Define.UIEvent.Click);
    }

    // 레시피를 연결하고 결과물 아이콘 표시
    public void SetRecipe(int id)
    {
        recipeId = id;

        // 레시피에서 결과 아이템 데이터 가져오기
        if (Managers.Data.RecipeDict.TryGetValue(recipeId, out var recipe))
        {
            var resultItem = Managers.Data.ItemDict[recipe.resultId];

            // UI에 표시할 데이터 세팅
            slotData = new ItemSlot { Item = resultItem };
            Refresh();
        }
    }

    // 클릭 시 해당 레시피의 상세 재료 UI를 띄움
    void OnClick(PointerEventData data)
    {
        if (recipeId == 0)
            return;

        Debug.Log($"[CraftingSlot] 레시피 클릭됨: {recipeId}");

        //Managers.UI.ShowPopupUI<UI_RecipeDetail>().SetRecipe(RecipeId);

        // 상세화면 띄우는코드는 여기에 넣으면댐

        var popup = Managers.UI.ShowPopupUI<UI_RecipeDetail>();
        popup.Init();

        // CraftingManager에게 “슬롯 채워라” 요청
        Managers.Crafting.ShowRecipeDetail(popup, recipeId);
    }

    public override void Refresh()
    {
        base.Refresh();
    }
}

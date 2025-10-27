using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BasicScene : UI_Scene
{
    enum Buttons { UI_MenuButton, }
    enum GameObjects
    {
        UI_Clock,
        UI_Inventory,
        UI_RecipeGrid,
    }


    List<RecipeData> _RecipeList = new();

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        // 자식 오브젝트 자동 바인딩
        Bind<GameObject>(typeof(GameObjects));
        GetButton((int)Buttons.UI_MenuButton).gameObject.BindEvent((PointerEventData data) => { Managers.UI.ShowPopupUI<UI_Menu>(); });
    }

    public void UpdateRecipe(List<RecipeData> recipelist)
    {
        // 레시피 가져오는함수
        // 네트워크추가이후 추가해야함.
        _RecipeList = recipelist;

        ///

        var recipeGrid = GetObject((int)GameObjects.UI_RecipeGrid).transform;
        foreach (Transform child in recipeGrid)
            GameObject.Destroy(child.gameObject); // 기존 UI 삭제

        for (int i = 0; i < _RecipeList.Count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Recipe", recipeGrid);
            var recipe = go.GetComponent<UI_Recipe>();
            recipe.Init();
            recipe.SetImageUI(recipelist[i]);
        }
    }

    /// <summary>
    /// 손님이 주문할 때 호출됨 (아이템데이터로)
    /// </summary>
    public void AddOrder(ItemData orderedItem)
    {
        RecipeData recipe = null;

        // 1? 주문한 아이템이 레시피 결과인지 확인
        foreach (var r in Managers.Data.RecipeDict.Values)
        {
            if (r.resultId == orderedItem.id)
            {
                recipe = r;
                break;
            }
        }

        // 2? UI 생성
        var recipeGrid = GetObject((int)GameObjects.UI_RecipeGrid).transform;
        var go = Managers.Resource.Instantiate("UI/Scene/UI_Recipe", recipeGrid);
        var recipeUI = go.GetComponent<UI_Recipe>();
        recipeUI.Init();

        // 3? 레시피 존재 여부에 따라 표시 방식 변경
        if (recipe != null)
        {
            recipeUI.SetRecipe(recipe);
            _RecipeList.Add(recipe);
        }
        else
        {
            recipeUI.SetSingleItem(orderedItem);
        }

        Debug.Log($"[UI_BasicScene] 주문 추가됨 → {orderedItem.name}");
    }

    /// <summary>
    /// 주문 완료 시 주문내역에서 제거
    /// </summary>
    public void RemoveOrder(ItemData servedItem)
    {
        var recipeGrid = GetObject((int)GameObjects.UI_RecipeGrid).transform;

        foreach (Transform child in recipeGrid)
        {
            var recipeUI = child.GetComponent<UI_Recipe>();
            if (recipeUI != null && recipeUI.MatchesItemId(servedItem.id))
            {
                GameObject.Destroy(child.gameObject);
                Debug.Log($"[UI_BasicScene] 주문 제거됨 → {servedItem.name}");
                break;
            }
        }
    }

    /// <summary>
    /// 현재 주문 수 반환 (최대 8개 제한 체크용)
    /// </summary>
    public int GetOrderCount()
    {
        var recipeGrid = GetObject((int)GameObjects.UI_RecipeGrid).transform;
        return recipeGrid.childCount;
    }

    // 주문내역 표시
    public void SetRecipeVisible(bool visible)
    {
        var grid = GetObject((int)GameObjects.UI_RecipeGrid);
        if (grid != null)
            grid.SetActive(visible);
    }
}
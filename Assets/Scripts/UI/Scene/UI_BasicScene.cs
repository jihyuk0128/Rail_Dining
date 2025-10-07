using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UI_BasicScene : UI_Scene
{
    enum GameObjects
    {
        UI_Clock,
        UI_Inventory,
        UI_RecipeGrid
    }

    List<RecipeData> _RecipeList = new();

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 자식 오브젝트 자동 바인딩
        Bind<GameObject>(typeof(GameObjects));
        if (Managers.Data.RecipeDict.TryGetValue(1, out var recipe))
        {
            List<RecipeData> _RecipeList1 = new List<RecipeData>();
            _RecipeList.Add(recipe);
            UpdateRecipe(_RecipeList);
            Debug.Log("레시피찾음");
        }
        else
        {
            Debug.LogWarning("레시피 ID 1을 찾을 수 없습니다!");
        }
    }

    public void UpdateRecipe(List<RecipeData> recipelist)
    {
        // 레시피 가져오는함수
        // 네트워크추가이후 추가해야함.
        _RecipeList = recipelist;

        ///

        for (int i = 0; i < _RecipeList.Count; i++)
        {
            var recipeGrid = GetObject((int)GameObjects.UI_RecipeGrid).transform;

            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Recipe", recipeGrid);
            var recipe = go.GetComponent<UI_Recipe>();
            recipe.Init();
            recipe.SetImageUI(recipelist[i]);
            
        }
    }
}
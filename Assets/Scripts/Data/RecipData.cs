using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecipeData
{
    public int recipeId;
    public List<int> ingredients; // 재료 아이템 ID 목록
    public int resultId;          // 결과 아이템 ID
    public int resultAmount;      // 결과 아이템 개수
}

[Serializable]
public class RecipeDataLoader : ILoader<int, RecipeData>
{
    public List<RecipeData> recipes = new List<RecipeData>();

    public Dictionary<int, RecipeData> MakeDict()
    {
        Dictionary<int, RecipeData> dict = new Dictionary<int, RecipeData>();
        foreach (var recipe in recipes)
            dict[recipe.recipeId] = recipe;
        return dict;
    }
}

using System;
using System.Collections.Generic;

[Serializable]
public class RecipeData
{
    public int recipeId;
    public List<int> ingredients;
    public int resultId;
}

[Serializable]
public class RecipeDataLoader : ILoader<int, RecipeData>
{
    public List<RecipeData> drink_recipes = new List<RecipeData>();

    public Dictionary<int, RecipeData> MakeDict()
    {
        var dict = new Dictionary<int, RecipeData>();
        foreach (var recipe in drink_recipes)
            dict[recipe.recipeId] = recipe;
        return dict;
    }
}
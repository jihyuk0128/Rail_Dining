using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, ItemData> ItemDict { get; private set; }
    public Dictionary<int, RecipeData> RecipeDict { get; private set; }

    public void Init()
    {
        ItemDict = LoadJson<ItemDataLoader, int, ItemData>("ItemData").MakeDict();
        RecipeDict = LoadJson<RecipeDataLoader, int, RecipeData>("RecipeData").MakeDict();

        Debug.Log($"[DataManager] Loaded Items: {ItemDict.Count}, Recipes: {RecipeDict.Count}");
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}

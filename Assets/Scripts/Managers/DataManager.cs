using NUnit.Framework.Interfaces;
using System;
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
        // 아이템 데이터 로드
        ItemDict = LoadJson<ItemDataLoader, int, ItemData>("ItemData").MakeDict();

        // 레시피 데이터 로드
        RecipeDict = LoadJson<RecipeDataLoader, int, RecipeData>("drink_recipes").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
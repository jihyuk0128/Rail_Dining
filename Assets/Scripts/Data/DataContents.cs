using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int id;
    public string name;
    public int maxStack;
    public string iconPath; // 아이콘 리소스 경로
}

[Serializable]
public class ItemDataLoader : ILoader<int, ItemData>
{
    public List<ItemData> items = new List<ItemData>();

    public Dictionary<int, ItemData> MakeDict()
    {
        Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
        foreach (var item in items)
            dict[item.id] = item;
        return dict;
    }
}

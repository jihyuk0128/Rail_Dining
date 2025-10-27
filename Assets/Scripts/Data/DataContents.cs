using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int id;
    public string name;
    public string iconPath;
}

[Serializable]
public class ItemDataLoader : ILoader<int, ItemData>
{
    public List<ItemData> items = new List<ItemData>();

    public Dictionary<int, ItemData> MakeDict()
    {
        var dict = new Dictionary<int, ItemData>();
        foreach (var item in items)
            dict[item.id] = item;
        return dict;
    }
}

[System.Serializable]
public class ItemSlot
{
    public ItemData Item;
    public int Amount;

    public const int MAX_STACK = 1; // 슬롯당 최대 개수

    public bool IsFull => Amount >= MAX_STACK;

    public void Clear()
    {
        Item = null;
        Amount = 0;
    }

    public bool CanAddItem(int addAmount)
    {
        return Item != null && Amount + addAmount <= MAX_STACK;
    }

    public int AddItem(int addAmount)
    {
        if (Item == null) return addAmount; // 아무것도 없으면 실패
        int spaceLeft = MAX_STACK - Amount;

        if (addAmount <= spaceLeft)
        {
            Amount += addAmount;
            return 0; // 다 들어감
        }
        else
        {
            Amount = MAX_STACK;
            return addAmount - spaceLeft; // 남은 양 반환
        }
    }
}
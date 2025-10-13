using UnityEngine;
using System.Collections.Generic;

public class StoregeChest : MonoBehaviour, IInteractable
{
    UI_Chest chest;
    // IInteractable ±¸Çö
    public void Interact(GameObject player)
    {
        if (chest != null) return;  
        
        chest = Managers.UI.ShowPopupUI<UI_Chest>();
        Managers.Inventory.AddItemToChest(101);
        Managers.Inventory.AddItemToChest(102);
        Managers.Inventory.AddItemToChest(103);
        Managers.Inventory.AddItemToChest(104);
        Managers.Inventory.AddItemToChest(106);
        Managers.Inventory.AddItemToChest(108);
        Managers.Inventory.AddItemToChest(112);
        Managers.Inventory.AddItemToChest(113);
        Managers.Inventory.AddItemToChest(114);
        Managers.Inventory.AddItemToChest(116);
        Managers.Inventory.AddItemToChest(119);
        Managers.Inventory.AddItemToChest(120);
    }

    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 2;
}
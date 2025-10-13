using UnityEngine;
using System.Collections.Generic;

public class StoregeChest : MonoBehaviour, IInteractable
{
    // IInteractable 구현
    public void Interact(GameObject player)
    {
        Debug.Log($"{chestName} 상자와 상호작용함!");
        //chest = Managers.UI.ShowPopupUI<UI_Chest>().SetChest(this);
    }

    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 2;

   
}
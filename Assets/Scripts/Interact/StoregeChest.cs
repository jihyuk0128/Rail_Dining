using UnityEngine;
using System.Collections.Generic;

public class StoregeChest : MonoBehaviour, IInteractable
{
    // IInteractable ±¸Çö
    public void Interact(GameObject player)
    {
        //chest = Managers.UI.ShowPopupUI<UI_Chest>().SetChest(this);
    }

    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 2;

   
}
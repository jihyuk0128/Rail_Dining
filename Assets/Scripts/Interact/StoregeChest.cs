using UnityEngine;
using System.Collections.Generic;

public class StoregeChest : MonoBehaviour, IInteractable
{
    [SerializeField] private int ItemID = 0;
    // IInteractable ±¸Çö
    public void Interact(GameObject player)
    {
        Managers.NewInventory.AddItem(ItemID);
        SoundManager.Instance.PlaySFX("GetItem_SFX");
    }

    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 2;
}
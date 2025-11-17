using UnityEngine;
using UnityEngine.EventSystems;

public class BarTable : MonoBehaviour, IInteractable
{
    UI_CraftingBox craftingBox;
    public void Interact(GameObject player) 
    {
        if (craftingBox == null)
        {
             
        }
           
    }   
    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

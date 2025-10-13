using UnityEngine;

public class BarTable : MonoBehaviour, IInteractable
{
    UI_CraftingBox craftingBox;
    public void Interact(GameObject player) 
    {
        if (craftingBox == null)
            craftingBox = Managers.UI.ShowPopupUI<UI_CraftingBox>();
    }   
    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

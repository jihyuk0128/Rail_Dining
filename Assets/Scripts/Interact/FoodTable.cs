using UnityEngine;

public class FoodTable : MonoBehaviour, IInteractable
{
    UI_FoodBox foodBox;
    public void Interact(GameObject player)
    {
        if (foodBox == null)
            foodBox = Managers.UI.ShowPopupUI<UI_FoodBox>();
    }
    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

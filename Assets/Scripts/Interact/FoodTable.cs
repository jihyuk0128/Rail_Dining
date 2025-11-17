using UnityEngine;

public class FoodTable : MonoBehaviour, IInteractable
{
    UI_Menu foodBox;
    public void Interact(GameObject player)
    {
        if (foodBox == null)
        {
            var popup = Managers.UI.ShowPopupUI<UI_Menu>();
            popup.MenuInit(4);
        }
    }
    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

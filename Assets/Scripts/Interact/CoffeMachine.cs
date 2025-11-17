using UnityEngine;

public class CoffeMachine : MonoBehaviour, IInteractable
{
    UI_Menu CoffeBox;
    public void Interact(GameObject player)
    {
        if (CoffeBox == null)
        {
            var popup = Managers.UI.ShowPopupUI<UI_Menu>();
            popup.MenuInit(3);
        }
    }
    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

using UnityEngine;

public class JuiceTable : MonoBehaviour, IInteractable
{
    UI_Menu JuiceBox;
    public void Interact(GameObject player)
    {
        if (JuiceBox == null)
        {
            var popup = Managers.UI.ShowPopupUI<UI_Menu>();
            popup.MenuInit(2);
        }
    }
    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}


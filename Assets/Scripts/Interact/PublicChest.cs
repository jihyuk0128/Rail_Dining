using UnityEngine;

public class PublicChest : MonoBehaviour, IInteractable
{
   UI_PublicChest chest;
    public void Interact(GameObject player)
    {
        if (chest == null)
        {
            var popup = Managers.UI.ShowPopupUI<UI_PublicChest>();
        }
    }
    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

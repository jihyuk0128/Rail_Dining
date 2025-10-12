using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject InteractionIcon; // E 아이콘 오브젝트 (자식 오브젝트 등)

    private void Awake()
    {
        if (InteractionIcon != null)
            InteractionIcon.SetActive(false);
    }

    public void SetVisible(bool show)
    {
        if (InteractionIcon != null)
            InteractionIcon.SetActive(show);
    }
}

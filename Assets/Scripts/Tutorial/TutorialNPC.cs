using UnityEngine;

public class TutorialNPC : MonoBehaviour, IInteractable
{
    public bool IsServed { get; private set; } = false;
    private bool canInteract = false;

    public void EnableInteraction(bool value)
    {
        canInteract = value;
    }

    public void Interact(GameObject player)
    {
        if (!canInteract || IsServed) return;
        Debug.Log("튜토리얼 NPC: 음료 받음");
        IsServed = true;
    }

    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

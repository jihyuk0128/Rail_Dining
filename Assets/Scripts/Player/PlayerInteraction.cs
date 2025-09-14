using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private List<IInteractable> interactables = new List<IInteractable>();

    public void Interacting(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("상호작용 키 누름");
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (interactables.Count == 0) return;

        var best = interactables
            .OrderBy(i => i.GetPriority(gameObject))
            .ThenBy(i => Vector3.Distance(transform.position, i.GetPosition()))
            .FirstOrDefault();

        best?.Interact(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("상호작용 콜라이더 충돌");
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !interactables.Contains(interactable))
            interactables.Add(interactable);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("상호작용 콜라이더 충돌 끝");
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            interactables.Remove(interactable);
    }
}

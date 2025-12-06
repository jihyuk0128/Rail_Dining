using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private List<IInteractable> interactables = new List<IInteractable>();
    private PlayerController player;
    private TrainEventManager train;

    private IInteractable currentTarget; // 현재 가장 가까운 상호작용 대상 캐싱

    private bool interactActive = true;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        train = FindObjectOfType<TrainEventManager>();
    }

    private void Update()
    {
        HighlightNearestInteractable();
    }

    public void Interacting(InputAction.CallbackContext context)
    {
        if (context.started && !player.isEventActive && !player.IsFall && !train.IsEventActive() && !Managers.UI.IsPopupOpen())
        {
            Debug.Log("상호작용 키 누름");
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (currentTarget == null) return;
        currentTarget.Interact(gameObject);
    }

    private void HighlightNearestInteractable()
    {
        if (interactables.Count == 0)
        {
            ClearHighlight();
            return;
        }

        // 상호작용 불가능한 동안 상호작용 아이콘 끄기
        if (player.isEventActive || player.IsFall || train.IsEventActive())
        {
            ClearHighlight(false);
            return;
        }

        // 우선순위 및 거리 기준으로 가장 가까운 상호작용 대상 찾기
        var best = interactables
            .OrderBy(i => i.GetPriority(gameObject))
            .ThenBy(i => Vector3.Distance(transform.position, i.GetPosition()))
            .FirstOrDefault();

        // 이전 대상과 다르면 하이라이트 갱신
        if (best != currentTarget)
        {
            ClearHighlight();

            currentTarget = best;

            // 손님이라면 UI 활성화
            if (currentTarget is Customer customer)
            {
                var ui = customer.GetComponentInChildren<CustomerOrderUI>();
                if (ui != null && !player.isEventActive && !player.IsFall && !train.IsEventActive())
                    ui.SetInteractionVisible(true);
            }
            else if (currentTarget is TutorialNPC Npc)
            {
                var ui = Npc.GetComponentInChildren<CustomerOrderUI>();
                if (ui != null)
                    ui.SetInteractionVisible(true);
            }
            else
            {
                // 일반 오브젝트의 InteractionUI 표시
                var ui = (currentTarget as MonoBehaviour)?.GetComponentInChildren<InteractionUI>();
                if (ui != null && !player.isEventActive && !player.IsFall && !train.IsEventActive())
                    ui.SetVisible(true);
            }
        }
        else if (!interactActive)
        {
            // 손님이라면 UI 활성화
            if (currentTarget is Customer customer)
            {
                var ui = customer.GetComponentInChildren<CustomerOrderUI>();
                if (ui != null && !player.isEventActive && !player.IsFall && !train.IsEventActive())
                    ui.SetInteractionVisible(true);
            }
            else
            {
                // 일반 오브젝트의 InteractionUI 표시
                var ui = (currentTarget as MonoBehaviour)?.GetComponentInChildren<InteractionUI>();
                if (ui != null && !player.isEventActive && !player.IsFall && !train.IsEventActive())
                    ui.SetVisible(true);
            }
            interactActive = true;
        }
    }

    private void ClearHighlight(bool clear = true)
    {
        if (currentTarget != null)
        {
            if (currentTarget is Customer prevCustomer)
            {
                var ui = prevCustomer.GetComponentInChildren<CustomerOrderUI>();
                if (ui != null)
                    ui.SetInteractionVisible(false);
            }
            else if (currentTarget is TutorialNPC prevNpc)
            {
                var ui = prevNpc.GetComponentInChildren<CustomerOrderUI>();
                if (ui != null)
                    ui.SetInteractionVisible(false);
            }
            else
            {
                var ui = (currentTarget as MonoBehaviour)?.GetComponentInChildren<InteractionUI>();
                if (ui != null)
                    ui.SetVisible(false);
            }
            if (clear)
                currentTarget = null;
            else
                interactActive = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !interactables.Contains(interactable))
            interactables.Add(interactable);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactables.Remove(interactable);

            // 만약 빠져나간 대상이 현재 하이라이트 중이라면 지우기
            if (currentTarget == interactable)
                ClearHighlight();
        }
    }
}
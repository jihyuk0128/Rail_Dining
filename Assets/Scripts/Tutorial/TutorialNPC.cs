using UnityEngine;

public enum TutorialState
{
    WaitingForOrder,
    WaitingForDrink
}
// 바뀐거적용점

public class TutorialNPC : MonoBehaviour, IInteractable
{
    public bool IsServed { get; private set; } = false;
    private bool canInteract = false;
    public TutorialState State;
    public ItemData orderMenu;
    public int servCount = 0;

    private CustomerOrderUI orderUI;

    private void Start()
    {
        orderUI = GetComponentInChildren<CustomerOrderUI>();
        State = TutorialState.WaitingForOrder;
        orderUI.ShowWaiting();
    }

    public void EnableInteraction(bool value)
    {
        canInteract = value;

    }


    public void Interact(GameObject player)
    {
        if (!canInteract || IsServed) return;

        IsServed = true;

        if (State == TutorialState.WaitingForOrder)
        {
            State = TutorialState.WaitingForDrink;
            orderMenu = Managers.Data.ItemDict[210];
            Debug.Log($"손님이 {orderMenu.name} 를 주문했습니다!");
            orderUI.ShowOrder(orderMenu, 0, 2);
        }
        else
        {

            //if (orderMenu != )
            //    return;
            
            if (servCount < 2 )
            {
                if (Managers.Inventory.CheckItemToRemove(orderMenu))
                {
                    servCount++;
                    
                }
            }
            if(servCount == 2)
            {
                orderUI.HideOrder();
                Debug.Log("튜토리얼 NPC: 음료 받음");
                IsServed = true;
            }
        }
    }

    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

using System.Collections;
using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [Header("설정")]
    public float autoCloseDelay = 2f;  // 문이 자동으로 닫히기까지 시간
    public GameObject BarPoint;
    public GameObject StoregePoint;
    private bool isBarPoint = false;

    public bool isOpen { get; private set; } = false;
    private Coroutine autoCloseCoroutine;

    // 플레이어가 상호작용 키를 눌렀을 때
    public void Interact(GameObject player)
    {
        if (isOpen)
            return;

        //OpenDoor();
        ChangePosition(player);
    }

    private void ChangePosition(GameObject player)
    {
        if (isBarPoint == true)
            player.transform.position = StoregePoint.transform.position;
        else
            player.transform.position = BarPoint.transform.position;
    }

    public void SetIsBarPoint(bool set)
    {
        isBarPoint = set;
    }

    private void OpenDoor()
    {
        isOpen = true;

        // 문 통과 가능하게 충돌체 비활성화
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

    }

    public void CloseDoor()
    {
        isOpen = false;

        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;
    }

    // IInteractable용
    public Vector3 GetPosition() => transform.position;
    public int GetPriority(GameObject player) => 1;
}

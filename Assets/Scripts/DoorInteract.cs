using System.Collections;
using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [Header("설정")]
    public float autoCloseDelay = 2f;  // 문이 자동으로 닫히기까지 시간

    private bool isOpen = false;
    private Coroutine autoCloseCoroutine;

    // 플레이어가 상호작용 키를 눌렀을 때
    public void Interact(GameObject player)
    {
        if (isOpen)
            return;

        OpenDoor();
    }

    private void OpenDoor()
    {
        isOpen = true;

        // 문 통과 가능하게 충돌체 비활성화
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // 자동 닫힘 시작
        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);
        autoCloseCoroutine = StartCoroutine(AutoCloseDoor());
    }

    private IEnumerator AutoCloseDoor()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        CloseDoor();
    }

    private void CloseDoor()
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

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("카메라 이동 속도")]
    public float moveSpeed = 5f;

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void MoveToRoom(Vector3 roomPosition)
    {
        targetPosition = new Vector3(roomPosition.x, roomPosition.y, transform.position.z);
    }
}

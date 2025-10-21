using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("이 트리거가 보여줄 카메라 위치")]
    public Transform cameraTarget;
    public DoorInteract door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어만 반응
        {
            CameraController cam = Camera.main.GetComponent<CameraController>();
            if (cam != null && cameraTarget != null)
            {
                cam.MoveToRoom(cameraTarget.position);
            }
            if(door != null)
            {
                /*
                if (door.isOpen)
                    door.CloseDoor();
                */
                if (cameraTarget.transform.position.x == 0)
                    door.SetIsBarPoint(true);
                else
                    door.SetIsBarPoint(false);
            }
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("카메라 이동 속도")]
    public float moveSpeed = 5f;
    public float zoomSpeed = 5f;
    public GameObject player;
    public float CloseSize = 3.0f;
    public float DefaultSize = 6.0f;

    private TrainEventManager train;
    private Vector3 targetPosition;
    private Camera camera;

    private void Start()
    {
        targetPosition = transform.position;
        train = FindObjectOfType<TrainEventManager>();
        player = GameObject.FindWithTag("Player");
        camera = gameObject.GetComponent<Camera>();
    }

    private void Update()
    {

        if (train != null && train.IsEventActive()) 
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10.0f);
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, CloseSize, zoomSpeed * Time.deltaTime);
        }
        else
        {
            // 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            camera.orthographicSize = DefaultSize;   
        }
    }

    public void MoveToRoom(Vector3 roomPosition)
    {
        targetPosition = new Vector3(roomPosition.x, roomPosition.y, transform.position.z);
    }
}

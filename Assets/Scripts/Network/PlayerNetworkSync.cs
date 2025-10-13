using UnityEngine;

public class PlayerNetworkSync : MonoBehaviour
{
    [Header("보간 속도 설정")]
    public float positionSmoothTime = 0.07f;
    public float rotationSmoothTime = 0.07f;

    private Vector3 velocity = Vector3.zero;
    private float angularVelocity = 0f;

    [HideInInspector] public Vector3 targetPos;
    [HideInInspector] public float targetRotZ;

    // 다시 추가된 필드
    [HideInInspector] public Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
    }

    public void SetTarget(Vector3 newPos, float newRotZ)
    {
        if (Vector3.Distance(newPos, transform.position) > 2f)
            transform.position = newPos;

        targetPos = newPos;
        targetRotZ = newRotZ;
    }

    void Update()
    {
        // 위치 보간
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            positionSmoothTime
        );

        // 회전 보간
        float currentZ = transform.eulerAngles.z;
        float newZ = Mathf.SmoothDampAngle(
            currentZ,
            targetRotZ,
            ref angularVelocity,
            rotationSmoothTime
        );

        transform.rotation = Quaternion.Euler(0, 0, newZ);
    }
}
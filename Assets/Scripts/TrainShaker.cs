using UnityEngine;

public class TrainShaker : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeInterval = 10f;  // 흔들림 발생 주기 (초)
    public float shakeDuration = 0.5f; // 흔들림 지속 시간
    public float shakeMagnitude = 0.2f; // 흔들림 강도 (위아래 이동량)
    public float shakeRotation = 2f;    // 흔들림 시 회전량 (도 단위)

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float shakeTimer;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        shakeTimer = shakeInterval; // 시작하자마자 흔들림 발생
    }

    private void Update()
    {
        // 주기 타이머
        shakeTimer += Time.deltaTime;
        if (shakeTimer >= shakeInterval)
        {
            shakeTimer = 0f;
            StartCoroutine(ShakeRoutine());
        }
    }

    private System.Collections.IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // 위아래 흔들림
            float yOffset = Mathf.Sin(elapsed * Mathf.PI * 4) * shakeMagnitude;

            // 좌우 회전 흔들림
            float zRotation = Mathf.Sin(elapsed * Mathf.PI * 4) * shakeRotation;

            transform.localPosition = initialPosition + new Vector3(0f, yOffset, 0f);
            transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

            yield return null;
        }

        // 흔들림 끝나면 원래 위치와 회전으로 복원
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
    }
}

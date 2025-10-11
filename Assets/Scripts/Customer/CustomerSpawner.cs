using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("손님 생성 설정")]
    public GameObject[] customerPrefab;   // 생성할 손님 Prefab
    public Transform spawnPoint;        // 손님이 등장할 위치
    public int spawnCount = 5;          // 생성할 손님 총 수
    public float spawnInterval = 2f;    // 손님 생성 간격 (초)

    private int spawned = 0;

    private void Start()
    {
        if (customerPrefab != null && spawnPoint != null)
        {
            StartCoroutine(SpawnCustomersRoutine());
        }
        else
        {
            Debug.LogWarning("CustomerSpawner: Prefab 또는 SpawnPoint가 할당되지 않았습니다.");
        }
    }

    private IEnumerator SpawnCustomersRoutine()
    {
        while (spawned < spawnCount)
        {
            SpawnCustomer();
            spawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCustomer()
    {
        // 임시 남여 랜덤 스폰
        int i = 0;
        if (Random.value < 0.5f) i = 1;
        else i = 0;
            GameObject newCustomer = Instantiate(customerPrefab[i], spawnPoint.position, Quaternion.identity, spawnPoint);
        newCustomer.name = $"Customer_{spawned + 1}";
        // 필요하면 Customer 스크립트 초기화 코드 추가 가능
    }

    // 인스펙터에서 손님 생성 즉시 테스트용 메서드
    [ContextMenu("스폰 테스트")]
    private void SpawnOneCustomer()
    {
        SpawnCustomer();
    }
}

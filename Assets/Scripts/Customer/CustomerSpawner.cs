using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UI_ShakeTrainEvent;

public class CustomerSpawner : MonoBehaviour
{
    [Header("손님 생성 설정")]
    public GameObject[] customerPrefab;   // 생성할 손님 Prefab
    public Transform spawnPoint;        // 손님이 등장할 위치
    //public int spawnCount = 5;          // 생성할 손님 총 수
    public float spawnInterval = 2f;    // 손님 생성 간격 (초)

    public int spawnedCount { get; private set; } = 0;
    public int SuccessCount { get; private set; } = 0;

    private bool isSpwaning = false;


    private void Start()
    {
        if (customerPrefab != null && spawnPoint != null)
        {
            Managers.Network.OnCustomerSpawn += SpawnCustomer;
        }
        else
        {
            Debug.LogWarning("CustomerSpawner: Prefab 또는 SpawnPoint가 할당되지 않았습니다.");
        }
    }

    //private IEnumerator SpawnCustomersRoutine()
    //{
    //    while (spawnedCount < spawnCount)
    //    {
    //        if(isSpwaning)
    //            SpawnCustomer();
    //        yield return new WaitForSeconds(spawnInterval);
    //    }
    //}

    private void SpawnCustomer(int gender,int customerid , float searchTime) // 여기서스폰
    {
        Managers.MainThread.Enqueue(() =>
        {

            GameObject newCustomer = Instantiate(customerPrefab[gender], spawnPoint.position, Quaternion.identity, transform);
            newCustomer.name = $"Customer_{customerid}";

            // Customer 컴포넌트 가져와서 초기화
            Customer customer = newCustomer.GetComponent<Customer>();
            if (customer != null)
            {
                customer.Init(customerid, gender, searchTime);
            }
            else
            {
                Debug.LogWarning("Customer 컴포넌트가 프리팹에 없습니다!");
            }
        });
    }

    //인스펙터에서 손님 생성 즉시 테스트용 메서드
    //[ContextMenu("스폰 테스트")]
    //private void SpawnOneCustomer()
    //{
    //    SpawnCustomer();
    //}
    public void AddSuccessCount()
    {
        SuccessCount++;
        Debug.Log(SuccessCount);
    }
    public void ReStart()
    {
        spawnedCount = 0;
        SuccessCount = 0;
    }
    public void StartSpawning() => isSpwaning = true;
    public void StopSpawning() => isSpwaning = false;

    private void OnDestroy()
    {
        Managers.Network.OnCustomerSpawn -= SpawnCustomer;
    }
}

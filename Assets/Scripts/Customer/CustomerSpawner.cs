using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UI_ShakeTrainEvent;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance { get; private set; }

    [Header("손님 생성 설정")]
    public GameObject[] customerPrefab;   // 생성할 손님 Prefab
    public Transform spawnPoint;        // 손님이 등장할 위치
    //public int spawnCount = 5;          // 생성할 손님 총 수
    public float spawnInterval = 2f;    // 손님 생성 간격 (초)

    
    public List<Customer> customers = new List<Customer>();


    public int spawnedCount { get; private set; } = 0;
    public int SuccessCount { get; private set; } = 0;

    private bool isSpwaning = false;


    private void Start()
    {
        if (customerPrefab != null && spawnPoint != null)
        {
            Managers.Network.OnCustomerSpawn += SpawnCustomer;
            Managers.Network.OnCustomerLeave += LeaveCustomer;
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

            Debug.Log($"[Spawn] 손님 생성 완료 → ID:{customerid}, Gender:{gender}, SearchTime:{searchTime}");

            // Customer 컴포넌트 가져와서 초기화
            Customer customer = newCustomer.GetComponent<Customer>();
            if (customer != null)
            {
                customer.Init(customerid, gender, searchTime);
                customers.Add(customer);
                Debug.Log($"[Spawn] Customer Init 완료 → {customer.name} , {customer.customerid}");
            }
            else
            {
                Debug.LogWarning("Customer 컴포넌트가 프리팹에 없습니다!");
            }
        });
    }

    public void LeaveCustomer(int id)
    {
        Managers.MainThread.Enqueue(() =>
        {
            Customer target = customers.Find(c => c.customerid == id);
            if (target == null)
            {
                Debug.Log("못찾음");
                return;

            }

            target.Leave();
            customers.Remove(target);
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
        Managers.Network.OnCustomerLeave -= LeaveCustomer;
    }
}

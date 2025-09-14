using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject player);      // 상호작용 실행
    Vector3 GetPosition();                 // 플레이어와 거리 계산용
    int GetPriority(GameObject player);    // 우선순위 계산 (가까운 오브젝트 선택 등)
}
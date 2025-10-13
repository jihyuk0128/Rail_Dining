using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject player);      // 상호작용으로 실행될 함수
    Vector3 GetPosition();                 // 플레이어와 거리 계산용 위치 가져오기
    int GetPriority(GameObject player);    // 우선순위 계산 (숫자가 낮을수록 높은 우선순위)
}
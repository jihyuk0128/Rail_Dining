using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    private int _money = 0;

    // 인스턴스 추가
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetMoney();
    }

    // 리셋
    public void ResetMoney()
    {
        _money = 0;
    }

    // 추가
    public void AddMoney(int amount)
    {
        _money += amount;
    }

    // 내보내기
    public int GetMoney()
    {
        return _money;
    }
}

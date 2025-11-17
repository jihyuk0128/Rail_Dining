using System;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance
    {
        get
        {
            if (s_instance == null)
                Init();
            return s_instance;
        }
    }

    // == 기본 매니저 ==
    ResourceManager _resource = new();
    UIManager _ui = new();
    DataManager _data = new();
    InventoryManager _inventory = new();
    NetworkManager _network = new();

    // == 새 시스템 ==
    NewSlotManager _slot = new();
    NewInventoryManager _newInventory = new();
    NewChestManager _newChest = new();
    NewCraftingManager _newCrafting = new();

    // == 기본 매니저 접근 ==
    public static ResourceManager Resource => Instance._resource;
    public static UIManager UI => Instance._ui;
    public static DataManager Data => Instance._data;
    public static InventoryManager Inventory => Instance._inventory;
    public static NetworkManager Network => Instance._network;

    // == 새 시스템 접근 ==
    public static NewSlotManager Slot => Instance._slot;
    public static NewInventoryManager NewInventory => Instance._newInventory;
    public static NewChestManager Chest => Instance._newChest;
    public static NewCraftingManager Crafting => Instance._newCrafting;

    // == 메인 스레드 디스패처 ==
    public static class MainThread
    {
        public static void Enqueue(Action action)
        {
            var dispatcher = UnityMainThreadDispatcher.Instance;

            if (dispatcher != null)
            {
                dispatcher.Enqueue(action);
            }
            else
            {
                Debug.LogWarning("[Managers.MainThread] Dispatcher Instance 없음!");
            }
        }
    }

    // == Unity ==
    void Start()
    {
        Application.runInBackground = true;
        Init();
    }

    void Update() { }

    // == 초기화 ==
    static void Init()
    {
        if (s_instance != null)
            return;

        // @Managers 오브젝트 찾거나 생성
        GameObject go = GameObject.Find("@Managers");

        if (go == null)
        {
            go = new GameObject("@Managers");
            s_instance = go.AddComponent<Managers>();
        }
        else
        {
            s_instance = go.GetComponent<Managers>();
            if (s_instance == null)
                s_instance = go.AddComponent<Managers>();
        }

        // Dispatcher 없으면 추가
        if (UnityMainThreadDispatcher.Instance == null)
        {
            go.AddComponent<UnityMainThreadDispatcher>();
        }

        DontDestroyOnLoad(go);

        // === 초기화 ===
        s_instance._data.Init();
        //s_instance._newChest.Init(8);        //  Chest 초기화
        s_instance._newInventory.Init(4); //  필요하면 여기서 초기화
        s_instance._newChest.Init(8);
        // 테스트용 아이템 몇 개 추가
        s_instance._newInventory.AddItem(105);
        s_instance._newInventory.AddItem(105);
        s_instance._newInventory.AddItem(102);
        //Managers.NewInventory.AddItem(104);
        // s_instance._newCrafting.Init();   //  필요하면 추가
    }

    public static void Clear() { }
}

using System;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    ResourceManager _resource = new();
    UIManager _ui = new();
    DataManager _data = new();
    InventoryManager _inventory = new();
    NetworkManager _network = new();

    // === 새 시스템 ===
    NewSlotManager _slot = new();
    NewInventoryManager _newInventory = new();
    NewChestManager _newChest = new();
    NewCraftingManager _newCrafting = new();


    public static ResourceManager Resource => Instance._resource;
    public static UIManager UI => Instance._ui;
    public static DataManager Data => Instance._data;
    public static InventoryManager Inventory => Instance._inventory;
    public static NetworkManager Network => Instance._network;

    // === 새 시스템 ===
    public static NewSlotManager Slot => Instance._slot;
    public static NewInventoryManager NewInventory => Instance._newInventory;
    public static NewChestManager Chest => Instance._newChest;
    public static NewCraftingManager Crafting => Instance._newCrafting;



    // Dispatcher 래퍼
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

    void Start() {
        Application.runInBackground = true;
        Init(); 
    }
    void Update() { }
     
    static void Init()
    {
        if (s_instance != null)
            return;

        // @Managers 오브젝트 찾거나 새로 생성
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

        // Dispatcher 없으면 추가 (한 번만)
        if (UnityMainThreadDispatcher.Instance == null)
        {
            go.AddComponent<UnityMainThreadDispatcher>();
        }

        DontDestroyOnLoad(go);
        s_instance._data.Init();
    }

    public static void Clear() { }
}
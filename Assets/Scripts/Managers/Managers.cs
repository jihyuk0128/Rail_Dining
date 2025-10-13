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

    public static ResourceManager Resource => Instance._resource;
    public static UIManager UI => Instance._ui;
    public static DataManager Data => Instance._data;
    public static InventoryManager Inventory => Instance._inventory;
    public static NetworkManager Network => Instance._network;


    void Start() { Init(); }
    void Update() { }

    static void Init()
    {
        if (s_instance != null) return;

        GameObject go = GameObject.Find("@Managers");
        if (go == null)
        {
            go = new GameObject("@Managers");
            go.AddComponent<Managers>();
            go.AddComponent<UnityMainThreadDispatcher>();
        }

        DontDestroyOnLoad(go);
        s_instance = go.GetComponent<Managers>();

        s_instance._data.Init();
    }

    public static void Clear() { }


}

using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();
    DataManager _data = new DataManager();
    InventoryManager _inventory = new InventoryManager();   // ÀÎº¥Åä¸® 8Ä­

    public static ResourceManager Resource { get { return Instance._resource; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static DataManager Data { get { return Instance._data; } }
    public static InventoryManager Inventory { get { return Instance._inventory; } }

    void Start() { Init(); }
    void Update() { }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
        }
    }

    public static void Clear() { }
}
using System.Collections;
using UnityEngine;

public class testsc : MonoBehaviour
{
    //public UI_Slot slot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {

        Managers.UI.ShowSceneUI<UI_BasicScene>();
        Managers.UI.ShowPopupUI<UI_CraftingBox>();

        // 한 프레임 뒤에 아이템 투입(초기화 완료 보장)
        StartCoroutine(DelayAdd());

        IEnumerator DelayAdd()
        {
            yield return null;
            Managers.Inventory.AddItemToInventory(1, 5); // Egg
                                                         // Managers.Inventory.AddItemToInventory(1, 1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
 
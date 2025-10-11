using System.Collections;
using UnityEngine;

public class testsc : MonoBehaviour
{
    //public UI_Slot slot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        Managers.Inventory.Init(8, 4);  //  먼저!
        // 기본 UI 띄우기
        Managers.UI.ShowSceneUI<UI_BasicScene>();   // 항상 켜지는 인벤토리
        Managers.UI.ShowPopupUI<UI_FoodBox>();  // 작업대 UI

        // 아이템 테스트용으로 몇 개 넣기
        Managers.Inventory.AddItemToInventory(1, 5);  // Egg 5개
        Managers.Inventory.AddItemToInventory(1, 20);  // Egg 5개

        Managers.UI.ShowPopupUI<UI_Button>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
 
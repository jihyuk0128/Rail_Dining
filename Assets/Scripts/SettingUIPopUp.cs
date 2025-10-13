using UnityEngine;
using UnityEngine.InputSystem;

public class SettingUIPopUp : MonoBehaviour
{
    private UI_Settings settingsUI;

    private void Start()
    {
        Managers.Inventory.Init(8, 4);  //  먼저!
        // 기본 UI 띄우기
        Managers.UI.ShowSceneUI<UI_BasicScene>();   // 항상 켜지는 인벤토리

        Managers.Inventory.AddItemToInventory(1, 5);  // Egg 5개
        Managers.Inventory.AddItemToInventory(1, 20);  // Egg 5개
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsUI == null)
            {
                settingsUI = Managers.UI.ShowPopupUI<UI_Settings>();
            }
            else
            {
                Managers.UI.ClosePopupUI();
                settingsUI = null;
            }
        }
    }
}

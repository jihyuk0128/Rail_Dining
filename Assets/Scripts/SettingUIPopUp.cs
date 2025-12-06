using UnityEngine;
using UnityEngine.InputSystem;

public class SettingUIPopUp : MonoBehaviour
{
    private UI_Settings settingsUI;
    private UI_BasicScene basicSceneUI;

    private void Start()
    {
        Managers.Inventory.Init(8, 4);  //  먼저!
        // 기본 UI 띄우기
        basicSceneUI = Managers.UI.ShowSceneUI<UI_BasicScene>();   // 항상 켜지는 인벤토리

        settingsUI = Managers.UI.ShowPopupUI<UI_Settings>();
        Managers.UI.ClosePopupUI();
        settingsUI = null;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!Managers.UI.IsPopupOpen())
            {
                settingsUI = Managers.UI.ShowPopupUI<UI_Settings>();
            }
            else if (GameManager.Instance.IsPlaying() && !GameManager.Instance.IsMiniPlaying())
            {
                Managers.UI.ClosePopupUI();
                settingsUI = null;
            }
        }

        if (basicSceneUI != null && settingsUI == null)
        {
            if (Keyboard.current.qKey.isPressed)
            {
                basicSceneUI?.SetRecipeVisible(true);
            }
            else if (Keyboard.current.qKey.wasReleasedThisFrame)
            {
                basicSceneUI?.SetRecipeVisible(false);
            }
        }

    }
}

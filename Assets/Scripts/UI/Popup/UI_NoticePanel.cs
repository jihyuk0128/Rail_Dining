using UnityEngine;
using UnityEngine.InputSystem;

public class UI_NoticePanel : UI_Popup
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePopupUI();
        }
    }
}

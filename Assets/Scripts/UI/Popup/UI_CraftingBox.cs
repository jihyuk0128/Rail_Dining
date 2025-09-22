using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

using TMPro;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class UI_CraftingBox: UI_Popup
{
    enum Buttons
    {
        CloseButton,
    }

    enum GameObjects
    {
        ItemGrid,
    }

    enum Images
    {
        ItemIcon,
    }

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        // 닫기 버튼 이벤트 연결
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
     
        // Title 설정
        //GetTextMeshProUGUI((int)Texts.TitleText).text = "Inventory";
    }

    void OnCloseButtonClicked(PointerEventData data)
    {
        Debug.Log("Inventory Closed");
        Managers.UI.ClosePopupUI();
    }
}

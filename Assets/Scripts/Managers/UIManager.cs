using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class UIManager
{
    int _sortorder = 10;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;
    private GameObject _dragIcon;
    private Image _dragIconImage;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _sortorder;
            _sortorder++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Utils.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }

    public UI_Popup ShowPopupUI(Type popupType)
    {
        if (popupType == null)
        {
            Debug.LogWarning("[UIManager] popupType이 null입니다!");
            return null;
        }

        string name = popupType.Name;

        // 프리팹 경로 동일하게 구성
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");

        // UI_Popup으로 캐스팅
        UI_Popup popup = go.GetComponent(popupType) as UI_Popup;
        if (popup == null)
            popup = go.AddComponent(popupType) as UI_Popup;

        _popupStack.Push(popup);
        go.transform.SetParent(Root.transform);

        return popup;
    }


    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        popup.Unregister();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _sortorder--;
    }


    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        Debug.Log($"[UIManager] {name} 프리팹을 로드 경로: UI/Scene/{name}");
        T SceneUI = Utils.GetOrAddComponent<T>(go);
        _sceneUI = SceneUI;

        go.transform.SetParent(Root.transform);

        return SceneUI;
    }

    public void ShowDragIcon(string iconPath, Canvas cv)
    {
        if (_dragIcon == null)
        {
            var canvas = cv;
            if (canvas == null)
            {
                Debug.LogError("[UIManager] Canvas를 찾을 수 없습니다.");
                return;
            }

            _dragIcon = new GameObject("DragIcon");
            _dragIcon.transform.SetParent(canvas.transform, false);

            _dragIconImage = _dragIcon.AddComponent<Image>();
            _dragIconImage.raycastTarget = false;
        }

        var sprite = Resources.Load<Sprite>(iconPath);
        if (sprite == null)
        {
            Debug.LogWarning($"[UIManager] 아이콘 스프라이트를 찾을 수 없음: {iconPath}");
            return;
        }

        _dragIconImage.sprite = sprite;
        _dragIcon.SetActive(true);
    }

    public void UpdateDragIcon(Vector2 screenPos)
    {
        if (_dragIcon == null) return;
        _dragIcon.transform.position = screenPos;
    }

    public void HideDragIcon()
    {
        if (_dragIcon != null)
        {
            GameObject.Destroy(_dragIcon);
            _dragIcon = null;
            _dragIconImage = null;
        }
    }

    public bool IsPopupOpen()
    {
        return _popupStack.Count > 0; // 팝업 UI가 하나라도 열려 있다면 true
    }
}




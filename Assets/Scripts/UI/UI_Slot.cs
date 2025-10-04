using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class UI_Slot : UI_Base
{
    enum Images { ItemIcon }
    enum Texts { AmountText }

    public SlotType SlotType { get; private set; }
    public int SlotIndex { get; private set; }

    private Image _icon;
    private TextMeshProUGUI _amountText;
    private static GameObject _dragIcon;

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _icon = GetImage((int)Images.ItemIcon);
        _amountText = GetTextMeshProUGUI((int)Texts.AmountText);
        Clear();

        gameObject.BindEvent(OnBeginDrag, Define.UIEvent.BeginDrag);
        gameObject.BindEvent(OnDrag, Define.UIEvent.Drag);
        gameObject.BindEvent(OnEndDrag, Define.UIEvent.EndDrag);
    }

    public void Configure(SlotType slotType, int index)
    {
        SlotType = slotType;
        SlotIndex = index;
    }

    public void SetItem(ItemData item, int amount = 1)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        _icon.sprite = Resources.Load<Sprite>(item.iconPath);
        _icon.enabled = true;
        _amountText.text = (amount > 1) ? amount.ToString() : "";
    }

    public void Clear()
    {
        if (_icon != null) _icon.enabled = false;
        if (_amountText != null) _amountText.text = "";
    }

    void OnBeginDrag(PointerEventData data)
    {
        if (_icon == null || _icon.sprite == null) return;

        _dragIcon = new GameObject("DragIcon");
        var image = _dragIcon.AddComponent<Image>();
        image.sprite = _icon.sprite;
        image.raycastTarget = false;
        image.preserveAspect = true;
        image.rectTransform.sizeDelta = new Vector2(80, 80);

        _dragIcon.transform.SetParent(Managers.UI.Root.transform, false);
        _dragIcon.transform.position = data.position;
    }

    void OnDrag(PointerEventData data)
    {
        if (_dragIcon != null)
            _dragIcon.transform.position = data.position;
    }

    void OnEndDrag(PointerEventData data)
    {
        if (_dragIcon != null)
            Destroy(_dragIcon);

        UI_Slot target = null;
        if (data.pointerEnter != null)
            target = data.pointerEnter.GetComponentInParent<UI_Slot>();
        if (target == null)
            target = RaycastFindSlot(data);

        if (target != null && (target.SlotType != SlotType.Result))
            Managers.Inventory.MoveItem(this.SlotType, this.SlotIndex, target.SlotType, target.SlotIndex);
    }

    UI_Slot RaycastFindSlot(PointerEventData data)
    {
        if (EventSystem.current == null) return null;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        foreach (var r in results)
        {
            var slot = r.gameObject.GetComponentInParent<UI_Slot>();
            if (slot != null) return slot;
        }
        return null;
    }
}

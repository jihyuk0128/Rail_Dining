using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : UI_Base
{
    enum Images { ItemIcon }
    enum Texts { AmountText }

    public SlotType SlotType;
    public int Index;
    private ItemSlot slotData;
    private Canvas parentCanvas;
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        BindEvent(gameObject, OnBeginDrag, Define.UIEvent.BeginDrag);
        BindEvent(gameObject, OnDrag, Define.UIEvent.Drag);
        BindEvent(gameObject, OnEndDrag, Define.UIEvent.EndDrag);
        BindEvent(gameObject, OnClick, Define.UIEvent.Click);

        // 부모 canvas 찾기 
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
            Debug.LogWarning("[UI_Slot] 부모 Canvas를 찾을 수 없음!");

        //if (SlotType == SlotType.Chest)
        //{
        //    GetObject((int)Texts.AmountText).SetActive(false);
        //}
    }

    public void SetData(ItemSlot data)
    {
        slotData = data;
        Refresh();
    }

    public void Refresh()
    {
        Image icon = GetImage((int)Images.ItemIcon);
        TextMeshProUGUI amountText = GetTextMeshProUGUI((int)Texts.AmountText);

        if (slotData == null || slotData.Item == null)
        {
            icon.gameObject.SetActive(false);
            amountText.text = "";
        }
        else
        {
            icon.gameObject.SetActive(true);
            icon.sprite = Resources.Load<Sprite>(slotData.Item.iconPath);
            amountText.text = slotData.Amount > 1 ? slotData.Amount.ToString() : "";
        }

       if(SlotType.Chest == SlotType)
        {
            amountText.text = "";
        }
    }

    // 드래그 시작
    void OnBeginDrag(PointerEventData data)
    {
        if (SlotType.Chest == SlotType || SlotType.Result == SlotType) return;

        if (slotData == null || slotData.Item == null) return;

        Managers.UI.ShowDragIcon(slotData.Item.iconPath, parentCanvas);
        Managers.UI.UpdateDragIcon(data.position);
        Managers.Inventory.DragSourceSlot = this; // 현재 드래그 중인 슬롯 기억
    }

    // 드래그 중
    void OnDrag(PointerEventData data)
    {
        if (SlotType.Chest == SlotType || SlotType.Result == SlotType) return;
        Managers.UI.UpdateDragIcon(data.position);
    }

    // 드래그 끝
    void OnEndDrag(PointerEventData data)
    {
        if (SlotType.Chest == SlotType || SlotType.Result == SlotType) return;
        Managers.UI.HideDragIcon();
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        UI_Slot targetSlot = null;
        foreach (var hit in results)
        {
            targetSlot = hit.gameObject.GetComponent<UI_Slot>();
            if (targetSlot != null) break;
        }

        if (targetSlot == null)
        {
            Debug.Log("드랍된 슬롯이 없음.");
            return;
        }

        // 실제 데이터 이동
        // Shift 누르고 있으면 전부 이동
        bool moveAll = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        Managers.Inventory.MoveItem(this.SlotType, this.Index, targetSlot.SlotType, targetSlot.Index, moveAll);

    }

    void OnClick(PointerEventData data)
    {
        Debug.Log($"{SlotType}타입 {Index}선택됨");

        // 인벤토리에 추가
        if (SlotType == SlotType.Result)    
        {
            var result = Managers.Inventory.ResultSlot;

            if (result != null && result.Item != null && result.Amount > 0)
            {
                Debug.Log($"[결과 수령] {result.Item.name} x{result.Amount}");

                // 인벤토리에 추가
                Managers.Inventory.AddItemToInventory(result.Item.id, result.Amount);

                // 결과 슬롯 초기화
                result.Clear();

                // UI 갱신
                Managers.Inventory.RefreshAllUI();
                return;
            }
        }

        if (SlotType == SlotType.Chest)
        {
            var chest = Managers.Inventory.ChestSlots[Index];

            if (chest != null && chest.Item != null && chest.Amount > 0)
            {
                Debug.Log($"[결과 수령] {chest.Item.name} x 1");

                // 인벤토리에 추가
                Managers.Inventory.AddItemToInventory(chest.Item.id, chest.Item.maxStack);

                // 갯수추가
                chest.Amount++;

                // UI 갱신
                Managers.Inventory.RefreshAllUI();
            }

        }

    
        return;
    }
}

    
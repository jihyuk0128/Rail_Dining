using UnityEngine;
using UnityEngine.UI;

public class UI_ParentSlot : UI_Base
{
    enum Images { ItemIcon }

    [SerializeField] protected Image itemIcon;
    protected ItemSlot slotData;
    private bool isInitialized = false;
    


    public override void Init()
    {
        Bind<Image>(typeof(Images));
        itemIcon = GetImage((int)Images.ItemIcon);

        if (itemIcon == null)
            Debug.LogError($"[UI_ParentSlot] ItemIcon 바인드 실패! ({name})");

        isInitialized = true; 
    }

    public virtual void SetData(ItemSlot data)
    {
        slotData = data;
        if (isInitialized)
            Refresh();
    }

    public virtual void Refresh()
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"[UI_ParentSlot] 아직 Init이 안 됨 ({name})");
            return;
        }

        if (itemIcon == null)
        {
            Debug.LogWarning($"[UI_ParentSlot] itemIcon이 NULL ({name})");
            return;
        }

        if (slotData == null || slotData.Item == null)
        {
            itemIcon.enabled = false;
            return;
        }

        var sprite = Resources.Load<Sprite>(slotData.Item.iconPath);
        if (sprite != null)
        {
            itemIcon.enabled = true;
            itemIcon.sprite = sprite;
        }
        else
        {
            itemIcon.enabled = false;
            Debug.LogWarning($"[UI_ParentSlot] 스프라이트 로드 실패: {slotData.Item.iconPath}");
        }

        if (slotData.Item.id == 103)
        {
            
            RectTransform rt = itemIcon.rectTransform;

            rt.sizeDelta = new Vector2(30f, 30f);
            return;
        }

        itemIcon.SetNativeSize();
    }

    public virtual void Clear()
    {
        slotData?.Clear();
        if (isInitialized)
            Refresh();
    }
}
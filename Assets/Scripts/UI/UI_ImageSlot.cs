using UnityEngine;
using UnityEngine.UI;

public class UI_ImageSlot : UI_Base
{
    enum Images
    {
        Image
    }

    ItemData slotData;

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
    }

    public void SetImage(ItemData itemdata)
    {
        slotData = itemdata;

        Image icon = GetImage((int)Images.Image);


        if (slotData == null) icon.gameObject.SetActive(false);
        else
        {
            Debug.Log($"{slotData.id} 이거들어있음");
            icon.gameObject.SetActive(true);
            icon.sprite = Resources.Load<Sprite>(slotData.iconPath);
        }
    }
}

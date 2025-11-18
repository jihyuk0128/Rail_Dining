using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CreateResult : UI_Popup
{
    enum Images { Image,ResultSlot,ResultTextSlot }
    enum Buttons { ResultTextSlot}

    private int _recipeId;

    public void Awake()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
    }

    public void TryCraft(bool isSuccess , int recipeid)
    {
        _recipeId = recipeid;


        // --- 결과 아이템 표시 ---
        var recipe = Managers.Data.RecipeDict[_recipeId];
        var result = Managers.Data.ItemDict[recipe.resultId];
        Debug.Log($"레시피설정화면 결과 {recipe.resultId}");

        var _image = GetImage((int)Images.ResultSlot); 
        _image.sprite = Resources.Load<Sprite>(result.iconPath);
        _image.SetNativeSize();

        if (!isSuccess)
        {
            GetImage((int)Images.Image).sprite = Resources.Load<Sprite>("Art/UI/Ingame_new_uI/IngameNew/cookbook/Food_Completion_fail_Window");
            GetImage((int)Images.ResultTextSlot).sprite = Resources.Load<Sprite>("Art/UI/Ingame_new_uI/IngameNew/cookbook/food_Failed_acquire_button");
            GetButton((int)Buttons.ResultTextSlot).gameObject.BindEvent((PointerEventData data) => {
                Managers.Crafting.FailCraft(_recipeId);
                Managers.UI.ClosePopupUI(); 
            });

            var img = GetImage((int)Images.ResultSlot);
            if (img != null)
            {
                img.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            }
            return;
        }

        GetImage((int)Images.Image).sprite = Resources.Load<Sprite>("Art/UI/Ingame_new_uI/IngameNew/cookbook/Food_Completion_Perfact_Window");
        GetImage((int)Images.ResultTextSlot).sprite = Resources.Load<Sprite>("Art/UI/Ingame_new_uI/IngameNew/cookbook/food_acquisition_button");
        GetButton((int)Buttons.ResultTextSlot).gameObject.BindEvent((PointerEventData data) => 
        { 
            Managers.Crafting.SuccessCraft(_recipeId);
            Managers.UI.ClosePopupUI(); 

        });
        return;

    }
}

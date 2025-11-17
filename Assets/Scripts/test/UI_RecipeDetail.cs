using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RecipeDetail : UI_Popup
{
    enum Images { ResultSlot }
    enum Buttons { CloseButton , StartButton}

    enum GameObjects { Grid}

    private int _recipeId;

    public void Awake()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClose);
        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnStart);
    }

    public void SetRecipe(int recipeId)
    {
        _recipeId = recipeId;

        // --- 결과 아이템 표시 ---
        var recipe = Managers.Data.RecipeDict[_recipeId];
        var result = Managers.Data.ItemDict[recipe.resultId];
        Debug.Log($"레시피설정화면 결과 {recipe.resultId}");

        GetImage((int)Images.ResultSlot).sprite = Resources.Load<Sprite>(result.iconPath);

        // --- 기존 그리드 정리 ---
        var grid = GetObject((int)GameObjects.Grid).transform;
        foreach (Transform child in grid)
            Object.Destroy(child.gameObject);

        // --- 재료 슬롯 생성 ---
        foreach (int ingId in recipe.ingredients)
        {
            if (ingId == 0)
                continue; // 빈 칸 제외

            GameObject slotObj = Managers.Resource.Instantiate("TestPrefabs/UI_ParentSlot", grid);
            var slot = slotObj.GetComponent<UI_ParentSlot>();
            slot.Init();

            var ingItem = Managers.Data.ItemDict[ingId];
            var slotData = new ItemSlot { Item = ingItem, Amount = 1 };
            slot.SetData(slotData);
        }

        Debug.Log($"[UI_RecipeDetail] 레시피 상세 표시 완료 - 재료 {recipe.ingredients.Count}개");
    }

    void OnClose(PointerEventData data)
    {
        Managers.UI.ClosePopupUI();
    }

    void OnStart(PointerEventData data)
    {
        if (!Managers.Crafting.TryCraft(_recipeId)) // 재료가 전부있나 검사.
        {
            return;
        }

        Managers.UI.ClosePopupUI(); 
        Managers.UI.ClosePopupUI();

        // 여기서 미니게임
        Managers.Crafting.StartMiniGame(_recipeId); 

        //bool isplaying = minigame.isPlaying;
        //
        //bool issuceesed = minigame.isSuccess;
        //
        //
        //

    }
}

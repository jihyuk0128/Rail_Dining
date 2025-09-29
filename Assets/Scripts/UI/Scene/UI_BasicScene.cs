using UnityEngine;

public class UI_BasicScene : UI_Scene
{
    enum GameObjects
    {
        UI_Clock,
        UI_Inventory
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 자식 오브젝트 자동 바인딩
        Bind<GameObject>(typeof(GameObjects));
    }
}
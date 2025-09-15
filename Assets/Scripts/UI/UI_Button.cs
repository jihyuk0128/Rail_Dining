using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

using TMPro;                
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Profiling.Memory.Experimental;
public class UI_Button : MonoBehaviour
{
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    // EventSystem 사용해서 버튼부분 누르지않게하기
    // ex) if (EventSystem.current.IsPointerOverGameObject()) return;

    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        PointText,
    }

    enum TextMeshProUGUIS
    {
        ScoreText,
    }

    enum GameObjects
    {
        TestObject,
    }

    private void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIS));
        Bind<GameObject>(typeof(GameObjects));


        Get<TextMeshProUGUI>((int)TextMeshProUGUIS.ScoreText).text = "BindTest";
    }

    void Bind<T>(Type type) where T : UnityEngine.Object               
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if(typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);
            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"failed to bind({names[i]})");
        }
    }


    T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;                                             

        return objects[index] as T;
    }

    int _score = 0;

     
    public void OnButtonClicked()
    {
        Debug.Log("ButtonClicked");

        _score++;
    }
}

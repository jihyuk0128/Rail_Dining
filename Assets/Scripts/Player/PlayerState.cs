using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public bool HasCocktail { get; private set; }
    public string CocktailName { get; private set; }

    public void HoldCocktail(string name)
    {
        HasCocktail = true;
        CocktailName = name;
    }

    public void ClearCocktail()
    {
        HasCocktail = false;
        CocktailName = null;
    }
}

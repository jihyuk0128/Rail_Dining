using UnityEngine;

public class TileBackground : MonoBehaviour
{
    public bool blockedTile = false; // 역과 함께 등장한 타일인지
    public GameObject treeObject;
    public GameObject extraTreeGroup;

    public void SetTreeVisible(bool visible)
    {
        if (treeObject != null)
            treeObject.SetActive(visible);
    }

    public void SetExtraTreeVisible(bool visible)
    {
        if (extraTreeGroup != null)
            extraTreeGroup.SetActive(visible);
    }
}


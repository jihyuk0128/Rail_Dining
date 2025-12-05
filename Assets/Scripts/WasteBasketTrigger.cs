using UnityEngine;

public class WasteBasketTrigger : MonoBehaviour
{
    UI_WasteBasket wasteBasket; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어만 반응
        {
            if (wasteBasket == null) wasteBasket = Managers.UI.ShowSceneUI<UI_WasteBasket>();
            wasteBasket.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (wasteBasket != null)
                wasteBasket.gameObject.SetActive(false);
        }
    }

}

using UnityEngine;

public class TrainOpacityController : MonoBehaviour
{
    public SpriteRenderer spriteRend;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어만 반응
        {
            Color color = spriteRend.color;
            color.a = 0.5f;
            spriteRend.color = color;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어만 반응
        {
            Color color = spriteRend.color;
            color.a = 1f;
            spriteRend.color = color;
        }
    }
}

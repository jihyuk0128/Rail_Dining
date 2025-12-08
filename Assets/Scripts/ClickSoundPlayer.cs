using UnityEngine;

public class ClickSoundPlayer : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);  // 씬 전환해도 유지
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.Instance.PlaySFX("Click_SFX");
        }
    }
}

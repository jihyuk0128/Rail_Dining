using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private bool isDialogFinished = false;
    public TutorialNPC npc;
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        player?.SetEvent(true);
        StartCoroutine(TutorialRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        // 튜토리얼 대화창 시작
        UI_TutorialDialog dialog = Managers.UI.ShowPopupUI<UI_TutorialDialog>();
        dialog.PlayDialogueRange(0,7);

        // 대화가 끝날 때까지 대기
        yield return new WaitUntil(() => isDialogFinished);
        player?.SetEvent(false);

        // NPC 상호작용 가능
        npc.EnableInteraction(true);
        yield return new WaitUntil(() => npc.IsServed);

        // 종료 대화창 시작
        player?.SetEvent(true);
        isDialogFinished = false;
        dialog = Managers.UI.ShowPopupUI<UI_TutorialDialog>();
        dialog.PlayDialogueRange(8, 10);

        // 대화가 끝날 때까지 대기
        yield return new WaitUntil(() => isDialogFinished);
        player?.SetEvent(false);
        Debug.Log("튜토리얼 완료!");

        SceneManager.LoadScene("TestScene");
    }

    public void OnDialogFinished()
    {
        isDialogFinished = true;
    }
}
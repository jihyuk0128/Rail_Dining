using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TutorialDialog : UI_Popup
{
    public enum Images
    {
        CharacterImage
    }

    public enum Texts
    {
        DialogueText
    }

    private List<DialogueData> dialogueList = new List<DialogueData>();
    private int currentIndex = 0;
    private bool isActive = false;

    private TutorialManager tutorialManager;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        foreach (var pair in _objects)
            Debug.Log($"[BindCheck] {pair.Key} count={pair.Value.Length}");

        tutorialManager = FindObjectOfType<TutorialManager>();
    }

    public void PlayDialogueRange(int startId, int endId)
    {
        StopAllCoroutines(); // 혹시 기존 진행 중인 대사 있으면 중단
        StartCoroutine(ShowDialogueRangeCoroutine(startId, endId));
    }

    private IEnumerator ShowDialogueRangeCoroutine(int startId, int endId)
    {
        for (int i = startId; i <= endId; i++)
        {
            if (!Managers.Data.DialogueDict.ContainsKey(i))
                continue;

            var dialogue = Managers.Data.DialogueDict[i];
            Get<Image>((int)Images.CharacterImage).sprite = Managers.Resource.Load<Sprite>($"Art/UI/Tutorial/{dialogue.CharImg}");
            Get<TextMeshProUGUI>((int)Texts.DialogueText).text = dialogue.CharDialogue;
            yield return new WaitWhile(() => Input.anyKey );
            yield return new WaitUntil(() => Input.anyKeyDown);
        }
        EndDialogue();
    }

    public void EndDialogue()
    {
        tutorialManager.OnDialogFinished();
        Managers.UI.ClosePopupUI();
    }
}
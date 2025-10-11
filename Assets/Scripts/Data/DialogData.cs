using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueData
{
    public int id;             // 대사 번호
    public string CharImg;     // 캐릭터 이미지 파일명
    public string CharDialogue;// 대사 내용
}

[Serializable]
public class DialogueDataLoader : ILoader<int, DialogueData>
{
    public List<DialogueData> dialogues = new List<DialogueData>();

    public Dictionary<int, DialogueData> MakeDict()
    {
        Dictionary<int, DialogueData> dict = new Dictionary<int, DialogueData>();
        foreach (var dialogue in dialogues)
            dict[dialogue.id] = dialogue;
        return dict;
    }
}
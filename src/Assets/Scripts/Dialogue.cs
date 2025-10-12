using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueChoice
{
    [Header("Choice Settings")]
    public string choiceText;          // The text shown on the button

    [Header("Optional Jump (within same dialogue)")]
    public bool jumpToLineInSameDialogue;
    public int targetLineIndex;

    [Header("Next Dialogue (Optional)")]
    public Dialogue nextDialogue;      // The dialogue this choice leads to

    [Header("Flag Setting (Optional)")]
    public string flagToSet;           // 🏳️ Name of the flag to modify
    public bool flagValue;             // 🟢 Value to assign to that flag
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)]
    public string line;
    public bool hasChoices;
    public List<DialogueChoice> choices;

    [Header("Conditional Requirements (Optional)")]
    public List<FlagCondition> conditions; // 🧩 Line only displays if these conditions pass
}

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("Dialogue Lines")]
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    [Header("Repeatable Settings")]
    [Tooltip("If true, this dialogue can be triggered multiple times (like generic NPC talk).")]
    public bool isRepeatable = false;

    [Header("Branching / Ending Settings")]
    [Tooltip("If true, this dialogue represents an ending branch (e.g. 'Good Ending').")]
    public bool isEndingDialogue = false;

    [Tooltip("Optional flag to set when this dialogue finishes (e.g. 'GoodEndingReached').")]
    public string endingFlagToSet;
    public bool endingFlagValue;
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections; 

public class DialogueAction : CutsceneAction //Script responsible for showing dialogue
{
    [SerializeField] Dialogue dialogue;

    public override IEnumerator Play()
    {
        yield return DialogueManager.Instance.StartDialogue(dialogue);
    }
}

using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    [Tooltip("Unique ID for this trigger (e.g. 'intro_library_greeting')")]
    public string triggerID;

    private bool hasPlayed;

    [Header("Story Progression")]
    public StoryFlags storyFlags;
    public List<FlagCondition> conditions; // optional

    private void Awake()
    {
        if (string.IsNullOrEmpty(triggerID))
            triggerID = $"{gameObject.scene.name}_{gameObject.name}";
    }

    private void Start()
    {
        hasPlayed = PlayerPrefs.GetInt(triggerID, 0) == 1;
        Debug.Log($"🎯 Loaded triggerID '{triggerID}', played before: {hasPlayed}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"🚪 Trigger entered by {collision.name}");

        if (dialogue == null)
        {
            Debug.LogError($"Dialogue is missing for trigger '{triggerID}'!");
            return;
        }

        // 🟡 If already played and not repeatable, skip
        if (hasPlayed && !dialogue.isRepeatable)
        {
            Debug.Log($"Dialogue '{triggerID}' already played and is not repeatable.");
            return;
        }

        if (!collision.CompareTag("Player"))
        {
            Debug.Log("Not a player trigger.");
            return;
        }

        if (!AreConditionsMet())
        {
            Debug.Log($"⚪ Conditions not met for dialogue '{triggerID}'.");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager.Instance is null!");
            return;
        }

        // Trigger dialogue
        StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));

        // Mark as played unless repeatable
        if (!dialogue.isRepeatable)
        {
            hasPlayed = true;
            PlayerPrefs.SetInt(triggerID, 1);
            PlayerPrefs.Save();
            Debug.Log($"Dialogue '{triggerID}' played and saved!");
        }
    }

    private bool AreConditionsMet()
    {
        if (conditions == null || conditions.Count == 0)
            return true;

        foreach (var c in conditions)
        {
            if (storyFlags == null) return true;
            if (storyFlags.GetFlag(c.flagName) != c.requiredValue)
                return false;
        }
        return true;
    }
}

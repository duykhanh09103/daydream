using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    [SerializeReference]
    [SerializeField] private List<CutsceneAction> actions;

    [Tooltip("Unique ID for this cutscene (e.g. 'intro_castle_gate')")]
    public string triggerID;

    public static bool cutsceneActive = false;
    private bool hasPlayed;
    private Scene m_Scene;
    string sceneName;

    [Header("Story Progression")]
    public StoryFlags storyFlags; 
    public List<FlagCondition> conditions; // optional

    private void Awake()
    {
        if (string.IsNullOrEmpty(triggerID))
            triggerID = $"{gameObject.scene.name}_{gameObject.name}";
        m_Scene = SceneManager.GetActiveScene();
        sceneName = m_Scene.name;
    }

    private void Start()
    {
        hasPlayed = PlayerPrefs.GetInt(triggerID, 0) == 1;
        Debug.Log($"🎬 Loaded cutscene '{triggerID}', played before: {hasPlayed}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (hasPlayed)
        {
            Debug.Log($"⛔ Cutscene '{triggerID}' already played — skipping.");
            return;
        }

        if (!AreConditionsMet())
        {
            Debug.Log($"Conditions not met for cutscene '{triggerID}'.");
            return;
        }

        StartCoroutine(PlayAndSave());
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

    private IEnumerator PlayAndSave()
    {
        hasPlayed = true;
        PlayerPrefs.SetInt(triggerID, 1);
        PlayerPrefs.Save();
        Debug.Log($"Cutscene '{triggerID}' started and marked as played.");

        cutsceneActive = true;

        foreach (var action in actions)
            yield return action.Play();

        cutsceneActive = false;
        Debug.Log($"🎬 Cutscene '{triggerID}' finished.");

        if (GameFlow.Instance.currentDay == 1)
        {
            if (sceneName == "school") SceneManager.LoadScene("warehouse");
        }
    }

    public void AddAction(CutsceneAction action)
    {
        if (actions == null)
            actions = new List<CutsceneAction>();

        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    [ContextMenu("Reset Cutscene Played State")]
    public void ResetPlayedState()
    {
        hasPlayed = false;
        PlayerPrefs.DeleteKey(triggerID);
        Debug.Log($"Reset cutscene state for '{triggerID}'.");
    }
}

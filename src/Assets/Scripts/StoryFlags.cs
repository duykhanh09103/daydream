using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryFlags", menuName = "Story/Story Flags")]
public class StoryFlags : ScriptableObject
{
    [System.Serializable]
    public class FlagEntry
    {
        public string key;
        public bool value;
    }

    [Header("Default Flags")]
    public List<FlagEntry> defaultFlags = new List<FlagEntry>();

    private Dictionary<string, bool> runtimeFlags = new Dictionary<string, bool>();

    private static StoryFlags _instance;
    public static StoryFlags Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<StoryFlags>("GlobalStoryFlags");
                if (_instance == null)
                {
                    Debug.LogError("❌ GlobalStoryFlags asset not found in Resources folder!");
                }
                else
                {
                    _instance.Initialize();
                }
            }
            return _instance;
        }
    }

    private bool isInitialized = false;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) return;
        runtimeFlags.Clear();

        foreach (var f in defaultFlags)
        {
            bool savedValue = PlayerPrefs.GetInt(f.key, f.value ? 1 : 0) == 1;
            runtimeFlags[f.key] = savedValue;
        }

        isInitialized = true;
        Debug.Log("StoryFlags initialized. Runtime flags count: " + runtimeFlags.Count);
    }

    public bool GetFlag(string key)
    {
        Initialize();
        if (!runtimeFlags.ContainsKey(key))
        {
            Debug.LogWarning($"Tried to get missing flag: {key}. Defaulting to false.");
            runtimeFlags[key] = false;
        }

        bool val = runtimeFlags[key];
        Debug.Log($"GetFlag({key}) → {val}");
        return val;
    }

    public void SetFlag(string key, bool value)
    {
        Initialize();
        runtimeFlags[key] = value;
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"SetFlag: {key} = {value}");
    }

    public void SaveFlags()
    {
        foreach (var kvp in runtimeFlags)
            PlayerPrefs.SetInt(kvp.Key, kvp.Value ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Story flags saved.");
    }

    public void LoadFlags()
    {
        foreach (var f in defaultFlags)
            runtimeFlags[f.key] = PlayerPrefs.GetInt(f.key, f.value ? 1 : 0) == 1;
        Debug.Log("Story flags reloaded.");
    }
}

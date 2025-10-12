using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public StoryFlags storyFlags; // reference to your global story flags

    private void Awake()
    {
        // --- Singleton setup ---
        storyFlags = Resources.Load<StoryFlags>("GlobalStoryFlags");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- Load StoryFlags asset automatically ---
        if (storyFlags == null)
        {
            storyFlags = Resources.Load<StoryFlags>("GlobalStoryFlags");


#if UNITY_EDITOR
            // In the Editor, if we still don't have one, try to create it automatically
            if (storyFlags == null)
            {
                string path = "Assets/Resources/GlobalStoryFlags.asset";
                var existing = AssetDatabase.LoadAssetAtPath<StoryFlags>(path);

                if (existing == null)
                {
                    storyFlags = ScriptableObject.CreateInstance<StoryFlags>();
                    System.IO.Directory.CreateDirectory("Assets/Resources");
                    AssetDatabase.CreateAsset(storyFlags, path);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Created new GlobalStoryFlags.asset at " + path);
                }
                else
                {
                    storyFlags = existing;
                }
            }
#endif
        }

        // --- Safety check ---
        if (storyFlags == null)
            Debug.LogWarning("StoryFlags not found or created. Make sure you have a GlobalStoryFlags asset in Resources.");
        else
            Debug.Log("GameController loaded StoryFlags successfully.");
    }
}

using UnityEngine;

public class GameFlow: MonoBehaviour
{
    private static GameFlow _instance;
    public static GameFlow Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindFirstObjectByType<GameFlow>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("GameFlow");
                    _instance = go.AddComponent<GameFlow>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }
    public int currentDay = 0;
    public bool day1Completed = false;
    public bool day2Completed = false;
    public bool day3Completed = false;

    public bool hasTriggered = false; //Keep track whether the dialogue o

    public bool friendEnding = false;
    public bool teacherEnding = false;
    public bool badEnding = false;
    public bool badgoodEnding = false;

    public bool battle2done = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void CompleteDay1()
    {
        day1Completed = true;
        Debug.Log("Day 1 is done");
        currentDay++;
    }
    public void CompleteDay2()
    {
        day2Completed = true;
        Debug.Log("Day 2 is done");
        currentDay++;
    }
    public void CompleteDay3()
    {
        day3Completed = true;
        Debug.Log("Day 3 is done");
        currentDay++;
    }

}

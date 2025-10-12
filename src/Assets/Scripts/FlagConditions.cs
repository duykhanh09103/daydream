using System;
using UnityEngine;

[Serializable]
public class FlagCondition
{
    [Tooltip("The name of the story flag to check (must exist in StoryFlags asset).")]
    public string flagName;

    [Tooltip("The required value of that flag for this condition to pass.")]
    public bool requiredValue = true;
}

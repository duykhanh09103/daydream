using UnityEngine;
using System.Collections.Generic;
using System.Collections; 

[System.Serializable]
public class CutsceneAction //Main file, the other files will inherit from this
{
    [SerializeField] string name;

    public virtual IEnumerator Play()
    {
        yield break;
    }
    public string Name
    {
        get => name;
        set => name = value;
    }
}

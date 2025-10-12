using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;       

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor //Inherit from Unity's standard Editor namespace
{
    public override void OnInspectorGUI()
    {
        var cutscene = target as Cutscene;
        if (GUILayout.Button("Add dialogue action"))
        {
            cutscene.AddAction(new DialogueAction());
        }
        else if (GUILayout.Button("Add Move Actor action"))
        {
            cutscene.AddAction(new MoveActorAction());
        }
        base.OnInspectorGUI();
    }
}

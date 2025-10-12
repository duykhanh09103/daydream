using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MoveActorAction : CutsceneAction //Script responsible for moving characters
{
    [SerializeField] Character character;

    [SerializeField] List<Vector2> movePatterns;

    CharacterController controller;

    public override IEnumerator Play()
    {
        foreach (var moveVec in movePatterns)
        {
            yield return character.Move(moveVec);
        }
    }
}


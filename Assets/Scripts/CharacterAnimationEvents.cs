using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    Character character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponentInParent<Character>();
    }

    void AttackEnd()
    {        
        character.CurrentState = Character.State.RunningFromEnemy;
        character.KillTarget();
    }

    void ShootEnd()
    {
        character.CurrentState = Character.State.Idle;
        character.KillTarget();
    }
}

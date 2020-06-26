using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle,
        RunningToEnemy,
        RunningFromEnemy,
        BeginAttack,
        Attack,
        TurningToEnemy,
        BeginShoot,
        Shoot,
        Dying,
        Dead
    }

    public enum Weapon
    {
        Pistol,
        Bat,
        Fist
    }

    public Weapon weapon;
    public float runSpeed;
    public float distanceFromEnemy;
    public GameObject[] possibleTargets;
    
    public State CurrentState { get; set; }
    Animator animator;
    Vector3 originalPosition;
    Quaternion originalRotation;
    bool isAlive;
    GameObject target;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        CurrentState = State.Idle;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        isAlive = true;
        target = possibleTargets[0];
    }

    [ContextMenu("Attack")]
    void AttackEnemy()
    {
        bool isTargetAlive = target.GetComponent<Character>().isAlive;

        if (!isTargetAlive)
        {
            target = possibleTargets[1];
            isTargetAlive = target.GetComponent<Character>().isAlive;
        }
            
       
        if (isTargetAlive && isAlive)
        {
            switch (weapon)
            {
                case Weapon.Bat:
                case Weapon.Fist:
                    CurrentState = State.RunningToEnemy;
                    break;

                case Weapon.Pistol:
                    CurrentState = State.TurningToEnemy;
                    break;
            }
        }
        
    }

    bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
    {
        Vector3 distance = targetPosition - transform.position;
        if (distance.magnitude < 0.00001f) {
            transform.position = targetPosition;
            return true;
        }

        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        targetPosition -= direction * distanceFromTarget;
        distance = (targetPosition - transform.position);

        Vector3 step = direction * runSpeed;
        if (step.magnitude < distance.magnitude) {
            transform.position += step;
            return false;
        }

        transform.position = targetPosition;
        return true;
    }

    void TurnTo(Vector3 targetPosition)
    {
        Vector3 distance = targetPosition - transform.position;
        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void KillTarget()
    {
        target.GetComponent<Character>().Die();
    }

    public void Die()
    {
        animator.SetFloat("Speed", 0.0f);
        CurrentState = State.Dying;
        isAlive = false;
    }
    void FixedUpdate()
    {
        switch (CurrentState) {
            case State.Idle:

                if (weapon != Weapon.Pistol)
                {
                    animator.SetFloat("Speed", 0.0f);
                    transform.rotation = originalRotation;

                }
                break;

            case State.RunningToEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(target.transform.position, distanceFromEnemy))
                    CurrentState = State.BeginAttack;
                break;

            case State.BeginAttack:
                animator.SetTrigger("MeleeAttack");
                CurrentState = State.Attack;
                break;

            case State.Attack:
                break;

            case State.TurningToEnemy:
                TurnTo(target.transform.position);
                CurrentState = State.BeginShoot;
                break;

            case State.BeginShoot:
                animator.SetTrigger("Shoot");
                CurrentState = State.Shoot;
                break;

            case State.Shoot:
                break;

            case State.RunningFromEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(originalPosition, 0.0f))
                    CurrentState = State.Idle;
                break;

            case State.Dying:
                animator.SetTrigger("Death");
                CurrentState = State.Dead;
                break;

            case State.Dead:                
                break;

        }
    }
}

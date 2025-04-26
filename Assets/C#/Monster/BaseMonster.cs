using UnityEngine;

public abstract class BaseMonster : MonoBehaviour
{
    public float hp = 100f;
    public float moveSpeed = 2f;
    public Transform target;
    protected Animator animator;

    protected enum State { Idle, Patrol, Chase, Attack, Dead }
    protected State currentState;

    protected virtual void Start()
    {
        currentState = State.Patrol;
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        
        switch (currentState)
        {
            case State.Idle: Idle(); break;
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Attack: break;
            case State.Dead: Dead(); break;
        }

        if (hp <= 0 && currentState != State.Dead)
        {
            currentState = State.Dead;
        }
    }

    protected abstract void Idle();
    protected abstract void Patrol();
    protected abstract void Chase();
    protected abstract void Attack();
    protected abstract void Dead();

    protected bool PlayerInRange(float range)
    {
        if (target == null) return false;
        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= range;
    }
}

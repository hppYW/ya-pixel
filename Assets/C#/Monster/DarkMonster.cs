using UnityEngine;
using UnityEngine.EventSystems;

public class DarkMonster : BaseMonster
{
    
    private SpriteRenderer spriteRenderer;
    private float patrolTimer = 0f;
    private float changeDirectionTime = 2f;
    private int moveDirection = -1;
    private bool isAttacking = false;
    private float attackCooldown = 1.5f;
    private float lastAttackTime = -Mathf.Infinity;
    

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        if (currentState == State.Dead) return;
        base.Update();

        if (currentState == State.Attack) 
        {
            if (!isAttacking)
            {
                Attack();
            }
            else
            {
                if (IsAnimationFinished("attack 1") || IsAnimationFinished("attack 2") || IsAnimationFinished("hit"))
                {
                    isAttacking = false;
                    currentState = State.Chase;
                }
            }
        }
        if (currentState == State.Patrol && PlayerInRange(5f))
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x <0;
        }
    }
    private bool IsAnimationFinished(string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1.0f;
    }

  
    protected override void Attack()
    {

        if (isAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;
        int randomAttack = Random.Range(0, 10);

        if (randomAttack < 4)
        {
            animator.Play("hit");
            Debug.Log("hit 공격");
            Invoke(nameof(BackToChase), 0.500f);

        }
        else 
        {
            int normalAttack = Random.Range(0, 2);
            if (normalAttack == 0){
                animator.Play("attack 1");
                Debug.Log("Attack1 공격");
                Invoke(nameof(BackToChase), 1.200f);
            }
            else {
                animator.Play("attack 2");
                Debug.Log("attack 2 공격");
                Invoke(nameof(BackToChase), 1.500f);
            }
        }
        Debug.Log("공격");

        
    }

    private void BackToChase()
    {
        isAttacking = false;
        currentState = State.Chase;
    }

    protected override void Chase()
    {
        if (target == null) return;
        float dist = Vector2.Distance(transform.position, target.position);
        Debug.Log($"[Chase] dist = {dist:F2}");

        if (dist <= 3f)
        {
           if (Time.time >= lastAttackTime + attackCooldown)
           {
                 currentState = State.Attack;
                Debug.Log("→ State changed to Attack");
                return;
           }
        }
        if (dist > 4f)
        {
            currentState = State.Patrol;
            return;
        }

        Vector2 dir = (target.position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);
        if (spriteRenderer != null)
            spriteRenderer.flipX = dir.x < 0;
    }

    protected override void Dead()
    {
        animator.Play("death");
        Debug.Log("사망");
        
    }

    protected override void Idle()
    {
        animator.Play("idle");

    }

    protected override void Patrol()
    {
        animator.Play("walk");

        patrolTimer += Time.deltaTime;
        if (patrolTimer >= changeDirectionTime)
        {
            moveDirection *= -1;
            patrolTimer = 0f;
        }

        Vector2 moveDir = new Vector2(moveDirection, 0);
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        if (spriteRenderer != null) 
        {
            spriteRenderer.flipX = moveDir.x < 0;
        }
        if (PlayerInRange(3f)) currentState = State.Chase;
    }
}

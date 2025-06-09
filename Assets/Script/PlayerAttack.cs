using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("기본 공격 데미지")]
    public float attackDamage = 25f;
    
    public float comboWindow = 3f; // 콤보 입력 가능 시간
    public float comboResetTime = 3f; // 콤보 완전 리셋 시간


    [Header("References")]
    [Tooltip("공격 콜라이더 오브젝트 (PlayerAttackCollider 스크립트가 붙은)")]
    public GameObject attackCollider;
    
    [Tooltip("플레이어 애니메이터")]
    public Animator animator;

    // 공격 상태 관리
    private int queuedCombo = 0;
    private int currentCombo = 0;               // 현재 콤보 단계 (0=1타, 1=2타, 2=3타)
    private bool isAttacking = false;           // 현재 공격 중인지
    private bool hasQueuedAttack = false;

    private float lastAttackTime = -Mathf.Infinity;  // 마지막 공격 시간
    
    private float lastInputTime = -Mathf.Infinity;   // 마지막 콤보 시간

    // 입력 관리
    private bool attackInput;                   // 공격 입력 감지

    /// <summary>
    /// 게임 시작 시 초기화
    /// </summary>
    void Start()
    {
        // 애니메이터가 연결되지 않았으면 자동으로 찾기
        if (animator == null)
            animator = GetComponent<Animator>();
            
        // 공격 콜라이더 비활성화 (기본 상태)
        if (attackCollider != null)
            attackCollider.SetActive(false);
    }

    /// <summary>
    /// 매 프레임마다 입력 처리 및 공격 관리
    /// </summary>
    void Update()
    {
        HandleInput();
        HandleComboSystem();
    }

    /// <summary>
    /// 플레이어 입력 감지
    /// </summary>
    void HandleInput()
    {
        // Z키 또는 마우스 왼쪽 클릭으로 공격
        attackInput = Input.GetKeyDown(KeyCode.Z);
        if (attackInput)
        {
            lastInputTime = Time.time;
            Debug.Log("Z키 입력 감지!");
            
            // 공격 중이 아니면 즉시 공격
            if (!isAttacking)
            {
                StartAttack();
            }
            // 공격 중이면 다음 공격 예약
            else
            {
                QueueNextAttack();
            }
        }
    }


    void HandleComboSystem()
    {
        // 콤보 완전 리셋 (3초 동안 입력 없으면)
        if (Time.time >= lastInputTime + comboResetTime)
        {
            if (currentCombo != 0)
            {
                Debug.Log("콤보 완전 리셋!");
                currentCombo = 0;
                queuedCombo = 0;
                hasQueuedAttack = false;
            }
        }

        // 공격이 끝났는지 체크
        if (isAttacking && IsCurrentAttackFinished())
        {
            FinishCurrentAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // 현재 콤보에 따라 공격 실행
        ExecuteAttack(currentCombo);

        Debug.Log($"{currentCombo + 1}타 공격 실행!");
        
        Invoke(nameof(FinishCurrentAttack), 0.8f);
    }

    void QueueNextAttack()
    {
        // 콤보 윈도우 내에서만 예약 가능
        if (Time.time <= lastAttackTime + comboWindow)
        {
            queuedCombo = (currentCombo + 1) % 3;
            hasQueuedAttack = true;
            Debug.Log($"{queuedCombo + 1}타 공격 예약됨!");
        }
        else
        {
            Debug.Log("콤보 윈도우 벗어남 - 예약 실패");
        }
    }

    void ExecuteAttack(int comboIndex)
    {
        if (animator != null)
        {
            // 애니메이터에 공격 상태 설정
            animator.SetInteger("AttackState", comboIndex + 1);
        }

        // 공격 콜라이더 활성화 (타이밍에 맞춰)
        float activationDelay = GetAttackTiming(comboIndex);
        Invoke(nameof(ActivateAttackCollider), activationDelay);
    }

    float GetAttackTiming(int comboIndex)
    {
        // 각 콤보별로 다른 타이밍
        switch (comboIndex)
        {
            case 0: return 0.3f;  // 1타
            case 1: return 0.35f; // 2타
            case 2: return 0.4f;  // 3타
            default: return 0.3f;
        }
    }

    void FinishCurrentAttack()
    {
        isAttacking = false;
        
        // 애니메이터 리셋
        if (animator != null)
        {
            animator.SetInteger("AttackState", 0);
        }

        // 콤보 진행
        currentCombo = (currentCombo + 1) % 3;
        
        Debug.Log($"공격 완료! 다음 콤보: {currentCombo + 1}타");

        // 예약된 공격이 있으면 즉시 실행
        if (hasQueuedAttack)
        {
            hasQueuedAttack = false;
            Debug.Log("예약된 공격 실행!");
            StartAttack();
        }
    }

    bool IsCurrentAttackFinished()
    {
        if (animator == null) return true;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        // 현재 실행 중인 공격 애니메이션 확인
        string currentAttackName = $"attack{currentCombo + 1}";
        
        // 애니메이션이 80% 진행되면 완료로 간주
        return stateInfo.IsName(currentAttackName) && stateInfo.normalizedTime >= 0.8f;
    }

    void ActivateAttackCollider()
    {
        if (attackCollider != null)
        {
            float currentDamage = GetComboDamage(currentCombo);

            PlayerAttackCollider colliderScript = attackCollider.GetComponent<PlayerAttackCollider>();
            if (colliderScript != null)
            {
                Debug.Log("PlayerAttackCollider 스크립트 찾음!");
                colliderScript.SetDamage(currentDamage);
                colliderScript.StartAttack();
            }
            else
            {
                Debug.Log("PlayerAttackCollider 스크립트 못 찾음!");
            }

            // 0.2초 후 비활성화
            Invoke(nameof(DeactivateAttackCollider), 0.2f);
        }
        else
        {
            Debug.Log("attackCollider가 null임!");
        }
    }

    void DeactivateAttackCollider()
    {
        if (attackCollider != null)
        {
            PlayerAttackCollider colliderScript = attackCollider.GetComponent<PlayerAttackCollider>();
            if (colliderScript != null)
            {
                colliderScript.EndAttack();
            }
        }
    }

    float GetComboDamage(int comboIndex)
    {
        switch (comboIndex)
        {
            case 0: return attackDamage;         // 1타: 100%
            case 1: return attackDamage * 1.2f;  // 2타: 120%
            case 2: return attackDamage * 1.5f;  // 3타: 150%
            default: return attackDamage;
        }
    }

    // 공개 메서드들
    public bool IsAttacking() => isAttacking;
    public int GetCurrentCombo() => currentCombo;
    public bool HasQueuedAttack() => hasQueuedAttack;
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    [Header("Player Stats")]
    [Tooltip("플레이어의 최대 HP")]
    public float maxHP = 100f;

    [Tooltip("현재 HP (Inspector에서 확인용)")]
    public float currentHP;

    [Header("UI References")]
    [Tooltip("HP 바 UI (Slider 컴포넌트)")]
    [SerializeField]
    private Slider hpBar;

    // 컴포넌트 참조
    private SpriteRenderer spriteRenderer;

    // 상태 플래그
    private bool isDead = false;


    void Start()
    {
        // HP를 최대값으로 초기화
        currentHP = maxHP;

        // UI 업데이트
        UpdateHealthUI();

        // SpriteRenderer 컴포넌트 가져오기 (피격 효과 등에 사용)
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 테스트용 키보드 입력
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(10f); // Q키로 10 데미지
        }
    
        if (Input.GetKeyDown(KeyCode.E))
        {
            Heal(15f); // E키로 15 회복
        }
    
}

    public void TakeDamage(float MonsterDamage)
    {
        // 이미 죽었으면 더 이상 데미지 받지 않음
        if (isDead) return;

        // 현재 HP에서 데미지만큼 감소
        currentHP -= MonsterDamage;

        // HP가 0 미만으로 떨어지지 않도록 제한 (0 ~ maxHP 사이로 고정)
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        // HP 바 UI 업데이트
        UpdateHealthUI();

        // Hit 애니메이션 재생 (PlayerState 7번)
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetInteger("state", 7); // Hit 상태값 7
            Debug.Log("Hit 애니메이션 재생!");
            
            // 0.5초 후 Idle로 돌아가기
            Invoke(nameof(ResetHitAnimation), 0.7f);
        }

        // 디버그 로그 출력
        Debug.Log($"플레이어가 {MonsterDamage} 데미지를 받았습니다. 현재 HP: {currentHP}");

        // HP가 0이 되면 사망 처리
        if (currentHP <= 0)
        {
            PlayerDie();
        }
    }
    
    void ResetHitAnimation()
{
    Animator animator = GetComponent<Animator>();
    if (animator != null)
    {
        animator.SetInteger("state", 0); // Idle로 돌아가기
        Debug.Log("Hit 애니메이션 종료 - Idle로 복귀");
    }
}

    public void Heal(float healAmount)
    {
        // 죽은 상태에서는 회복 불가
        if (isDead) return;

        // 현재 HP에 회복량 추가
        currentHP += healAmount;

        // 최대 HP를 넘지 않도록 제한
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        // HP 바 UI 업데이트
        UpdateHealthUI();


        Debug.Log($"플레이어가 {healAmount} 회복했습니다. 현재 HP: {currentHP}");
    }

    void UpdateHealthUI()
    {
        // HP 바가 연결되어 있다면
        if (hpBar != null)
        {
            // Slider의 value는 0~1 사이의 값
            // 현재HP ÷ 최대HP = HP 비율
            hpBar.value = currentHP / maxHP;
        }
    }

    void PlayerDie()
    {
        // 사망 상태로 변경
        isDead = true;

        Debug.Log("플레이어 사망!");

        // 사망 애니메이션 재생 (애니메이터가 있다면)
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 이동 및 공격 비활성화
        DisablePlayerControls();

        // 2초 후 게임 오버 처리
        Invoke(nameof(GameOver), 2f);
    }

    void DisablePlayerControls()
    {
        // 이동 스크립트 비활성화
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // Rigidbody2D 정지
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");

        // 게임 오버 UI 표시
        // GameObject gameOverUI = GameObject.Find("GameOverUI");
        // if (gameOverUI != null) gameOverUI.SetActive(true);

        // 시간 정지
        // Time.timeScale = 0f;

        // 씬 재시작
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    

    // =========================
    // 외부에서 사용할 수 있는 공개 메서드들
    // =========================

    /// 현재 HP 반환  
    public float GetCurrentHP() => currentHP;

   
    /// 최대 HP 반환
    public float GetMaxHP() => maxHP;

    
    /// 사망 상태 확인
    public bool IsDead() => isDead;

    /// HP 비율 반환 (0~1 사이)
    public float GetHPRatio() => currentHP / maxHP;

   
    /// 최대 HP 설정 (레벨업 등에서 사용)
    public void SetMaxHP(float newMaxHP)
    {
        maxHP = newMaxHP;
        // 현재 HP가 새로운 최대값을 넘지 않도록 조정
        currentHP = Mathf.Min(currentHP, maxHP);
        UpdateHealthUI();
    }

    
    
}

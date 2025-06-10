using UnityEngine;

public class MonsterAttackCollider : MonoBehaviour
{
    private float damage = 10f;
    private bool hasHit = false;
    private bool isActive = false;

    void Start()
    {
        isActive = true;
        gameObject.SetActive(false); // 기본 비활성화
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || hasHit) return;
        
        if (other.CompareTag("Player"))
        {
            PlayerStatus player = other.GetComponent<PlayerStatus>();
            if (player != null)
            {
                player.TakeDamage(damage);
                hasHit = true;
                Debug.Log($"몬스터가 플레이어에게 {damage} 데미지!");
            }
        }
    }

    public void StartAttack()
    {
        hasHit = false;
        isActive = true;
        gameObject.SetActive(true);
    }

    public void EndAttack()
    {
        isActive = false;
        gameObject.SetActive(false);
        hasHit = false;
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
}

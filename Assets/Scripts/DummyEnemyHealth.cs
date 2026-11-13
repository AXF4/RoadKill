using UnityEngine;

public class DummyEnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHp = 50f;
    [SerializeField] private float currentHp;
    [SerializeField] private bool destroyOnDeath = true;

    private Rigidbody2D rb;

    private void Awake()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage, Vector2 hitDirection, GameObject attacker)
    {
        currentHp -= damage;
        Debug.Log($"[RoadKill] {gameObject.name} 피격! 남은 HP: {currentHp}");

        // 넉백 반응 (적이 Rigidbody2D를 가지고 있을 경우)
        if (rb != null)
        {
            rb.AddForce(hitDirection * 15f, ForceMode2D.Impulse);
        }

        // 사망 처리 (임시로 파괴 또는 비활성화)
        if (currentHp <= 0)
        {
            Debug.Log($"[RoadKill] {gameObject.name} 처치!");
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}
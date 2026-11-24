using UnityEngine;

public class EnemyHealthEX : MonoBehaviour
{
    [SerializeField] private PlayerHealthManager playerHealth;
    [SerializeField] private float damage = 15f;

    public void AttackPlayer()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("EnemyHealthEX에 PlayerHealthManager를 연결하세요.");
            return;
        }

        playerHealth.TakeDamage(damage, Vector2.zero, gameObject);
    }
}

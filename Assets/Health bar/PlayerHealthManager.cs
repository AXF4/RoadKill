using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private playerHealth healthBar;

    private float currentHealth;
    private bool isDead;

    private VehicleController vehicleController;
    private Rigidbody2D rb;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = maxHealth;

        vehicleController = GetComponent<VehicleController>();
        rb = GetComponent<Rigidbody2D>();

        // Inspector에서 체력바를 연결하지 않았을 때 씬에서 찾습니다.
        if (healthBar == null)
        {
            healthBar = FindFirstObjectByType<playerHealth>();
        }
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    public void TakeDamage(
        float damage,
        Vector2 hitDirection,
        GameObject attacker)
    {
        if (isDead || damage <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        UpdateHealthBar();

        Debug.Log($"플레이어 체력: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning("체력바의 playerHealth를 찾지 못했습니다.");
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        // 사망하면 플레이어 차량 이동을 멈춥니다.
        if (vehicleController != null)
        {
            vehicleController.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Debug.Log("플레이어 사망");
    }
}
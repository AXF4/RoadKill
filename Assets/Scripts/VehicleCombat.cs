using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleController), typeof(Rigidbody2D))]
public class VehicleCombat : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private float rushDamage = 100f;
    [SerializeField] private float minRushSpeed = 8f;
    [SerializeField] private float boostStartGraceTime = 0.2f; // 돌진 시작 직후에는 속도가 0이어도 타격 허용
    [SerializeField] private float hitStopDuration = 0.08f;
    [SerializeField] private float hitCooldown = 0.35f;

    [Header("피격자 태그")]
    [SerializeField] private string enemyTag = "Enemy";

    private VehicleController controller;
    private Rigidbody2D rb;
    private readonly Dictionary<GameObject, float> lastHitTimes = new Dictionary<GameObject, float>();

    private void Awake()
    {
        controller = GetComponent<VehicleController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) => TryRushAttack(collision.gameObject);
    private void OnCollisionStay2D(Collision2D collision) => TryRushAttack(collision.gameObject);
    private void OnTriggerEnter2D(Collider2D other) => TryRushAttack(other.gameObject);
    private void OnTriggerStay2D(Collider2D other) => TryRushAttack(other.gameObject);

    private void TryRushAttack(GameObject target)
    {
        var damageable = target.GetComponent<IDamageable>();
        if (damageable == null && !target.CompareTag(enemyTag)) return;

        // 1. 돌진 중인지 확인
        // - 일반 주행 돌진: 부스트 중 + 최소 속도 이상
        // - 이미 붙어있는 상태: 부스트를 누른 직후(boostStartGraceTime 이내)라면 속도가 낮아도 초기 돌진 타격 인정
        bool isJustStartedBoost = controller.IsBoosting && (Time.time - controller.BoostStartTime <= boostStartGraceTime);
        bool isHighSpeedBoost = controller.IsBoosting && rb.linearVelocity.magnitude >= minRushSpeed;

        if (!isJustStartedBoost && !isHighSpeedBoost) return;

        // 2. 개별 적 피격 쿨타임 체크
        if (lastHitTimes.TryGetValue(target, out float lastTime))
        {
            if (Time.time - lastTime < hitCooldown) return;
        }
        lastHitTimes[target] = Time.time;

        // 3. 넉백 방향 계산 (정지 상태에서 들이박을 경우 적 방향 또는 차체 정면 방향 사용)
        Vector2 knockbackDir;
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            knockbackDir = rb.linearVelocity.normalized;
        }
        else
        {
            Vector2 toTarget = (Vector2)target.transform.position - rb.position;
            knockbackDir = toTarget.sqrMagnitude > 0.001f ? toTarget.normalized : (Vector2)transform.up;
        }

        damageable?.TakeDamage(rushDamage, knockbackDir, gameObject);

        if (HitStopManager.Instance != null)
        {
            HitStopManager.Instance.TriggerHitStop(gameObject, target, hitStopDuration);
        }
    }
}
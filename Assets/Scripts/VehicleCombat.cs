using System.Collections;
using UnityEngine;

[RequireComponent(typeof(VehicleController), typeof(Rigidbody2D))]
public class VehicleCombat : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private float rushDamage = 100f;       // 돌진 시 가하는 대미지
    [SerializeField] private float minRushSpeed = 8f;        // 공격 판정이 인정되는 최소 돌진 속도
    [SerializeField] private float hitStopDuration = 0.05f;  // 역경직(타격감) 지속 시간

    [Header("피격자 태그")]
    [SerializeField] private string enemyTag = "Enemy";

    private VehicleController controller;
    private Rigidbody2D rb;

    private void Awake()
    {
        controller = GetComponent<VehicleController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleImpact(collision.gameObject, collision.contacts[0].point);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleImpact(other.gameObject, other.ClosestPoint(transform.position));
    }

    private void HandleImpact(GameObject target, Vector2 impactPoint)
    {
        // 1. 대상이 적인지 확인 (태그 또는 IDamageable)
        var damageable = target.GetComponent<IDamageable>();
        if (damageable == null && !target.CompareTag(enemyTag)) return;

        // 2. 현재 돌진(부스트) 중이고 일정 속도 이상인지 체크
        bool isRushing = controller.IsBoosting && rb.linearVelocity.magnitude >= minRushSpeed;

        if (isRushing)
        {
            Vector2 knockbackDir = rb.linearVelocity.normalized;
            damageable?.TakeDamage(rushDamage, knockbackDir, gameObject);

            // HitStopManager 호출: 공격자(플레이어)와 피격자(적)를 전달하여 두 대상만 살리고 화면 회색조 탈색
            if (HitStopManager.Instance != null)
            {
                HitStopManager.Instance.TriggerHitStop(gameObject, target, hitStopDuration);
            }
        }
        else
        {
            // [일반 저속 충돌]
            // 단순 접촉 시에는 대미지를 주지 않고 차체가 튕기거나 멈춤
            Debug.Log("[RoadKill] 일반 접촉: 돌진 상태가 아니어서 적을 들이받지 못했습니다.");
        }
    }

    private IEnumerator HitStopRoutine()
    {
        // 충돌 순간 찰나의 프레임 정지 (타격감)
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }
}
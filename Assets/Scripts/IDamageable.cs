using UnityEngine;

public interface IDamageable
{
    // 피격 처리: 대미지 양, 충돌 지점/넉백 방향, 공격 주체
    void TakeDamage(float damage, Vector2 hitDirection, GameObject attacker);
}
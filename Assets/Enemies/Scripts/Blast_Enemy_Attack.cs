using UnityEngine;

public class Blast_Enemy_Attack : MonoBehaviour
{
    [Header("연결")]
    [SerializeField]
    private Blast_Enemy_Projectile projectilePrefab;

    [SerializeField]
    private Transform attackPoint;

    [Header("공격 설정")]
    [SerializeField, Min(0.1f)]
    private float attackInterval = 2f;

    private float timer;

    private void Start()
    {
        if (projectilePrefab == null || attackPoint == null)
        {
            Debug.LogError(
                "Blast_Enemy_Attack: 발사체 프리팹과 발사 위치를 연결해주세요.",
                this
            );

            enabled = false;
            return;
        }

        // 시작하고 2초 뒤 첫 공격을 한다.
        timer = attackInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Attack();
            timer = attackInterval;
        }
    }

    private void Attack()
    {
        // 발사 위치와 방향에 맞춰 발사체를 생성한다.
        Instantiate(
            projectilePrefab,
            attackPoint.position,
            attackPoint.rotation
        );
    }
}
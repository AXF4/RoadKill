using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Dealer_AI : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private Transform target;
    [SerializeField] private Dealer_Projectile projectilePrefab;

    [Header("이동")]
    [SerializeField] private float playerBaseSpeed = 6f;
    [SerializeField] private float speedPercent = 80f;

    [Header("유지 거리")]
    [SerializeField] private float cellSize = 4f;
    [SerializeField] private float keepDistanceCells = 10f;
    [SerializeField] private float toleranceCells = 0.5f;

    [Header("발사 - 임시 테스트 값")]
    [SerializeField, Min(0.1f)] private float attackInterval = 2f;
    [SerializeField] private float spreadAngle = 15f;

    private Rigidbody2D rb;
    private float attackTimer;

    private float MoveSpeed =>
        playerBaseSpeed * speedPercent / 100f;

    private float KeepDistance =>
        cellSize * keepDistanceCells;

    private float Tolerance =>
        cellSize * toleranceCells;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        attackTimer = attackInterval;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 toPlayer = (Vector2)target.position - rb.position;
        float distance = toPlayer.magnitude;

        Vector2 direction = distance > 0.001f
            ? toPlayer / distance
            : Vector2.up;

        float nearDistance = KeepDistance - Tolerance;
        float farDistance = KeepDistance + Tolerance;

        if (distance > farDistance)
        {
            // 멀면 플레이어를 따라간다.
            rb.linearVelocity = direction * MoveSpeed;
        }
        else if (distance < nearDistance)
        {
            // 너무 가까우면 반대 방향으로 물러난다.
            rb.linearVelocity = -direction * MoveSpeed;
        }
        else
        {
            // 목표 거리 근처에서는 멈춘다.
            rb.linearVelocity = Vector2.zero;
        }

        attackTimer = Mathf.Max(
            0f,
            attackTimer - Time.fixedDeltaTime
        );

        // 42유닛 이내에서만 발사한다.
        // 가까워서 후퇴하는 중에도 발사할 수 있다.
        if (distance <= farDistance &&
            attackTimer <= 0f &&
            projectilePrefab != null)
        {
            ShootThreeCards(direction);
            attackTimer = attackInterval;
        }
    }

    private void ShootThreeCards(Vector2 direction)
    {
        // 스프라이트 위쪽(+Y)을 플레이어 방향에 맞춘다.
        float centerAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        for (int i = -1; i <= 1; i++)
        {
            float angle = centerAngle + spreadAngle * i;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 spawnPosition =
                new Vector3(rb.position.x, rb.position.y, 0f);

            Dealer_Projectile card = Instantiate(
                projectilePrefab,
                spawnPosition,
                rotation
            );

            Vector2 shootDirection = rotation * Vector3.up;
            card.Launch(shootDirection);
        }
    }

    private void OnDisable()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        attackTimer = attackInterval;
    }
}
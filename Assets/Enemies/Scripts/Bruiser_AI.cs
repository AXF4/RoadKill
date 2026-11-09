using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bruiser_AI : MonoBehaviour
{
    private enum State
    {
        Chase,
        Windup,
        Recovery
    }

    [Header("연결")]
    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private SpriteRenderer attackWarning;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("이동")]
    [SerializeField] private float playerBaseSpeed = 6f;
    [SerializeField] private float speedPercent = 100f;

    [Header("공격 - 임시 테스트 값")]
    [SerializeField] private float cellSize = 4f;
    [SerializeField] private float attackRangeCells = 1f;
    [SerializeField] private float windupTime = 0.7f;
    [SerializeField] private float recoveryTime = 0.8f;

    private Rigidbody2D rb;
    private State state = State.Chase;
    private float timer;
    private Vector2 attackCenter;
    private Color originalColor;

    private float MoveSpeed =>
        playerBaseSpeed * speedPercent / 100f;

    private float AttackRange =>
        cellSize * attackRangeCells;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (visual != null)
            originalColor = visual.color;

        if (attackWarning != null)
            attackWarning.enabled = false;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;
            ResetAttack();
            return;
        }

        Vector2 toPlayer = (Vector2)target.position - rb.position;

        switch (state)
        {
            case State.Chase:
                if (toPlayer.magnitude > AttackRange)
                {
                    rb.linearVelocity =
                        toPlayer.normalized * MoveSpeed;
                }
                else
                {
                    BeginAttack();
                }
                break;

            case State.Windup:
                rb.linearVelocity = Vector2.zero;
                timer -= Time.fixedDeltaTime;

                // 예고 시작 시 정한 위치를 유지한다.
                if (attackWarning != null)
                {
                    attackWarning.transform.position =
                        new Vector3(attackCenter.x, attackCenter.y, 0f);
                }

                if (timer <= 0f)
                {
                    CheckAttackHit();

                    if (attackWarning != null)
                        attackWarning.enabled = false;

                    if (visual != null)
                        visual.color = Color.red;

                    state = State.Recovery;
                    timer = recoveryTime;
                }
                break;

            case State.Recovery:
                rb.linearVelocity = Vector2.zero;
                timer -= Time.fixedDeltaTime;

                if (timer <= 0f)
                    ResetAttack();
                break;
        }
    }

    private void BeginAttack()
    {
        rb.linearVelocity = Vector2.zero;

        // 공격 위치를 고정해서 플레이어가 피할 수 있게 한다.
        attackCenter = rb.position;
        state = State.Windup;
        timer = windupTime;

        if (visual != null)
            visual.color = Color.yellow;

        if (attackWarning != null)
        {
            attackWarning.transform.position =
                new Vector3(attackCenter.x, attackCenter.y, 0f);

            // 기본 Circle 스프라이트의 지름은 1유닛.
            attackWarning.transform.localScale =
                new Vector3(AttackRange * 2f, AttackRange * 2f, 1f);

            attackWarning.color = new Color(1f, 0f, 0f, 0.25f);
            attackWarning.enabled = true;
        }
    }

    private void CheckAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackCenter,
            AttackRange,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            // 지정한 플레이어와 그 자식 충돌체만 확인한다.
            bool isTarget =
                hit.transform == target ||
                hit.transform.IsChildOf(target);

            if (!isTarget)
                continue;

            // 사이에 벽이 있으면 공격하지 않는다.
            RaycastHit2D wall = Physics2D.Linecast(
                attackCenter,
                hit.bounds.center,
                wallLayer
            );

            if (wall.collider != null)
                continue;

            Debug.Log("난동꾼 근접 공격 명중", this);

            // 수아의 체력 기능이 완성되면 여기서 피해 함수를 호출.
            // 플레이어 충돌체가 여러 개여도 한 번만 처리한다.
            break;
        }
    }

    private void ResetAttack()
    {
        state = State.Chase;

        if (visual != null)
            visual.color = originalColor;

        if (attackWarning != null)
            attackWarning.enabled = false;
    }

    private void OnDisable()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        ResetAttack();
    }
}
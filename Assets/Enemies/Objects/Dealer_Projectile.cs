using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Dealer_Projectile : MonoBehaviour
{
    [Header("발사체 설정")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 4f;

    [Header("충돌 대상")]
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool launched;

    private ContactFilter2D filter;
    private readonly RaycastHit2D[] results = new RaycastHit2D[8];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        filter = new ContactFilter2D();
        filter.SetLayerMask(hitLayers);
        filter.useTriggers = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        launched = direction.sqrMagnitude > 0f;
    }

    private void FixedUpdate()
    {
        if (!launched)
            return;

        float moveDistance = speed * Time.fixedDeltaTime;

        int hitCount = rb.Cast(
            direction,
            filter,
            results,
            moveDistance
        );

        if (hitCount > 0)
        {
            // 가장 먼저 부딪히는 대상을 고른다.
            RaycastHit2D nearest = results[0];

            for (int i = 1; i < hitCount; i++)
            {
                if (results[i].distance < nearest.distance)
                    nearest = results[i];
            }

            int hitLayer = nearest.collider.gameObject.layer;
            bool hitPlayer = (playerLayer.value & (1 << hitLayer)) != 0;

            if (hitPlayer)
            {
                Debug.Log("딜러 카드 명중", this);

                // 수아의 체력 기능이 완성되면 여기서 피해 함수를 호출.
            }

            launched = false;
            Destroy(gameObject);
            return;
        }

        rb.MovePosition(rb.position + direction * moveDistance);
    }
}
using UnityEngine;

public class Blast_Enemy_Projectile : MonoBehaviour
{
    [Header("발사체 설정")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0.1f)] private float lifeTime = 3f;

    private void Start()
    {
        // 생성 후 지정한 시간이 지나면 제거한다.
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 발사체 자신의 위쪽 방향으로 이동한다.
        transform.position +=
            transform.up * moveSpeed * Time.deltaTime;
    }
}
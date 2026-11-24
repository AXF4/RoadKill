using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;                           // PlayerVehicle
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // 2D 카메라 Z축 유지

    // 물리 기반 오브젝트는 FixedUpdate에서 정확히 같은 타이밍에 위치를 덮어써야 칼고정됩니다.
    private void FixedUpdate()
    {
        if (target == null) return;

        // 지연 보간 없이 플레이어 위치 + 오프셋으로 바로 고정
        transform.position = target.position + offset;
    }
}
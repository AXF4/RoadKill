using UnityEngine;

[CreateAssetMenu(fileName = "NewVehicleData", menuName = "RoadKill/Vehicle Data")]
public class VehicleData : ScriptableObject
{
    [Header("기본 이동 (WASD)")]
    public float normalSpeed = 6f;
    public float normalAcceleration = 20f;
    public float normalDeceleration = 15f;
    public float turnSpeed = 8f; // 차체 회전 부드러움

    [Header("돌진 & 관성 드리프트 (Space)")]
    public float boostMaxSpeed = 18f;     // 탈것마다 다른 최대 돌진 속도 
    public float boostAcceleration = 12f; // 점점 빨라지는 가속도
    public float driftSteerForce = 3.5f;   // 관성 강도 (낮을수록 미끄러지듯 드리프트)
    public float driftAngularSpeed = 5f;  // 마우스 방향으로 차체가 회전하는 속도.

    [Header("운전석 위치 세팅")]
    public Vector2 driverSeatOffset = new Vector2(0f, 0.3f);
}
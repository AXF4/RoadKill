using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleController : MonoBehaviour
{
    [Header("탈것 데이터")]
    [SerializeField] private VehicleData vehicleData;

    [Header("참조 트랜스폼")]
    [SerializeField] private Transform driverSeat;
    [SerializeField] private Camera mainCam;

    [Header("벽 충돌 설정")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float boostWallBounceForce = 12f; // 돌진 중 벽 충돌 반동 세기
    [SerializeField] private float normalWallBounceForce = 4f; // 일반 주행 중 벽 충돌 반동 세기
    [SerializeField] private float wallStunDuration = 0.2f;    // 벽 충돌 후 조작 경직 시간

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isBoosting;
    public bool IsBoosting => isBoosting;
    public float BoostStartTime { get; private set; }

    private float currentBoostSpeed;
    private bool isWallStunned;
    private Coroutine wallStunRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (mainCam == null) mainCam = Camera.main;

        ApplySeatPosition();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 벽 충돌 경직 중에는 입력 무시
        if (isWallStunned)
        {
            moveInput = Vector2.zero;
            return;
        }

        // 1. 이동 입력
        float h = 0f;
        float v = 0f;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) v += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) v -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) h -= 1f;

        moveInput = new Vector2(h, v).normalized;

        // 2. 스페이스바 돌진 체크
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            isBoosting = true;
            BoostStartTime = Time.time;
            float baseSpeed = vehicleData != null ? vehicleData.normalSpeed : 6f;
            currentBoostSpeed = Mathf.Max(rb.linearVelocity.magnitude, baseSpeed);
        }
        else if (keyboard.spaceKey.wasReleasedThisFrame)
        {
            isBoosting = false;
        }
    }

    private void FixedUpdate()
    {
        // 벽에 부딪혀 튕겨나가는 동안에는 관성 감속만 적용
        if (isWallStunned)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 5f * Time.fixedDeltaTime);
            return;
        }

        float normalSpd = vehicleData != null ? vehicleData.normalSpeed : 6f;
        float normalAcc = vehicleData != null ? vehicleData.normalAcceleration : 20f;
        float normalDec = vehicleData != null ? vehicleData.normalDeceleration : 15f;
        float turnSpd = vehicleData != null ? vehicleData.turnSpeed : 8f;

        float boostMax = vehicleData != null ? vehicleData.boostMaxSpeed : 18f;
        float boostAcc = vehicleData != null ? vehicleData.boostAcceleration : 12f;
        float driftSteer = vehicleData != null ? vehicleData.driftSteerForce : 3.5f;
        float driftTurn = vehicleData != null ? vehicleData.driftAngularSpeed : 5f;

        if (isBoosting)
        {
            HandleBoostMovement(boostMax, boostAcc, driftSteer, driftTurn);
        }
        else
        {
            HandleNormalMovement(normalSpd, normalAcc, normalDec, turnSpd);
        }
    }

    private void HandleNormalMovement(float speed, float accel, float decel, float turnSpeed)
    {
        Vector2 targetVelocity = moveInput * speed;
        float rate = (moveInput.sqrMagnitude > 0.01f) ? accel : decel;

        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, rate * Time.fixedDeltaTime);

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = Mathf.LerpAngle(rb.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleBoostMovement(float maxSpeed, float accel, float steerForce, float turnSpeed)
    {
        var mouse = Mouse.current;
        Vector2 aimDirection = transform.up;

        if (mouse != null && mainCam != null)
        {
            Vector2 mouseScreenPos = mouse.position.ReadValue();
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCam.transform.position.z));
            aimDirection = ((Vector2)mouseWorldPos - rb.position).normalized;
        }

        currentBoostSpeed = Mathf.MoveTowards(currentBoostSpeed, maxSpeed, accel * Time.fixedDeltaTime);

        Vector2 desiredVelocity = aimDirection * currentBoostSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, steerForce * Time.fixedDeltaTime);

        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = Mathf.LerpAngle(rb.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(wallTag)) return;

        // 벽 표면의 수직 바깥 방향(법선 벡터)
        Vector2 normal = collision.contactCount > 0 ? collision.GetContact(0).normal : -rb.linearVelocity.normalized;

        if (isBoosting)
        {
            // 돌진 중 벽에 박으면 부스트 강제 해제 + 강하게 튕겨나감 + 짧은 조작 경직
            isBoosting = false;
            currentBoostSpeed = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(normal * boostWallBounceForce, ForceMode2D.Impulse);

            if (wallStunRoutine != null) StopCoroutine(wallStunRoutine);
            wallStunRoutine = StartCoroutine(WallStunRoutine());
        }
        else
        {
            // 일반 주행 충돌: 가볍게 밀어냄
            rb.AddForce(normal * normalWallBounceForce, ForceMode2D.Impulse);
        }
    }

    private IEnumerator WallStunRoutine()
    {
        isWallStunned = true;
        yield return new WaitForSeconds(wallStunDuration);
        isWallStunned = false;
        wallStunRoutine = null;
    }

    public void ApplySeatPosition()
    {
        if (vehicleData != null && driverSeat != null)
        {
            driverSeat.localPosition = vehicleData.driverSeatOffset;
        }
    }

    private void OnValidate()
    {
        ApplySeatPosition();
    }
}
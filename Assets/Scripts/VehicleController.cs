using UnityEngine;
using UnityEngine.InputSystem; // New Input System 네임스페이스

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleController : MonoBehaviour
{
    [Header("탈것 데이터")]
    [SerializeField] private VehicleData vehicleData;

    [Header("참조 트랜스폼")]
    [SerializeField] private Transform driverSeat;
    [SerializeField] private Camera mainCam;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isBoosting;
    public bool IsBoosting => isBoosting;
    private float currentBoostSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (mainCam == null) mainCam = Camera.main;

        ApplySeatPosition();
    }

    private void Update()
    {
        // 1. 하드웨어 키보드 입력 직접 감지 (New Input System)
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

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
        // vehicleData가 없으면 기본값으로라도 움직이게 방어 코드 처리
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
        Vector2 aimDirection = transform.up; // 마우스 없을 때 기본 전방

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
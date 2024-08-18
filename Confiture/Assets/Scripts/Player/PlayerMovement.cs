using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{

    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    public PlayerEntity player;

    [SerializeField] LayerMask blobMask;

    [Header("Global parameters")]
    public bool speedDoubleMetrics = true;
    public bool jumpDoubleMetrics = true;
    public bool dashDoubleMetrics = true;

    public Vector2 initialDirection = Vector3.zero;
    public float initialVelocityTime = .5f;

    float speed = 2;
    [Header("Walk parameters")]
    [Range(1f, 100f)] public float minWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float minGroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float minGroundDeceleration = 20f;
    [Range(0.25f, 50f)] public float minAirAcceleration = 5f;
    [Range(0.25f, 50f)] public float minAirDeceleration = 5f;

    [Header("Ground parameters")]
    public LayerMask groundLayer;
    public float groundRayLenght;
    public float headRayLenght;
    [Range(0f, 1f)]public float headWidth = .75f;

    [Header("Jump parameters")]
    public float minJumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float minTimeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;
    [Range(1, 5)] int numberOfJumpAllowed = 1;

    [Header("Jump cut")]
    [Range(0.02f, 0.03f)] public float timeForUpwardCancel = 0.027f;

    [Header("Jump apex")]
    [Range(0.05f, 1f)] public float apexTreshold = 0.97f;
    [Range(0.01f, 1f)] public float apexHangTime = 0.075f;

    [Header("Jump buffer")]
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;

    [Header("Jump coyote time")]
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;

    [Header("Dash")]
    //[Range(0f, 1f)] public float minDashTime = 0.11f;
    [Range(1f, 200f)] public float minDashSpeed = 40f;
    [Range(.5f, 1f)] public float dashAimPrecision = .8f;
    public bool manualDashNumber = false;
    [Range(0, 10)] public int numberOfDashes = 5;
    [Space]
    [Range(0.01f, 100f)] public float dashBlobRange = 5f;
    [Range(0f, 1f)] public float dashBufferTime = 0.125f;

    [Header("Dash Cancel Time")]
    [Range(0.01f, 5f)] public float dashGravityOnReleaseMultiplier = 1f;
    [Range(0.02f, 0.3f)] public float dashTimeForUpwardsCancel = 0.027f;

    [Header("Walk Max parameters")]
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float maxGroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float maxGroundDeceleration = 20f;
    [Range(0.25f, 50f)] public float maxAirAcceleration = 5f;
    [Range(0.25f, 50f)] public float maxAirDeceleration = 5f;

    [Header("Jump Max parameters")]
    public float maxJumpHeight = 7f;
    public float maxTimeTillJumpApex = 0.35f;

    [Header("Dash Max parameters")]
    //public float maxDashTime = 0.11f;
    public float maxDashSpeed = 40f;
    private float dashTime = 1f;

    private float gravity;
    private float initialJumpVelocity;
    private float adjustedJumpHeight;

    [HideInInspector] public float verticalVelocity;
    [HideInInspector] public bool isJumping;
    bool isFastFalling;
    bool isFalling;
    float fastFallTime;
    float fastFallReleaseTime;
    int numberOfJumpUsed;

    float apexPoint;
    float timePastApexTreshold;
    bool isPastApexTreshold;

    float jumpBufferTimer;
    bool jumpReleaseDuringBuffer;

    float dashBufferTimer;
    bool dashBuffered;

    float coyoteTimer;
    bool waitForJumpRelease = false;

    [HideInInspector] public bool isDashing;
    private float dashTimer;
    private int numberOfDashesUsed;
    private Vector3 dashDirection;
    [HideInInspector] public bool isDashFastFalling;
    [HideInInspector] public float dashFastFallTime;
    [HideInInspector] public float dashFastFallReleaseSpeed;
    bool waitForDashRelease = false;

    bool startDashing = false;

    bool addInitialVelocity = false;

    Rigidbody rb;
    [SerializeField] Collider feetCol;
    [SerializeField] Collider headCol;

    float horizontalVelocity;
    bool isFacingRight;

    RaycastHit groundHit;
    RaycastHit headHit;

    bool isGrounded = false;
    bool bumpedHead = false;

    LineRenderer lineRenderer;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        moveAction = input.actions.FindAction("Move");
        jumpAction = input.actions.FindAction("Jump");
        dashAction = input.actions.FindAction("Dash");
    }

    private void Update()
    {
        UpdateTimer();
        JumpCheck();
        LandCheck();
        DashCheck();
    }

    void FixedUpdate()
    {
        CollisionCheck();
        Jump();
        Fall();
        Dash();

        float input = player.lockInput ? 0 : moveAction.ReadValue<float>();

        if(isGrounded)
        {
            if(speedDoubleMetrics)
            {
                float groundAcceleration = Mathf.Lerp(minGroundAcceleration, maxGroundAcceleration, player.blobRatio);
                float groundDeceleration = Mathf.Lerp(minGroundDeceleration, maxGroundDeceleration, player.blobRatio);
                MovePlayer(groundAcceleration, groundDeceleration, input);
            }
            else
            {
                MovePlayer(minGroundAcceleration, minGroundDeceleration, input);
            }

        }
        else
        {
            if (speedDoubleMetrics)
            {
                float airAcceleration = Mathf.Lerp(minAirAcceleration, maxAirAcceleration, player.blobRatio);
                float airDeceleration = Mathf.Lerp(minAirDeceleration, maxAirDeceleration, player.blobRatio);
                MovePlayer(airAcceleration, airDeceleration, input);
            }
            else
            {
                MovePlayer(minAirAcceleration, minAirDeceleration, input);
            }
        }


        if(!addInitialVelocity)
        {
            if(initialVelocityTime > 0f)
            {
                rb.velocity = initialDirection;
                initialVelocityTime -= Time.fixedTime;
            }
            else
                addInitialVelocity = true;
        }
        else
            ApplyVelocity();

    }

    private void ApplyVelocity()
    {
        // Clamp Fall Speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, 50f);

        rb.velocity = new Vector3(horizontalVelocity, verticalVelocity, rb.velocity.z);
    }

    private void MovePlayer(float acceleration, float deceleration, float input)
    {

        if(input != 0f)
        {
            TurnCheck(input);

            float actualWalkSpeed = Mathf.Lerp(minWalkSpeed, maxWalkSpeed, player.blobRatio);

            float targetVelocity = input * actualWalkSpeed;

            horizontalVelocity = Mathf.Lerp(horizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else if(input == 0f)
        {
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
        }

    }

    private void TurnCheck(float input)
    {
        if (isFacingRight && input < 0) Turn(false);
        else if (!isFacingRight && input > 0) Turn(true);
    }

    private void Turn(bool right)
    {
        if(right)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    private void IsGrounded()
    {
        Vector3 boxCastOrigin = new Vector3(feetCol.bounds.center.x, feetCol.bounds.min.y + 1, feetCol.bounds.center.z);

        isGrounded = Physics.Raycast(boxCastOrigin, Vector3.down, out groundHit, groundRayLenght, groundLayer);
    }

    private void BumbedHead()
    {
        Vector3 boxCastOrigin = new Vector3(feetCol.bounds.center.x, headCol.bounds.max.y, feetCol.bounds.center.z);

        bumpedHead = Physics.Raycast(boxCastOrigin, Vector3.up, out headHit, headRayLenght, groundLayer);
    }

    private void CollisionCheck()
    {
        IsGrounded();
    }

    private void OnValidate()
    {
        CalculateValues();
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        float jumpHeight = minJumpHeight;
        float timeTillJumpApex = minTimeTillJumpApex;

        if (jumpDoubleMetrics && player)
        {
            jumpHeight = Mathf.Lerp(minJumpHeight, maxJumpHeight, player.blobRatio);
            timeTillJumpApex = Mathf.Lerp(minTimeTillJumpApex, maxTimeTillJumpApex, player.blobRatio);
        }

        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;

        gravity = (-2f * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initialJumpVelocity = Mathf.Abs(gravity) * timeTillJumpApex;
    }

    private void JumpCheck()
    {
        // When Pressed
        if (jumpAction.ReadValue<float>() > 0 && !waitForJumpRelease && !player.lockInput)
        {
            waitForJumpRelease = true;
             
            jumpBufferTimer = jumpBufferTime;
            jumpReleaseDuringBuffer = false;
        }

        // When Release
        if (jumpAction.ReadValue<float>() == 0 && waitForJumpRelease && !player.lockInput)
        {
            waitForJumpRelease = false;

            if (jumpBufferTimer > 0f)
            {
                jumpReleaseDuringBuffer = true;
            }

            if(isJumping && verticalVelocity > 0f)
            {
                if(isPastApexTreshold)
                {
                    isPastApexTreshold = false;
                    isFastFalling = true;
                    fastFallTime = timeForUpwardCancel;
                    verticalVelocity = 0f;
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseTime = verticalVelocity;
                }
            }
        }
        

        // Initiate jump with buffering and coyote
        if(jumpBufferTimer > 0 && !isJumping && (isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if(jumpReleaseDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseTime = verticalVelocity;
            }
        }
        
        // double jump
        else if(jumpBufferTimer > 0f && isJumping && numberOfJumpUsed < numberOfJumpAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }

        // Air jump after coyote
        else if(jumpBufferTimer > 0f && isFalling && numberOfJumpUsed < numberOfJumpAllowed - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }
    }

    private void LandCheck()
    {
        if ((isJumping || isFalling) && isGrounded && verticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexTreshold = false;
            numberOfJumpUsed = 0;

            verticalVelocity = Physics.gravity.y;
        }

        if(startDashing && isGrounded)
        {
            startDashing = false;
        }
    }

    private void Fall()
    {
        // Normal Gravity while falling
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }

            verticalVelocity += gravity * Time.fixedDeltaTime;
        }
    }

    private void InitiateJump(int numberOfJumpUsed)
    {
        CalculateValues();

        if (!isJumping)
        {
            isJumping = true;
        }

        jumpBufferTimer = 0f;
        this.numberOfJumpUsed += numberOfJumpUsed;
        verticalVelocity = initialJumpVelocity;
    }

    private void Jump()
    {
        // Apply gravity
        if(isJumping)
        {
            // Check for headBump
            if(bumpedHead)
            {
                isFastFalling = true;
            }

            // Gravity on ascending
            if (verticalVelocity >= 0f)
            {
                // Apex controls
                apexPoint = Mathf.InverseLerp(initialJumpVelocity, 0f, verticalVelocity);

                if (apexPoint > apexTreshold)
                {
                    if (!isPastApexTreshold)
                    {
                        isPastApexTreshold = true;
                        timePastApexTreshold = 0f;
                    }

                    if (isPastApexTreshold)
                    {
                        timePastApexTreshold += Time.fixedDeltaTime;
                        if (timePastApexTreshold < apexHangTime)
                        {
                            verticalVelocity = 0f;
                        }
                        else
                        {
                            verticalVelocity = -0.01f;
                        }
                    }
                }
                else
                {
                    verticalVelocity += gravity * Time.fixedDeltaTime;
                    if (isPastApexTreshold)
                        isPastApexTreshold = false;
                }
            }
            // Gravity on descending
            else if (!isFastFalling)
            {
                verticalVelocity += gravity * gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (verticalVelocity < 0f)
            {
                if(!isFalling)
                {
                    isFalling = true;
                }
            }
        }

        // Jump cut
        if(isFastFalling)
        {
            if(fastFallTime >= timeForUpwardCancel)
            {
                verticalVelocity += gravity * gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if(fastFallTime < timeForUpwardCancel)
            {
                verticalVelocity = Mathf.Lerp(fastFallReleaseTime, 0f, (fastFallTime / timeForUpwardCancel));
            }

            fastFallTime += Time.fixedDeltaTime;
        }

        
    }

    private void ResetJumpValues()
    {
        isJumping = false;
        isFalling = false;
        isFastFalling = false;
        fastFallTime = 0f;
        isPastApexTreshold = false;
    }

    private void DashCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, dashBlobRange, blobMask);

        Blob closestBlob = null;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            if (collider.gameObject.TryGetComponent<Blob>(out Blob blob))
            {

                Vector3 direction = (blob.transform.position - transform.position).normalized;
                float dot = Vector3.Dot((player.playerShoot.mouseWorldPosition - transform.position).normalized, direction);

                float distancePlayerToBlob = Vector3.Distance(blob.transform.position, transform.position);

                bool wallBetween = Physics.Raycast(transform.position, direction, distancePlayerToBlob, groundLayer);

                if (closestBlob == null && dot > dashAimPrecision && blob.dashable && !wallBetween)
                {
                    closestBlob = collider.GetComponent<Blob>();
                    continue;
                }
                else if (closestBlob == null) continue;


                float distancePlayerToClosestBlob = Vector3.Distance(closestBlob.transform.position, transform.position);

                if (distancePlayerToClosestBlob < distancePlayerToBlob
                    && dot > dashAimPrecision
                    && blob.dashable)
                {
                    closestBlob = blob;
                }
            }
        }

        if (closestBlob != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, closestBlob.transform.position);


        }
        else
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
        }

        if (dashAction.ReadValue<float>() > 0 && !waitForDashRelease && !player.lockInput)
        {
            waitForDashRelease = true;

            

            if (closestBlob != null)
            {
                if (!startDashing)
                {
                    Invoke(nameof(StartDash), .2f);

                    if(!manualDashNumber)
                    {
                        numberOfDashes = player.blobNumber;
                    }
                    numberOfDashesUsed = numberOfDashes;
                }

                if (numberOfDashesUsed > 0)
                {
                    closestBlob.rb.velocity = Vector3.zero;
                    closestBlob.ActiveCollision();

                    dashDirection = (closestBlob.transform.position - transform.position).normalized;

                    InitiateDash();
                }
            }
            
        }

        if (dashAction.ReadValue<float>() == 0 && waitForDashRelease && !player.lockInput)
        {
            waitForDashRelease = false;
        }
    }

    private void StartDash()
    {
        startDashing = true;
    }

    private void InitiateDash()
    {
        numberOfDashesUsed--;
        isDashing = true;
        dashTimer = 0f;
        
        GameObject splashGo = Instantiate(player.splashPrefabDash, player.feetPos.position, Quaternion.identity);
        splashGo.transform.forward = dashDirection;

        Destroy(splashGo, 2f);

        player.LockInput(true);

        ResetJumpValues();
    }

    private void Dash()
    {
        if(isDashing)
        {
            dashTimer += Time.fixedDeltaTime;

            //float dashTime = minDashTime;
            float dashSpeed = minDashSpeed;

            if (dashDoubleMetrics)
            {
                //dashTime = Mathf.Lerp(minDashTime, maxDashTime, player.blobRatio);
                dashSpeed = Mathf.Lerp(minDashSpeed, maxDashSpeed, player.blobRatio);
            }

            if (dashTimer > dashTime)
            {
                if(isGrounded) ResetDashes();
                isDashing = false;
                player.LockInput(false);
                if (!isJumping)
                {
                    dashFastFallTime = 0f;
                    dashFastFallReleaseSpeed = verticalVelocity;

                    //if(!isGrounded)
                    //{
                    //    isDashFastFalling = true;
                    //}
                }

                return;
            }

            horizontalVelocity = dashSpeed * dashDirection.x;

            verticalVelocity = dashSpeed * dashDirection.y;
        }

        //else if (isDashFastFalling)
        //{
        //    if(verticalVelocity > 0f)
        //    {
        //        if (dashFastFallTime < dashTimeForUpwardsCancel)
        //        {
        //            verticalVelocity = Mathf.Lerp(dashFastFallReleaseSpeed, 0f, (dashFastFallTime / dashTimeForUpwardsCancel));
        //        }
        //        else if (dashFastFallTime >= dashTimeForUpwardsCancel)
        //        {
        //            verticalVelocity += gravity * dashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
        //        }

        //        dashFastFallTime -= Time.fixedDeltaTime;
        //    }
        //    else
        //    {
        //        verticalVelocity += gravity * dashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
        //    }
        //}
    }

    private void ResetDashValues()
    {
        isDashFastFalling = false;
    }

    private void ResetDashes()
    {
        numberOfDashesUsed = 0;
    }

    private void UpdateTimer()
    {
        jumpBufferTimer -= Time.deltaTime;

        if(!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = jumpCoyoteTime;
        }
        
    }
}

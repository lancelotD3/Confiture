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

public class PlayerMovement : MonoBehaviour
{
    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;

    [Header("Walk parameters")]
    float speed = 2;
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float groundAcceleration = 5f;
    [Range(0.25f, 50f)] public float groundDeceleration = 20f;
    [Range(0.25f, 50f)] public float airAcceleration = 5f;
    [Range(0.25f, 50f)] public float airDeceleration = 5f;

    [Header("Ground parameters")]
    public LayerMask groundLayer;
    public float groundRayLenght;
    public float headRayLenght;
    [Range(0f, 1f)]public float headWidth = .75f;

    [Header("Jump parameters")]
    public float jumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;
    [Range(1, 5)] int numberOfJumpAllowed = 2;

    [Header("Jump cut")]
    [Range(0.02f, 0.03f)] public float timeForUpwardCancel = 0.027f;

    [Header("Jump apex")]
    [Range(0.05f, 1f)] public float apexTreshold = 0.97f;
    [Range(0.01f, 1f)] public float apexHangTime = 0.075f;

    [Header("Jump buffer")]
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;

    [Header("Jump coyote time")]
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;

    private float gravity;
    private float initialJumpVelocity;
    private float adjustedJumpHeight;


    float verticalVelocity;
    bool isJumping;
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

    float coyoteTimer;
    bool waitForJumpRelease = false;
    
    Rigidbody rb;
    [SerializeField] Collider feetCol;
    [SerializeField] Collider headCol;

    Vector3 moveVelocity;
    bool isFacingRight;

    RaycastHit groundHit;
    RaycastHit headHit;

    bool isGrounded = false;
    bool bumpedHead = false;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        moveAction = input.actions.FindAction("Move");
        jumpAction = input.actions.FindAction("Jump");
    }

    private void Update()
    {
        UpdateTimer();
        JumpCheck();
    }

    void FixedUpdate()
    {
        CollisionCheck();
        Jump();

        float input = moveAction.ReadValue<float>();

        if(isGrounded)
        {
            MovePlayer(groundAcceleration, groundDeceleration, input);
        }
        else
        {
            MovePlayer(airAcceleration, airDeceleration, input);
        }

    }

    private void MovePlayer(float acceleration, float deceleration, float input)
    {

        if(input != 0f)
        {
            TurnCheck(input);

            Vector3 targetVelocity = new Vector3(input, 0f, 0f) * maxWalkSpeed;

            moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, rb.velocity.z);
        }
        else if(input == 0f)
        {
            moveVelocity = Vector3.Lerp(moveVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, rb.velocity.z);
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

        Debug.DrawLine(boxCastOrigin, boxCastOrigin + Vector3.down * groundRayLenght);
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
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;

        gravity = (-2f * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initialJumpVelocity = Mathf.Abs(gravity) * timeTillJumpApex;
    }

    private void JumpCheck()
    {
        // When Pressed
        if (jumpAction.ReadValue<float>() > 0 && !waitForJumpRelease)
        {
            waitForJumpRelease = true;
             
            jumpBufferTimer = jumpBufferTime;
            jumpReleaseDuringBuffer = false;
        }

        // When Release
        if (jumpAction.ReadValue<float>() == 0 && waitForJumpRelease)
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

        // Landed
        if((isJumping || isFalling) && isGrounded && verticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexTreshold = false;
            numberOfJumpUsed = 0;

            verticalVelocity = Physics.gravity.y;
        }
    }

    private void InitiateJump(int numberOfJumpUsed)
    {
        if(!isJumping)
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

        // Normal Gravity while falling
        if(!isGrounded && !isJumping)
        {
            if(!isFalling)
            {
                isFalling = true;
            }

            verticalVelocity += gravity * Time.fixedDeltaTime;
        }

        // Clamp Fall Speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, 50f);

        rb.velocity = new Vector3 (rb.velocity.x, verticalVelocity, rb.velocity.z);
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

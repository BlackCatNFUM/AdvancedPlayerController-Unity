using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AdvancedPlayerController : MonoBehaviour
{
    
    public enum MovementType
    {
        RealisticMovementRb,
        MoveSystemTransformRb,
        MoveSystemForceRb,
        RealisticMovementAddForceRb,
        RealisticMovementCc,
        MoveSystemCC,
    }
    
    
    // Select The Component
    [Header("--------------------------------------")]
    [Header("Select the Type Of Your Character")]
    [Header("Rigid Body")]
    [SerializeField] private Rigidbody rb;
    [Header("Character Controller")]
    [SerializeField] private CharacterController cc;
    
    
    // Selecting Movement Type From Menu
    [Header("--------------------------------------")]
    [Header("Select Your Movement Type")]
    public MovementType selectedMovementType;
    
    
    // Camera Setting
    [Header("--------------------------------------")]
    [Header("Camera Setting")]
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float lookXLimit = 45f;
    [SerializeField] private Camera playerCamera;
    private float rotationX = 0;
    
    
    // Realistic Normal RigidBody
    [Header("--------------------------------------")]
    [Header("Speed For Realistic RigidBody")]
    [SerializeField] private float forwardRb = 1.8f;
    [SerializeField] private float forwardRunRb = 4.0f;
    [SerializeField] private float backwardRb = 1.5f;
    [SerializeField] private float backwardRunRb = 3.5f;
    [SerializeField] private float rightRb = 0.5f;
    [SerializeField] private float rightRunRb = 3.8f;
    [SerializeField] private float leftRb = 0.5f;
    [SerializeField] private float leftRunRb = 3.8f;
    
    
    // Character Setting Transform (RigidBody)
    [Header("--------------------------------------")]
    [Header("Speed For Transform (RigidBody)")]
    [SerializeField] private float movementSpeed = 1.7f;
    [SerializeField] private float runSpeed = 5.2f;

    
    // Character Setting Add-Force (RigidBody)
    [Header("--------------------------------------")]
    [Header("Speed For Add-Force (RigidBody)")]
    [SerializeField] private float movementSpeedForce = 1f;
    [SerializeField] private float runSpeedForce = 1.2f;

    
    // Realistic AddForce RigidBody
    [Header("--------------------------------------")]
    [Header("Speed For Realistic AddForce RigidBody")]
    [SerializeField] private float forwardAddForceRb = 1.5f;
    [SerializeField] private float forwardRunAddForceRb = 3.5f;
    [SerializeField] private float backwardAddForceRb = 1.0f;
    [SerializeField] private float backwardRunAddForceRb = 1.4f;
    [SerializeField] private float rightAddForceRb = 1.3f;
    [SerializeField] private float rightRunAddForceRb = 2.0f;
    [SerializeField] private float leftAddForceRb = 1.3f;
    [SerializeField] private float leftRunAddForceRb = 2.0f;
    
    
    // Realistic CharacterController
    [Header("--------------------------------------")]
    [Header("Speed For Realistic CharacterController")]
    [SerializeField] private float forwardCc = 2.5f;
    [SerializeField] private float forwardRunCc = 5.5f;
    [SerializeField] private float backwardCc = 1.7f;
    [SerializeField] private float backwardRunCc = 4.5f;
    [SerializeField] private float rightCc = 0.8f;
    [SerializeField] private float rightRunCc = 5.1f;
    [SerializeField] private float leftCc = 0.8f;
    [SerializeField] private float leftRunCc = 5.1f;
    
    // Normal Character Controller
    [Header("--------------------------------------")]
    [Header("Speed For Character Controller")]
    [SerializeField] private float movementSpeedCC = 3.0f;
    [SerializeField] private float runSpeedCC = 4.5f;
    
    
    // WASD for addForce
    Vector3 InputKey;
    
    // Fall Speed For Character Controller
    [Header("--------------------------------------")]
    [Header("Falling And Gravity For CharacterController")]
    [SerializeField] private float fallSpeedCc = 4.0f;
    [SerializeField] private float gravityForce = 2.0f;
    // Layer mask for ground detection
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayCastHeight = 0.1f;
    
    // Used For RealisticMovementRb & RealisticMovementCc
    [Header("--------------------------------------")]
    [Header("Acceleration and Deceleration for RealisticMovement")]
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    private float currentSpeed = 0f;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 desiredDirection = Vector3.zero;
    private bool changingDirection = false;


    private void Start()
    {
        // Find The Components
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        cc = GetComponent<CharacterController>();
        
        if (rb != null)
        {
            Debug.Log("RigidBody Founded");
        }
        if (playerCamera != null)
        {
            Debug.Log("PlayerCamera Founded");
        }
        if (cc != null)
        {
            Debug.Log("CharacterController Founded");
        }
        
    }

    void Update()
    {
        // Handling Errors
        FatalErrorHandler();
        
        // Camera System
        LookSystem();
        
        // Execute the MovementMode
        MovementTypeSwitch();
    }

    private void FatalErrorHandler()
    {
        if (cc && rb)
        {
            Debug.LogError("Your Character Cant Use CharacterController and RigidBody in the Same Time!");
        }
    }
    
    // Function For Handling Dropdown of MovementMode Select in Inspector Menu
    void MovementTypeSwitch()
    {
        switch (selectedMovementType)
        {
            case MovementType.RealisticMovementRb:
                RealisticMovementRb();
                break;
            case MovementType.RealisticMovementAddForceRb:
                RealisticMovementAddForceRb();
                break;
            case MovementType.RealisticMovementCc:
                RealisticMovementCc();
                break;
            case MovementType.MoveSystemCC:
                MoveSystemCC();
                break;
            case MovementType.MoveSystemTransformRb:
                MoveSystemTransformRb();
                break;
            case MovementType.MoveSystemForceRb:
                MoveSystemForceRb();
                break;
        }
    }
    
    // Camera System
    void LookSystem()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
    
    // Move System Using Force Rigidbody
    void MoveSystemForceRb()
    {
        // Keys
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        // Apply Speed
        if (forwardPressed)
            rb.AddForce(transform.forward * (runPressed ? runSpeedForce : movementSpeedForce));
        if (leftPressed)
            rb.AddForce(-transform.right * (runPressed ? runSpeedForce : movementSpeedForce));
        if (rightPressed)
            rb.AddForce(transform.right * (runPressed ? runSpeedForce : movementSpeedForce));
        if (backwardPressed)
            rb.AddForce(-transform.forward * (runPressed ? runSpeedForce : movementSpeedForce));
    }
    
    // Move System Using Transform (Rigidbody)
    void MoveSystemTransformRb()
    {
        Vector3 moveDirection = Vector3.zero;
        
        
        // Keys
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        
        
        // set moveDirection Based on Character Movement
        if (forwardPressed)
            moveDirection += transform.forward * (runPressed ? runSpeed : movementSpeed);
        if (leftPressed)
            moveDirection -= transform.right * (runPressed ? runSpeed : movementSpeed);
        if (rightPressed)
            moveDirection += transform.right * (runPressed ? runSpeed : movementSpeed);
        if (backwardPressed)
            moveDirection -= transform.forward * (runPressed ? runSpeed : movementSpeed);
        
        // Apply Speed
        rb.MovePosition(rb.position + moveDirection * Time.deltaTime);
    }
    
    // Move System (Character Controller)
    void MoveSystemCC()
    {
        // Keys
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        
        Vector3 moveDirection = Vector3.zero;
        
        // set moveDirection Based on Character Movement
        if (forwardPressed)
            moveDirection += cc.transform.forward;
        if (backwardPressed)
            moveDirection -= cc.transform.forward;
        if (leftPressed)
            moveDirection -= cc.transform.right;
        if (rightPressed)
            moveDirection += cc.transform.right;
        

        moveDirection.Normalize();
        float currentSpeed = runPressed ? runSpeedCC : movementSpeedCC;
        
        // Handle Gravity
        GravityHandlerCc();
        
        cc.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    // RealisticMovement with Transform(RigidBody)
    void RealisticMovementRb()
    {
        // Keys
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        // Calculate target direction based on input
        Vector3 direction = Vector3.zero;
        if (forwardPressed) direction += Vector3.forward;
        if (leftPressed) direction -= Vector3.right;
        if (rightPressed) direction += Vector3.right;
        if (backwardPressed) direction -= Vector3.forward;

        // Normalize direction to prevent faster diagonal movement
        direction.Normalize();

        // Detect change in direction
        if (direction != Vector3.zero && direction != moveDirection.normalized)
        {
            changingDirection = true;
            desiredDirection = direction;
        }

        // Set target speed based on Movement
        float targetSpeed = 0f;
        if (forwardPressed)
            targetSpeed = runPressed ? forwardRunRb : forwardRb;
        else if (leftPressed)
            targetSpeed = runPressed ? leftRunRb : leftRb;
        else if (rightPressed)
            targetSpeed = runPressed ? rightRunRb : rightRb;
        else if (backwardPressed)
            targetSpeed = runPressed ? backwardRunRb : backwardRb;

        // Adjust current speed and handle direction change
        if (changingDirection)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
            if (currentSpeed == 0)
            {
                changingDirection = false;
                moveDirection = desiredDirection * targetSpeed;
            }
        }
        else
        {
            if (direction != Vector3.zero)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
                moveDirection = direction * currentSpeed;
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
                moveDirection = direction * currentSpeed;
            }
        }

        // Apply movement to the character
        rb.MovePosition(rb.position + transform.TransformDirection(moveDirection) * Time.deltaTime);
    
    }
    
    // RealisticMovement (Character Controller)
    void RealisticMovementCc()
    {
        // Keys
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        // Calculate target direction based on input
        Vector3 direction = Vector3.zero;
        if (forwardPressed) direction += Vector3.forward;
        if (leftPressed) direction -= Vector3.right;
        if (rightPressed) direction += Vector3.right;
        if (backwardPressed) direction -= Vector3.forward;

        // Normalize direction to prevent faster diagonal movement
        direction.Normalize();

        // Detect change in direction
        if (direction != Vector3.zero && direction != moveDirection.normalized)
        {
            changingDirection = true;
            desiredDirection = direction;
        }

        // Set the targetSpeed Based On player Movement
        float targetSpeed = 0f;
        if (forwardPressed)
            targetSpeed = runPressed ? forwardRunCc : forwardCc;
        else if (leftPressed)
            targetSpeed = runPressed ? leftRunCc : leftCc;
        else if (rightPressed)
            targetSpeed = runPressed ? rightRunCc : rightCc;
        else if (backwardPressed)
            targetSpeed = runPressed ? backwardRunCc : backwardCc;

        // Adjust current speed and handle direction change
        if (changingDirection)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
            if (currentSpeed == 0)
            {
                changingDirection = false;
                moveDirection = desiredDirection * targetSpeed;
            }
        }
        else
        {
            if (direction != Vector3.zero)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
                moveDirection = direction * currentSpeed;
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
                moveDirection = direction * currentSpeed;
            }
        }

        GravityHandlerCc();
        
        // Apply movement to the character
        cc.Move(transform.TransformDirection(moveDirection) * Time.deltaTime);
    }

    // RealisticMovement with AddForce(RigidBody)
    void RealisticMovementAddForceRb()
    {
        // Keys
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        
        
        if (forwardPressed)
            rb.AddForce(transform.forward * (runPressed ? forwardRunAddForceRb : forwardAddForceRb));
        
        if (leftPressed)
            rb.AddForce(-transform.right * (runPressed ? leftRunAddForceRb : leftAddForceRb));

        if (rightPressed)
            rb.AddForce(transform.right * (runPressed ? rightRunAddForceRb : rightAddForceRb));

        if (backwardPressed)
            rb.AddForce(-transform.forward * (runPressed ? backwardRunAddForceRb : backwardAddForceRb));
        
    }
    
    // Seeing The Gizmoz Of The HeightDetector
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayStart = transform.position;
        Vector3 rayDirection = Vector3.down * rayCastHeight;
        Gizmos.DrawLine(rayStart, rayStart + rayDirection);
    }

    // Handle Gravity And Falling For Cc(Character Controller) & with RayCast
    void GravityHandlerCc()
    {
        // Drop a RayCast And If Hit Ground Detect(you can set that in inspector menu)
        RaycastHit hit;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, rayCastHeight, groundLayer);
        Vector3 moveDirection = Vector3.zero;
        
        // Do the Logic
        if (!isGrounded)
        {
            moveDirection -= cc.transform.up * gravityForce;
        }
        cc.Move(moveDirection * fallSpeedCc * Time.deltaTime);
    }
}
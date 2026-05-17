using UnityEngine;
using UnityEngine.InputSystem;

// V11 (Nihai) - Final optimized version for movement, Roblox Shift-Lock, Jumping and clean animation management.
public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator; 
    private Vector2 moveInput;

    [Header("Movement Physics Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;   // Initial upward impulse force applied during a jump event
    public float gravity = 9.81f;  // Continual downward acceleration multiplier representing earth gravity
    
    private float verticalVelocity; // Master internal variable tracking current vertical momentum state (falling or jumping)

    void Start()
    {
        // Gather essential component references required for runtime operations
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); 
        
        // Apply cursor soft-locking and hide it from view to finalize standard FPS/TPS player experience
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnMove(InputValue value)
    {
        // Extract movement input vector data utilizing the New Input System framework architecture
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // CRITICAL VITAL GUARD: Immediately cease all movement logic processing if the CharacterController is disabled (e.g., Player is Dead, or Game Won state)
        if (!controller.enabled) return;
        
        Transform cameraTransform = Camera.main.transform;
        
        // Determine horizontal movement vector in world space, calculated relative to the primary camera's current viewing orientation
        Vector3 moveDirection = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        moveDirection.y = 0f; // Zero out vertical vector component to strictly isolate horizontal mathematics
        
        // Define threshold magnitude required for the Animator state machine to transition from Idle to Walking states
        bool isMovingHorizontally = moveDirection.magnitude >= 0.1f;

        // 1. DYNAMIC GROUND STATUS & ANIMATION LOGIC (Refined in V11 for robust air state transitions)
        if (controller.isGrounded)
        {
            // CHARACTER IS ON TERRAIN: Normalize gravity force and handle walking animations
            
            // Snap small constant downward force to prevent subtle bouncing glitches when moving over uneven surfaces
            if (verticalVelocity < 0) verticalVelocity = -2f; 

            // Manage standard ground-level walking animation boolean states only while grounded
            if (animator != null) {
                animator.SetBool("isWalking", isMovingHorizontally);
            }

            // Detect primary Spacebar input for immediate jump sequence initiation
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                // Apply immediate vertical velocity impulse upwards
                verticalVelocity = jumpForce;
                
                // Supress walking boolean immediately before launching to allow cleaner animation transitions airborne
                if (animator != null) {
                    animator.SetBool("isWalking", false);
                    animator.SetTrigger("Jump"); // Fire specific jump animation sequence trigger event
                }
            }
        }
        else
        {
            // CHARACTER IS AIRBORNE: Supress walking states and process falling physics
            
            // Ensure walking boolean is strictly forced to false while floating to prevent awkward airborne walking looks
            if (animator != null) {
                animator.SetBool("isWalking", false);
            }

            // Continually apply gravitational downward acceleration pull to the vertical velocity tracking variable over time delta
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // 2. CONSOLIDATE AND APPLY TOTAL DISPLACEMENT VECTOR
        // Construct finalized velocity vector: Normalized horizontal direction * speed magnitude, integrated with the calculated dynamic vertical momentum component
        Vector3 finalMovementVelocity = moveDirection.normalized * speed; 
        finalMovementVelocity.y = verticalVelocity;                       

        // Apply displacement using advanced internal collision physics calculations
        controller.Move(finalMovementVelocity * Time.deltaTime);

        // 3. SECURE CHARACTER ROTATION ORIENTATION (The "Roblox Shift-Lock" Precision Camera Targeting Logic)
        // Query current realtime state of the Right Mouse Button hold status via modern input system architecture
        bool isAiming = Mouse.current != null && Mouse.current.rightButton.isPressed;

        if (isAiming)
        {
            // AIM ENGAGED: Force character to rigidly align orientation facing identical vector direction as primary camera forward view
            Vector3 currentCameraLookVector = cameraTransform.forward;
            currentCameraLookVector.y = 0f; // Keep the character's orientation strictly locked to the horizontal navigational plane

            if (currentCameraLookVector.sqrMagnitude > 0.01f) // Ensure valid vector magnitude before calculating rotation matrix
            {
                Quaternion intendedAimRotation = Quaternion.LookRotation(currentCameraLookVector);
                // Rotate character snapping quickly to target orientation for immediate responsiveness during high-stakes combat
                transform.rotation = Quaternion.Slerp(transform.rotation, intendedAimRotation, 15f * Time.deltaTime); 
            }
        }
        else if (isMovingHorizontally)
        {
            // FREE NAVIGATION: Smoothly interpolate the character's rotation orientation to align with actual intended world movement vector direction
            Quaternion intendedMoveRotation = Quaternion.LookRotation(moveDirection);
            // Standard turn smoothing rate for polished navigation feel when exploring terrain freely
            transform.rotation = Quaternion.Slerp(transform.rotation, intendedMoveRotation, 10f * Time.deltaTime);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class RagdollController : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;

    private bool isRagdoll = false;

    void Start()
    {
        
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        
        
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        
        SetRagdoll(false);
    }

    void Update()
    {
        
        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            isRagdoll = !isRagdoll;
            SetRagdoll(isRagdoll);
        }
    }

    void SetRagdoll(bool state)
    {
        
        if (animator != null) animator.enabled = !state;
        if (characterController != null) characterController.enabled = !state;

        
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !state; 
        }

        foreach (Collider col in colliders)
        {
            
            if (col.gameObject == this.gameObject) continue;
            col.enabled = state;
        }
    }
}
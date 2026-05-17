using UnityEngine;
using UnityEngine.InputSystem;

public class MinimapController : MonoBehaviour
{
    [Header("Minimap Settings")]
    public Transform playerTarget; 
    public Camera minimapCamera;   
    public float cameraHeight = 20f; 

    void Start()
    {
        
        if (minimapCamera != null)
        {
            minimapCamera.enabled = false; 
        }
    }

    void Update()
    {
        
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            if (minimapCamera != null)
            {
                minimapCamera.enabled = !minimapCamera.enabled;
            }
        }
    }

    void LateUpdate()
    {
       
        if (playerTarget != null && minimapCamera != null && minimapCamera.enabled)
        {
           
            Vector3 topDownPosition = playerTarget.position;
            topDownPosition.y += cameraHeight;
            
           
            minimapCamera.transform.position = topDownPosition;
        }
    }
}
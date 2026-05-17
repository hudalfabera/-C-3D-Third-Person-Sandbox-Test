using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement; 

public class PlayerInteract : MonoBehaviour
{
    public float rayRange = 10f;
    public LineRenderer laserBeamRenderer;
    public float beamDuration = 0.2f;
    
    [Header("Feedback Effects")]
    public GameObject collectParticle; 
    public AudioClip collectSound;     

    [Header("UI System")]
    public TMP_Text scoreText; 
    public GameObject gameOverPanel; 
    public GameObject victoryPanel; // Reference to the hidden Victory screen
    
    private int currentScore = 0; 
    private bool isGameOver = false; // State flag to stop gameplay loops on end

    void Start()
    {
        // Setup initial UI text layouts at runtime
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
        if (gameOverPanel != null) gameOverPanel.SetActive(false); 
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        // Standard guard clause to freeze controls if the match has concluded
        if (isGameOver) return; 

        // Handle interaction fire state using the New Input System framework
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Ray ray = new Ray(laserBeamRenderer.transform.position, transform.forward);
            RaycastHit hit;

            StartCoroutine(ShootLaser(ray));

            if (Physics.Raycast(ray, out hit, rayRange))
            {
                // WHITE CUBE INTERACTION: Core scoring loop
                if (hit.collider.CompareTag("Item"))
                {
                    ItemObject currentItem = hit.collider.GetComponent<ItemObject>();
                    if (currentItem != null)
                    {
                        if (collectSound != null) AudioSource.PlayClipAtPoint(collectSound, hit.transform.position);
                        if (collectParticle != null) Instantiate(collectParticle, hit.transform.position, Quaternion.identity);

                        currentScore += currentItem.data.pointValue;
                        if (scoreText != null) scoreText.text = "Score: " + currentScore;

                        Destroy(hit.collider.gameObject); 
                        
                        // Check if this was the final remaining collectible item in the scene
                        // Since Destroy executes at the end of frame, we check if count is 1 or less
                        if (GameObject.FindGameObjectsWithTag("Item").Length <= 1)
                        {
                            TriggerWin();
                        }
                    }
                }
                // RED CUBE INTERACTION: Hazardous fail state handling
                else if (hit.collider.CompareTag("Obstacle"))
                {
                    TriggerDeath();
                }
            }
        }
    }

    void TriggerWin()
    {
        isGameOver = true;
        Debug.Log("Victory achieved! All targets cleared.");

        // 1. CRITICAL FIX: Stop character movement and force Idle animation
        PlayerMove moveScript = GetComponent<PlayerMove>();
        if (moveScript != null) moveScript.enabled = false; // Disable the movement script completely

        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.SetBool("isWalking", false); // Force the animation back to idle standing

        // 2. Display the winner layout interface container
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // 3. Shut down the rendering operations of the top-down minimap camera overlay
        MinimapController minimap = GetComponent<MinimapController>();
        if (minimap != null)
        {
            if (minimap.minimapCamera != null) minimap.minimapCamera.enabled = false;
            minimap.enabled = false;
        }

        // 4. Unlock OS hardware cursor functionality for user UI selection
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void TriggerDeath()
    {
        if (isGameOver) return; 
        isGameOver = true;

        Debug.Log("Player death initiated at position: " + transform.position);

        // 1. Kill animation timelines and movement controllers instantly
        Animator animator = GetComponent<Animator>();
        if (animator != null) animator.enabled = false;

        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = false;
        
        // 2. Grant full physical mass reactions to the inner skeleton rig bones
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero; 
        }

        // 3. Re-engage environmental colliders to sustain structural terrain collision physics
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            if (col.gameObject == this.gameObject) continue;
            col.enabled = true;
        }

        // 4. Shut down minimap tracking views to avoid UI stacking bleed artifacts
        MinimapController minimap = GetComponent<MinimapController>();
        if (minimap != null)
        {
            if (minimap.minimapCamera != null) minimap.minimapCamera.enabled = false;
            minimap.enabled = false;
        }

        // 5. Open the game over modal UI layer on screen
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 6. Free the hardware mouse cursor bounds safely
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator ShootLaser(Ray ray)
    {
        laserBeamRenderer.SetPosition(0, ray.origin);
        RaycastHit hitCheck;
        if (Physics.Raycast(ray, out hitCheck, rayRange)) laserBeamRenderer.SetPosition(1, hitCheck.point);
        else laserBeamRenderer.SetPosition(1, ray.origin + (ray.direction * rayRange));

        laserBeamRenderer.enabled = true;
        yield return new WaitForSeconds(beamDuration);
        laserBeamRenderer.enabled = false;
    }
}
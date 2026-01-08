using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuracion de Velocidad")]
    public float walkSpeed = 5f;
    public float runSpeed = 8.5f;
    
    [Range(0f, 2f)]
    public float speedModifier = 1f;

    private Rigidbody2D rb;
    private PlayerStats stats;
    private Vector2 moveInput;
    private float currentSpeed;
    private bool isRunning = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (isRunning && stats != null && stats.stamina > 0)
        {
            currentSpeed = runSpeed;
            stats.ConsumirStamina(10f * Time.deltaTime);
        }
        else
        {
            currentSpeed = walkSpeed;
            if (stats != null && moveInput.magnitude > 0.1f)
            {
                stats.RecuperarStamina(5f * Time.deltaTime);
            }
            else if (stats != null)
            {
                stats.RecuperarStamina(15f * Time.deltaTime);
            }
        }

        FlipSprite();
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector2 movement = moveInput.normalized * (currentSpeed * speedModifier) * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    private void FlipSprite()
    {
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }

    public void SetSpeedModifier(float modifier)
    {
        speedModifier = Mathf.Clamp(modifier, 0f, 2f);
    }

    public void DetenerMovimiento()
    {
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }
}
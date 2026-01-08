using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuracion de Velocidad")]
    public float walkSpeed = 5f;
    public float runSpeed = 8.5f;

    private Rigidbody2D rb;
    private PlayerStats stats;
    private Animator anim;
    private Vector2 moveInput;
    private float currentSpeed;
    private bool isRunning = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // LEER INPUT
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // DETECTAR SI CORRE (Shift izquierdo)
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // LOGICA DE VELOCIDAD Y STAMINA
        if (isRunning && stats != null && stats.stamina > 0 && moveInput.magnitude > 0.1f)
        {
            currentSpeed = runSpeed;
            stats.ConsumirStamina(15f * Time.deltaTime);
        }
        else
        {
            currentSpeed = walkSpeed;
            if (stats != null)
            {
                float recoveryRate = (moveInput.magnitude > 0.1f) ? 5f : 15f;
                stats.RecuperarStamina(recoveryRate * Time.deltaTime);
            }
        }

        // --- ANIMACIONES ORIGINALES ---
        if (anim != null)
        {
            // isWalking: true si se esta moviendo
            bool moving = moveInput.magnitude > 0.1f;
            anim.SetBool("isWalking", moving);
            
            // Run: true si esta corriendo y moviendose
            anim.SetBool("Run", isRunning && moving && (stats == null || stats.stamina > 0));
            
            // isDrunk: lo sacamos de stats si existe la logica, por ahora lo mantenemos si estaba en el controller
            // anim.SetBool("isDrunk", stats != null && stats.temperature > 80f); // Ejemplo
        }

        FlipSprite();
    }

    void FixedUpdate()
    {
        if (moveInput.magnitude > 0.1f)
        {
            rb.linearVelocity = moveInput * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
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
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidad")]
    public float walkSpeed = 5f;
    public float runSpeed = 8.5f;

    [Header("Ajustes de Stamina")]
    public float gastoStaminaCarrera = 20f;
    public float recuperacionQuieto = 12f;
    public float recuperacionCaminando = 5f;

    private Rigidbody2D rb;
    private PlayerStats stats;
    private PlayerActions actions;
    private Animator anim;
    private Vector2 moveInput;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        actions = GetComponent<PlayerActions>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Movimiento
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // Gestion de Velocidad y Stamina
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && stats.stamina > 0 && moveInput.magnitude > 0.1f;
        
        // --- PENALIZACION DE CANSANCIO ---
        float speedMultiplier = 1f;
        if (stats.stamina <= 0) {
            speedMultiplier = 0.5f; // Camina a mitad de velocidad si esta cansado
        }

        currentSpeed = isRunning ? runSpeed : (walkSpeed * speedMultiplier);
        
        if (isRunning) stats.ConsumirStamina(gastoStaminaCarrera * Time.deltaTime);
        else {
            // Recupera según la nueva configuración regulable
            float recoveryRate = (moveInput.magnitude > 0.1f) ? recuperacionCaminando : recuperacionQuieto;
            stats.RecuperarStamina(recoveryRate * Time.deltaTime);
        }

        // Animaciones
        if (anim != null) {
            anim.SetBool("isWalking", moveInput.magnitude > 0.1f);
            anim.SetBool("Run", isRunning);
        }

        FlipSprite();
    }
    
    float moveX, moveY;

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * currentSpeed;
    }

    void FlipSprite()
    {
        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}
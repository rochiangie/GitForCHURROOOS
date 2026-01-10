using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidad")]
    public float walkSpeed = 5f;
    public float runSpeed = 8.5f;

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

        // Ataque (Tecla F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (actions != null) actions.RealizarAtaque();
        }

        // Gestion de Velocidad y Stamina
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && stats.stamina > 0 && moveInput.magnitude > 0.1f;
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        if (isRunning) stats.ConsumirStamina(15f * Time.deltaTime);
        else stats.RecuperarStamina(10f * Time.deltaTime);

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
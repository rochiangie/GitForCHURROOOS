using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float speed = 5f;
    public float drunkInertia = 0.2f;
    public bool isDrunk = false;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.juegoPausado) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Flip del sprite
        if (moveInput.x != 0)
            spriteRenderer.flipX = moveInput.x < 0;

        bool walking = moveInput.magnitude > 0;
        animator.SetBool("isWalking", walking);
        animator.SetBool("isDrunk", isDrunk);
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput.normalized * speed;

        if (isDrunk)
        {
            rb.linearVelocity = Vector2.SmoothDamp(
                rb.linearVelocity,
                targetVelocity,
                ref currentVelocity,
                drunkInertia
            );
        }
        else
        {
            rb.linearVelocity = targetVelocity;
        }
    }

    // Llamada desde SunSystem
    public void SetDrunk(bool state)
    {
        isDrunk = state;
        spriteRenderer.color = state ? new Color(1f, 0.7f, 0.7f) : Color.white;
    }
}

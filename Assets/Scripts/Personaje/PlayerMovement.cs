using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    private float currentSpeed;

    [Header("Referencias")]
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;
    private PlayerStats stats;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // 1. Capturar Movimiento
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 2. Lógica de Correr (Shift)
        // Solo puede correr si se está moviendo y tiene suficiente hidratación
        if (Input.GetKey(KeyCode.LeftShift) && moveInput.sqrMagnitude > 0 && stats.hydration > 10f)
        {
            currentSpeed = runSpeed;
            anim.SetBool("Run", true); // Activamos tu nueva animación

            // Correr cansa más rápido (opcional, pero realista)
            stats.hydration -= Time.deltaTime * 2f;
        }
        else
        {
            currentSpeed = walkSpeed;
            anim.SetBool("Run", false);
        }

        // 3. Animación de Velocidad Base
        anim.SetFloat("Speed", moveInput.sqrMagnitude);

        // 4. Volteo (Flip) por Escala
        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        // Aplicar modificador de cansancio extremo
        float exhaustionMod = (stats.hydration <= 20f) ? 0.5f : 1f;

        Vector2 force = moveInput.normalized * (currentSpeed * exhaustionMod);
        rb.MovePosition(rb.position + force * Time.fixedDeltaTime);
    }
}
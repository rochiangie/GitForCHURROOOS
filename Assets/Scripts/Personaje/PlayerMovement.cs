using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    public float walkSpeed = 5f;
    public float runSpeed = 8.5f;
    private float currentSpeed;

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
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Lógica de Correr: Necesita Shift, estarse moviendo y tener Stamina > 5
        if (Input.GetKey(KeyCode.LeftShift) && moveInput.sqrMagnitude > 0 && stats.stamina > 5f)
        {
            currentSpeed = runSpeed;
            if (anim != null) anim.SetBool("Run", true);
            stats.stamina -= 15f * Time.deltaTime; // Consume energía
        }
        else
        {
            currentSpeed = walkSpeed;
            if (anim != null) anim.SetBool("Run", false);

            // Recupera energía si no corre
            if (stats.stamina < stats.staminaMax)
                stats.stamina += 10f * Time.deltaTime;
        }

        if (anim != null) anim.SetFloat("Speed", moveInput.sqrMagnitude);

        // Volteo (Flip)
        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        // Aplicar cansancio extremo si no hay hidratación
        float mod = (stats.hydration <= 10f) ? 0.5f : 1f;
        rb.MovePosition(rb.position + moveInput.normalized * (currentSpeed * mod) * Time.fixedDeltaTime);
    }
}
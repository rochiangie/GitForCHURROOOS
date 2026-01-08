using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
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

        // Parametro "Speed" con S mayúscula
        anim.SetFloat("Speed", moveInput.sqrMagnitude);

        // Volteo por escala
        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        // Velocidad reducida si hay poca hidratación
        float speedMod = (stats.hydration <= 20f) ? 0.4f : 1f;
        rb.MovePosition(rb.position + moveInput.normalized * (moveSpeed * speedMod) * Time.fixedDeltaTime);
    }
}
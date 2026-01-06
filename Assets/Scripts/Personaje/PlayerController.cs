using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes")]
    public float speed = 5f;
    public float drunkInertia = 0.2f;
    public bool isDrunk = false;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private SpriteRenderer sr;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.juegoPausado) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.x != 0) sr.flipX = moveInput.x < 0;
        anim.SetBool("isWalking", moveInput.magnitude > 0);
    }

    void FixedUpdate()
    {
        Vector2 targetVel = moveInput.normalized * speed;
        if (isDrunk)
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVel, ref currentVelocity, drunkInertia);
        else
            rb.linearVelocity = targetVel;
    }

    public void SetDrunk(bool state)
    {
        isDrunk = state;
        sr.color = state ? new Color(1, 0.7f, 0.7f) : Color.white;
    }
}
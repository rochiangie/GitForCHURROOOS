using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Atributos del Jugador")]
    public float temperature = 0f;
    public float hydration = 100f;
    public float money = 0f;
    public float maxStat = 100f;

    [Header("Movimiento y Animación")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;
    private bool isDrunkEfecto = false;

    [Header("Interfaz (UI)")]
    public Slider tempSlider;
    public Slider drinkSlider;
    public Text moneyText;

    [Header("Configuración de Desgaste")]
    public float heatGainRate = 2.5f;
    public float thirstRate = 1.8f;

    [Header("Detección de Entorno")]
    public bool isInsideWater = false;
    public bool isShadowed = false;
    public float interactionRadius = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Configuración básica para Top-Down
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. Capturar Entrada
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 2. Control de Animaciones
        float speedMagnitude = moveInput.sqrMagnitude;
        anim.SetFloat("Speed", speedMagnitude);

        // Estado "Drunk" (por sed o por birra)
        bool shouldBeDrunk = (hydration <= 20f || isDrunkEfecto);
        anim.SetBool("setdrunk", shouldBeDrunk);

        // 3. Sistema de Volteo (Flip) por Escala
        // Esto voltea el objeto completo, incluyendo todos sus Colliders
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // 4. Interacción (Tecla F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interactuar();
        }

        HandleSurvivalLogic();
        UpdateUI();
    }

    void FixedUpdate()
    {
        // Aplicar movimiento físico
        // Reducimos velocidad si está deshidratado o borracho
        float finalSpeed = (hydration <= 20f || isDrunkEfecto) ? moveSpeed * 0.4f : moveSpeed;
        rb.MovePosition(rb.position + moveInput.normalized * finalSpeed * Time.fixedDeltaTime);
    }

    void HandleSurvivalLogic()
    {
        // Lógica de Temperatura e Hidratación
        if (isInsideWater)
        {
            temperature -= Time.deltaTime * 15f; // Enfría rápido
        }
        else if (isShadowed)
        {
            temperature -= Time.deltaTime * 5f; // Enfría lento
        }
        else
        {
            // Solo sube si hay movimiento o estamos bajo el sol
            temperature += Time.deltaTime * heatGainRate;
            if (moveInput.sqrMagnitude > 0)
                hydration -= Time.deltaTime * thirstRate;
        }

        // Limitar valores entre 0 y 100
        temperature = Mathf.Clamp(temperature, 0, maxStat);
        hydration = Mathf.Clamp(hydration, 0, maxStat);

        // Condición de Game Over (Agotamiento)
        if (temperature >= maxStat)
        {
            Debug.Log("EL VENDEDOR SE DESMAYÓ POR CALOR");
            // Aquí llamarías a un método de muerte o reinicio
        }
    }

    void Interactuar()
    {
        // Buscamos objetos cercanos con Colliders
        Collider2D[] cercanos = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        foreach (var col in cercanos)
        {
            // Interactuar con otros Vendedores
            if (col.CompareTag("Vendedor"))
            {
                NPCVendedor npc = col.GetComponent<NPCVendedor>();
                if (npc != null)
                {
                    npc.Interactuar(10f); // Sube afinidad
                    Debug.Log("Saludaste a " + npc.nombreVendedor);
                }
            }

            // Interactuar con Clientes (Venta de churros)
            if (col.CompareTag("Cliente"))
            {
                VenderChurro();
            }
        }
    }

    void VenderChurro()
    {
        money += 15f;
        hydration -= 3f; // Vender da sed
        Debug.Log("¡Churro vendido! Dinero actual: " + money);
    }

    public void SetDrunk(bool state)
    {
        isDrunkEfecto = state;
    }

    void UpdateUI()
    {
        if (tempSlider != null) tempSlider.value = temperature / maxStat;
        if (drinkSlider != null) drinkSlider.value = hydration / maxStat;
        if (moneyText != null) moneyText.text = "$" + money.ToString("F0");
    }

    // --- DETECCIÓN DE TRIGGERS (Agua y Sombra) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water")) isInsideWater = true;
        if (other.CompareTag("Shadow")) isShadowed = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water")) isInsideWater = false;
        if (other.CompareTag("Shadow")) isShadowed = false;
    }

    // Dibujar el radio de interacción en el editor para referencia
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
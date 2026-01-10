using UnityEngine;

public class BossRival : MonoBehaviour
{
    public float velocidadChase = 3.5f;
    public float distanciaRobo = 1.2f;
    public float intervaloRobo = 3f;
    public float roboDineroMin = 50f;
    public float roboDineroMax = 150f;

    private Transform player;
    private PlayerStats stats;
    private float timerRobo;
    private Rigidbody2D rb;
    private float knockbackTimer = 0f;

    void Start()
    {
        // ... (resto del start igual)
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p) {
            player = p.transform;
            stats = p.GetComponent<PlayerStats>();
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null || GameManager.Instance.juegoTerminado) return;

        // Si estamos bajo efecto de empuje, no sobreescribimos la velocidad
        if (knockbackTimer > 0) {
            knockbackTimer -= Time.deltaTime;
            return;
        }

        // Persecucion simple
        float distancia = Vector2.Distance(transform.position, player.position);
        
        if (distancia > distanciaRobo)
        {
            Vector2 direccion = (player.position - transform.position).normalized;
            rb.linearVelocity = direccion * velocidadChase;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ManejarRobo();
        }
    }

    // Metodo para que el PlayerActions nos avise que nos empujo
    public void RecibirEmpuje(float duracion = 0.5f) {
        knockbackTimer = duracion;
    }

    void ManejarRobo()
    {
        timerRobo += Time.deltaTime;
        if (timerRobo >= intervaloRobo)
        {
            timerRobo = 0;
            float robo = Random.Range(roboDineroMin, roboDineroMax);
            
            if (stats.money > 0)
            {
                stats.money = Mathf.Max(0, stats.money - robo);
                Debug.Log("¡EL RIVAL TE ROBO $" + robo + "!");
                // Aqui podrias activar un sonido de risa malvada
            }
        }
    }
}

using UnityEngine;

public class BossRival : MonoBehaviour
{
    [Header("Configuracion Base")]
    public float velocidadChase = 3.5f;
    public float velocidadHuida = 5f;
    public float distanciaRobo = 1.2f;
    public float intervaloRobo = 3f;
    
    [Header("Stats de Pelea")]
    public float vidaMax = 100f;
    public float vidaActual;
    public float danioAlJugador = 10f;

    private Transform player;
    private PlayerStats stats;
    private float timerRobo;
    private Rigidbody2D rb;
    private float knockbackTimer = 0f;
    private bool derrotado = false;
    private Transform metaHuida;

    void Start()
    {
        vidaActual = vidaMax;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p) {
            player = p.transform;
            stats = p.GetComponent<PlayerStats>();
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.Instance.juegoTerminado) return;

        if (derrotado) {
            ManejarHuida();
            return;
        }

        if (player == null) return;

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

    public void TomarDanio(float danio) {
        if (derrotado) return;

        vidaActual -= danio;
        Debug.Log($"[BOSS] ¡Auch! Vida restante: {vidaActual}");

        if (vidaActual <= 0) {
            Derrota();
        }
    }

    public void RecibirEmpuje(float duracion = 0.5f, float danio = 5f) {
        knockbackTimer = duracion;
        TomarDanio(danio);
    }

    void ManejarRobo()
    {
        timerRobo += Time.deltaTime;
        if (timerRobo >= intervaloRobo)
        {
            timerRobo = 0;
            if (stats != null) {
                // Ahora tambien hace daño de vida si queremos
                stats.RecibirDanio(danioAlJugador);
                Debug.Log("¡El Boss te golpeó!");
            }
        }
    }

    void Derrota() {
        derrotado = true;
        Debug.Log("<color=red><b>[BOSS] ¡Derrrotado! Escapando de la playa...</b></color>");
        
        // --- SOLUCION PARA NO TRABARSE ---
        // Convertimos el collider en Trigger para que atraviese paredes y NPCs
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // Cambiamos color o transparencia para feedbak visual (opcional)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1, 1, 1, 0.5f); 

        // Buscar el limite mas cercano
        GameObject[] limites = GameObject.FindGameObjectsWithTag("limites");
        float minDist = Mathf.Infinity;
        foreach (GameObject l in limites) {
            float d = Vector2.Distance(transform.position, l.transform.position);
            if (d < minDist) {
                minDist = d;
                metaHuida = l.transform;
            }
        }

        if (metaHuida == null) Destroy(gameObject, 2f);
    }

    void ManejarHuida() {
        if (metaHuida != null) {
            Vector2 direccion = (metaHuida.position - transform.position).normalized;
            rb.linearVelocity = direccion * velocidadHuida;

            if (Vector2.Distance(transform.position, metaHuida.position) < 1f) {
                GameManager.Instance.PresionarBoton_SiguienteNivel();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (derrotado && other.CompareTag("limites")) {
            GameManager.Instance.PresionarBoton_SiguienteNivel();
            Destroy(gameObject);
        }
    }
}

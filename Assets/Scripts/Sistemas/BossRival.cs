using UnityEngine;
using UnityEngine.AI; // Necesario para NavMesh

public class BossRival : MonoBehaviour
{
    [Header("Configuracion NavMesh")]
    private NavMeshAgent agent;
    public float velocidadChase = 3.5f;
    public float velocidadHuida = 5f;

    [Header("Configuracion Base")]
    public float distanciaRobo = 1.2f;
    public float intervaloRobo = 3f;
    
    [Header("Stats de Pelea")]
    public float vidaMax = 100f;
    public float vidaActual;
    public float danioAlJugador = 10f;

    [Header("Defensa")]
    [Range(0, 100)]
    public float probabilidadBloqueo = 20f; 
    public float cooldownBloqueo = 1f;
    private float lastBlockTime;

    private Transform player;
    private PlayerStats stats;
    private float timerRobo;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float knockbackTimer = 0f;
    private bool derrotado = false;
    private Transform metaHuida;
    private Color baseColor;

    void Start()
    {
        vidaActual = vidaMax;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;

        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        // Configuración para que el NavMeshAgent funcione bien en 2D
        if (agent != null) {
            agent.updateRotation = false; // El sprite no debe girar como un avion
            agent.updateUpAxis = false;   // No queremos que se incline en 3D
            agent.speed = velocidadChase;
        }

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p) {
            player = p.transform;
            stats = p.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.juegoTerminado) return;

        if (derrotado) {
            ManejarHuida();
            return;
        }

        if (player == null) return;

        // Si estamos bajo efecto de empuje o bloqueo, pausamos el agente
        if (knockbackTimer > 0) {
            knockbackTimer -= Time.deltaTime;
            if(agent != null) agent.isStopped = true;
            return;
        }

        if(agent != null) agent.isStopped = false;

        float distancia = Vector2.Distance(transform.position, player.position);
        
        if (distancia > distanciaRobo)
        {
            // --- MOVIMIENTO CON NAVMESH ---
            if (agent != null && agent.enabled) {
                agent.SetDestination(player.position);
                agent.speed = velocidadChase;
            }
        }
        else
        {
            if(agent != null) agent.isStopped = true;
            rb.linearVelocity = Vector2.zero;
            ManejarRobo();
        }
    }

    public void RecibirImpactoProyectil(float danio, Vector2 origenImpacto) {
        if (derrotado) return;

        float azar = Random.Range(0, 100);
        if (azar < probabilidadBloqueo && Time.time > lastBlockTime + cooldownBloqueo) {
            BloquearAtaque();
            return;
        }

        Vector2 dirEmpuje = ((Vector2)transform.position - origenImpacto).normalized;
        RecibirEmpuje(0.3f, danio);
        rb.AddForce(dirEmpuje * 5f, ForceMode2D.Impulse);
    }

    void BloquearAtaque() {
        lastBlockTime = Time.time;
        Debug.Log("<color=blue>[BOSS] ¡BLOQUEADO!</color>");
        if (sr != null) {
            StopAllCoroutines();
            StartCoroutine(EfectoColor(Color.blue, 0.2f));
        }
        knockbackTimer = 0.2f; 
        if(agent != null) agent.isStopped = true;
    }

    System.Collections.IEnumerator EfectoColor(Color c, float dur) {
        sr.color = c;
        yield return new WaitForSeconds(dur);
        if (!derrotado) sr.color = baseColor;
        else sr.color = Color.gray;
    }

    public void TomarDanio(float danio) {
        if (derrotado) return;

        vidaActual -= danio;

        if (sr != null) {
            StopAllCoroutines();
            Color colorHit = (danio < 10f) ? new Color(1f, 0.5f, 0.5f) : new Color(0.6f, 0f, 0f);
            StartCoroutine(EfectoColor(colorHit, 0.15f));
        }

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
                stats.RecibirDanio(danioAlJugador);
            }
        }
    }

    void Derrota() {
        derrotado = true;
        if(agent != null) agent.enabled = false; // Apagamos el agente para que no luche contra la fisica
        
        StopAllCoroutines();
        Debug.Log("<color=red><b>[BOSS] ¡Derrotado! Escapando...</b></color>");
        
        Collider2D[] todosLosColliders = GetComponents<Collider2D>();
        foreach (Collider2D col in todosLosColliders) col.isTrigger = true;

        if (sr != null) sr.color = Color.gray; 

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
            // En huida usamos velocidad directa porque ya es trigger y no necesita esquivar
            Vector2 direccion = (metaHuida.position - transform.position).normalized;
            rb.linearVelocity = direccion * velocidadHuida;

            if (Vector2.Distance(transform.position, metaHuida.position) < 1f) {
                GameManager.Instance.FinalizarNivel();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (derrotado && other.CompareTag("limites")) {
            GameManager.Instance.FinalizarNivel();
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Configuracion de Ataque/Empujon")]
    public float fuerzaEmpuje = 15f;
    public float radioAtaque = 2.0f;
    public float duracionAturdimiento = 0.4f;
    public float danioPatada = 10f;
    public LayerMask capaNPC;
    public Transform puntoAtaque; 

    [Header("Ataque a Distancia")]
    public GameObject prefabChurro;
    public float cooldownLanzamiento = 0.5f;
    private float lastShootTime;

    private PlayerStats stats;
    private Animator anim;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        if (puntoAtaque == null) puntoAtaque = transform;
    }

    void Update() {
        // Lanzar Churro con Espacio
        if (Input.GetKeyDown(KeyCode.Space)) {
            LanzarChurro();
        }
    }

    public void LanzarChurro() {
        if (stats == null || stats.churrosCantidad <= 0 || Time.time < lastShootTime + cooldownLanzamiento) return;

        stats.ConsumirChurro(); // Usamos el metodo oficial para descontar
        lastShootTime = Time.time;

        if (anim != null) anim.SetTrigger("attack");

        GameObject go = Instantiate(prefabChurro, puntoAtaque.position, Quaternion.identity);
        ChurroProyectil p = go.GetComponent<ChurroProyectil>();
        if (p != null) {
            Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            p.Lanzar(dir);
        }
    }

    public void RealizarAtaque()
    {
        if (anim != null) anim.SetTrigger("attack");

        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.golpePelea);
        }

        Collider2D[] golpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque, capaNPC);

        foreach (Collider2D npcCol in golpeados)
        {
            Rigidbody2D rbNPC = npcCol.GetComponent<Rigidbody2D>();
            if (rbNPC != null)
            {
                Vector2 direccion = (npcCol.transform.position - transform.position).normalized;
                if (direccion == Vector2.zero) direccion = transform.right;

                BossRival boss = npcCol.GetComponent<BossRival>();
                if (boss != null) boss.RecibirEmpuje(duracionAturdimiento, danioPatada);

                rbNPC.linearVelocity = Vector2.zero;
                rbNPC.AddForce(direccion * fuerzaEmpuje, ForceMode2D.Impulse);
            }
        }

        var cam = FindFirstObjectByType<BeatEmUpCamera>();
    }

    public void TomarAgua(float cantidad = 1f)
    {
        if (stats == null) return;
        stats.RecuperarHidratacion(30f * cantidad);
        stats.ReducirTemperatura(20f * cantidad);
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.destapeBirra);
    }

    public void ComerChurro()
    {
        if (stats != null && stats.churrosCantidad > 0) {
            stats.churrosCantidad--;
            stats.RecuperarStamina(25f);
            if (anim != null) anim.SetTrigger("attack");
        }
    }

    public bool VenderChurro(float precio)
    {
        if (stats == null || stats.churrosCantidad <= 0) return false;
        stats.churrosCantidad--;
        stats.AgregarDinero(precio);
        if (anim != null) anim.SetTrigger("attack");
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.ventaExitosa);
        return true;
    }
}
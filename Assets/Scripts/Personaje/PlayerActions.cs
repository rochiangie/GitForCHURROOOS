using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Configuracion de Ataque/Empujon")]
    public float fuerzaEmpuje = 15f;
    public float radioAtaque = 2.0f;
    public float duracionAturdimiento = 0.4f;
    public LayerMask capaNPC;
    public Transform puntoAtaque; 

    private PlayerStats stats;
    private Animator anim;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        if (puntoAtaque == null) puntoAtaque = transform;
    }

    public void RealizarAtaque()
    {
        if (anim != null) anim.SetTrigger("attack");

        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.golpePelea);
        }

        // Deteccion
        Collider2D[] golpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque, capaNPC);

        foreach (Collider2D npcCol in golpeados)
        {
            Rigidbody2D rbNPC = npcCol.GetComponent<Rigidbody2D>();
            if (rbNPC != null)
            {
                // 1. Calculamos direccion
                Vector2 direccion = (npcCol.transform.position - transform.position).normalized;
                if (direccion == Vector2.zero) direccion = transform.right; // Fallback

                // 2. Si es un Boss, lo aturdimos para que no ignore la fisica
                BossRival boss = npcCol.GetComponent<BossRival>();
                if (boss != null) boss.RecibirEmpuje(duracionAturdimiento);

                // 3. Aplicamos la fuerza (limpiamos velocidad previa para que se sienta el golpe)
                rbNPC.linearVelocity = Vector2.zero;
                rbNPC.AddForce(direccion * fuerzaEmpuje, ForceMode2D.Impulse);
                
                Debug.Log($"<color=red>¡PUM!</color> Empujaste a {npcCol.name} con fuerza {fuerzaEmpuje}");
            }
        }

        // Efecto visual extra: Shake de camara si hay BeatEmUpCamera
        var cam = FindFirstObjectByType<BeatEmUpCamera>();
        // if(cam) cam.Shake(0.1f, 0.1f); // Opcional si implementas Shake
    }

    // --- Metodos de Venta/Consumo (Ya existentes) ---
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
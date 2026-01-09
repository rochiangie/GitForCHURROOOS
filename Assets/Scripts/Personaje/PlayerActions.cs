using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Configuracion de Ataque/Empujon")]
    public float fuerzaEmpuje = 10f;
    public float radioAtaque = 1.5f;
    public LayerMask capaNPC;
    public Transform puntoAtaque; // Un objeto vacio frente al player

    private PlayerStats stats;
    private Animator anim;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        
        // Si no asignaste un punto de ataque, usamos la posicion del jugador
        if (puntoAtaque == null) puntoAtaque = transform;
    }

    public void RealizarAtaque()
    {
        // 1. Animacion
        if (anim != null) anim.SetTrigger("attack");

        // 2. Sonido
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.golpePelea);
        }

        // 3. Deteccion de NPCs para empujar
        Collider2D[] golpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque, capaNPC);

        foreach (Collider2D npcCol in golpeados)
        {
            Rigidbody2D rbNPC = npcCol.GetComponent<Rigidbody2D>();
            if (rbNPC != null)
            {
                // Calculamos direccion del empuje (desde el jugador hacia el NPC)
                Vector2 direccion = (npcCol.transform.position - transform.position).normalized;
                rbNPC.AddForce(direccion * fuerzaEmpuje, ForceMode2D.Impulse);
                
                Debug.Log("¡PUM! Empujaste a un NPC.");
            }
        }
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
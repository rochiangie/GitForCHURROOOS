using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    public float radius = 2.5f;        // Radio del círculo de detección
    public LayerMask interactionLayer; // Capa donde deben estar los NPCs (Vendedores y Clientes)
    public KeyCode interactionKey = KeyCode.F;

    [Header("Referencias Modulares")]
    private PlayerStats stats;
    private PlayerHealthSystem health;

    void Start()
    {
        // Buscamos las referencias en el mismo objeto
        stats = GetComponent<PlayerStats>();
        health = GetComponent<PlayerHealthSystem>();

        if (stats == null) Debug.LogError("PlayerInteraction: No se encontró PlayerStats en el jugador.");
    }

    void Update()
    {
        // Detectar si se presiona la tecla de interacción
        if (Input.GetKeyDown(interactionKey))
        {
            IntentarInteractuar();
        }
    }

    private void IntentarInteractuar()
    {
        // Detectar todos los colliders dentro del radio que pertenezcan a la capa seleccionada
        Collider2D[] cercanos = Physics2D.OverlapCircleAll(transform.position, radius, interactionLayer);

        Debug.Log($"<color=white>[INTERACCIÓN]</color> Buscando... Objetos detectados en capa: {cercanos.Length}");

        foreach (var col in cercanos)
        {
            // --- CASO 1: VENDEDOR ---
            if (col.CompareTag("Vendedor"))
            {
                NPCVendedor npc = col.GetComponent<NPCVendedor>();
                if (npc != null)
                {
                    Debug.Log($"<color=green>[NPC]</color> Hablando con {npc.nombreVendedor}");
                    npc.AbrirConversacion();

                    // Si el vendedor es Doña Rosa y tienes afinidad, que te cure un poco
                    if (npc.nombreVendedor == "Doña Rosa" && npc.afinidad > 50)
                    {
                        stats.temperature -= 20f;
                        Debug.Log("Doña Rosa te refrescó por ser buenos amigos.");
                    }

                    return; // Salimos para no interactuar con dos cosas a la vez
                }
            }

            // --- CASO 2: CLIENTE ---
            if (col.CompareTag("Cliente"))
            {
                if (stats.churrosCantidad > 0)
                {
                    VenderACliente(col.gameObject);
                    return;
                }
                else
                {
                    Debug.Log("<color=red>[AVISO]</color> ¡Te quedaste sin churros! Busca un puesto de recarga.");
                }
            }
        }
    }

    private void VenderACliente(GameObject cliente)
    {
        // Lógica de venta
        stats.churrosCantidad--;
        stats.AddMoney(15f);

        // Vender cansa y da calor
        stats.temperature += 2f;
        stats.hydration -= 4f;

        Debug.Log($"<color=yellow>[VENTA]</color> Churro entregado a {cliente.name}. Quedan {stats.churrosCantidad} churros.");

        // Aquí podrías añadir un efecto visual o sonido
        // AudioSource.PlayClipAtPoint(sonidoVenta, transform.position);
    }

    // Dibuja el círculo de interacción en el Editor para que puedas ajustarlo visualmente
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuracion")]
    public float radioInteraccion = 3.5f;
    public LayerMask capaNPC;
    public KeyCode teclaInteractuar = KeyCode.F;

    private NPCConversacion npcCercano;

    void Update()
    {
        DetectarNPCs();
        if (Input.GetKeyDown(teclaInteractuar) && npcCercano != null)
        {
            npcCercano.Interactuar();
        }
    }

    private void DetectarNPCs()
    {
        // Detectamos a todos en el area
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radioInteraccion, capaNPC);
        
        npcCercano = null;
        float distanciaCercana = Mathf.Infinity;

        foreach (var col in cols)
        {
            NPCConversacion npc = col.GetComponent<NPCConversacion>();
            if (npc != null)
            {
                // PRIORIDAD: 
                // 1. Cliente que quiere comprar (Hambre activo)
                // 2. Vendedor (Siempre disponible)
                // 3. Otros
                float dist = Vector3.Distance(transform.position, col.transform.position);
                
                if (npc.quiereComprar || npc.esVendedor) {
                    // Si encontramos a alguien con quien realmente podemos tratar, lo elegimos
                    npcCercano = npc;
                    return; 
                }

                if (dist < distanciaCercana) {
                    distanciaCercana = dist;
                    npcCercano = npc;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuracion")]
    public float radioInteraccion = 2.5f;
    public LayerMask capaNPC;

    [Header("Tecla de Interaccion")]
    public KeyCode teclaInteractuar = KeyCode.E;

    private NPCConversacion npcCercano;
    private bool hayNPCCercano = false;

    void Update()
    {
        DetectarNPCsCercanos();

        // INTERACCION DIRECTA CON TECLA
        if (Input.GetKeyDown(teclaInteractuar))
        {
            IntentarInteractuar();
        }
    }

    private void DetectarNPCsCercanos()
    {
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, radioInteraccion, capaNPC);
        
        npcCercano = null;
        hayNPCCercano = false;

        foreach (Collider2D col in colisiones)
        {
            NPCConversacion npc = col.GetComponent<NPCConversacion>();
            if (npc != null)
            {
                // Prioridad a clientes que quieran comprar
                if (npc.esCliente && npc.quiereComprar)
                {
                    npcCercano = npc;
                    hayNPCCercano = true;
                    return;
                }
                // Si no hay clientes activos, cualquier NPC/Vendedor sirve
                npcCercano = npc;
                hayNPCCercano = true;
            }
        }
    }

    private void IntentarInteractuar()
    {
        if (hayNPCCercano && npcCercano != null)
        {
            npcCercano.Interactuar();
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuracion")]
    public float radioInteraccion = 2.5f;
    public LayerMask capaNPC;
    public LayerMask capaObjetos;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    private NPCConversacion npcCercano;
    private bool hayNPCCercano = false;

    void Update()
    {
        DetectarNPCsCercanos();
    }

    private void DetectarNPCsCercanos()
    {
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, radioInteraccion, capaNPC);
        
        npcCercano = null;
        hayNPCCercano = false;

        foreach (Collider2D col in colisiones)
        {
            if (col.gameObject != this.gameObject)
            {
                NPCConversacion npc = col.GetComponent<NPCConversacion>();
                if (npc != null)
                {
                    npcCercano = npc;
                    hayNPCCercano = true;
                    return;
                }
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IntentarInteractuar();
        }
    }

    private void IntentarInteractuar()
    {
        if (hayNPCCercano && npcCercano != null)
        {
            npcCercano.Interactuar();
            Debug.Log("Interactuando con: " + npcCercano.gameObject.name);
        }
    }

    public void Interactuar()
    {
        IntentarInteractuar();
    }

    public bool HayNPCCerca()
    {
        return hayNPCCercano;
    }

    public NPCConversacion GetNPCCercano()
    {
        return npcCercano;
    }

    private void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;
        Gizmos.color = hayNPCCercano ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}
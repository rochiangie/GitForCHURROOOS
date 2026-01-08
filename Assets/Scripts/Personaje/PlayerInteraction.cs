using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuracion")]
    public float radioInteraccion = 3f; // Un poco mas grande para facilitar
    public LayerMask capaNPC;
    public KeyCode teclaInteractuar = KeyCode.F; // Cambiado a F

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
        Collider2D col = Physics2D.OverlapCircle(transform.position, radioInteraccion, capaNPC);
        if (col != null)
        {
            npcCercano = col.GetComponent<NPCConversacion>();
            if (npcCercano != null) {
                // Debug opcional para que veas en consola si lo detecta
                // Debug.Log("Cerca de: " + npcCercano.nombre);
            }
        }
        else
        {
            npcCercano = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}
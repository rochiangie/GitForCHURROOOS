using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    public float radioInteraccion = 2.5f;
    public LayerMask capaNPC;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RealizarInteraccion();
        }
    }

    void RealizarInteraccion()
    {
        // Usamos OverlapCircleAll para capturar todo lo que esté en el radio
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, radioInteraccion, capaNPC);

        // Ordenar por distancia para hablar siempre con el más cercano primero
        System.Array.Sort(colisiones, (a, b) =>
            Vector2.Distance(transform.position, a.transform.position).CompareTo(
            Vector2.Distance(transform.position, b.transform.position)));

        foreach (Collider2D col in colisiones)
        {
            NPCConversacion npc = col.GetComponent<NPCConversacion>();

            // Si el objeto tiene el script y NO es el jugador mismo
            if (npc != null && col.gameObject != this.gameObject)
            {
                npc.Interactuar();
                return; // Salimos inmediatamente al encontrar el primer NPC válido
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float radio = 3.5f;
    public LayerMask capaNPC;
    public KeyCode tecla = KeyCode.F;

    private NPCConversacion cercano;

    void Update() {
        Detectar();
        if (Input.GetKeyDown(tecla) && cercano != null) cercano.Interactuar();
    }

    void Detectar() {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radio, capaNPC);
        cercano = null;
        float minDist = Mathf.Infinity;

        foreach (var col in cols) {
            NPCConversacion npc = col.GetComponent<NPCConversacion>();
            if (npc == null) continue;

            float d = Vector2.Distance(transform.position, col.transform.position);
            // Prioridad a los que quieren comprar o venden
            if (npc.quiereComprar || npc.esVendedorBebidas) {
                cercano = npc;
                return;
            }
            if (d < minDist) {
                minDist = d;
                cercano = npc;
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}
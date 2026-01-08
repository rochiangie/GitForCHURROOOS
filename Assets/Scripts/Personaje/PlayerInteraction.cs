using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    [Tooltip("El alcance del churrero para hablar con la gente.")]
    public float radioInteraccion = 2.5f;

    [Tooltip("Asegurate de que los NPCs estén en esta capa.")]
    public LayerMask capaNPC;

    void Update()
    {
        // Detectamos la tecla F
        if (Input.GetKeyDown(KeyCode.F))
        {
            RealizarInteraccion();
        }
    }

    void RealizarInteraccion()
    {
        // Detectamos colisiones en un círculo alrededor del jugador
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, radioInteraccion, capaNPC);

        bool encontreAlguien = false;

        foreach (Collider2D col in colisiones)
        {
            // Buscamos el componente unificado en el objeto que tocamos
            NPCConversacion npc = col.GetComponent<NPCConversacion>();

            if (npc != null)
            {
                // Le pedimos al NPC que se encargue de hablar con el Manager
                npc.Interactuar();
                encontreAlguien = true;

                // Usamos break para que si hay dos personas pegadas, 
                // solo hablemos con la primera y no se abran dos paneles.
                break;
            }
        }

        if (!encontreAlguien)
        {
            Debug.Log("No hay nadie cerca en la capa NPC para hablar.");
        }
    }

    // Esto sirve para que en la pestaña "Scene" de Unity veas el círculo amarillo
    // y sepas si el radio es muy chico o muy grande.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}
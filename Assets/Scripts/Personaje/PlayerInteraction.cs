using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float radioInteraccion = 2.5f;
    public LayerMask capaInteraccion;
    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interactuar();
        }
    }

    void Interactuar()
    {
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, radioInteraccion, capaInteraccion);

        foreach (Collider2D col in colisiones)
        {
            if (col.CompareTag("Cliente"))
            {
                ClientePersonalidad cliente = col.GetComponent<ClientePersonalidad>();
                ClienteDialogos dialogosExtra = col.GetComponent<ClienteDialogos>();

                if (cliente != null && cliente.quiereComprar && stats.churrosCantidad > 0)
                {
                    // Lógica de venta
                    cliente.ReaccionarVenta();
                    stats.churrosCantidad--;
                    stats.AddMoney(15f);

                    // LLAMADA AL DIALOGO
                    if (dialogosExtra != null && DialogoManager.Instance != null)
                    {
                        string frase = dialogosExtra.ObtenerFrase(cliente.personalidad);
                        Dialogo d = ScriptableObject.CreateInstance<Dialogo>();
                        d.nombrePersonaje = cliente.nombreCliente;
                        d.frases = new string[] { frase };

                        // Esta llamada ahora coincide con la Version 1 del Manager
                        DialogoManager.Instance.IniciarDialogo(d);
                    }
                    break;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}
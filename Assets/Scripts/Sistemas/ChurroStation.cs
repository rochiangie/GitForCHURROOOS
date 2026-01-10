using UnityEngine;

public class ChurroStation : MonoBehaviour
{
    [Header("Configuracion")]
    public int cantidadRecarga = 50;
    public float radioInteraccion = 2.5f;
    public KeyCode teclaRecarga = KeyCode.E;

    [Header("Visual (Opcional)")]
    public GameObject mensajeVisual; // Un objeto con el texto "Presiona E para recargar"

    private Transform player;
    private PlayerStats stats;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p)
        {
            player = p.transform;
            stats = p.GetComponent<PlayerStats>();
        }
        
        if (mensajeVisual) mensajeVisual.SetActive(false);
    }

    void Update()
    {
        if (player == null || stats == null) return;

        // Medir distancia al jugador
        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= radioInteraccion)
        {
            // Mostrar mensaje si lo tenemos configurado
            if (mensajeVisual) mensajeVisual.SetActive(true);

            // Detectar la tecla E
            if (Input.GetKeyDown(teclaRecarga))
            {
                Recargar();
            }
        }
        else
        {
            if (mensajeVisual) mensajeVisual.SetActive(false);
        }
    }

    void Recargar()
    {
        stats.AgregarChurros(cantidadRecarga);
        
        // Feedback en consola
        Debug.Log("<color=yellow><b>[Fábrica]</b> ¡Canasto lleno! +50 churros.</color>");
        
        // Aqui podrias agregar un sonido de bolsa o de campana
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}

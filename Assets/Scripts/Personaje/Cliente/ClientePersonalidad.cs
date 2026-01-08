using UnityEngine;

public class ClientePersonalidad : MonoBehaviour
{
    public enum TipoCliente { Amistoso, Molesto, Indiferente, Ayudante }

    [Header("Configuración de Personalidad")]
    public TipoCliente personalidad;
    public bool quiereComprar = true;
    public string nombreCliente = "Turista";

    [Header("Referencias Visuales")]
    [Tooltip("Arrastra aquí el objeto hijo que tiene el Sprite del Churro")]
    public GameObject iconoDeseo;

    private PlayerStats pStats;

    void Awake()
    {
        // Buscamos al jugador por Tag al iniciar el juego
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            pStats = player.GetComponent<PlayerStats>();
        }
    }

    void Start()
    {
        ConfigurarCliente();
    }

    // Esta es la parte que generaba el error. 
    // Usamos UnityEditor.EditorApplication.delayCall para que el cambio 
    // ocurra justo después de la validación, evitando el conflicto.
    void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null && iconoDeseo != null)
            {
                iconoDeseo.SetActive(quiereComprar);
            }
        };
#endif
    }

    public void ConfigurarCliente()
    {
        if (personalidad == TipoCliente.Indiferente)
        {
            quiereComprar = Random.value > 0.5f;
        }

        ActualizarVisual();
    }

    public void ActualizarVisual()
    {
        if (iconoDeseo != null)
        {
            iconoDeseo.SetActive(quiereComprar);
        }
    }

    public void ReaccionarVenta()
    {
        if (pStats == null) return;

        switch (personalidad)
        {
            case TipoCliente.Amistoso:
                pStats.money += 5f;
                Debug.Log(nombreCliente + ": ¡Qué grandes los churros de Mardel!");
                break;

            case TipoCliente.Molesto:
                pStats.temperature += 10f;
                Debug.Log(nombreCliente + ": ¡Están re caros!");
                break;

            case TipoCliente.Ayudante:
                pStats.hydration += 20f;
                Debug.Log(nombreCliente + ": Tomá un poco de mi agua.");
                break;
        }

        quiereComprar = false;
        ActualizarVisual();
    }
}
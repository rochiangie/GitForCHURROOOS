using UnityEngine;

public class SunSystem : MonoBehaviour
{
    [Header("Configuración del Sol")]
    public float heatIntensity = 2.5f; // Intensidad base del calor

    private PlayerStats playerStats;
    private PlayerActions playerActions;
    private PlayerHealthSystem playerHealth;

    void Start()
    {
        // Buscamos los componentes en el objeto con el Tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            playerActions = player.GetComponent<PlayerActions>();
            playerHealth = player.GetComponent<PlayerHealthSystem>();

            // Sincronizamos la intensidad del calor con el sistema de salud
            playerHealth.heatGainRate = heatIntensity;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el Tag 'Player'.");
        }
    }

    void Update()
    {
        // Si el jugador se desmaya, podrías detener la lógica aquí
        if (playerStats != null && playerStats.temperature >= playerStats.maxStat)
        {
            return;
        }

        // Tecla E para refrescarse (Usando el sistema de acciones)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerActions != null)
            {
                playerActions.TomarBebida();
                // El efecto de "SetDrunk" ya está manejado dentro de TomarBebida() en PlayerActions
            }
        }

        // Aquí podrías añadir lógica de tiempo (ej: a mediodía sube la intensidad)
        SimularPasoDelTiempo();
    }

    void SimularPasoDelTiempo()
    {
        // Ejemplo: Si quisiéramos que el calor aumente con el tiempo
        // heatIntensity += Time.deltaTime * 0.01f;
        // if(playerHealth != null) playerHealth.heatGainRate = heatIntensity;
    }
}
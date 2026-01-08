using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider heatSlider;
    public TextMeshProUGUI churrosText;
    public TextMeshProUGUI plataText;

    // Referencias a los scripts modulares
    private PlayerStats stats;
    private PlayerInteraction interaction; // Si los churros están aquí

    void Start()
    {
        // Buscamos al jugador por su Tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            stats = player.GetComponent<PlayerStats>();
            interaction = player.GetComponent<PlayerInteraction>();
        }
        else
        {
            Debug.LogError("UIManager: No se encontró al objeto con el Tag 'Player'.");
        }
    }

    void Update()
    {
        if (stats != null)
        {
            // La temperatura ahora viene de PlayerStats
            if (heatSlider != null)
            {
                heatSlider.value = stats.temperature / stats.staminaMax;
            }

            // El dinero (plata) ahora viene de PlayerStats
            if (plataText != null)
            {
                plataText.text = "PLATA: $" + stats.money.ToString("F0");
            }
        }

        // Para los churros: Si aún no creamos la variable 'churrosCantidad', 
        // puedes poner un valor temporal o añadir 'public int churrosCantidad' a PlayerStats
        if (churrosText != null && stats != null)
        {
            // Suponiendo que añadiremos 'churros' a PlayerStats pronto
            churrosText.text = "CHURROS: " + "10"; // Valor temporal
        }
    }
}
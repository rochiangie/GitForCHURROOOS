using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider hydrationSlider;
    public Slider staminaSlider;
    public Slider temperatureSlider;
    public TextMeshProUGUI churrosText;
    public TextMeshProUGUI moneyText;

    private PlayerStats playerStats;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        if (playerStats == null) return;

        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (hydrationSlider != null)
        {
            hydrationSlider.maxValue = playerStats.hydrationMax;
            hydrationSlider.value = playerStats.hydration;
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = playerStats.staminaMax;
            staminaSlider.value = playerStats.stamina;
        }

        if (temperatureSlider != null)
        {
            temperatureSlider.maxValue = playerStats.temperatureMax;
            temperatureSlider.value = playerStats.temperature;
        }

        if (churrosText != null)
        {
            churrosText.text = "CHURROS: " + playerStats.churrosCantidad;
        }

        if (moneyText != null)
        {
            moneyText.text = "$" + playerStats.money.ToString("F2");
        }
    }
}
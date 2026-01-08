using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Sliders de Estado")]
    public Slider sliderHidratacion;
    public Slider sliderStamina;
    public Slider sliderTemperatura;

    [Header("Textos")]
    public TextMeshProUGUI textoDinero;
    public TextMeshProUGUI textoChurros;
    public TextMeshProUGUI textoReloj;

    [Header("Feedback Visual")]
    public Image overlayCalor; // Imagen roja/naranja que parpadea si hay mucho calor

    private PlayerStats stats;
    private SunSystem sol;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) stats = p.GetComponent<PlayerStats>();
        sol = FindFirstObjectByType<SunSystem>();
    }

    void Update()
    {
        if (stats == null) return;

        // Actualizar Sliders
        sliderHidratacion.value = stats.hydration / stats.hydrationMax;
        sliderStamina.value = stats.stamina / stats.staminaMax;
        sliderTemperatura.value = stats.temperature / stats.temperatureMax;

        // Actualizar Textos
        textoDinero.text = "$" + stats.money.ToString("F0");
        textoChurros.text = "CHURROS: " + stats.churrosCantidad;

        // Formatear Reloj (Hora:Minutos)
        if (sol != null)
        {
            int horas = (int)sol.horaActual;
            int minutos = (int)((sol.horaActual - horas) * 60);
            textoReloj.text = string.Format("{0:00}:{1:00}", horas, minutos);
        }

        // Feedback de Calor Critico
        if (overlayCalor != null)
        {
            float alpha = (stats.temperature > 70) ? (stats.temperature - 70) / 30f : 0f;
            Color c = overlayCalor.color;
            c.a = alpha * 0.4f; // Maximo 40% de opacidad
            overlayCalor.color = c;
        }
    }
}
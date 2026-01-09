using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderHidratacion;
    public Slider sliderStamina;
    public Slider sliderTemperatura;

    [Header("Textos")]
    public TextMeshProUGUI textoDinero;
    public TextMeshProUGUI textoChurros;
    public TextMeshProUGUI textoReloj;

    [Header("Efecto Calor (Overlay)")]
    public Image overlayCalor; 
    [Range(0, 1)] public float opacidadMaxima = 0.8f; // Subimos la intensidad

    private PlayerStats stats;
    private SunSystem sol;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) stats = p.GetComponent<PlayerStats>();
        sol = FindFirstObjectByType<SunSystem>();

        if (overlayCalor != null) {
            // Aseguramos que ocupe TODA la pantalla por codigo
            RectTransform rect = overlayCalor.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Forzar que este por DELANTE de todo
            overlayCalor.transform.SetAsLastSibling();
            overlayCalor.raycastTarget = false;
        }
    }

    void Update()
    {
        if (stats == null) return;

        // Sliders (Usamos el Maximo del PlayerStats para que sea real)
        if(sliderHidratacion) sliderHidratacion.value = stats.hydration / stats.hydrationMax;
        if(sliderStamina) sliderStamina.value = stats.stamina / stats.staminaMax;
        if(sliderTemperatura) sliderTemperatura.value = stats.temperature / stats.temperatureMax;

        if(textoDinero) textoDinero.text = "$" + stats.money.ToString("F0");
        if(textoChurros) textoChurros.text = "CHURROS: " + stats.churrosCantidad;

        if (sol != null && textoReloj != null) {
            int horas = (int)sol.horaActual;
            int minutos = (int)((sol.horaActual - horas) * 60);
            textoReloj.text = string.Format("{0:00}:{1:00}", horas, minutos);
        }

        // --- CALCULO DE CALOR SEGUN TU MAXIMO (42) ---
        if (overlayCalor != null)
        {
            // Si la temperatura es 42 y el maximo es 42, el factor es 1
            float factorCalor = stats.temperature / stats.temperatureMax; 
            
            // Hacemos que el efecto empiece a notarse fuerte despues del 50% de temp
            float intensidadEfecto = Mathf.Clamp01((factorCalor - 0.5f) * 2f);
            
            // Pulso de "sofoco"
            float pulso = Mathf.Sin(Time.time * 2.5f) * 0.1f;
            
            Color c = overlayCalor.color;
            c.a = Mathf.Clamp01((intensidadEfecto * opacidadMaxima) + (intensidadEfecto > 0.1f ? pulso : 0f));
            overlayCalor.color = c;
        }
    }
}
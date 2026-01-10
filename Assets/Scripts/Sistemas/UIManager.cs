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

        // --- EFECTO VISUAL DE CALOR / ROJO ---
        if (overlayCalor != null)
        {
            float factorCalor = stats.temperature / stats.temperatureMax; // 20/42 = 0.47 aprox
            
            // Empezamos a teñir desde el 40% de la barra (aprox 17 grados) para que sea progresivo
            float intensidadEfecto = Mathf.Clamp01((factorCalor - 0.4f) / 0.6f);
            
            // Pulso de "sofoco" mas rapido e intenso
            float pulso = Mathf.Sin(Time.time * 3.5f) * 0.15f;
            
            // Forzamos que el color sea rojo tirando a naranja
            overlayCalor.color = new Color(1f, 0.2f, 0f, 0f); 
            
            Color c = overlayCalor.color;
            c.a = Mathf.Clamp01((intensidadEfecto * opacidadMaxima) + (intensidadEfecto > 0.2f ? pulso : 0f));
            overlayCalor.color = c;
        }
    }
}
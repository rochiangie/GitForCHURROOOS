using UnityEngine;

public class SunSystem : MonoBehaviour
{
    [Header("Configuracion de Tiempo")]
    public float horaActual = 8f; 
    public float velocidadTiempo = 0.5f; // Mas rapido para testear
    
    [Header("Color del Cielo (Background)")]
    public Camera camaraPrincipal;
    public Gradient colorCielo; // Configura: Celeste mañana, Naranja mediodía, Violeta tarde

    [Header("Mecanica de Calor")]
    public float horaPico = 14f;

    private PlayerStats stats;

    void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) stats = p.GetComponent<PlayerStats>();
    }

    void Update()
    {
        // 1. Pasar el tiempo
        horaActual += Time.deltaTime * velocidadTiempo;
        if (horaActual > 20) horaActual = 8; 

        // 2. Cambiar color del fondo de la camara
        if (camaraPrincipal != null)
        {
            float factorDia = (horaActual - 8) / 12f; 
            camaraPrincipal.backgroundColor = colorCielo.Evaluate(Mathf.Clamp01(factorDia));
        }

        // 3. Afectar al Jugador
        if (stats != null)
        {
            // Cuanto mas cerca de las 14:00, mas calor
            float cercaniaPico = 1f - Mathf.Abs(horaActual - horaPico) / 4f;
            float calorActual = Mathf.Clamp01(cercaniaPico);
            
            stats.ReducirHidratacion((0.5f + (calorActual * 2f)) * Time.deltaTime);
            stats.AumentarTemperatura(calorActual * 1.5f * Time.deltaTime);
        }
    }
}
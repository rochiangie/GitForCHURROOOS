using UnityEngine;

public class SunSystem : MonoBehaviour
{
    [Header("Configuracion de Tiempo")]
    public float horaActual = 8f; 
    public float velocidadTiempo = 0.3f; // Un poco mas lento para dar tiempo
    
    [Header("Mecanica de Calor")]
    public float horaPico = 14f;

    private PlayerStats stats;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) stats = p.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.juegoTerminado) return;

        // Pasar el tiempo
        horaActual += Time.deltaTime * velocidadTiempo;
        
        // ECONOMIA INCREMENTAL: El calor y la deshidratacion suben con el nivel
        int nivel = (GameManager.Instance != null) ? GameManager.Instance.nivelActualIndex : 0;
        float multDificultad = 1f + (nivel * 0.2f); // 20% mas dificil por nivel

        // Afectar al Jugador
        if (stats != null)
        {
            float cercaniaPico = 1f - Mathf.Abs(horaActual - horaPico) / 4f;
            float calorActual = Mathf.Clamp01(cercaniaPico);
            
            // Perdida de hidratacion base + impacto del nivel
            float factorDeshidratacion = (0.5f + (calorActual * 2.5f)) * multDificultad;
            stats.ReducirHidratacion(factorDeshidratacion * Time.deltaTime);
            
            // Aumento de temperatura con impacto del nivel
            float factorCalor = calorActual * 1.5f * multDificultad;
            stats.AumentarTemperatura(factorCalor * Time.deltaTime);
        }
    }
}
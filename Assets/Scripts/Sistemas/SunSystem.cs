using UnityEngine;

public class SunSystem : MonoBehaviour
{
    [Header("Configuracion de Tiempo")]
    public float horaActual = 8f; 
    
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

        // ECONOMIA INCREMENTAL: El tiempo, calor y deshidratacion se ajustan segun el NivelData
        if (GameManager.Instance == null || GameManager.Instance.niveles == null || GameManager.Instance.nivelActualIndex >= GameManager.Instance.niveles.Count) return;

        NivelData dataNivel = GameManager.Instance.niveles[GameManager.Instance.nivelActualIndex];
        
        // Usamos la velocidad definida especificamente en el Asset del Nivel
        float velocidadActual = dataNivel.velocidadReloj;

        // Pasar el tiempo
        horaActual += Time.deltaTime * velocidadActual;
        
        float multEfectos = 1f + (GameManager.Instance.nivelActualIndex * 0.25f); 

        // Afectar al Jugador
        if (stats != null)
        {
            float cercaniaPico = 1f - Mathf.Abs(horaActual - horaPico) / 4f;
            float calorActual = Mathf.Clamp01(cercaniaPico);
            
            // Perdida de hidratacion base + impacto del nivel
            float factorDeshidratacion = (0.5f + (calorActual * 2.5f)) * multEfectos;
            stats.ReducirHidratacion(factorDeshidratacion * Time.deltaTime);
            
            // Aumento de temperatura con impacto del nivel
            float factorCalor = calorActual * 1.5f * multEfectos;
            stats.AumentarTemperatura(factorCalor * Time.deltaTime);
        }
    }
}
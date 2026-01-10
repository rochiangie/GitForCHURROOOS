using UnityEngine;

public class SunSystem : MonoBehaviour
{
    [Header("Estado del Tiempo")]
    public float horaActual = 8f; // Empieza a las 8 AM
    
    [Header("Mecanica de Calor")]
    public float horaPico = 14f;

    private PlayerStats stats;
    private float horasTotalesTurno = 12f; // De 8 AM a 8 PM

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) stats = p.GetComponent<PlayerStats>();
    }

    void Update()
    {
        // SEGURIDAD: Si no hay GameManager o el juego esta detenido, no pasa el tiempo
        if (GameManager.Instance == null || GameManager.Instance.juegoTerminado || GameManager.Instance.enPausa) return;
        
        // SEGURIDAD: Si la lista de niveles esta mal configurada
        if (GameManager.Instance.niveles == null || GameManager.Instance.nivelActualIndex >= GameManager.Instance.niveles.Count) {
            Debug.LogWarning("[SunSystem] No hay niveles configurados en el GameManager.");
            return;
        }

        NivelData dataNivel = GameManager.Instance.niveles[GameManager.Instance.nivelActualIndex];
        
        // CALCULO DE VELOCIDAD POSITIVA:
        float duracionMinutos = Mathf.Max(0.1f, dataNivel.duracionDiaMinutos); // Evitamos division por cero
        float duracionSegundos = duracionMinutos * 60f;
        float velocidadTiempo = horasTotalesTurno / duracionSegundos;

        // Pasar el tiempo
        horaActual += Time.deltaTime * velocidadTiempo;
        
        // Multiplicador de dificultad para efectos (sed/calor)
        float multEfectos = 1f + (GameManager.Instance.nivelActualIndex * 0.25f); 

        // Afectar al Jugador
        if (stats != null)
        {
            float cercaniaPico = 1f - Mathf.Abs(horaActual - horaPico) / 4f;
            float calorActual = Mathf.Clamp01(cercaniaPico);
            
            float factorDeshidratacion = (0.5f + (calorActual * 2.5f)) * multEfectos;
            stats.ReducirHidratacion(factorDeshidratacion * Time.deltaTime);
            
            float factorCalor = calorActual * 1.5f * multEfectos;
            stats.AumentarTemperatura(factorCalor * Time.deltaTime);
        }
    }
}
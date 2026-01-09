using UnityEngine;

public class SunSystem : MonoBehaviour
{
    [Header("Configuracion de Tiempo")]
    public float horaActual = 8f; 
    public float velocidadTiempo = 0.5f; 
    
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
        // Pasar el tiempo
        horaActual += Time.deltaTime * velocidadTiempo;
        if (horaActual > 20) horaActual = 8; 

        // Afectar al Jugador
        if (stats != null)
        {
            float cercaniaPico = 1f - Mathf.Abs(horaActual - horaPico) / 4f;
            float calorActual = Mathf.Clamp01(cercaniaPico);
            
            stats.ReducirHidratacion((0.5f + (calorActual * 2.5f)) * Time.deltaTime);
            stats.AumentarTemperatura(calorActual * 2f * Time.deltaTime);
        }
    }
}
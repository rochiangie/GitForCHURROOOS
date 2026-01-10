using UnityEngine;

public class SunSystem : MonoBehaviour
{
    [Header("Estado del Tiempo")]
    public float horaActual = 8f; // Empieza a las 8 AM
    public float horaPico = 14f;
    private float horasTotalesTurno = 12f; // De 8 AM a 8 PM
    
    [Header("Visual del Sol")]
    public SpriteRenderer sunSprite;
    public Transform sunrisePoint; // Punto donde aparece (8 AM)
    public Transform zenithPoint;  // Punto mas alto (Pico de calor)
    public Transform sunsetPoint;  // Punto donde se oculta (8 PM)

    [Header("Color del Sol")]
    public Gradient sunGradient; // Definilo en el Inspector: Amarillo -> Rojo (Midday) -> Naranja

    private PlayerStats stats;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) stats = p.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.juegoTerminado || GameManager.Instance.enPausa) return;
        
        if (GameManager.Instance.niveles == null || GameManager.Instance.nivelActualIndex >= GameManager.Instance.niveles.Count) return;

        NivelData dataNivel = GameManager.Instance.niveles[GameManager.Instance.nivelActualIndex];
        
        float duracionMinutos = Mathf.Max(0.1f, dataNivel.duracionDiaMinutos); 
        float duracionSegundos = duracionMinutos * 60f;
        float velocidadTiempo = horasTotalesTurno / duracionSegundos;

        // Pasar el tiempo
        horaActual += Time.deltaTime * velocidadTiempo;
        
        // --- LOGICA VISUAL DEL SOL ---
        ManejarVisualSol();

        // Multiplicador de dificultad
        float multEfectos = 1f + (GameManager.Instance.nivelActualIndex * 0.25f); 

        // Afectar al Jugador
        if (stats == null) {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) stats = p.GetComponent<PlayerStats>();
        }

        if (stats != null)
        {
            // Ampliamos el rango de 4 a 6 para que empiece a calentar desde las 8 AM
            float cercaniaPico = 1f - Mathf.Abs(horaActual - horaPico) / 6f;
            float calorActual = Mathf.Clamp01(cercaniaPico);
            
            // Incrementamos la velocidad de calentamiento (subimos de 1.5 a 2.5)
            float factorDeshidratacion = (0.7f + (calorActual * 3.0f)) * multEfectos;
            stats.ReducirHidratacion(factorDeshidratacion * Time.deltaTime);
            
            float factorCalor = (0.2f + (calorActual * 2.8f)) * multEfectos;
            stats.AumentarTemperatura(factorCalor * Time.deltaTime);
        }
    }

    void ManejarVisualSol() {
        if (sunSprite == null || sunrisePoint == null || zenithPoint == null || sunsetPoint == null) return;

        // Calculamos el factor del dia (0 a 1)
        float factorDia = (horaActual - 8f) / horasTotalesTurno;
        factorDia = Mathf.Clamp01(factorDia);

        // Movimiento curvo (Bezier Cuadratico)
        Vector3 pos = CalculateBezierPoint(factorDia, sunrisePoint.position, zenithPoint.position, sunsetPoint.position);
        sunSprite.transform.position = new Vector3(pos.x, pos.y, 0f); // Forzamos Z=0 para que sea visible

        // Color segun el gradiente
        if (sunGradient != null) {
            sunSprite.color = sunGradient.Evaluate(factorDia);
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0; 
        p += 2 * u * t * p1; 
        p += tt * p2; 
        return p;
    }
}
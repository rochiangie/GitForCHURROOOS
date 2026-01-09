using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Recursos")]
    public float money = 0f;
    public int churrosCantidad = 20;

    [Header("Limites")]
    public float hydration = 100f;
    public float hydrationMax = 100f;
    public float stamina = 100f;
    public float staminaMax = 100f;

    [Header("Ebriedad y Temperatura")]
    public float temperature = 0f;
    public float temperatureMax = 42f; 
    [Range(0, 100)] public float ebriedad = 0f;

    private bool estaEnAgua = false;
    private bool estaEnSombra = false;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.juegoTerminado) return;

        ManejarStats();
        ChequearCondicionesVictoriaDerrota();
    }

    void ManejarStats()
    {
        hydration = Mathf.Clamp(hydration, 0f, hydrationMax);
        stamina = Mathf.Clamp(stamina, 0f, staminaMax);
        temperature = Mathf.Clamp(temperature, 0f, temperatureMax);
        ebriedad = Mathf.Clamp(ebriedad, 0f, 100f);

        if (estaEnAgua) ReducirTemperatura(8f * Time.deltaTime);
        else if (estaEnSombra) ReducirTemperatura(3f * Time.deltaTime);
    }

    void ChequearCondicionesVictoriaDerrota()
    {
        if (GameManager.Instance == null) return;

        // CONDICIONES DE DERROTA
        if (hydration <= 0) 
            GameManager.Instance.PerderJuego("Te moriste de sed bajo el sol.");
        
        if (temperature >= temperatureMax) 
            GameManager.Instance.PerderJuego("Te desmayaste por un golpe de calor.");

        // CONDICION DE VICTORIA
        if (money >= GameManager.Instance.metaDinero)
            GameManager.Instance.GanarJuego();
    }

    public void AgregarDinero(float cantidad) { money += cantidad; }
    public bool GastarDinero(float cantidad) {
        if (money >= cantidad) { money -= cantidad; return true; }
        return false;
    }

    public void AgregarChurros(int cantidad) { churrosCantidad += cantidad; }
    public bool ConsumirChurro() {
        if (churrosCantidad > 0) { churrosCantidad--; return true; }
        return false;
    }

    public void RecuperarHidratacion(float cantidad) { hydration = Mathf.Min(hydration + cantidad, hydrationMax); }
    public void ReducirHidratacion(float cantidad) { hydration = Mathf.Max(hydration - cantidad, 0f); }
    public void ConsumirStamina(float cantidad) { stamina = Mathf.Max(stamina - cantidad, 0f); }
    public void RecuperarStamina(float cantidad) { stamina = Mathf.Min(stamina + cantidad, staminaMax); }
    public void ReducirTemperatura(float cantidad) { temperature = Mathf.Max(temperature - cantidad, 0f); }
    public void AumentarTemperatura(float cantidad) { temperature = Mathf.Min(temperature + cantidad, temperatureMax); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water")) estaEnAgua = true;
        if (other.CompareTag("Shadow")) estaEnSombra = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water")) estaEnAgua = false;
        if (other.CompareTag("Shadow")) estaEnSombra = false;
    }
}
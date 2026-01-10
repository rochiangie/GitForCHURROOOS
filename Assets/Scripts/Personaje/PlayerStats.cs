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

    [Header("Configuracion de Ambiente")]
    public float enfriamientoWater = 8f;
    public float enfriamientoShadow = 3f;

    private bool estaEnAgua = false;
    private bool estaEnSombra = false;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.juegoTerminado) return;

        // Limites
        hydration = Mathf.Clamp(hydration, 0f, hydrationMax);
        stamina = Mathf.Clamp(stamina, 0f, staminaMax);
        temperature = Mathf.Clamp(temperature, 0f, temperatureMax);
        ebriedad = Mathf.Clamp(ebriedad, 0f, 100f);

        // Enfriamiento
        if (estaEnAgua) ReducirTemperatura(enfriamientoWater * Time.deltaTime);
        else if (estaEnSombra) ReducirTemperatura(enfriamientoShadow * Time.deltaTime);

        // Chequeo de derrota
        if (hydration <= 0) GameManager.Instance.PerderNivel("Te moriste de sed.");
        if (temperature >= temperatureMax) GameManager.Instance.PerderNivel("Te desmayaste por el calor.");
    }

    public void AgregarDinero(float cant) { money += cant; }
    public bool GastarDinero(float cant) {
        if (money >= cant) { money -= cant; return true; }
        return false;
    }

    public void AgregarChurros(int cant) { churrosCantidad += cant; }
    public bool ConsumirChurro() {
        if (churrosCantidad > 0) { churrosCantidad--; return true; }
        return false;
    }

    public void RecuperarHidratacion(float cant) { hydration = Mathf.Min(hydration + cant, hydrationMax); }
    public void ReducirHidratacion(float cant) { hydration = Mathf.Max(hydration - cant, 0f); }
    public void RecuperarStamina(float cant) { stamina = Mathf.Min(stamina + cant, staminaMax); }
    public void ConsumirStamina(float cant) { stamina = Mathf.Max(stamina - cant, 0f); }
    public void ReducirTemperatura(float cant) { temperature = Mathf.Max(temperature - cant, 0f); }
    public void AumentarTemperatura(float cant) { temperature = Mathf.Min(temperature + cant, temperatureMax); }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Water")) estaEnAgua = true;
        if (other.CompareTag("Shadow")) estaEnSombra = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Water")) estaEnAgua = false;
        if (other.CompareTag("Shadow")) estaEnSombra = false;
    }
}
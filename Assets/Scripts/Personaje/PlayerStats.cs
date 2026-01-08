using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Recursos")]
    public float money = 0f;
    public int churrosCantidad = 10;

    [Header("Limites")]
    public float hydration = 100f;
    public float hydrationMax = 100f;
    
    public float stamina = 100f;
    public float staminaMax = 100f;

    [Header("Temperatura")]
    public float temperature = 0f;
    public float temperatureMax = 100f;

    private void Update()
    {
        ClampearValores();
    }

    private void ClampearValores()
    {
        hydration = Mathf.Clamp(hydration, 0f, hydrationMax);
        stamina = Mathf.Clamp(stamina, 0f, staminaMax);
        temperature = Mathf.Clamp(temperature, 0f, temperatureMax);
        money = Mathf.Max(0f, money);
        churrosCantidad = Mathf.Max(0, churrosCantidad);
    }

    public void AgregarDinero(float cantidad)
    {
        money += cantidad;
        Debug.Log("Dinero: " + money);
    }

    public bool GastarDinero(float cantidad)
    {
        if (money >= cantidad)
        {
            money -= cantidad;
            return true;
        }
        return false;
    }

    public void AgregarChurros(int cantidad)
    {
        churrosCantidad += cantidad;
    }

    public bool ConsumirChurro()
    {
        if (churrosCantidad > 0)
        {
            churrosCantidad--;
            return true;
        }
        return false;
    }

    public void RecuperarHidratacion(float cantidad)
    {
        hydration = Mathf.Min(hydration + cantidad, hydrationMax);
    }

    public void ReducirHidratacion(float cantidad)
    {
        hydration = Mathf.Max(hydration - cantidad, 0f);
        if (hydration <= 0f)
        {
            GameEvents.OnGameOver?.Invoke();
        }
    }

    public void RecuperarStamina(float cantidad)
    {
        stamina = Mathf.Min(stamina + cantidad, staminaMax);
    }

    public void ConsumirStamina(float cantidad)
    {
        stamina = Mathf.Max(stamina - cantidad, 0f);
    }

    public void AumentarTemperatura(float cantidad)
    {
        temperature = Mathf.Min(temperature + cantidad, temperatureMax);
    }

    public void ReducirTemperatura(float cantidad)
    {
        temperature = Mathf.Max(temperature - cantidad, 0f);
    }
}
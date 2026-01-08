using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Configuracion de Recuperacion")]
    public float recuperacionAguaBase = 30f;
    public float reduccionTemperaturaAgua = 20f;
    public float recuperacionStaminaChurro = 25f;

    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerActions requiere PlayerStats");
        }
    }

    public void TomarAgua(float cantidad = 1f)
    {
        if (stats == null) return;

        float recuperacion = recuperacionAguaBase * cantidad;
        stats.RecuperarHidratacion(recuperacion);
        stats.ReducirTemperatura(reduccionTemperaturaAgua * cantidad);
        
        Debug.Log("Tomaste agua. Hidratacion: " + stats.hydration);
    }

    public void ComerChurro()
    {
        if (stats == null) return;

        if (stats.ConsumirChurro())
        {
            stats.RecuperarStamina(recuperacionStaminaChurro);
            Debug.Log("Comiste un churro. Stamina: " + stats.stamina);
        }
    }

    public bool VenderChurro(float precio)
    {
        if (stats == null) return false;

        if (stats.ConsumirChurro())
        {
            stats.AgregarDinero(precio);
            Debug.Log("Churro vendido por $" + precio);
            GameEvents.OnChurroVendido?.Invoke();
            return true;
        }
        return false;
    }

    public bool ComprarChurros(int cantidad, float precioUnitario)
    {
        if (stats == null) return false;

        float costoTotal = cantidad * precioUnitario;
        if (stats.GastarDinero(costoTotal))
        {
            stats.AgregarChurros(cantidad);
            Debug.Log("Compraste " + cantidad + " churros");
            return true;
        }
        return false;
    }

    public void Descansar()
    {
        if (stats == null) return;
        stats.RecuperarStamina(50f * Time.deltaTime);
    }
}
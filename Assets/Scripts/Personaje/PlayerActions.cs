using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    // Definimos la función con el nombre exacto: TomarAgua
    public void TomarAgua(float cantidad)
    {
        if (stats != null)
        {
            // Sumamos a la hidratación actual sin pasarnos del máximo
            stats.hydration = Mathf.Min(stats.hydration + cantidad, stats.hydrationMax);

            // Efecto extra: bajar un poco la temperatura corporal
            stats.temperature = Mathf.Max(stats.temperature - 2f, 36f);

            Debug.Log("Tomaste agua. Hidratación actual: " + stats.hydration);
        }
    }

    public void ComerChurro()
    {
        if (stats != null && stats.churrosCantidad > 0)
        {
            stats.churrosCantidad--;
            stats.stamina = Mathf.Min(stats.stamina + 20f, stats.staminaMax);
        }
    }
}
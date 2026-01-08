using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float money = 0f;
    public int churrosCantidad = 10;

    [Header("Límites (No dejar en 0)")]
    public float hydration = 100f;
    public float hydrationMax = 100f;
    public float stamina = 100f;
    public float staminaMax = 100f;
    public float temperature = 36f;
    public float temperatureMax = 42f;

    public void AddMoney(float amount) => money += amount;

    void Update()
    {
        hydration = Mathf.Clamp(hydration, 0, hydrationMax);
        stamina = Mathf.Clamp(stamina, 0, staminaMax);
        temperature = Mathf.Clamp(temperature, 30, temperatureMax);
    }
}
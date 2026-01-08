using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Settings de Atributos")]
    public float maxTemperature = 100f;
    public float maxHydration = 100f;

    [Header("Estado Actual")]
    public float currentTemperature = 0f;
    public float currentHydration = 100f;
    public float currentMoney = 0f;

    [Header("UI Elements")]
    public Slider tempSlider;
    public Slider drinkSlider;
    public Text moneyText;

    void Update()
    {
        SimulateSunHeat();
        UpdateUI();
        CheckHealth();
    }

    void SimulateSunHeat()
    {
        // El calor sube constantemente bajo el sol
        currentTemperature += Time.deltaTime * 1.5f;

        // La hidratación baja constantemente
        currentHydration -= Time.deltaTime * 1.0f;

        // Limitar valores
        currentTemperature = Mathf.Clamp(currentTemperature, 0, maxTemperature);
        currentHydration = Mathf.Clamp(currentHydration, 0, maxHydration);
    }

    public void DrinkWater(float amount, float cost)
    {
        if (currentMoney >= cost)
        {
            currentMoney -= cost;
            currentHydration += amount;
            currentTemperature -= amount / 2; // Beber algo frío refresca
        }
    }

    public void EnterWater()
    {
        // Enfriamiento rápido
        currentTemperature -= Time.deltaTime * 10f;
    }

    void UpdateUI()
    {
        tempSlider.value = currentTemperature / maxTemperature;
        drinkSlider.value = currentHydration / maxHydration;
        moneyText.text = "$" + currentMoney.ToString("F2");
    }

    void CheckHealth()
    {
        if (currentTemperature >= maxTemperature)
        {
            Debug.Log("¡Golpe de calor! Te desmayaste.");
            // Aquí iría la lógica de reinicio o penalización
        }
    }

    public void SellChurro(float price)
    {
        currentMoney += price;
        // Vender cansa y da calor
        currentTemperature += 2f;
    }
}
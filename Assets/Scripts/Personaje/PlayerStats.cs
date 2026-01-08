using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Atributos")]
    public float temperature = 0f;
    public float hydration = 100f;
    public float money = 0f;
    public float maxStat = 100f;
    public int churrosCantidad = 20; // Nueva variable para los churros

    [Header("UI Elements")]
    public Slider tempSlider;
    public Slider drinkSlider;
    public TextMeshProUGUI moneyText;

    void Update()
    {
        // El Slider se actualiza aquí
        if (tempSlider) tempSlider.value = temperature / maxStat;
        if (drinkSlider) drinkSlider.value = hydration / maxStat;
        if (moneyText) moneyText.text = "$" + money.ToString("F0");
    }

    public void AddMoney(float amount)
    {
        money += amount;
    }
}
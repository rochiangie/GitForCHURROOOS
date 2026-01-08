using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    private PlayerStats stats;
    public float heatGainRate = 2.5f;
    public float thirstRate = 1.8f;

    [HideInInspector] public bool isInsideWater, isShadowed;
    private float lastTempLog = 0f;

    void Start() => stats = GetComponent<PlayerStats>();

    void Update()
    {
        float prevTemp = stats.temperature;

        if (isInsideWater) stats.temperature -= Time.deltaTime * 15f;
        else if (isShadowed) stats.temperature -= Time.deltaTime * 5f;
        else stats.temperature += Time.deltaTime * heatGainRate;

        // Desgaste de hidratación si no está en el agua
        if (!isInsideWater) stats.hydration -= Time.deltaTime * thirstRate;

        stats.temperature = Mathf.Clamp(stats.temperature, 0, stats.maxStat);
        stats.hydration = Mathf.Clamp(stats.hydration, 0, stats.maxStat);

        LogStatus();
    }

    void LogStatus()
    {
        if (Mathf.Abs(stats.temperature - lastTempLog) >= 10f)
        {
            //Debug.Log($"[TERMÓMETRO] Temperatura: {stats.temperature.ToString("F1")}°C");
            lastTempLog = Mathf.Round(stats.temperature);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water")) { isInsideWater = true; Debug.Log("<color=cyan>¡Al agua!</color>"); }
        if (other.CompareTag("Shadow")) { isShadowed = true; Debug.Log("<color=white>Sombra...</color>"); }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water")) isInsideWater = false;
        if (other.CompareTag("Shadow")) isShadowed = false;
    }
}
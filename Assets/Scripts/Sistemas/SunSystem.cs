using UnityEngine;

public class SunSystem : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerActions actions;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            stats = player.GetComponent<PlayerStats>();
            actions = player.GetComponent<PlayerActions>();
        }
    }

    void Update()
    {
        if (stats == null) return;

        // El sol sube la temperatura si no hay hidratación
        if (stats.hydration <= 0)
            stats.temperature += 0.5f * Time.deltaTime;
        else
            stats.hydration -= 0.2f * Time.deltaTime;
    }
}
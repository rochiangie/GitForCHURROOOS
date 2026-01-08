using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    public float health = 100f;
    public float healthMax = 100f;
    private PlayerStats stats;

    void Start() => stats = GetComponent<PlayerStats>();

    void Update()
    {
        // Daño por deshidratación
        if (stats.hydration <= 0) health -= 2f * Time.deltaTime;

        health = Mathf.Clamp(health, 0, healthMax);
    }
}
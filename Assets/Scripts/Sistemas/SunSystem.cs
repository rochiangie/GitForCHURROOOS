using UnityEngine;

public class SunSystem : MonoBehaviour
{
    public float heat = 0f;
    public float heatRate = 2f;
    private PlayerController player;

    void Start() => player = FindObjectOfType<PlayerController>();

    void Update()
    {
        // Si el GameManager no existe o el juego terminó, no procesar
        if (GameManager.Instance != null && GameManager.Instance.juegoTerminado) return;

        // El calor sube constantemente
        heat += heatRate * Time.deltaTime;

        // Si llega a 100, Game Over a través de eventos o GameManager
        if (heat >= 100)
        {
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
        }

        // Tecla E para refrescarse
        if (Input.GetKeyDown(KeyCode.E)) TomarBirra();
    }

    void TomarBirra()
    {
        heat = Mathf.Max(0, heat - 20f);

        if (player != null)
        {
            player.SetDrunk(true);
            // Avisar al sistema de eventos que tomamos una birra
            GameEvents.TriggerBeerDrunk();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.destapeBirra);
        }

        // Se le pasa el efecto en 5 segundos
        CancelInvoke("PassDrunk"); // Resetear el tiempo si toma otra
        Invoke("PassDrunk", 5f);
    }

    void PassDrunk()
    {
        if (player != null) player.SetDrunk(false);
    }
}
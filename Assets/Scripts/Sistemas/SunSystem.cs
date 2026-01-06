using UnityEngine;

public class SunSystem : MonoBehaviour
{
    public float heat = 0f;
    public float heatRate = 2f;
    private PlayerController player;

    void Start() => player = FindObjectOfType<PlayerController>();

    void Update()
    {
        if (GameManager.Instance.juegoTerminado) return;

        heat += heatRate * Time.deltaTime;
        if (heat >= 100) GameManager.Instance.GameOver();

        if (Input.GetKeyDown(KeyCode.E)) TomarBirra();
    }

    void TomarBirra()
    {
        heat = Mathf.Max(0, heat - 20f);
        player.SetDrunk(true);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.destapeBirra);
        // Podrías hacer un Invoke para que se le pase la borrachera después de X segundos
        Invoke("PassDrunk", 5f);
    }

    void PassDrunk() => player.SetDrunk(false);
}
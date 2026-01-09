using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Objetivos")]
    public float metaDinero = 10000f;
    public bool juegoTerminado = false;

    [Header("UI Paneles")]
    public GameObject panelVictoria;
    public GameObject panelDerrota;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        if (panelVictoria) panelVictoria.SetActive(false);
        if (panelDerrota) panelDerrota.SetActive(false);
    }

    public void GanarJuego()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Time.timeScale = 0f; // Pausar juego
        if (panelVictoria) panelVictoria.SetActive(true);
        Debug.Log("¡GANASTE! Sos el rey de los churros.");
    }

    public void PerderJuego(string motivo)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Time.timeScale = 0f; // Pausar juego
        if (panelDerrota) panelDerrota.SetActive(true);
        Debug.Log("PERDISTE: " + motivo);
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
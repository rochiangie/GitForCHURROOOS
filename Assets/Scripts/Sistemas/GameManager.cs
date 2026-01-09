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
        
        // Asegurar que el cursor se vea al inicio si es necesario
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
    }

    public void GanarJuego()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Time.timeScale = 0f; 
        if (panelVictoria) panelVictoria.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("¡GANASTE! Sos el rey de los churros.");
    }

    public void PerderJuego(string motivo)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Time.timeScale = 0f; 
        if (panelDerrota) panelDerrota.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("PERDISTE: " + motivo);
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrACreditos()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Creditos");
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
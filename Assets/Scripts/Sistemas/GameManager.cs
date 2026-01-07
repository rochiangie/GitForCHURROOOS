using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado del Juego")]
    public int diaActual = 1;
    public bool juegoPausado = false;
    public bool juegoTerminado = false;

    [Header("Referencias UI")]
    public GameObject panelGameOver;
    public GameObject panelVictoria;

    private void Awake()
    {
        // Singleton prolijo para que no se duplique
        if (Instance == null)
        {
            Instance = this;
            // Si es la escena de juego, no queremos que se destruya, 
            // pero en una Jam a veces es mejor dejarlo por escena.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Aseguramos que el tiempo corra al empezar
        Time.timeScale = 1f;
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);
    }

    public void IniciarDia()
    {
        juegoTerminado = false;
        juegoPausado = false;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        Debug.Log("¡CHURROOOOOS! El vendedor se desmayó por el calor.");

        // Activar panel de Game Over
        if (panelGameOver != null) panelGameOver.SetActive(true);

        // Frenamos el tiempo DESPUÉS de mostrar la UI para evitar errores de frustum
        Invoke("PausarTiempo", 0.1f);

        GameEvents.TriggerGameOver();
    }

    private void PausarTiempo()
    {
        Time.timeScale = 0f;
    }

    public void ReiniciarNivel()
    {
        // Importante resetear el tiempo antes de cargar la escena
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}
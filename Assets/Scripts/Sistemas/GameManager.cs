using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado del Juego")]
    public int diaActual = 1;
    public bool juegoEnPausa = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        if (GameEvents.OnGameOver != null)
        {
            GameEvents.OnGameOver += ManejarGameOver;
        }
    }

    private void OnDestroy()
    {
        if (GameEvents.OnGameOver != null)
        {
            GameEvents.OnGameOver -= ManejarGameOver;
        }
    }

    public void PausarTiempo()
    {
        Time.timeScale = 0f;
        juegoEnPausa = true;
    }

    public void ReanudarTiempo()
    {
        Time.timeScale = 1f;
        juegoEnPausa = false;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void CargarEscenaJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Juego");
    }

    public void ReiniciarEscena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CargarSiguienteEscena()
    {
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual + 1);
    }

    public void AvanzarDia()
    {
        diaActual++;
        Debug.Log("Dia " + diaActual + " comenzado");
    }

    private void ManejarGameOver()
    {
        Debug.Log("Game Over detectado");
        PausarTiempo();
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
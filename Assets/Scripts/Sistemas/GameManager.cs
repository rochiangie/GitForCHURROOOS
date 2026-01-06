using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado del Juego")]
    public int diaActual = 1;
    public bool juegoPausado = false;
    public bool juegoTerminado = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void IniciarDia()
    {
        juegoTerminado = false;
        Time.timeScale = 1;
        // Aquí podrías disparar un evento de inicio
    }

    public void TerminarDia()
    {
        juegoTerminado = true;
        Time.timeScale = 0;
        // Lógica para mostrar panel de resultados
    }

    public void GameOver()
    {
        juegoTerminado = true;
        Time.timeScale = 0;
        Debug.Log("Game Over: El sol ganó.");
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
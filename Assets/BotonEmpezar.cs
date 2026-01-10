using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonEmpezar : MonoBehaviour
{
    // Cambiamos el nombre para que en el Inspector sepas que es para empezar
    public void EmpezarJuego()
    {
        // Aseguramos que el tiempo corra (por si venis de un Game Over pausado)
        Time.timeScale = 1f;

        // Cargamos la escena del juego
        SceneManager.LoadScene("Lore");
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}
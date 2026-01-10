using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreTester : MonoBehaviour
{
    private bool visible = false;
    private PlayerStats stats;
    private SunSystem sun;
    private GameManager gm;

    void Start()
    {
        // Persistir entre escenas para testear transiciones
        if (FindObjectsByType<CoreTester>(FindObjectsSortMode.None).Length > 1) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Tecla de Testeo (F1)
        if (Input.GetKeyDown(KeyCode.F1)) {
            visible = !visible;
            // Si el menu esta abierto, liberamos el mouse
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }

        // Buscar referencias si se pierden por cambio de escena
        if (stats == null) stats = FindFirstObjectByType<PlayerStats>();
        if (sun == null) sun = FindFirstObjectByType<SunSystem>();
        if (gm == null) gm = GameManager.Instance;
    }

    void OnGUI()
    {
        if (!visible) return;

        GUI.Box(new Rect(10, 10, 300, 450), "CHURRERO SIMULATOR - DEBUG MENU (F1)");

        float y = 40;

        // --- ESTADO DEL TIEMPO ---
        GUI.Label(new Rect(20, y, 280, 20), "<b>--- RELOJ Y TIEMPO ---</b>");
        y += 25;
        if (sun != null) {
            GUI.Label(new Rect(20, y, 280, 20), $"Hora Actual: {sun.horaActual:F2}");
            if (GUI.Button(new Rect(220, y, 80, 20), "+1 Hora")) sun.horaActual += 1f;
            y += 25;
            GUI.Label(new Rect(20, y, 280, 20), $"TimeScale: {Time.timeScale}");
            y += 25;
        } else {
            GUI.Label(new Rect(20, y, 280, 20), "<color=red>SunSystem no encontrado</color>");
            y += 25;
        }

        // --- ESTADO DEL JUEGO ---
        GUI.Label(new Rect(20, y, 280, 20), "<b>--- GAME MANAGER ---</b>");
        y += 25;
        if (gm != null) {
            GUI.Label(new Rect(20, y, 280, 20), $"Nivel: {gm.nivelActualIndex + 1} / {gm.niveles.Count}");
            y += 25;
            GUI.Label(new Rect(20, y, 280, 20), $"Estado: {(gm.juegoTerminado ? "TERMINADO" : "EN CURSO")} | {(gm.enPausa ? "PAUSADO" : "ACTIVO")}");
            y += 25;
            if (GUI.Button(new Rect(20, y, 130, 20), "Forzar Victoria")) gm.PresionarBoton_SiguienteNivel();
            if (GUI.Button(new Rect(160, y, 130, 20), "Forzar Derrota")) gm.PerderNivel("Debug Kill");
            y += 25;
        }

        // --- STATS DEL JUGADOR ---
        GUI.Label(new Rect(20, y, 280, 20), "<b>--- PLAYER STATS ---</b>");
        y += 25;
        if (stats != null) {
            GUI.Label(new Rect(20, y, 280, 20), $"Dinero: ${stats.money}");
            if (GUI.Button(new Rect(220, y, 80, 20), "+$100")) stats.AgregarDinero(100);
            y += 25;
            GUI.Label(new Rect(20, y, 280, 20), $"Churros: {stats.churrosCantidad}");
            if (GUI.Button(new Rect(220, y, 80, 20), "+10 Ch")) stats.AgregarChurros(10);
            y += 25;
            GUI.Label(new Rect(20, y, 280, 20), $"Hidratacion: {stats.hydration:F0}%");
            if (GUI.Button(new Rect(220, y, 80, 20), "Beber")) stats.RecuperarHidratacion(20);
            y += 25;
            GUI.Label(new Rect(20, y, 280, 20), $"Calor: {stats.temperature:F0}%");
            if (GUI.Button(new Rect(220, y, 80, 20), "Enfriar")) stats.temperature -= 10;
            y += 25;
        }

        // --- ESCENAS ---
        GUI.Label(new Rect(20, y, 280, 20), "<b>--- SISTEMA ---</b>");
        y += 25;
        if (GUI.Button(new Rect(20, y, 130, 20), "Reiniciar Escena")) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (GUI.Button(new Rect(160, y, 130, 20), "Menu Principal")) SceneManager.LoadScene("Menu");
        y += 25;
        
        GUI.Label(new Rect(20, y, 280, 20), "<i>Pulsa F1 para cerrar este menu</i>");
    }
}

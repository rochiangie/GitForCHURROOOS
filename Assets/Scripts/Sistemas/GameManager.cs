using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuracion de Niveles")]
    public List<NivelData> niveles;
    public int nivelActualIndex = 0;
    public bool juegoTerminado = false;

    [Header("UI Paneles")]
    public GameObject panelNivelCompletado; 
    public GameObject panelGameOver;
    public GameObject panelFinalVictoria;

    private PlayerStats stats;
    private SunSystem sun;

    void Awake() { Instance = this; }

    void Start() {
        stats = FindFirstObjectByType<PlayerStats>();
        sun = FindFirstObjectByType<SunSystem>();
        CargarNivel(nivelActualIndex);
    }

    public void CargarNivel(int index) {
        if (index >= niveles.Count) return;
        
        nivelActualIndex = index;
        NivelData data = niveles[nivelActualIndex];
        
        juegoTerminado = false;
        Time.timeScale = 1f;
        
        if(panelNivelCompletado) panelNivelCompletado.SetActive(false);
        if(panelGameOver) panelGameOver.SetActive(false);
        if(panelFinalVictoria) panelFinalVictoria.SetActive(false);

        ActualizarAmbientacion(data);
    }

    void ActualizarAmbientacion(NivelData data) {
        GameObject[] clientes = GameObject.FindGameObjectsWithTag("Cliente");
        foreach (var c in clientes) {
            NPCConversacion npc = c.GetComponent<NPCConversacion>();
            if (npc != null) {
                float rng = Random.value;
                npc.personalidad = (rng < data.porcentajeAmigables) ? PersonalidadNPC.Amable : PersonalidadNPC.Molesto;
                npc.quiereComprar = (Random.value * 100 < data.probabilidadCompra);
                npc.churrosDeseados = Random.Range(1, data.maxChurrosPorPedido + 1);
            }
        }
        
        if (data.esNivelBoss && data.prefabBoss != null) {
            Instantiate(data.prefabBoss, Vector3.zero, Quaternion.identity);
        }
    }

    void Update() {
        if (juegoTerminado || niveles == null || niveles.Count == 0 || nivelActualIndex >= niveles.Count) return;

        NivelData data = niveles[nivelActualIndex];

        // Victoria
        if (stats != null && stats.money >= data.metaDinero) FinalizarNivel();

        // Derrota por tiempo
        if (sun != null && sun.horaActual >= 20f) PerderNivel("Anocheció...");
    }

    void FinalizarNivel() {
        juegoTerminado = true;
        Time.timeScale = 0f;
        if (nivelActualIndex == niveles.Count - 1) panelFinalVictoria.SetActive(true);
        else panelNivelCompletado.SetActive(true);
    }

    public void PerderNivel(string m) {
        juegoTerminado = true;
        Time.timeScale = 0f;
        if (panelGameOver) panelGameOver.SetActive(true);
    }

    public void SiguienteNivel() { nivelActualIndex++; CargarNivel(nivelActualIndex); }
    public void Reintentar() { CargarNivel(nivelActualIndex); }
    public void IrACreditos() { SceneManager.LoadScene("Creditos"); }
    public void VolverMenu() { SceneManager.LoadScene("Menu"); }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuracion de Niveles")]
    public List<NivelData> niveles;
    public int nivelActualIndex = 0;
    public bool juegoTerminado = false;
    public bool enPausa = false;

    [Header("UI Paneles")]
    public GameObject panelNivelCompletado; 
    public GameObject panelGameOver;        
    public GameObject panelFinalVictoria;
    public GameObject panelPausa;

    [Header("Textos de Feedback")]
    public TextMeshProUGUI textoDerrota;

    private PlayerStats stats;
    private SunSystem sun;

    void Awake() { 
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);
    }

    void Start() {
        stats = FindFirstObjectByType<PlayerStats>();
        sun = FindFirstObjectByType<SunSystem>();
        CargarNivel(nivelActualIndex);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            AlternarPausa();
        }

        if (juegoTerminado || enPausa) return;
        if (niveles == null || niveles.Count == 0 || nivelActualIndex >= niveles.Count) return;

        NivelData data = niveles[nivelActualIndex];

        bool hablando = DialogoManager.Instance != null && DialogoManager.Instance.panelUI.activeInHierarchy;
        if (stats != null && stats.money >= data.metaDinero && !hablando) {
            FinalizarNivel();
        }

        if (sun != null && sun.horaActual >= 20f) {
            PerderNivel("Te quedaste sin luz y sin ventas...");
        }
    }

    public void AlternarPausa() {
        if (juegoTerminado) return;
        enPausa = !enPausa;
        Time.timeScale = enPausa ? 0f : 1f;
        CerrarTodosLosPaneles();
        if (panelPausa) panelPausa.SetActive(enPausa);
    }

    public void CargarNivel(int index) {
        if (niveles == null || index < 0 || index >= niveles.Count) return;
        
        nivelActualIndex = index;
        NivelData data = niveles[nivelActualIndex];
        
        juegoTerminado = false;
        enPausa = false;
        Time.timeScale = 1f;
        
        CerrarTodosLosPaneles();
        
        if (stats != null) {
            stats.money = 0; 
            stats.temperature = 0;
            stats.hydration = 100;
        }

        if (sun != null) sun.horaActual = 8f; 

        ActualizarAmbientacion(data);
        Debug.Log("<color=cyan><b>[GameManager] NIVEL " + (nivelActualIndex + 1) + " CARGADO CON EXITO.</b></color>");
    }

    void CerrarTodosLosPaneles() {
        if(panelNivelCompletado) panelNivelCompletado.SetActive(false);
        if(panelGameOver) panelGameOver.SetActive(false);
        if(panelFinalVictoria) panelFinalVictoria.SetActive(false);
        if(panelPausa) panelPausa.SetActive(false);
        if (DialogoManager.Instance != null) DialogoManager.Instance.Cerrar();
    }

    void ActualizarAmbientacion(NivelData data) {
        GameObject[] clientes = GameObject.FindGameObjectsWithTag("Cliente");
        foreach (var c in clientes) {
            NPCConversacion npc = c.GetComponent<NPCConversacion>();
            if (npc != null && !npc.esVendedorBebidas) {
                float rng = Random.value;
                npc.personalidad = (rng < data.porcentajeAmigables) ? PersonalidadNPC.Amable : PersonalidadNPC.Molesto;
                npc.quiereComprar = (Random.value * 100 < data.probabilidadCompra);
                npc.churrosDeseados = Random.Range(1, data.maxChurrosPorPedido + 1);
                npc.pagoBaseChurro = Random.Range(15f, 25f);
            }
        }
        
        if (data.esNivelBoss && data.prefabBoss != null) {
            Instantiate(data.prefabBoss, Vector3.zero, Quaternion.identity);
        }
    }

    void FinalizarNivel() {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Time.timeScale = 0f;
        CerrarTodosLosPaneles();

        if (nivelActualIndex == niveles.Count - 1) {
            if (panelFinalVictoria) panelFinalVictoria.SetActive(true);
        } else {
            if (panelNivelCompletado) panelNivelCompletado.SetActive(true);
        }
    }

    public void PerderNivel(string m) {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Time.timeScale = 0f;
        CerrarTodosLosPaneles();
        
        if (panelGameOver) panelGameOver.SetActive(true);
        if (textoDerrota) textoDerrota.text = m;
        
        Debug.Log("PERDISTE: " + m);
    }

    public void SiguienteNivel() {
        Time.timeScale = 1f; 
        nivelActualIndex++;
        if (nivelActualIndex < niveles.Count) CargarNivel(nivelActualIndex);
        else IrACreditos();
    }

    public void Reintentar() {
        CargarNivel(nivelActualIndex);
    }

    public void IrACreditos() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Creditos");
    }

    public void VolverMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
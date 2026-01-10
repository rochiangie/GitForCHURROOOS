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
    public List<Dialogo> dialogosPostVentaGlobal; // Lista de posibles dialogos tras vender
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
        if (niveles == null || niveles.Count == 0) {
            Debug.LogError("[GameManager] ¡No hay niveles configurados en la lista!");
            return;
        }

        if (index < 0 || index >= niveles.Count) {
            Debug.LogWarning($"[GameManager] Intentando cargar nivel {index}, pero solo hay {niveles.Count}. Reseteando a 0.");
            nivelActualIndex = 0;
            index = 0;
        }
        
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
        int countAmables = 0;
        int countMolestos = 0;
        string nombresAmables = "";
        
        foreach (var c in clientes) {
            NPCConversacion npc = c.GetComponent<NPCConversacion>();
            if (npc != null && !npc.esVendedorBebidas) {
                float rng = Random.value;
                npc.personalidad = (rng < data.porcentajeAmigables) ? PersonalidadNPC.Amable : PersonalidadNPC.Molesto;
                
                if (npc.personalidad == PersonalidadNPC.Amable) {
                    countAmables++;
                    nombresAmables += npc.nombre + ", ";
                } else countMolestos++;

                npc.quiereComprar = (Random.value * 100 < data.probabilidadCompra);
                npc.churrosDeseados = Random.Range(1, data.maxChurrosPorPedido + 1);
                npc.pagoBaseChurro = Random.Range(15f, 25f);

                // Inyectamos un dialogo aleatorio del pool global post-venta
                if (dialogosPostVentaGlobal != null && dialogosPostVentaGlobal.Count > 0 && npc.poolYaCompro.Count == 0) {
                    Dialogo elegido = dialogosPostVentaGlobal[Random.Range(0, dialogosPostVentaGlobal.Count)];
                    npc.poolYaCompro.Add(elegido);
                }
            }
        }
        
        // --- REPORTE DETALLADO DEL NIVEL ---
        Debug.Log("<color=yellow><b>========= REPORTE DE NIVEL " + (nivelActualIndex + 1) + " =========</b></color>");
        Debug.Log($"<color=cyan>👥 Clientes Totales:</color> {clientes.Length}");
        Debug.Log($"<color=green>😊 Amables ({countAmables}):</color> {nombresAmables.TrimEnd(' ', ',')}");
        Debug.Log($"<color=red>😠 Molestos:</color> {countMolestos}");
        Debug.Log($"<color=orange>💰 Meta:</color> ${data.metaDinero}");
        Debug.Log($"<color=white>🕒 Duracion Real:</color> {data.duracionDiaMinutos} minutos (12h de juego)");
        Debug.Log($"<color=magenta>🔥 Dificultad Ambiental:</color> x{1f + (nivelActualIndex * 0.25f)} (Sed y Calor)");
        if (data.esNivelBoss) Debug.Log("<color=red><b>⚠️ NIVEL BOSS DETECTADO</b></color>");
        Debug.Log("<color=yellow><b>===========================================</b></color>");

        if (data.esNivelBoss && data.prefabBoss != null) {
            Vector3 spawnPos = Vector3.zero;
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnBoss");
            
            if (spawnPoint != null) {
                spawnPos = spawnPoint.transform.position;
            } else {
                // Si no hay punto, que aparezca lejos del player para que no sea injusto
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if(p) spawnPos = p.transform.position + new Vector3(10, 10, 0); 
            }

            Instantiate(data.prefabBoss, spawnPos, Quaternion.identity);
            Debug.Log($"<color=red><b>[BOSS]</b></color> ¡Apareció en {spawnPos}!");
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

    // --- ESTA ES LA FUNCION PARA TU BOTON "CONTINUAR" ---
    // No pide ningun numero (int). Al tocarlo, el script sabe solo que nivel sigue.
    public void PresionarBoton_SiguienteNivel() {
        Time.timeScale = 1f; // Reanudar el tiempo
        
        int proximoIndice = nivelActualIndex + 1;
        
        Debug.Log("<color=orange>[GameManager] Boton Continuar presionado. Entrando al Nivel " + (proximoIndice + 1) + "</color>");

        if (proximoIndice < niveles.Count) {
            CargarNivel(proximoIndice);
        } else {
            Debug.Log("<color=green>[GameManager] ¡No hay mas niveles! Yendo a creditos.</color>");
            IrACreditos();
        }
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
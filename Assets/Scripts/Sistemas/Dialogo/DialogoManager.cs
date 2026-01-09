using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance { get; private set; }

    [Header("UI Componentes")]
    public GameObject panelUI;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoCuerpo;

    [Header("Contenedores")]
    public GameObject groupOpciones;
    public Button[] botones;
    public TextMeshProUGUI[] textosBotones;

    [Header("Ajustes")]
    public float typingSpeed = 0.04f;

    private NPCConversacion npcActual;
    private Dialogo dialogoData;
    private PlayerStats stats;
    private bool escribiendo = false;

    void Awake() { 
        if (Instance == null) Instance = this; 
    }

    void Start() {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p) stats = p.GetComponent<PlayerStats>();
        if(panelUI != null) panelUI.SetActive(false);
    }

    public void AbrirPanel(NPCConversacion npc) {
        if (npc == null) return;
        npcActual = npc;
        dialogoData = npc.ObtenerDialogoDinamico();
        
        if (dialogoData == null) return;

        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = false;

        panelUI.SetActive(true);
        groupOpciones.SetActive(false);
        textoNombre.text = npcActual.nombre;
        
        StartCoroutine(EscribirLetras(dialogoData.propuesta));
    }

    IEnumerator EscribirLetras(string text) {
        escribiendo = true;
        textoCuerpo.text = "";
        
        float speed = (npcActual != null && npcActual.personalidad == PersonalidadNPC.Molesto) ? 0.02f : typingSpeed;

        foreach(char c in text) {
            textoCuerpo.text += c;
            yield return new WaitForSeconds(speed);
        }
        escribiendo = false;
        
        if (!dialogoData.esGrito) MostrarOpciones();
        else StartCoroutine(AutoCerrarGrito());
    }

    IEnumerator AutoCerrarGrito() {
        yield return new WaitForSeconds(2f);
        Cerrar();
    }

    void MostrarOpciones() {
        groupOpciones.SetActive(true);
        for(int i=0; i<3; i++) {
            if(i < dialogoData.opciones.Length && !string.IsNullOrEmpty(dialogoData.opciones[i])) {
                botones[i].gameObject.SetActive(true);
                textosBotones[i].text = dialogoData.opciones[i];
                int idx = i;
                botones[i].onClick.RemoveAllListeners();
                botones[i].onClick.AddListener(() => Seleccionar(idx));
            } else {
                botones[i].gameObject.SetActive(false);
            }
        }
    }

    void Seleccionar(int idx) {
        if(escribiendo) return;
        groupOpciones.SetActive(false);
        
        string reaccion = dialogoData.reacciones[idx];

        // LOGICA DE RECURSOS
        if (dialogoData.esVenta && idx == 0) {
            if (npcActual.esCliente) {
                if (stats.churrosCantidad > 0) {
                    AplicarConsecuencia(idx);
                    npcActual.FinalizarVenta();
                } else {
                    reaccion = "¡No tenes churros! No me hagas perder el tiempo.";
                }
            } else if (npcActual.esVendedor) {
                if (stats.money >= npcActual.precioBaseAgua) {
                    AplicarConsecuencia(idx);
                } else {
                    reaccion = "No tenes suficiente plata, pibe.";
                }
            }
        } else {
            AplicarConsecuencia(idx);
        }

        StartCoroutine(ReaccionFinal(reaccion));
    }

    void AplicarConsecuencia(int idx) {
        if(stats == null || dialogoData.impactos == null || idx >= dialogoData.impactos.Length) return;

        Consecuencia c = dialogoData.impactos[idx];
        
        float mod = 1f;
        if(stats.ebriedad > 40 && stats.ebriedad < 75) mod = 1.4f;
        else if(stats.ebriedad >= 85) mod = 0.5f;

        // Si es venta de churros (cliente), el dinero del impacto se multiplica por ebriedad
        float dineroFinal = c.dinero;
        if (npcActual != null && npcActual.esCliente && c.dinero > 0) dineroFinal *= mod;

        stats.AgregarDinero(dineroFinal);
        stats.RecuperarStamina(c.stamina);
        stats.RecuperarHidratacion(c.hidratacion);
        
        // Manejo de churros
        if (c.churros < 0) {
            for(int i=0; i < Mathf.Abs(c.churros); i++) stats.ConsumirChurro();
        } else {
            stats.AgregarChurros(c.churros);
        }
    }

    IEnumerator ReaccionFinal(string r) {
        yield return StartCoroutine(EscribirLetras(r));
        yield return new WaitForSeconds(1.5f);
        Cerrar();
    }

    public void Cerrar() {
        panelUI.SetActive(false);
        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = true;
    }
}
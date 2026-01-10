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
    public GameObject groupOpciones;
    public Button[] botones;
    public TextMeshProUGUI[] textosBotones;

    [Header("Configuracion")]
    public float typingSpeed = 0.03f;

    private NPCConversacion npcActual;
    private Dialogo dialogoActual;
    private PlayerStats stats;
    private bool escribiendo = false;

    void Awake() { Instance = this; }

    void Start() {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p) stats = p.GetComponent<PlayerStats>();
        if(panelUI) panelUI.SetActive(false);
    }

    public void AbrirPanel(NPCConversacion npc) {
        if (npc == null || stats == null) return;
        npcActual = npc;
        
        dialogoActual = npcActual.ObtenerDialogoDinamico();
        if (dialogoActual == null) return;

        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = false;

        panelUI.SetActive(true);
        groupOpciones.SetActive(false);
        
        // El nombre puede venir del archivo de dialogo o del NPC
        textoNombre.text = !string.IsNullOrEmpty(dialogoActual.nombreNPC) ? dialogoActual.nombreNPC : npcActual.nombre;
        
        if (npcActual.esVendedorBebidas) StartCoroutine(MenuBebidas());
        else StartCoroutine(CicloDeDialogo());
    }

    IEnumerator CicloDeDialogo() {
        yield return StartCoroutine(EscribirLetras(dialogoActual.propuesta));
        
        if (dialogoActual.esGrito) {
            yield return new WaitForSeconds(1.5f);
            Cerrar();
        } else {
            MostrarOpciones();
        }
    }

    void MostrarOpciones() {
        groupOpciones.SetActive(true);
        foreach(var b in botones) b.gameObject.SetActive(false);

        for(int i=0; i < dialogoActual.respuestas.Length; i++) {
            if(i >= botones.Length) break;
            
            RamaDialogo rama = dialogoActual.respuestas[i];
            if(!string.IsNullOrEmpty(rama.textoOpcion)) {
                int index = i;
                botones[i].gameObject.SetActive(true);
                textosBotones[i].text = rama.textoOpcion;
                botones[i].onClick.RemoveAllListeners();
                botones[i].onClick.AddListener(() => SeleccionarOpcion(index));
            }
        }
    }

    void SeleccionarOpcion(int idx) {
        if(escribiendo) return;
        groupOpciones.SetActive(false);

        RamaDialogo ramaElegida = dialogoActual.respuestas[idx];

        // ¿Hay una ramificacion?
        if (ramaElegida.siguienteDialogo != null) {
            dialogoActual = ramaElegida.siguienteDialogo;
            StartCoroutine(CicloDeDialogo());
            return;
        }

        // Si es el final, aplicamos el impacto (dinero, churros, etc)
        ProcesarImpacto(ramaElegida.impacto);
        
        // Si el dialogo estaba marcado como venta, marcamos al NPC como "ya vendido"
        if (dialogoActual.esVenta) npcActual.FinalizarVenta();

        // Mostramos la reaccion final que escribiste y cerramos
        StartCoroutine(ReaccionFinal(ramaElegida.reaccionSiTermina));
    }

    void ProcesarImpacto(Consecuencia c) {
        // Solo sumamos dinero si tenemos los churros suficientes para el trato (si el trato pide quitar churros)
        if (c.churros < 0 && stats.churrosCantidad < Mathf.Abs(c.churros)) {
            Debug.Log("[Dialogo] No hay churros para cumplir el trato de este dialogo.");
            return; 
        }

        stats.AgregarDinero(c.dinero); 
        stats.RecuperarStamina(c.stamina);
        stats.RecuperarHidratacion(c.hidratacion);
        
        if (c.churros < 0) {
            for(int i=0; i<Mathf.Abs(c.churros); i++) stats.ConsumirChurro();
        } else {
            stats.AgregarChurros(c.churros);
        }
    }

    IEnumerator EscribirLetras(string frase) {
        escribiendo = true;
        textoCuerpo.text = "";
        foreach(char c in frase) {
            textoCuerpo.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        escribiendo = false;
    }

    IEnumerator ReaccionFinal(string r) {
        if(string.IsNullOrEmpty(r)) r = "...";
        yield return StartCoroutine(EscribirLetras(r));
        yield return new WaitForSeconds(1.2f);
        Cerrar();
    }

    public void Cerrar() {
        panelUI.SetActive(false);
        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = true;
    }

    // Bebidas
    IEnumerator MenuBebidas() {
        yield return StartCoroutine(EscribirLetras("¿Qué vas a llevar fresco?"));
        groupOpciones.SetActive(true);
        foreach(var b in botones) b.gameObject.SetActive(false);
        ConfigurarBotonBebida(0, "Agua ($8)", () => { if(stats.GastarDinero(8)){ stats.RecuperarHidratacion(30); Cerrar();} });
        ConfigurarBotonBebida(1, "Birra ($15)", () => { if(stats.GastarDinero(15)){ stats.ebriedad += 25; Cerrar();} });
        ConfigurarBotonBebida(2, "Gaseosa ($12)", () => { if(stats.GastarDinero(12)){ stats.RecuperarHidratacion(15); Cerrar();} });
    }

    void ConfigurarBotonBebida(int i, string txt, UnityEngine.Events.UnityAction accion) {
        botones[i].gameObject.SetActive(true);
        textosBotones[i].text = txt;
        botones[i].onClick.RemoveAllListeners();
        botones[i].onClick.AddListener(accion);
    }
}
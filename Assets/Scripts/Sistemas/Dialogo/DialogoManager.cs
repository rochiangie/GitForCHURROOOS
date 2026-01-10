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
        
        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = false;

        Time.timeScale = 0f; // Pausar tiempo durante charla
        panelUI.SetActive(true);
        groupOpciones.SetActive(false);
        
        if (npcActual.esVendedorBebidas) {
            textoNombre.text = npcActual.nombre;
            StartCoroutine(MenuBebidas());
        } else {
            dialogoActual = npcActual.ObtenerDialogoDinamico();
            if (dialogoActual == null) {
                Cerrar();
                return;
            }
            textoNombre.text = !string.IsNullOrEmpty(dialogoActual.nombreNPC) ? dialogoActual.nombreNPC : npcActual.nombre;
            StartCoroutine(CicloDeDialogo());
        }
    }

    IEnumerator CicloDeDialogo() {
        yield return StartCoroutine(EscribirLetras(dialogoActual.propuesta));
        
        if (dialogoActual.esGrito) {
            yield return new WaitForSecondsRealtime(1.5f);
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

        // Validar stock antes de procesar
        if (ramaElegida.impacto.churros < 0 && stats.churrosCantidad < Mathf.Abs(ramaElegida.impacto.churros)) {
            string msjError = "¡Uy! Solo tengo " + stats.churrosCantidad + " churros...";
            StartCoroutine(ReaccionFinal(msjError));
            return;
        }

        // Si es el final, aplicamos el impacto (dinero, churros, etc)
        ProcesarImpacto(ramaElegida.impacto);
        
        // Solo marcamos como "vendido" si la rama elegida realmente involucró entregar churros
        if (dialogoActual.esVenta && ramaElegida.impacto.churros < 0) {
            npcActual.FinalizarVenta();
        }

        // Mostramos la reaccion final que escribiste y cerramos
        StartCoroutine(ReaccionFinal(ramaElegida.reaccionSiTermina));
    }

    bool ProcesarImpacto(Consecuencia c) {
        if (c.churros < 0 && stats.churrosCantidad < Mathf.Abs(c.churros)) {
            return false; 
        }

        stats.AgregarDinero(c.dinero); 
        stats.RecuperarStamina(c.stamina);
        stats.RecuperarHidratacion(c.hidratacion);
        
        if (c.churros < 0) {
            for(int i=0; i<Mathf.Abs(c.churros); i++) stats.ConsumirChurro();
        } else {
            stats.AgregarChurros(c.churros);
        }
        return true;
    }

    IEnumerator EscribirLetras(string frase) {
        escribiendo = true;
        textoCuerpo.text = "";
        foreach(char c in frase) {
            textoCuerpo.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        escribiendo = false;
    }

    IEnumerator ReaccionFinal(string r) {
        if(string.IsNullOrEmpty(r)) r = "...";
        yield return StartCoroutine(EscribirLetras(r));
        yield return new WaitForSecondsRealtime(1.2f);
        Cerrar();
    }

    public void Cerrar() {
        Time.timeScale = 1f; // Reanudar tiempo
        panelUI.SetActive(false);
        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = true;
    }

    // Bebidas
    IEnumerator MenuBebidas() {
        yield return StartCoroutine(EscribirLetras("¿Qué vas a llevar fresco?"));
        groupOpciones.SetActive(true);
        foreach(var b in botones) b.gameObject.SetActive(false);
        
        ConfigurarBotonBebida(0, "Agua ($3)", () => { 
            if (stats.money >= 3) {
                stats.GastarDinero(3);
                stats.RecuperarHidratacion(40); 
                stats.ReducirTemperatura(15);
                Cerrar();
            } else {
                StartCoroutine(ReaccionFinal("¡No te alcanza, pibe! Son $3."));
            }
        });

        ConfigurarBotonBebida(1, "Birra ($5)", () => { 
            if (stats.money >= 5) {
                stats.GastarDinero(5);
                stats.ebriedad += 20; 
                stats.RecuperarHidratacion(15); 
                Cerrar();
            } else {
                StartCoroutine(ReaccionFinal("¡Faltan monedas! La birra sale $5."));
            }
        });
    }

    void ConfigurarBotonBebida(int i, string txt, UnityEngine.Events.UnityAction accion) {
        botones[i].gameObject.SetActive(true);
        textosBotones[i].text = txt;
        botones[i].onClick.RemoveAllListeners();
        botones[i].onClick.AddListener(accion);
    }
}
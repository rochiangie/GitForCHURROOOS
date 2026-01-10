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

    [Header("Ajustes")]
    public float typingSpeed = 0.04f;

    private NPCConversacion npcActual;
    private Dialogo dialogoData;
    private PlayerStats stats;
    private bool escribiendo = false;

    void Awake() { Instance = this; }

    void Start() {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p) stats = p.GetComponent<PlayerStats>();
        if(panelUI) panelUI.SetActive(false);
    }

    public void AbrirPanel(NPCConversacion npc) {
        if (npc == null) return;
        npcActual = npc;
        dialogoData = npc.ObtenerDialogoDinamico();
        
        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = false;

        panelUI.SetActive(true);
        groupOpciones.SetActive(false);
        textoNombre.text = npcActual.nombre;
        
        if (npcActual.esVendedorBebidas) StartCoroutine(MenuBebidas());
        else StartCoroutine(EscribirFrase(dialogoData.propuesta));
    }

    IEnumerator EscribirFrase(string frase) {
        escribiendo = true;
        textoCuerpo.text = "";
        float speed = (npcActual != null && npcActual.personalidad == PersonalidadNPC.Molesto) ? 0.02f : typingSpeed;
        foreach(char c in frase) {
            textoCuerpo.text += c;
            yield return new WaitForSeconds(speed);
        }
        escribiendo = false;
        
        // Si no es un grito, mostramos opciones despues de escribir
        if (dialogoData != null && !dialogoData.esGrito && !escribiendo) {
             // Solo mostramos si no estamos en fase de transaccion manual
        }
    }

    // --- FLUJO DE BEBIDAS ---
    IEnumerator MenuBebidas() {
        yield return StartCoroutine(EscribirFrase("¿Qué vas a llevar fresco?"));
        groupOpciones.SetActive(true);
        ConfigurarBoton(0, "Agua ($3)", () => TratoBebida(3, 30, 0, "¡Ahi tenes!"));
        ConfigurarBoton(1, "Birra ($5)", () => TratoBebida(5, 15, 20, "Cuidado con el sol."));
        ConfigurarBoton(2, "Gaseosa ($4)", () => TratoBebida(4, 20, 0, "¡Refrescante!"));
    }

    void TratoBebida(float precio, float hydration, float ebriedad, string r) {
        groupOpciones.SetActive(false);
        if (stats.GastarDinero(precio)) {
            stats.RecuperarHidratacion(hydration);
            stats.ebriedad += ebriedad;
            StartCoroutine(ReaccionFinal(r));
        } else {
            StartCoroutine(ReaccionFinal("No te alcanza la plata."));
        }
    }

    // --- FLUJO DE DIALOGO NORMAL ---
    void MostrarOpciones() {
        groupOpciones.SetActive(true);
        for(int i=0; i<3; i++) {
            if(i < dialogoData.opciones.Length && !string.IsNullOrEmpty(dialogoData.opciones[i])) {
                int index = i;
                ConfigurarBoton(i, dialogoData.opciones[i], () => SeleccionarOpcion(index));
            } else botones[i].gameObject.SetActive(false);
        }
    }

    void SeleccionarOpcion(int idx) {
        if(escribiendo) return;
        groupOpciones.SetActive(false);
        
        // Fase 2: Negociacion de Venta
        if (dialogoData.esVenta && idx == 0 && npcActual.esCliente) {
            StartCoroutine(FaseNegociacion());
            return;
        }

        // Reaccion normal
        ProcesarImpacto(idx);
        StartCoroutine(ReaccionFinal(dialogoData.reacciones[idx]));
    }

    IEnumerator FaseNegociacion() {
        float precioBase = npcActual.pagoBaseChurro != 0 ? npcActual.pagoBaseChurro : 120f;
        float precioFinal = precioBase * npcActual.churrosDeseados;
        
        // Ajuste por personalidad y ebriedad
        if (npcActual.personalidad == PersonalidadNPC.Amable) precioFinal *= 1.2f;
        if (npcActual.personalidad == PersonalidadNPC.Molesto) precioFinal *= 0.8f;
        
        if (stats.ebriedad > 40 && stats.ebriedad < 75) precioFinal *= 1.5f; // Chamullo exitoso
        else if (stats.ebriedad >= 85) precioFinal *= 0.5f; // Te ven la cara de borracho

        yield return StartCoroutine(EscribirFrase($"Quiero {npcActual.churrosDeseados} churros. Te doy ${precioFinal:F0} por todo."));
        
        groupOpciones.SetActive(true);
        ConfigurarBoton(0, "¡Trato hecho!", () => FinalizarTrato(true, precioFinal));
        ConfigurarBoton(1, "Ni loco, muy barato.", () => FinalizarTrato(false, 0));
        botones[2].gameObject.SetActive(false);
    }

    void FinalizarTrato(bool aceptado, float monto) {
        groupOpciones.SetActive(false);
        if (aceptado) {
            if (stats.churrosCantidad >= npcActual.churrosDeseados) {
                stats.AgregarDinero(monto);
                for(int i=0; i<npcActual.churrosDeseados; i++) stats.ConsumirChurro();
                npcActual.FinalizarVenta();
                StartCoroutine(ReaccionFinal("¡Buenisimo! Estan calentitos."));
            } else {
                StartCoroutine(ReaccionFinal("¡Me mentiste! No tenes suficientes churros."));
            }
        } else {
            StartCoroutine(ReaccionFinal("Y bueno, busco a otro."));
        }
    }

    void ProcesarImpacto(int idx) {
        Consecuencia c = dialogoData.impactos[idx];
        stats.AgregarDinero(c.dinero); 
        stats.RecuperarStamina(c.stamina);
        stats.RecuperarHidratacion(c.hidratacion);
        if (c.churros < 0) for(int i=0; i<Mathf.Abs(c.churros); i++) stats.ConsumirChurro();
        else stats.AgregarChurros(c.churros);
    }

    IEnumerator ReaccionFinal(string r) {
        yield return StartCoroutine(EscribirFrase(r));
        yield return new WaitForSeconds(1.5f);
        Cerrar();
    }

    void ConfigurarBoton(int i, string txt, UnityEngine.Events.UnityAction accion) {
        botones[i].gameObject.SetActive(true);
        textosBotones[i].text = txt;
        botones[i].onClick.RemoveAllListeners();
        botones[i].onClick.AddListener(accion);
    }

    public void Cerrar() {
        panelUI.SetActive(false);
        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = true;
    }

    // Sobrecarga de EscribirFrase que tambien muestra opciones al final para no repetir codigo
    IEnumerator EscribirFraseConOpciones(string frase) {
        yield return StartCoroutine(EscribirFrase(frase));
        if (dialogoData != null && !dialogoData.esGrito) MostrarOpciones();
    }
}
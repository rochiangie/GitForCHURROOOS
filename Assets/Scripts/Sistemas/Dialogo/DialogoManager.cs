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
        else StartCoroutine(EscribirDialogoInicial());
    }

    IEnumerator EscribirDialogoInicial() {
        yield return StartCoroutine(EscribirLetras(dialogoData.propuesta));
        
        if (dialogoData.esGrito) {
            yield return new WaitForSeconds(1.5f);
            Cerrar();
        } else {
            MostrarOpcionesPrincipales();
        }
    }

    IEnumerator EscribirLetras(string frase) {
        escribiendo = true;
        textoCuerpo.text = "";
        float speed = (npcActual != null && npcActual.personalidad == PersonalidadNPC.Molesto) ? 0.02f : typingSpeed;
        foreach(char c in frase) {
            textoCuerpo.text += c;
            yield return new WaitForSeconds(speed);
        }
        escribiendo = false;
    }

    // --- FLUJO DE BEBIDAS ---
    IEnumerator MenuBebidas() {
        yield return StartCoroutine(EscribirLetras("¿Qué vas a llevar fresco?"));
        groupOpciones.SetActive(true);
        ConfigurarBoton(0, "Agua ($8)", () => TratoBebida(8, 30, 0, "¡Ahi tenes!"));
        ConfigurarBoton(1, "Birra ($15)", () => TratoBebida(15, 15, 20, "Cuidado con el sol."));
        ConfigurarBoton(2, "Gaseosa ($12)", () => TratoBebida(12, 20, 0, "¡Refrescante!"));
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
    void MostrarOpcionesPrincipales() {
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
        
        // Fase 2: Negociacion de Venta (Si es cliente y es venta)
        if (dialogoData.esVenta && idx == 0 && npcActual != null && npcActual.esCliente) {
            if (stats.churrosCantidad > 0) {
                StartCoroutine(FaseNegociacion());
            } else {
                StartCoroutine(ReaccionFinal("¡No tenes churros, chanta! No me hagas perder el tiempo."));
            }
            return;
        }

        // Reaccion normal (Verificando si podemos procesar el impacto)
        if (ProcesarImpacto(idx)) {
            StartCoroutine(ReaccionFinal(dialogoData.reacciones[idx]));
        } else {
            StartCoroutine(ReaccionFinal("¡No tengo suficientes churros para darte!"));
        }
    }

    IEnumerator FaseNegociacion() {
        // ... (Cálculo de precio omitido por brevedad pero se mantiene igual)
        float precioBase = npcActual.pagoBaseChurro != 0 ? npcActual.pagoBaseChurro : 20f;
        float precioFinal = precioBase * npcActual.churrosDeseados;
        
        if (npcActual.personalidad == PersonalidadNPC.Amable) precioFinal *= 1.2f;
        if (npcActual.personalidad == PersonalidadNPC.Molesto) precioFinal *= 0.8f;
        if (stats.ebriedad > 40 && stats.ebriedad < 75) precioFinal *= 1.5f; 
        else if (stats.ebriedad >= 85) precioFinal *= 0.5f; 

        yield return StartCoroutine(EscribirLetras($"Quiero {npcActual.churrosDeseados} churros. Te doy ${precioFinal:F0} por todo."));
        
        groupOpciones.SetActive(true);
        ConfigurarBoton(0, "¡Trato hecho!", () => FinalizarTrato(true, precioFinal));
        ConfigurarBoton(1, "No, gracias.", () => FinalizarTrato(false, 0));
        botones[2].gameObject.SetActive(false);
    }

    void FinalizarTrato(bool aceptado, float monto) {
        groupOpciones.SetActive(false);
        if (aceptado) {
            // VERIFICACION CRITICA: ¿Tiene los churros que pidió el cliente?
            if (stats.churrosCantidad >= npcActual.churrosDeseados) {
                stats.AgregarDinero(monto);
                for(int i=0; i<npcActual.churrosDeseados; i++) stats.ConsumirChurro();
                npcActual.FinalizarVenta();
                StartCoroutine(ReaccionFinal("¡Buenisimo! Estan calentitos."));
            } else {
                StartCoroutine(ReaccionFinal("¡Pero pibe, no tenes los churros que me dijiste!"));
            }
        } else {
            StartCoroutine(ReaccionFinal("Y bueno, busco a otro."));
        }
    }

    bool ProcesarImpacto(int idx) {
        if (dialogoData == null || idx >= dialogoData.impactos.Length) return false;
        Consecuencia c = dialogoData.impactos[idx];

        // Si la consecuencia pide quitar churros (valor negativo), verificamos stock
        if (c.churros < 0 && stats.churrosCantidad < Mathf.Abs(c.churros)) {
            return false;
        }

        // Ejecutar impacto
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

    IEnumerator ReaccionFinal(string r) {
        yield return StartCoroutine(EscribirLetras(r));
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
}
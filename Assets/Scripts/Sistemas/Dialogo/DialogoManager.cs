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
    public float typingSpeed = 0.03f;

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
        if (npc == null || stats == null) return;
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

    // --- FLUJO DE VENTAS / NEGOCIACION ---
    void SeleccionarOpcion(int idx) {
        if(escribiendo) return;
        groupOpciones.SetActive(false);
        
        // Si es una conversación de venta y elegimos la opción positiva (0 o 1)
        if (dialogoData.esVenta && idx < 2 && npcActual.esCliente) {
            StartCoroutine(PreguntarCantidad());
            return;
        }

        // Conversación normal o negativa (idx 2)
        ProcesarImpacto(idx);
        StartCoroutine(ReaccionFinal(dialogoData.reacciones[idx]));
    }

    IEnumerator PreguntarCantidad() {
        yield return StartCoroutine(EscribirLetras("¿Y cuántos vas a querer hoy?"));
        
        groupOpciones.SetActive(true);
        int d1 = npcActual.churrosDeseados;
        int d2 = Mathf.Max(1, d1 / 2);
        
        ConfigurarBoton(0, $"Dame {d1} churros.", () => IniciarTrato(d1));
        ConfigurarBoton(1, $"Solo {d2}, por ahora.", () => IniciarTrato(d2));
        ConfigurarBoton(2, "Mejor nada, gracias.", () => StartCoroutine(ReaccionFinal("Uh, bueno... avisame si cambias de idea.")));
    }

    void IniciarTrato(int cantidad) {
        groupOpciones.SetActive(false);
        
        // Calculo de precio realista: $20 basico, ajustado por personalidad
        float precioIndividual = 20f;
        if (npcActual.personalidad == PersonalidadNPC.Amable) precioIndividual = 25f;
        if (npcActual.personalidad == PersonalidadNPC.Molesto) precioIndividual = 18f;
        
        // Bonus por "Chamullo" (Ebriedad media)
        if (stats.ebriedad > 30 && stats.ebriedad < 70) precioIndividual *= 1.4f;

        float total = precioIndividual * cantidad;
        StartCoroutine(FinalizarVentaNarrativa(cantidad, total));
    }

    IEnumerator FinalizarVentaNarrativa(int cant, float monto) {
        yield return StartCoroutine(EscribirLetras($"Dale, te pago ${monto:F0} por los {cant}."));
        yield return new WaitForSeconds(0.5f);

        if (stats.churrosCantidad >= cant) {
            stats.AgregarDinero(monto);
            for(int i=0; i<cant; i++) stats.ConsumirChurro();
            npcActual.FinalizarVenta();
            StartCoroutine(ReaccionFinal("¡Buenisimo! Están riquísimos."));
        } else {
            StartCoroutine(ReaccionFinal("¡Me mentiste! No tenés churros suficientes."));
        }
    }

    // --- BEBIDAS Y UTILIDADES ---
    IEnumerator MenuBebidas() {
        yield return StartCoroutine(EscribirLetras("¿Qué vas a llevar fresco?"));
        groupOpciones.SetActive(true);
        ConfigurarBoton(0, "Agua ($8)", () => TratoBebida(8, 30, 0, "¡Ahi tienes!"));
        ConfigurarBoton(1, "Birra ($15)", () => TratoBebida(15, 10, 25, "¡Salud!"));
        ConfigurarBoton(2, "Gaseosa ($12)", () => TratoBebida(12, 20, 0, "¡Fresquita!"));
    }

    void TratoBebida(float precio, float hydration, float ebriedad, string r) {
        groupOpciones.SetActive(false);
        if (stats.GastarDinero(precio)) {
            stats.RecuperarHidratacion(hydration);
            stats.ebriedad += ebriedad;
            StartCoroutine(ReaccionFinal(r));
        } else {
            StartCoroutine(ReaccionFinal("No te alcanza la plata, pibe."));
        }
    }

    void ProcesarImpacto(int idx) {
        if (dialogoData == null || idx >= dialogoData.impactos.Length) return;
        Consecuencia c = dialogoData.impactos[idx];
        stats.AgregarDinero(c.dinero); 
        stats.RecuperarStamina(c.stamina);
        stats.RecuperarHidratacion(c.hidratacion);
        if (c.churros < 0) {
            for(int i=0; i<Mathf.Abs(c.churros); i++) stats.ConsumirChurro();
        } else stats.AgregarChurros(c.churros);
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

    void MostrarOpcionesPrincipales() {
        groupOpciones.SetActive(true);
        for(int i=0; i<3; i++) {
            if(i < dialogoData.opciones.Length && !string.IsNullOrEmpty(dialogoData.opciones[i])) {
                int index = i;
                ConfigurarBoton(i, dialogoData.opciones[i], () => SeleccionarOpcion(index));
            } else botones[i].gameObject.SetActive(false);
        }
    }

    void ConfigurarBoton(int i, string txt, UnityEngine.Events.UnityAction accion) {
        botones[i].gameObject.SetActive(true);
        textosBotones[i].text = txt;
        botones[i].onClick.RemoveAllListeners();
        botones[i].onClick.AddListener(accion);
    }

    IEnumerator ReaccionFinal(string r) {
        yield return StartCoroutine(EscribirLetras(r));
        yield return new WaitForSeconds(1.2f);
        Cerrar();
    }

    public void Cerrar() {
        panelUI.SetActive(false);
        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = true;
    }
}
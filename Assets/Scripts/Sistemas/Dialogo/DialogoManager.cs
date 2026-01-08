using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance { get; private set; }

    [Header("UI Componentes")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoContenido;
    public GameObject contenedorOpciones;
    public Button[] botonesOpciones;
    public TextMeshProUGUI[] textosBotones;

    [Header("Parametros")]
    public float typingSpeed = 0.035f;

    private NPCConversacion npcActual;
    private Dialogo dialogoActual;
    private PlayerStats stats;
    private PlayerMovement movement;
    private bool estaEscribiendo = false;

    void Awake() { Instance = this; }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if(p) { stats = p.GetComponent<PlayerStats>(); movement = p.GetComponent<PlayerMovement>(); }
        panelDialogo.SetActive(false);
    }

    public void AbrirPanel(NPCConversacion npc)
    {
        npcActual = npc;
        // Elegimos un dialogo del pool que sea acorde a si es venta o no
        dialogoActual = npc.poolDialogos.Count > 0 ? npc.poolDialogos[Random.Range(0, npc.poolDialogos.Count)] : null;
        
        if (dialogoActual == null) {
            Debug.LogWarning("El NPC " + npc.nombre + " no tiene dialogos asignados.");
            return;
        }

        if(movement) movement.enabled = false;
        panelDialogo.SetActive(true);
        contenedorOpciones.SetActive(false);
        textoNombre.text = npcActual.nombre;

        StartCoroutine(EscribirDialogo(dialogoActual.propuesta, true));
    }

    IEnumerator EscribirDialogo(string lines, bool permitirOpciones)
    {
        estaEscribiendo = true;
        textoContenido.text = "";
        foreach (char c in lines)
        {
            textoContenido.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        estaEscribiendo = false;

        if (permitirOpciones && !dialogoActual.esGrito) MostrarOpcionesUI();
        else if (dialogoActual.esGrito) { yield return new WaitForSeconds(2f); CerrarPanel(); }
    }

    void MostrarOpcionesUI()
    {
        contenedorOpciones.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            if (i < dialogoActual.opciones.Length && !string.IsNullOrEmpty(dialogoActual.opciones[i]))
            {
                botonesOpciones[i].gameObject.SetActive(true);
                textosBotones[i].text = dialogoActual.opciones[i];
                int index = i;
                botonesOpciones[i].onClick.RemoveAllListeners();
                botonesOpciones[i].onClick.AddListener(() => SeleccionarRespuesta(index));
            }
            else { botonesOpciones[i].gameObject.SetActive(false); }
        }
    }

    public void SeleccionarRespuesta(int index)
    {
        if (estaEscribiendo) return;
        contenedorOpciones.SetActive(false);

        string reaccionFinal = dialogoActual.reacciones[index];

        // LÓGICA DE TRANSACCIÓN REAL
        if (index == 0 && dialogoActual.esVenta) // Opcion 0 es intentar el trato
        {
            if (npcActual.esCliente)
            {
                if (stats.churrosCantidad > 0) {
                    float pago = CalcularPagoCliente();
                    stats.AgregarDinero(pago);
                    stats.churrosCantidad--;
                    npcActual.FinalizarVenta();
                    reaccionFinal = "¡Excelente! Estos churros huelen increible.";
                } else {
                    reaccionFinal = "¿Me estas cargando? ¡No tenes churros! No me hagas perder el tiempo.";
                }
            }
            else if (npcActual.esVendedor)
            {
                if (stats.GastarDinero(npcActual.precioBaseAgua)) {
                    stats.RecuperarHidratacion(npcActual.recuperacionHidratacion);
                    reaccionFinal = "Aca tenes, pibe. Cuidate del sol.";
                } else {
                    reaccionFinal = "Aca no fiamos. Volve cuando tengas la plata.";
                }
            }
        }

        StartCoroutine(MostrarReaccion(reaccionFinal));
    }

    float CalcularPagoCliente()
    {
        float pago = npcActual.pagoBaseChurro;
        // Ebriedad
        if (stats.ebriedad > 40 && stats.ebriedad < 75) pago *= 1.4f;
        else if (stats.ebriedad > 85) pago *= 0.6f;

        // Personalidad
        if (npcActual.personalidad == PersonalidadNPC.Amable) pago += 25;
        else if (npcActual.personalidad == PersonalidadNPC.Molesto) pago -= 30;

        return pago;
    }

    IEnumerator MostrarReaccion(string reaccion)
    {
        yield return StartCoroutine(EscribirDialogo(reaccion, false));
        yield return new WaitForSeconds(1.8f);
        CerrarPanel();
    }

    public void CerrarPanel()
    {
        panelDialogo.SetActive(false);
        if(movement) movement.enabled = true;
    }
}
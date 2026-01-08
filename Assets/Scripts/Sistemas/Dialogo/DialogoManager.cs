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
    public float typingSpeed = 0.04f;

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
        dialogoActual = npc.poolDialogos.Count > 0 ? npc.poolDialogos[Random.Range(0, npc.poolDialogos.Count)] : null;
        
        if (dialogoActual == null) return;

        if(movement) movement.enabled = false;
        panelDialogo.SetActive(true);
        contenedorOpciones.SetActive(false);
        textoNombre.text = npcActual.nombre;

        StartCoroutine(EscribirDialogo(dialogoActual.propuesta, !dialogoActual.esGrito));
    }

    IEnumerator EscribirDialogo(string lines, bool mostrarOpciones)
    {
        estaEscribiendo = true;
        textoContenido.text = "";
        foreach (char c in lines)
        {
            textoContenido.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        estaEscribiendo = false;

        if (mostrarOpciones) 
        {
            MostrarOpcionesUI();
        }
        else 
        {
            // Si era un grito, esperamos y cerramos
            yield return new WaitForSeconds(2f);
            CerrarPanel();
        }
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

        // LOGICA DE NEGOCIACION BASADA EN EBRIEDAD Y PERSONALIDAD
        CalcularResultado(index);

        StartCoroutine(MostrarReaccion(dialogoActual.reacciones[index]));
    }

    void CalcularResultado(int index)
    {
        if (stats == null) return;

        // Opcion [0] suele ser la mejor (Amable/Vender/Comprar)
        if (index == 0 && dialogoActual.esVenta)
        {
            if (npcActual.esCliente && stats.churrosCantidad > 0)
            {
                float pago = npcActual.pagoBaseChurro;
                
                // EBRIEDAD (40-75: Gana mas por simpatico, 85+: Pierde por borracho)
                if (stats.ebriedad > 40 && stats.ebriedad < 75) pago *= 1.4f;
                else if (stats.ebriedad > 85) pago *= 0.5f;

                // PERSONALIDAD (Amable paga mas, Molesto paga menos)
                if (npcActual.personalidad == PersonalidadNPC.Amable) pago += 20;
                else if (npcActual.personalidad == PersonalidadNPC.Molesto) pago -= 30;

                stats.AgregarDinero(pago);
                stats.churrosCantidad--;
                npcActual.FinalizarVenta();
            }
            else if (npcActual.esVendedor)
            {
                if (stats.GastarDinero(npcActual.precioBaseAgua))
                    stats.RecuperarHidratacion(npcActual.recuperacionHidratacion);
            }
        }
    }

    IEnumerator MostrarReaccion(string reaccion)
    {
        yield return StartCoroutine(EscribirDialogo(reaccion, false));
        yield return new WaitForSeconds(1.5f);
        CerrarPanel();
    }

    public void CerrarPanel()
    {
        panelDialogo.SetActive(false);
        if(movement) movement.enabled = true;
    }
}
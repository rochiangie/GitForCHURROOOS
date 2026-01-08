using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance { get; private set; }

    [Header("UI General")]
    public GameObject panelUI;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoContenido;

    [Header("Opciones de Jugador")]
    public GameObject contenedorBotones;
    public Button[] botonesRespuesta = new Button[3];
    public TextMeshProUGUI[] textosRespuestas = new TextMeshProUGUI[3];

    [Header("Ajustes de Flujo")]
    public float velocidadEscritura = 0.03f;

    private NPCConversacion npcActual;
    private Dialogo dialogoData;
    private PlayerStats stats;
    private PlayerMovement pMovement;
    private Coroutine corrutinaEscritura;
    
    void Awake() { Instance = this; }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) {
            stats = p.GetComponent<PlayerStats>();
            pMovement = p.GetComponent<PlayerMovement>();
        }
        if (panelUI != null) panelUI.SetActive(false);
    }

    public void AbrirPanel(NPCConversacion npc)
    {
        if (npc == null || npc.archivoDialogo == null) return;
        
        npcActual = npc;
        dialogoData = npc.archivoDialogo;

        if (pMovement) pMovement.enabled = false;

        if (panelUI != null) panelUI.SetActive(true);
        if (contenedorBotones != null) contenedorBotones.SetActive(false);
        if (textoNombre != null) textoNombre.text = dialogoData.nombreNPC;

        if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
        corrutinaEscritura = StartCoroutine(EscribirTexto(dialogoData.propuesta, true));
    }

    IEnumerator EscribirTexto(string texto, bool mostrarOpciones)
    {
        if (textoContenido == null) yield break;
        
        textoContenido.text = "";
        foreach (char c in texto.ToCharArray())
        {
            textoContenido.text += c;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        if (mostrarOpciones) ConfigurarOpciones();
    }

    void ConfigurarOpciones()
    {
        if (contenedorBotones != null) contenedorBotones.SetActive(true);
        
        for (int i = 0; i < 3; i++)
        {
            // Seguridad: Verificamos que el boton y el texto existan en el inspector
            if (i < botonesRespuesta.Length && botonesRespuesta[i] != null)
            {
                // Y que el dialogo tenga esa opcion
                if (dialogoData.opciones != null && i < dialogoData.opciones.Length)
                {
                    botonesRespuesta[i].gameObject.SetActive(true);
                    if (i < textosRespuestas.Length && textosRespuestas[i] != null)
                    {
                        textosRespuestas[i].text = dialogoData.opciones[i];
                    }
                    
                    int index = i;
                    botonesRespuesta[i].onClick.RemoveAllListeners();
                    botonesRespuesta[i].onClick.AddListener(() => SeleccionarOpcion(index));
                }
                else
                {
                    // Si no hay opcion en el ScriptableObject, ocultamos el boton
                    botonesRespuesta[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void SeleccionarOpcion(int index)
    {
        if (contenedorBotones != null) contenedorBotones.SetActive(false);
        
        ProcesarConsecuencia(index);

        if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
        
        string reaccion = " (Sin reaccion) ";
        if (dialogoData.reacciones != null && index < dialogoData.reacciones.Length)
        {
            reaccion = dialogoData.reacciones[index];
        }
        
        corrutinaEscritura = StartCoroutine(MostrarReaccionFinal(reaccion));
    }

    void ProcesarConsecuencia(int index)
    {
        if (stats == null || npcActual == null) return;

        // Opcion [0] es Trato Hecho
        if (index == 0)
        {
            if (npcActual.esCliente && stats.churrosCantidad > 0)
            {
                float precioBase = 150f;
                float multiplicador = 1f;

                if (stats.ebriedad > 40 && stats.ebriedad < 75) multiplicador = 1.5f; 
                else if (stats.ebriedad >= 85) multiplicador = 0.5f; 

                stats.AgregarDinero(precioBase * multiplicador);
                stats.churrosCantidad--;
                npcActual.FinalizarVenta();
            }
            else if (npcActual.esVendedor)
            {
                if (stats.GastarDinero(npcActual.precioAgua)) {
                    stats.RecuperarHidratacion(npcActual.recuperacionHidratacion);
                }
            }
        }
    }

    IEnumerator MostrarReaccionFinal(string reaccion)
    {
        yield return StartCoroutine(EscribirTexto(reaccion, false));
        yield return new WaitForSeconds(1.5f);
        CerrarPanel();
    }

    public void CerrarPanel()
    {
        if (panelUI != null) panelUI.SetActive(false);
        if (pMovement) pMovement.enabled = true;
        npcActual = null;
    }
}
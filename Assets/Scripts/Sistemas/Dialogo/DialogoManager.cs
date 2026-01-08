using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance { get; private set; }

    [Header("UI del Dialogo")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDialogo;
    public Button botonContinuar;
    public Button botonComprar; // Para agua (Vendedores)
    public Button botonVender;   // Para churros (Clientes)
    public Button botonCerrar;

    private Dialogo dialogoActual;
    private NPCConversacion npcActual;
    private int indiceLinea = 0;
    
    private PlayerStats stats;
    private PlayerActions actions;

    void Awake() { Instance = this; }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            stats = player.GetComponent<PlayerStats>();
            actions = player.GetComponent<PlayerActions>();
        }
        if (panelDialogo != null) panelDialogo.SetActive(false);
        ConfigurarBotones();
    }

    private void ConfigurarBotones()
    {
        if (botonContinuar != null) botonContinuar.onClick.AddListener(MostrarSiguienteLinea);
        if (botonComprar != null) botonComprar.onClick.AddListener(ProcesarCompraAgua);
        if (botonVender != null) botonVender.onClick.AddListener(ProcesarVentaChurro);
        if (botonCerrar != null) botonCerrar.onClick.AddListener(CerrarDialogo);
    }

    public void AbrirPanel(NPCConversacion npc)
    {
        if (npc == null || npc.archivoDialogo == null) return;
        npcActual = npc;
        dialogoActual = npc.archivoDialogo;
        indiceLinea = 0;

        panelDialogo.SetActive(true);
        textoNombre.text = dialogoActual.nombreNPC;
        
        ActualizarBotonesAccion();
        MostrarSiguienteLinea();
    }

    private void ActualizarBotonesAccion()
    {
        // Solo mostrar boton de comprar si es vendedor
        if (botonComprar != null) botonComprar.gameObject.SetActive(npcActual.esVendedor);
        
        // Solo mostrar boton de vender si es cliente y tiene hambre
        if (botonVender != null) botonVender.gameObject.SetActive(npcActual.esCliente && npcActual.quiereComprar);
    }

    public void MostrarSiguienteLinea()
    {
        if (dialogoActual == null) return;

        if (indiceLinea < dialogoActual.lineas.Length)
        {
            textoDialogo.text = dialogoActual.lineas[indiceLinea];
            indiceLinea++;
        }
    }

    private void ProcesarCompraAgua()
    {
        if (npcActual == null || stats == null) return;
        if (stats.GastarDinero(npcActual.precioAgua))
        {
            stats.RecuperarHidratacion(npcActual.recuperacionHidratacion);
            CerrarDialogo();
        }
    }

    private void ProcesarVentaChurro()
    {
        if (npcActual == null || actions == null) return;
        // Precio fijo de venta (ejemplo 100 pesos)
        if (actions.VenderChurro(100f))
        {
            npcActual.FinalizarVenta();
            CerrarDialogo();
        }
    }

    public void CerrarDialogo()
    {
        panelDialogo.SetActive(false);
    }
}
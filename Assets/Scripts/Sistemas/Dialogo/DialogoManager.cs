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
    public Button botonComprar;
    public Button botonCerrar;

    private Dialogo dialogoActual;
    private NPCConversacion npcActual;
    private int indiceLinea = 0;
    private PlayerStats stats;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            stats = player.GetComponent<PlayerStats>();
        }

        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }

        ConfigurarBotones();
    }

    private void ConfigurarBotones()
    {
        if (botonContinuar != null)
        {
            botonContinuar.onClick.AddListener(MostrarSiguienteLinea);
        }

        if (botonComprar != null)
        {
            botonComprar.onClick.AddListener(ProcesarCompra);
        }

        if (botonCerrar != null)
        {
            botonCerrar.onClick.AddListener(CerrarDialogo);
        }
    }

    // Cambiado a AbrirPanel para coincidir con NPCConversacion.cs
    public void AbrirPanel(NPCConversacion npc)
    {
        if (npc == null || npc.archivoDialogo == null) return;

        npcActual = npc;
        dialogoActual = npc.archivoDialogo;
        indiceLinea = 0;

        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
        }

        if (textoNombre != null)
        {
            textoNombre.text = dialogoActual.nombreNPC;
        }

        // Si es vendedor, mostrar botón de comprar, si es cliente (y quiere comprar) tal vez queramos otra lógica
        // Por ahora, configuramos la visibilidad según sea necesario
        if (botonComprar != null)
        {
            botonComprar.gameObject.SetActive(npc.esVendedor);
        }

        MostrarSiguienteLinea();
    }

    public void MostrarSiguienteLinea()
    {
        if (dialogoActual == null || dialogoActual.lineas == null) return;

        if (indiceLinea < dialogoActual.lineas.Length)
        {
            if (textoDialogo != null)
            {
                textoDialogo.text = dialogoActual.lineas[indiceLinea];
            }
            indiceLinea++;
        }
        else
        {
            // Si es un cliente y terminamos de "hablar", tal vez se completa la venta de churros
            if (npcActual != null && npcActual.esCliente && npcActual.quiereComprar)
            {
                // Aquí podrías llamar a una lógica de venta automática o esperar a otra acción
                // Por ahora, solo cerramos
            }
            CerrarDialogo();
        }
    }

    private void ProcesarCompra()
    {
        if (npcActual == null || stats == null) return;

        if (stats.GastarDinero(npcActual.precioAgua))
        {
            stats.RecuperarHidratacion(npcActual.recuperacionHidratacion);
            Debug.Log("Compraste agua");
            npcActual.FinalizarVenta();
            CerrarDialogo();
        }
        else
        {
            Debug.Log("No tienes suficiente dinero");
        }
    }

    public void CerrarDialogo()
    {
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }

        dialogoActual = null;
        npcActual = null;
        indiceLinea = 0;
    }
}
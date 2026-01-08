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

    public void IniciarDialogo(Dialogo dialogo, NPCConversacion npc)
    {
        dialogoActual = dialogo;
        npcActual = npc;
        indiceLinea = 0;

        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
        }

        if (textoNombre != null && dialogo != null)
        {
            textoNombre.text = dialogo.nombreNPC;
        }

        MostrarSiguienteLinea();
    }

    private void MostrarSiguienteLinea()
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
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance { get; private set; }

    [Header("UI Referencias")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoNombre;

    // Aquí separamos los dos objetos de texto
    public TextMeshProUGUI textoPropuestaCliente;
    public TextMeshProUGUI textoPropuestaVendedor;

    public Button botonCerrarX;
    public Button[] botonesRespuesta;

    private NPCConversacion npcActual;
    private PlayerStats stats;
    private PlayerActions actions;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            stats = player.GetComponent<PlayerStats>();
            actions = player.GetComponent<PlayerActions>();
        }

        panelDialogo.SetActive(false);

        if (botonCerrarX != null)
        {
            botonCerrarX.onClick.RemoveAllListeners();
            botonCerrarX.onClick.AddListener(CerrarPanel);
        }
    }

    public void AbrirPanel(NPCConversacion npc)
    {
        npcActual = npc;
        panelDialogo.SetActive(true);
        textoNombre.text = npc.archivoDialogo.nombrePersonaje; // Usamos el nombre del personaje

        // LÓGICA DE VISIBILIDAD: Apagamos ambos y prendemos solo el que corresponde
        textoPropuestaCliente.gameObject.SetActive(npc.esCliente);
        textoPropuestaVendedor.gameObject.SetActive(npc.esVendedor);

        // Asignamos el texto al que esté activo
        if (npc.esCliente)
            textoPropuestaCliente.text = npc.archivoDialogo.propuestaInicial;
        else
            textoPropuestaVendedor.text = npc.archivoDialogo.propuestaInicial;

        ConfigurarBotones(npc.archivoDialogo);
    }

    void ConfigurarBotones(Dialogo data)
    {
        string[] textos = { data.respuestaAmable, data.respuestaNeutral, data.respuestaCerrada };

        for (int i = 0; i < botonesRespuesta.Length; i++)
        {
            botonesRespuesta[i].gameObject.SetActive(true);
            botonesRespuesta[i].GetComponentInChildren<TextMeshProUGUI>().text = textos[i];

            int indice = i;
            botonesRespuesta[i].onClick.RemoveAllListeners();
            botonesRespuesta[i].onClick.AddListener(() => ProcesarEleccion(indice));
        }
    }

    void ProcesarEleccion(int eleccion)
    {
        if (npcActual.esCliente)
        {
            if (eleccion == 0) stats.AddMoney(20f);
            else if (eleccion == 1) stats.AddMoney(15f);
        }
        else if (npcActual.esVendedor)
        {
            if ((eleccion == 0 || eleccion == 1) && stats.money >= npcActual.precioAgua)
            {
                stats.money -= npcActual.precioAgua;
                if (actions != null) actions.TomarAgua(npcActual.recuperacionHidratacion);
            }
        }
        CerrarPanel();
    }

    public void CerrarPanel() => panelDialogo.SetActive(false);
}
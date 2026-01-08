using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance { get; private set; }

    [Header("UI Referencias")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoFrase;

    [Header("Configuracion de Botones")]
    public GameObject contenedorBotones;
    public Button[] botonesRespuesta; // Asigna 3 botones en el Inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (panelDialogo != null) panelDialogo.SetActive(false);
    }

    // VERSION 1: Se usa en PlayerInteraction para ventas rapidas
    public void IniciarDialogo(Dialogo data)
    {
        if (panelDialogo == null) return;

        panelDialogo.SetActive(true);
        textoNombre.text = data.nombrePersonaje;
        textoFrase.text = (data.frases != null && data.frases.Length > 0) ? data.frases[0] : "...";

        // Opciones de cortesia marplatense
        string[] opcionesVenta = { "¡Gracias!", "Disculpe", "Por favor" };
        ConfigurarBotonesVenta(opcionesVenta);
    }

    // VERSION 2: Se usa en NPCVendedor para misiones (Aceptar/Rechazar)
    public void IniciarDialogo(Dialogo data, NPCVendedor vendedor)
    {
        if (panelDialogo == null) return;

        panelDialogo.SetActive(true);
        textoNombre.text = data.nombrePersonaje;
        textoFrase.text = (data.frases != null && data.frases.Length > 0) ? data.frases[0] : "...";

        ConfigurarBotonesMision(vendedor);
    }

    private void ConfigurarBotonesVenta(string[] opciones)
    {
        foreach (var b in botonesRespuesta) b.gameObject.SetActive(false);

        for (int i = 0; i < opciones.Length; i++)
        {
            if (i >= botonesRespuesta.Length) break;
            botonesRespuesta[i].gameObject.SetActive(true);
            botonesRespuesta[i].GetComponentInChildren<TextMeshProUGUI>().text = opciones[i];

            botonesRespuesta[i].onClick.RemoveAllListeners();
            botonesRespuesta[i].onClick.AddListener(CerrarDialogo);
        }
    }

    private void ConfigurarBotonesMision(NPCVendedor vendedor)
    {
        foreach (var b in botonesRespuesta) b.gameObject.SetActive(false);

        // Boton Aceptar
        botonesRespuesta[0].gameObject.SetActive(true);
        botonesRespuesta[0].GetComponentInChildren<TextMeshProUGUI>().text = "Aceptar";
        botonesRespuesta[0].onClick.RemoveAllListeners();
        botonesRespuesta[0].onClick.AddListener(() => { vendedor.EfectoDecision(true); CerrarDialogo(); });

        // Boton Rechazar
        if (botonesRespuesta.Length > 1)
        {
            botonesRespuesta[1].gameObject.SetActive(true);
            botonesRespuesta[1].GetComponentInChildren<TextMeshProUGUI>().text = "Rechazar";
            botonesRespuesta[1].onClick.RemoveAllListeners();
            botonesRespuesta[1].onClick.AddListener(() => { vendedor.EfectoDecision(false); CerrarDialogo(); });
        }
    }

    public void CerrarDialogo()
    {
        panelDialogo.SetActive(false);
    }
}
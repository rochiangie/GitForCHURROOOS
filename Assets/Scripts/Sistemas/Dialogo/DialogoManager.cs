using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance;

    [Header("UI Canvas Elements")]
    public GameObject panelDialogo;
    public TextMeshProUGUI nombreTexto;
    public TextMeshProUGUI dialogoTexto;

    [Header("Botones")]
    public Button botonSi;
    public Button botonNo;
    public Button botonCerrar; // La "X"

    private Dialogo dialogoActual;
    private NPCVendedor npcActual;

    void Awake() => Instance = this;

    void Start()
    {
        panelDialogo.SetActive(false);
        // Asignar funciones a los botones por código para evitar errores
        botonCerrar.onClick.AddListener(CerrarDialogo);
    }

    public void IniciarDialogo(Dialogo nuevoDialogo, NPCVendedor npc)
    {
        dialogoActual = nuevoDialogo;
        npcActual = npc;

        panelDialogo.SetActive(true);
        nombreTexto.text = dialogoActual.nombre;
        dialogoTexto.text = dialogoActual.frasePregunta;

        // Mostrar botones de decisión
        botonSi.gameObject.SetActive(true);
        botonNo.gameObject.SetActive(true);

        // Limpiar listeners anteriores para que no se acumulen
        botonSi.onClick.RemoveAllListeners();
        botonNo.onClick.RemoveAllListeners();

        // Asignar nuevas acciones
        botonSi.onClick.AddListener(AlElegirSi);
        botonNo.onClick.AddListener(AlElegirNo);
    }

    void AlElegirSi()
    {
        dialogoTexto.text = dialogoActual.respuestaSi;
        OcultarBotonesDecision();
        npcActual.EfectoDecision(true); // Ejecuta la consecuencia positiva
    }

    void AlElegirNo()
    {
        dialogoTexto.text = dialogoActual.respuestaNo;
        OcultarBotonesDecision();
        npcActual.EfectoDecision(false); // Ejecuta la consecuencia negativa
    }

    void OcultarBotonesDecision()
    {
        botonSi.gameObject.SetActive(false);
        botonNo.gameObject.SetActive(false);
    }

    public void CerrarDialogo()
    {
        panelDialogo.SetActive(false);
    }
}
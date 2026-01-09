using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance { get; private set; }

    [Header("UI Premium")]
    public GameObject panelUI;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoCuerpo;

    [Header("Contenedores")]
    public GameObject groupOpciones;
    public Button[] botones;
    public TextMeshProUGUI[] textosBotones;

    [Header("Ajustes")]
    public float typingSpeed = 0.04f;

    private NPCConversacion npcActual;
    private Dialogo dialogoData;
    private PlayerStats stats;
    private bool escribiendo = false;

    void Awake() { Instance = this; }

    void Start() {
        stats = FindFirstObjectByType<PlayerStats>();
        if(panelUI != null) panelUI.SetActive(false);
    }

    public void AbrirPanel(NPCConversacion npc) {
        if (npc == null) return;
        npcActual = npc;
        dialogoData = npc.ObtenerDialogoDinamico();
        
        if (dialogoData == null) return;

        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = false;

        panelUI.SetActive(true);
        groupOpciones.SetActive(false);
        textoNombre.text = npcActual.nombre;
        
        StartCoroutine(EscribirLetras(dialogoData.propuesta));
    }

    IEnumerator EscribirLetras(string text) {
        escribiendo = true;
        textoCuerpo.text = "";
        
        float speed = (npcActual.personalidad == PersonalidadNPC.Molesto) ? 0.02f : typingSpeed;

        foreach(char c in text) {
            textoCuerpo.text += c;
            yield return new WaitForSeconds(speed);
        }
        escribiendo = false;
        
        if (!dialogoData.esGrito) MostrarOpciones();
        else StartCoroutine(AutoCerrarGrito());
    }

    IEnumerator AutoCerrarGrito() {
        yield return new WaitForSeconds(2f);
        Cerrar();
    }

    void MostrarOpciones() {
        groupOpciones.SetActive(true);
        for(int i=0; i<3; i++) {
            if(i < dialogoData.opciones.Length && !string.IsNullOrEmpty(dialogoData.opciones[i])) {
                botones[i].gameObject.SetActive(true);
                textosBotones[i].text = dialogoData.opciones[i];
                int idx = i;
                botones[i].onClick.RemoveAllListeners();
                botonesOpcionesFix(botones[i], idx);
            } else {
                botones[i].gameObject.SetActive(false);
            }
        }
    }

    void botonesOpcionesFix(Button b, int i) {
        b.onClick.AddListener(() => Seleccionar(i));
    }

    void Seleccionar(int idx) {
        if(escribiendo) return;
        groupOpciones.SetActive(false);
        
        AplicarConsecuencia(idx);
        StartCoroutine(ReaccionFinal(dialogoData.reacciones[idx]));
    }

    void AplicarConsecuencia(int idx) {
        if(stats == null || dialogoData.impactos == null || idx >= dialogoData.impactos.Length) return;

        Consecuencia c = dialogoData.impactos[idx];
        
        // Multiplicador por ebriedad
        float mod = 1f;
        if(stats.ebriedad > 40 && stats.ebriedad < 75) mod = 1.4f;
        else if(stats.ebriedad >= 85) mod = 0.5f;

        stats.AgregarDinero(c.dinero * mod);
        stats.RecuperarStamina(c.stamina);
        stats.RecuperarHidratacion(c.hidratacion);
        
        if (dialogoData.esVenta && idx == 0) {
            if (npcActual.esCliente) npcActual.FinalizarVenta();
        }
    }

    IEnumerator ReaccionFinal(string r) {
        yield return StartCoroutine(EscribirLetras(r));
        yield return new WaitForSeconds(1.5f);
        Cerrar();
    }

    public void Cerrar() {
        panelUI.SetActive(false);
        var move = FindFirstObjectByType<PlayerMovement>();
        if(move) move.enabled = true;
    }
}
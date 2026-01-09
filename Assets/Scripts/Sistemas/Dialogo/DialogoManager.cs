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
    public Image avatarNPC; // Por si quieres ponerle caras

    [Header("Contenedores")]
    public GameObject groupOpciones;
    public Button[] botones;
    public TextMeshProUGUI[] textosBotones;

    [Header("Audios")]
    public AudioClip soundTyping;
    public AudioClip soundOption;

    private NPCConversacion npcActual;
    private Dialogo dialogoData;
    private PlayerStats stats;
    private bool escribiendo = false;

    void Awake() { Instance = this; }

    void Start() {
        stats = FindFirstObjectByType<PlayerStats>();
        panelUI.SetActive(false);
    }

    public void AbrirPanel(NPCConversacion npc) {
        npcActual = npc;
        dialogoData = npc.ObtenerDialogoDinamico();
        
        // Bloquear al player
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
        
        // Efecto de personalidad: Los molestos escriben MAS RAPIDO y NERVIOSO
        float speed = (npcActual.personalidad == PersonalidadNPC.Molesto) ? 0.02f : 0.04f;

        foreach(char c in text) {
            textoCuerpo.text += c;
            if (soundTyping) AudioManager.Instance.PlaySFX(soundTyping);
            yield return new WaitForSeconds(speed);
        }
        escribiendo = false;
        MostrarOpciones();
    }

    void MostrarOpciones() {
        groupOpciones.SetActive(true);
        for(int i=0; i<3; i++) {
            if(i < dialogoData.opciones.Length) {
                botones[i].gameObject.SetActive(true);
                textosBotones[i].text = dialogoData.opciones[i];
                int idx = i;
                botones[i].onClick.RemoveAllListeners();
                botones[i].onClick.AddListener(() => Seleccionar(idx));
            } else {
                botones[i].gameObject.SetActive(false);
            }
        }
    }

    void Seleccionar(int idx) {
        if(escribiendo) return;
        groupOpciones.SetActive(false);
        
        // APLICAR CONSECUENCIAS AL JUEGO
        AplicarEfectos(dialogoData.impactos[idx]);

        StartCoroutine(ReaccionFinal(dialogoData.reacciones[idx]));
    }

    void AplicarEfectos(Consecuencia c) {
        if(!stats) return;

        // Bonificacion por Ebriedad (Lore: estas borracho, sos mas convincente o mas tonto)
        float mod = 1f;
        if(stats.ebriedad > 50) mod = 1.3f; // Convences mas
        if(stats.ebriedad > 85) mod = 0.5f; // Te estafan

        stats.AgregarDinero(c.dinero * mod);
        stats.RecuperarStamina(c.stamina);
        stats.RecuperarHidratacion(c.hidratacion);
        
        // La reputacion podria afectar futuros precios (Siguiente fase)
        Debug.Log("Impacto de dialogo: $" + c.dinero + " | Reputacion: " + c.reputacion);
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
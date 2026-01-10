using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LoreLine
{
    public string personaje; 
    [TextArea(3, 10)]
    public string texto;
    public bool esBurbujaIzquierda; // [x] Izquierda / [ ] Derecha
    public AudioClip sonidoEspecial; // Opcional: Sonido que suena al empezar la linea
}

public class LoreDialogueManager : MonoBehaviour
{
    [Header("UI Burbujas")]
    public GameObject burbujaElla;
    public TextMeshProUGUI textoElla;
    
    public GameObject burbujaEl;
    public TextMeshProUGUI textoEl;

    [Header("Configuracion Sonido")]
    public AudioSource audioSource;
    public AudioClip clipEscribir;
    public AudioClip clipSiguiente;
    public AudioClip clipFinal;

    [Header("Ajustes")]
    public float typingSpeed = 0.04f;
    public string nombreEscenaSiguiente = "Juego";

    [Header("Contenido del Lore")]
    public List<LoreLine> conversacion = new List<LoreLine>();

    private int indiceActual = 0;
    private bool escribiendo = false;
    private Coroutine corrutinaActual;
    private string textoCompletoActual;
    private TextMeshProUGUI tmpActivo;

    void Start()
    {
        // Si no asignaste AudioSource, intentamos buscar uno
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        burbujaElla.SetActive(false);
        burbujaEl.SetActive(false);
        
        if (conversacion.Count > 0) StartCoroutine(IniciarConversacion());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (escribiendo)
            {
                CompletarTextoHuyendo();
            }
            else
            {
                SiguienteLinea();
            }
        }
    }

    IEnumerator IniciarConversacion()
    {
        yield return new WaitForSeconds(0.5f);
        MostrarLinea(0);
    }

    void SiguienteLinea()
    {
        indiceActual++;
        if (indiceActual < conversacion.Count)
        {
            if(audioSource && clipSiguiente) audioSource.PlayOneShot(clipSiguiente);
            MostrarLinea(indiceActual);
        }
        else
        {
            StartCoroutine(SecuenciaFinal());
        }
    }

    void MostrarLinea(int indice)
    {
        if(corrutinaActual != null) StopCoroutine(corrutinaActual);

        LoreLine linea = conversacion[indice];
        textoCompletoActual = linea.texto;
        
        burbujaElla.SetActive(false);
        burbujaEl.SetActive(false);

        if (linea.esBurbujaIzquierda)
        {
            burbujaElla.SetActive(true);
            tmpActivo = textoElla;
        }
        else
        {
            burbujaEl.SetActive(true);
            tmpActivo = textoEl;
        }

        // Si la linea tiene un sonido especial (ej: grito de ella), lo tocamos
        if (linea.sonidoEspecial && audioSource)
        {
            audioSource.PlayOneShot(linea.sonidoEspecial);
        }

        corrutinaActual = StartCoroutine(EscribirTexto(tmpActivo, linea.texto));
    }

    IEnumerator EscribirTexto(TextMeshProUGUI tmp, string frase)
    {
        escribiendo = true;
        tmp.text = "";
        
        int charIndex = 0;
        foreach (char c in frase)
        {
            tmp.text += c;
            
            // Tocar sonido de escritura cada 2 letras para que no sea molesto
            if (charIndex % 2 == 0 && audioSource && clipEscribir)
            {
                audioSource.PlayOneShot(clipEscribir, 0.5f);
            }
            
            charIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }
        escribiendo = false;
    }

    void CompletarTextoHuyendo()
    {
        StopCoroutine(corrutinaActual);
        tmpActivo.text = textoCompletoActual;
        escribiendo = false;
    }

    IEnumerator SecuenciaFinal()
    {
        // Tocar sonido final (portazo/suspiro)
        if (audioSource && clipFinal) 
        {
            audioSource.PlayOneShot(clipFinal);
            yield return new WaitForSeconds(clipFinal.length);
        }
        
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }
}

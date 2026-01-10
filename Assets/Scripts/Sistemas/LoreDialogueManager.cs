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
}

public class LoreDialogueManager : MonoBehaviour
{
    [Header("UI Burbujas")]
    public GameObject burbujaElla;
    public TextMeshProUGUI textoElla;
    
    public GameObject burbujaEl;
    public TextMeshProUGUI textoEl;

    [Header("Configuracion")]
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
            MostrarLinea(indiceActual);
        }
        else
        {
            FinalizarLore();
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

        corrutinaActual = StartCoroutine(EscribirTexto(tmpActivo, linea.texto));
    }

    IEnumerator EscribirTexto(TextMeshProUGUI tmp, string frase)
    {
        escribiendo = true;
        tmp.text = "";
        foreach (char c in frase)
        {
            tmp.text += c;
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

    void FinalizarLore()
    {
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }
}

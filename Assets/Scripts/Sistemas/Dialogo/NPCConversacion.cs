using UnityEngine;
using System.Collections.Generic;

public class NPCConversacion : MonoBehaviour
{
    [Header("Identidad")]
    public string nombre = "Vecino";
    public bool esCliente = true;
    public bool esVendedor = false;
    public bool quiereComprar = false;

    [Header("Economia")]
    public float precioAgua = 20f;
    public float recuperacionHidratacion = 40f;

    [Header("Dialogos")]
    public List<Dialogo> poolDeDialogos;
    public GameObject iconoHambre;

    public Dialogo archivoDialogo {
        get {
            if (poolDeDialogos != null && poolDeDialogos.Count > 0) 
                return poolDeDialogos[Random.Range(0, poolDeDialogos.Count)];
            return null;
        }
    }

    public void Interactuar()
    {
        // Ahora siempre deja interactuar para la "charla" y el lore
        if (DialogoManager.Instance != null)
        {
            DialogoManager.Instance.AbrirPanel(this);
        }
    }

    public void FinalizarVenta()
    {
        quiereComprar = false;
        if (iconoHambre != null) iconoHambre.SetActive(false);
        GameEvents.OnChurroVendido?.Invoke();
    }
}
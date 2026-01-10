using UnityEngine;
using System.Collections.Generic;

public class NPCConversacion : MonoBehaviour
{
    [Header("Identidad")]
    public string nombre = "Vecino";
    public PersonalidadNPC personalidad;
    public bool esVendedorBebidas;
    public bool esCliente;
    public bool quiereComprar;
    public int churrosDeseados = 1;

    [Header("Precios (Si es Vendedor)")]
    public float precioAgua = 3f;
    public float precioBirra = 5f;
    public float precioGaseosa = 4f;

    [Header("Dialogos")]
    public List<Dialogo> poolDialogos;
    public Dialogo dialogoGrito;

    public void Interactuar() {
        if (DialogoManager.Instance != null) DialogoManager.Instance.AbrirPanel(this);
    }

    public Dialogo ObtenerDialogoDinamico() {
        if (poolDialogos != null && poolDialogos.Count > 0) 
            return poolDialogos[Random.Range(0, poolDialogos.Count)];
        return dialogoGrito;
    }

    public void FinalizarVenta() {
        quiereComprar = false;
        GameEvents.OnChurroVendido?.Invoke();
    }
}
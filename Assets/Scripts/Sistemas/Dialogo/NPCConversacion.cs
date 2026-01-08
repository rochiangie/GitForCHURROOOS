using UnityEngine;
using System.Collections.Generic;

public class NPCConversacion : MonoBehaviour
{
    [Header("Identidad")]
    public string nombre = "Vecino";
    public PersonalidadNPC personalidad;
    public bool esVendedor;
    public bool esCliente;
    public bool quiereComprar; // Para los clientes con hambre

    [Header("Economia Individual")]
    public float precioBaseAgua = 20f;
    public float pagoBaseChurro = 120f;
    public float recuperacionHidratacion = 35f;

    [Header("Dialogos por Situacion")]
    public List<Dialogo> poolDialogos;
    public Dialogo dialogoGrito; // Lo que grita si es un "griton"

    public void Interactuar()
    {
        if (DialogoManager.Instance != null)
        {
            DialogoManager.Instance.AbrirPanel(this);
        }
    }

    public void FinalizarVenta()
    {
        quiereComprar = false;
        GameEvents.OnChurroVendido?.Invoke();
    }
}
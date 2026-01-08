using UnityEngine;
using System.Collections.Generic;

public class NPCConversacion : MonoBehaviour
{
    [Header("Tipo de Personaje")]
    public bool esCliente;
    public bool esVendedor;

    [Header("Estado de Venta")]
    public bool quiereComprar;
    public GameObject iconoChurro; // El objeto visual sobre la cabeza del cliente

    [Header("Contenido")]
    public Dialogo archivoDialogo;

    [Header("Ajustes de Vendedor")]
    public float precioAgua = 10f;
    public float recuperacionHidratacion = 30f;

    void Start()
    {
        ActualizarEstadoIcono();
    }

    public void Interactuar()
    {
        // Si es cliente pero NO tiene el icono activo, no puede interactuar
        if (esCliente && !quiereComprar) return;

        if (archivoDialogo != null)
        {
            DialogoManager.Instance.AbrirPanel(this);
        }
    }

    public void FinalizarVenta()
    {
        quiereComprar = false;
        ActualizarEstadoIcono();
        ActivarSiguienteClienteAleatorio();
    }

    public void ActivarComoCliente()
    {
        if (esCliente)
        {
            quiereComprar = true;
            ActualizarEstadoIcono();
        }
    }

    void ActualizarEstadoIcono()
    {
        if (iconoChurro != null)
        {
            iconoChurro.SetActive(quiereComprar);
        }
    }

    void ActivarSiguienteClienteAleatorio()
    {
        // Buscamos todos los NPCs en la escena
        NPCConversacion[] todos = Object.FindObjectsByType<NPCConversacion>(FindObjectsSortMode.None);
        List<NPCConversacion> candidatos = new List<NPCConversacion>();

        foreach (var npc in todos)
        {
            // Solo activamos a otros que sean clientes y que actualmente NO quieran comprar
            if (npc.esCliente && !npc.quiereComprar && npc != this)
            {
                candidatos.Add(npc);
            }
        }

        if (candidatos.Count > 0)
        {
            candidatos[Random.Range(0, candidatos.Count)].ActivarComoCliente();
        }
    }
}
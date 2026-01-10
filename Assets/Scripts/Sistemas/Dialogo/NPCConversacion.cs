using UnityEngine;
using System.Collections.Generic;

public class NPCConversacion : MonoBehaviour
{
    [Header("Identidad")]
    public string nombre = "Turista";
    public PersonalidadNPC personalidad;
    public bool esVendedorBebidas;
    public bool esCliente;
    public bool quiereComprar;
    public int churrosDeseados = 1;

    [Header("Memoria")]
    public bool yaCompro = false; 

    [Header("Economia Individual")]
    public float pagoBaseChurro = 20f; 
    
    [Header("Precios (Si es Vendedor)")]
    public float precioAgua = 3f;
    public float precioBirra = 5f;

    [Header("Pools de Diálogos")]
    public List<Dialogo> poolAmables = new List<Dialogo>();
    public List<Dialogo> poolMolestos = new List<Dialogo>();
    public List<Dialogo> poolYaCompro = new List<Dialogo>();

    public void Interactuar() {
        if (DialogoManager.Instance != null) DialogoManager.Instance.AbrirPanel(this);
    }

    public Dialogo ObtenerDialogoDinamico() {
        List<Dialogo> posibles = new List<Dialogo>();

        if (yaCompro) {
            // Si ya compro, el NPC "cambia de humor". Buscamos en TODOS sus pools 
            // cualquier dialogo que este marcado como 'soloPostVenta'.
            
            // 1. Miramos en el pool especifico de Ya Compro
            if (poolYaCompro != null) posibles.AddRange(poolYaCompro);

            // 2. Buscamos en los otros pools por si pusiste el "SaliDeAca" ahi con el flag marcado
            foreach(var d in poolAmables) if(d != null && d.soloPostVenta) posibles.Add(d);
            foreach(var d in poolMolestos) if(d != null && d.soloPostVenta) posibles.Add(d);
        } else {
            // Si NO compro, respetamos la personalidad asignada por el nivel
            List<Dialogo> poolBase = (personalidad == PersonalidadNPC.Amable) ? poolAmables : poolMolestos;
            foreach (Dialogo d in poolBase) {
                if (d != null && !d.soloPostVenta) posibles.Add(d);
            }
        }

        // Devolvemos uno al azar de los filtrados
        if (posibles.Count > 0) {
            return posibles[Random.Range(0, posibles.Count)];
        }

        Debug.LogWarning($"<color=yellow>[NPC {nombre}]</color> No encontro ningun dialogo compatible (yaCompro: {yaCompro}).");
        return null;
    }

    public void FinalizarVenta() {
        yaCompro = true;
        quiereComprar = false;
        GameEvents.OnChurroVendido?.Invoke();
    }
}
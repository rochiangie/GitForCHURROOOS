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
    public float precioAgua = 8f;
    public float precioBirra = 15f;
    public float precioGaseosa = 12f;

    [Header("Pools de Diálogos")]
    public List<Dialogo> poolAmables;
    public List<Dialogo> poolMolestos;
    public List<Dialogo> poolYaCompro;

    public void Interactuar() {
        if (DialogoManager.Instance != null) DialogoManager.Instance.AbrirPanel(this);
    }

    public Dialogo ObtenerDialogoDinamico() {
        // Si ya nos compro, priorizamos el pool de "Ya Compro" (molestia por reincidencia)
        if (yaCompro) {
            if (poolYaCompro != null && poolYaCompro.Count > 0) {
                return poolYaCompro[Random.Range(0, poolYaCompro.Count)];
            }
            // Si no tenes un dialogo especifico de "ya compre", usamos los molestos como respaldo
            if (poolMolestos != null && poolMolestos.Count > 0) {
                return poolMolestos[Random.Range(0, poolMolestos.Count)];
            }
        }

        // Casos normales: elegimos segun la personalidad asignada por el nivel
        List<Dialogo> poolARevisar = (personalidad == PersonalidadNPC.Amable) ? poolAmables : poolMolestos;

        if (poolARevisar != null && poolARevisar.Count > 0) {
            return poolARevisar[Random.Range(0, poolARevisar.Count)];
        }

        Debug.LogWarning($"<color=yellow>[NPC {nombre}]</color> No tiene diálogos cargados.");
        return null;
    }

    public void FinalizarVenta() {
        yaCompro = true;
        quiereComprar = false;
        GameEvents.OnChurroVendido?.Invoke();
    }
}
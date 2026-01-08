using UnityEngine;

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Sistema/Dialogo")]
public class Dialogo : ScriptableObject
{
    public string nombreNPC;
    [TextArea(3, 10)]
    public string propuesta; // El "gancho" del NPC

    [Header("Opciones de Jugador")]
    [Tooltip("0: Amable/Compra, 1: Neutral, 2: Cortante/Cerrar")]
    public string[] opciones = new string[3];

    [Header("Reacciones del NPC")]
    [Tooltip("Reaccion a cada opcion anterior")]
    public string[] reacciones = new string[3];
}
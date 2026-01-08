using UnityEngine;

public enum PersonalidadNPC { Amable, Molesto, Indiferente, Ayuda }

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Sistema/Dialogo")]
public class Dialogo : ScriptableObject
{
    public string nombreNPC;
    public PersonalidadNPC personalidad;
    
    [TextArea(3, 10)]
    public string propuesta;

    [Header("Opciones de Jugador")]
    public string[] opciones = new string[3]; // [0] suele ser la mejor, [2] la peor

    [Header("Reacciones del NPC")]
    public string[] reacciones = new string[3];

    [Header("Configuracion")]
    public bool esVenta; // ¿Este dialogo lleva a una transaccion?
    public bool esGrito; // ¿Es solo un NPC gritando algo (sin opciones)?
}
using UnityEngine;

public enum PersonalidadNPC { Amable, Molesto, Indiferente, Ayuda }

[System.Serializable]
public struct Consecuencia
{
    public float dinero;
    public int churros; // Negativo para quitar, positivo para dar
    public float stamina;
    public float hidratacion;
    public float reputacion;
}

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Sistema/Dialogo")]
public class Dialogo : ScriptableObject
{
    public string nombreNPC;
    public PersonalidadNPC personalidad;
    
    [TextArea(3, 10)]
    public string propuesta;

    [Header("Opciones y Reacciones")]
    public string[] opciones = new string[3];
    [TextArea(2, 5)]
    public string[] reacciones = new string[3];

    [Header("Impacto en el Juego")]
    public Consecuencia[] impactos = new Consecuencia[3];

    [Header("Flags")]
    public bool esVenta; 
    public bool esGrito;
}
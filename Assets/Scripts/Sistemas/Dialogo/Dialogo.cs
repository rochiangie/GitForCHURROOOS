using UnityEngine;

[System.Serializable]
public struct Consecuencia
{
    public float dinero;
    public float stamina;
    public float hidratacion;
    public float reputacion; // Nueva stat: Fama en la playa
}

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Sistema/Dialogo Pro")]
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
    public Consecuencia[] impactos = new Consecuencia[3]; // Consecuencia por cada opcion

    [Header("Flags")]
    public bool requiereDinero; // Para compras
    public bool requiereChurros; // Para ventas
    public bool esEventoEspecial; // Cambia la musica o camara
}
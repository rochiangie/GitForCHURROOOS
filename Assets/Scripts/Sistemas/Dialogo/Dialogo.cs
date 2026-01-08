using UnityEngine;

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Sistema/Dialogo")]
public class Dialogo : ScriptableObject
{
    public string nombrePersonaje;

    [TextArea(2, 5)]
    public string propuestaInicial;

    [Header("Opciones de respuesta del Jugador")]
    public string respuestaAmable;   // Botón 1
    public string respuestaNeutral;  // Botón 2
    public string respuestaCerrada;  // Botón 3 (Para irse o ser cortante)
}
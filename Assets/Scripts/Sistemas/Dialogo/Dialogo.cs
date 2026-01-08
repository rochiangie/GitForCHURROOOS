using UnityEngine;

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Sistema/Dialogo")]
public class Dialogo : ScriptableObject
{
    public string nombrePersonaje;
    [TextArea(3, 10)]
    public string[] frases;
}
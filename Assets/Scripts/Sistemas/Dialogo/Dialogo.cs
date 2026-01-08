using UnityEngine;

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Sistema/Dialogo")]
public class Dialogo : ScriptableObject
{
    public string nombreNPC;
    public string[] lineas;
}
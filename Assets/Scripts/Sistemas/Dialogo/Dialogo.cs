[System.Serializable]
public class Dialogo
{
    public string nombre;
    [UnityEngine.TextArea(3, 5)]
    public string frasePregunta; // La frase que hace el NPC antes de los botones
    public string respuestaSi;   // Lo que dice si aceptas
    public string respuestaNo;   // Lo que dice si rechazas
}
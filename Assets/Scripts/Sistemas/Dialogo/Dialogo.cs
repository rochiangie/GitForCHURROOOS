using UnityEngine;

// --- ESTRUCTURAS DE APOYO ---
public enum PersonalidadNPC { Amable, Molesto, Indiferente, Ayuda }

[System.Serializable]
public struct Consecuencia
{
    public float dinero;
    public int churros; 
    public float stamina;
    public float hidratacion;
    public float reputacion;
}

[System.Serializable]
public struct RamaDialogo
{
    [Header("Respuesta del Jugador")]
    public string textoOpcion;

    [Header("¿Que pasa después?")]
    [Tooltip("Si arrastras otro dialogo aqui, la charla continua con ese archivo.")]
    public Dialogo siguienteDialogo;
    
    [Tooltip("Si NO hay siguiente dialogo, el NPC dira esto antes de cerrar o vender.")]
    [TextArea(2, 4)]
    public string reaccionSiTermina;

    [Header("Impacto (Solo si la charla termina aqui)")]
    public Consecuencia impacto;
}

// --- CLASE PRINCIPAL ---
[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Sistema/Dialogo")]
public class Dialogo : ScriptableObject
{
    public string nombreNPC;
    public PersonalidadNPC personalidad;
    
    [TextArea(3, 10)]
    public string propuesta;

    [Header("Configuracion de las Opciones")]
    public RamaDialogo[] respuestas;

    [Header("Configuracion Especial")]
    public bool soloPostVenta; // ¿Solo puede aparecer si ya se compro antes?
    public bool esVenta; // Si termina la charla, ¿pasa a la fase de cobrar churros?
    public bool esGrito; // ¿Es un dialogo que se cierra solo?
}
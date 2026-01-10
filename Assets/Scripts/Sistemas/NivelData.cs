using UnityEngine;

[CreateAssetMenu(fileName = "NuevoNivel", menuName = "Sistema/NivelData")]
public class NivelData : ScriptableObject
{
    public float metaDinero;
    
    [Header("Configuracion de Tiempo")]
    public float duracionDiaMinutos = 5f; // Cuanto dura el dia de 8am a 8pm en minutos reales
    
    [Header("Dificultad Clientes")]
    public float probabilidadCompra = 50f;
    public float porcentajeAmigables = 0.8f; 
    public int minChurrosPorPedido = 1;
    public int maxChurrosPorPedido = 6;

    [Header("Dificultad Ambiente")]
    public float multiplicadorCalor = 1f;

    [Header("Boss")]
    public bool esNivelBoss = false;
    public GameObject prefabBoss;
}

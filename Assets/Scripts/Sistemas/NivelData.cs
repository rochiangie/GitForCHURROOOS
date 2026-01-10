using UnityEngine;

[CreateAssetMenu(fileName = "NuevoNivel", menuName = "Sistema/Datos de Nivel")]
public class NivelData : ScriptableObject
{
    [Header("Objetivos")]
    public float metaDinero;
    public float duracionDiaHoras = 12f; // De 8 AM a 8 PM por ejemplo

    [Header("Dificultad Clientes")]
    [Range(0, 100)] public float probabilidadCompra;
    [Range(0, 1)] public float porcentajeAmigables; // 1 = todos amigos, 0 = todos molestos
    public int maxChurrosPorPedido = 5;

    [Header("Ambiente")]
    public float multiplicadorCalor = 1f;
    public bool esNivelBoss = false;
    public GameObject prefabBoss;
}

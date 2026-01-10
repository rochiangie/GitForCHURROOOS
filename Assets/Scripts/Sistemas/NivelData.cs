using UnityEngine;

[CreateAssetMenu(fileName = "NuevoNivel", menuName = "Sistema/NivelData")]
public class NivelData : ScriptableObject
{
    public float metaDinero;
    public float duracionDiaHoras = 12f; 
    
    [Header("Dificultad Clientes")]
    public float probabilidadCompra = 50f;
    public float porcentajeAmigables = 0.8f; 
    public int maxChurrosPorPedido = 3;

    [Header("Dificultad Ambiente")]
    public float multiplicadorCalor = 1f;
    public float velocidadReloj = 0.2f; // <--- AGREGADO: Velocidad base del tiempo para este nivel

    [Header("Boss")]
    public bool esNivelBoss = false;
    public GameObject prefabBoss;
}

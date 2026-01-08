using UnityEngine;

public class NPCVendedor : MonoBehaviour
{
    public enum TipoVendedor { Amistoso, Rival, Neutro }
    public TipoVendedor personalidad;
    public string nombreVendedor;

    [Range(-100, 100)]
    public float afinidad = 0;

    private Transform playerTransform;

    void Start()
    {
        // Buscamos al jugador por su Tag para saber hacia dónde mirar
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        // Ejemplo: El NPC siempre mira hacia donde está el jugador cuando está cerca
        if (playerTransform != null)
        {
            float distancia = Vector2.Distance(transform.position, playerTransform.position);

            if (distancia < 5f) // Si el jugador está a menos de 5 metros
            {
                MirarObjetivo(playerTransform.position);
            }
        }
    }

    public void MirarObjetivo(Vector3 objetivo)
    {
        // Si el objetivo está a la derecha
        if (objetivo.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // Si el objetivo está a la izquierda
        else if (objetivo.x < transform.position.x)
        {
            // Volteamos TODO el objeto, incluyendo sus colliders
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void Interactuar(float cantidad)
    {
        afinidad += cantidad;
        afinidad = Mathf.Clamp(afinidad, -100, 100);
        Debug.Log($"{nombreVendedor} ahora tiene {afinidad} de afinidad.");
    }
}
using UnityEngine;

public class BeatEmUpCamera : MonoBehaviour
{
    public Transform target;

    [Header("Configuracion de Seguimiento")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 2, -10);

    [Header("Limites de Camara")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculamos la posicion deseada
        Vector3 targetPosition = target.position + offset;

        // Aplicamos los limites
        float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

        Vector3 desiredPosition = new Vector3(clampedX, clampedY, targetPosition.z);

        // Suavizamos el movimiento
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
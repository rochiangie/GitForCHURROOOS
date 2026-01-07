using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target == null)
        {
            Debug.Log("NO HAY TARGET");
            return;
        }

        transform.position = new Vector3(
            target.position.x,
            transform.position.y,
            -10
        );

        Debug.Log("Camara X: " + transform.position.x +
                  " | Player X: " + target.position.x);
    }
}


using UnityEngine;
using UnityEditor;

public class ColliderTool : EditorWindow
{
    [MenuItem("Juego Churros/Corregir Flip y Colliders")]
    public static void FixFlipAndColliders()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                // Si el sprite tiene el Flip activado
                if (sr.flipX)
                {
                    // 1. Quitamos el flip visual
                    sr.flipX = false;

                    // 2. Aplicamos el giro real mediante la escala
                    Vector3 currentScale = obj.transform.localScale;
                    obj.transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);

                    Debug.Log($"[CORREGIDO] {obj.name}: Se pasó el Flip a Scale X. Los colliders ahora rotarán con el objeto.");
                }
            }

            // Configurar o añadir PolygonCollider2D (Físico)
            PolygonCollider2D poly = obj.GetComponent<PolygonCollider2D>();
            if (poly == null)
            {
                poly = obj.AddComponent<PolygonCollider2D>();
                Debug.Log($"[NUEVO] PolygonCollider añadido a {obj.name}.");
            }
            poly.isTrigger = false;

            // Configurar o añadir BoxCollider2D (Trigger Interacción)
            BoxCollider2D box = obj.GetComponent<BoxCollider2D>();
            if (box == null)
            {
                box = obj.AddComponent<BoxCollider2D>();
                Debug.Log($"[NUEVO] BoxCollider Trigger añadido a {obj.name}.");
            }
            box.isTrigger = true;
            box.size = new Vector2(1.2f, 1.2f);
        }
    }
}
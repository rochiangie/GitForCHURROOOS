using UnityEngine;

public class ChurroProyectil : MonoBehaviour
{
    public float velocidad = 15f;
    public float danio = 15f;
    public float vidaUtil = 2f;
    public LayerMask capaNPC;

    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        // Si no tiene RB, se lo agregamos para que vuele con fisica
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
        rb.linearDamping = 0f; // Importante para que no pierda velocidad
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Lanzar(Vector2 dir) {
        // Aseguramos que la direccion sea pura (Derecha o Izquierda)
        Vector2 pureDir = dir.normalized;
        
        // Aplicamos velocidad al Rigidbody directamente
        rb.linearVelocity = pureDir * velocidad;

        // Rotar el churro visualmente
        float angle = Mathf.Atan2(pureDir.y, pureDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, vidaUtil);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        ProcesarImpacto(other.gameObject, other.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        ProcesarImpacto(collision.gameObject, collision.contacts[0].point);
    }

    void ProcesarImpacto(GameObject target, Vector3 puntoImpacto) {
        if (target.CompareTag("Player")) return;

        // Detectar impacto en NPCs o Boss
        if (((1 << target.layer) & capaNPC) != 0) {
            
            BossRival boss = target.GetComponent<BossRival>();
            if (boss != null) {
                boss.RecibirImpactoProyectil(danio, puntoImpacto);
                Debug.Log($"<color=green>[PROYECTIL]</color> Impacto confirmado en BOSS: {target.name}");
            } else {
                Rigidbody2D rbNPC = target.GetComponent<Rigidbody2D>();
                if (rbNPC != null) {
                    Vector2 forceDir = (target.transform.position - transform.position).normalized;
                    rbNPC.AddForce(forceDir * 8f, ForceMode2D.Impulse);
                }
            }
            ImpactoVisual();
        } 
        else if (target.CompareTag("limites")) {
            ImpactoVisual();
        }
    }

    void ImpactoVisual() {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.golpePelea);
        // Aqui podrias instanciar chispas de azucar o migas
        Destroy(gameObject);
    }
}

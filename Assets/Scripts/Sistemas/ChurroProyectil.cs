using UnityEngine;

public class ChurroProyectil : MonoBehaviour
{
    public float velocidad = 12f;
    public float danio = 15f;
    public float vidaUtil = 2f;
    public LayerMask capaNPC;

    private Vector2 direccion;

    public void Lanzar(Vector2 dir) {
        direccion = dir.normalized;
        // Rotar el churro para que apunte a donde vuela
        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Destroy(gameObject, vidaUtil);
    }

    void Update() {
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Si golpea un NPC o Boss
        if (((1 << other.gameObject.layer) & capaNPC) != 0) {
            
            BossRival boss = other.GetComponent<BossRival>();
            if (boss != null) {
                boss.RecibirEmpuje(0.3f, danio);
            } else {
                // Si es un NPC comun, solo lo empujamos
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null) {
                    Vector2 forceDir = (other.transform.position - transform.position).normalized;
                    rb.AddForce(forceDir * 10f, ForceMode2D.Impulse);
                }
            }

            // Efecto de impacto y destruir
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.golpePelea);
            Destroy(gameObject);
        }
    }
}

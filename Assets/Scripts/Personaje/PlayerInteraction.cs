using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float radius = 2f;
    private PlayerStats stats;
    private Animator anim;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider2D[] cercanos = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var col in cercanos)
            {
                if (col.CompareTag("Vendedor"))
                {
                    col.GetComponent<NPCVendedor>()?.Interactuar(10f);
                }
                if (col.CompareTag("Cliente"))
                {
                    stats.AddMoney(15f);
                    stats.hydration -= 5f;
                    Debug.Log("<color=yellow>¡Churro vendido!</color>");
                }
            }
        }

        // Actualizar parámetro drunk en el animator
        anim.SetBool("setdrunk", stats.hydration <= 20f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
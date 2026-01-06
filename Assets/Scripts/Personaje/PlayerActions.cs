using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public int churros = 24;
    public int plata = 0;
    public LayerMask customerLayer;
    public LayerMask npcLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Vender();
        if (Input.GetKeyDown(KeyCode.Q)) Pelear();
    }

    void Vender()
    {
        Collider2D customer = Physics2D.OverlapCircle(transform.position, 1.5f, customerLayer);
        if (customer != null && churros > 0)
        {
            churros--;
            plata += 1000;
            AudioManager.Instance.GritarChurros();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.ventaExitosa);
            // Aquí podrías avisar al NPC que ya compró
        }
    }

    void Pelear()
    {
        GetComponent<Animator>().SetTrigger("attack");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.golpePelea);
        // Lógica de Raycast para empujar NPCs maleducados
    }
}
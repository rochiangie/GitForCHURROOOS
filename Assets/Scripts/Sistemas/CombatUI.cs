using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    [Header("Player UI")]
    public Slider sliderVidaPlayer;
    public TextMeshProUGUI textoVidaPlayer;

    [Header("Boss UI")]
    public GameObject panelBoss;
    public Slider sliderVidaBoss;
    public TextMeshProUGUI nombreBoss;

    private PlayerStats player;
    private BossRival boss;

    void Start()
    {
        player = FindFirstObjectByType<PlayerStats>();
        if(panelBoss) panelBoss.SetActive(false);
    }

    void Update()
    {
        // Update Player Health
        if (player != null && sliderVidaPlayer != null) {
            sliderVidaPlayer.maxValue = player.vidaMax;
            sliderVidaPlayer.value = player.vida;
            if(textoVidaPlayer) textoVidaPlayer.text = $"HP: {player.vida:F0}";
        }

        // Detect and Update Boss Health
        if (boss == null) {
            boss = FindFirstObjectByType<BossRival>();
            if(boss != null && panelBoss != null) panelBoss.SetActive(true);
        } else {
            if (sliderVidaBoss != null) {
                sliderVidaBoss.maxValue = boss.vidaMax;
                sliderVidaBoss.value = boss.vidaActual;
            }
            // Si el boss muere o se va, ocultamos panel
            if (boss.vidaActual <= 0) {
                 // Podrias dejarlo un par de segundos
            }
        }
    }
}

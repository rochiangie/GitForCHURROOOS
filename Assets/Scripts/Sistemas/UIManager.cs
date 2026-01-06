using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Slider heatSlider;
    public TextMeshProUGUI churrosText;
    public TextMeshProUGUI plataText;

    private SunSystem sun;
    private PlayerActions actions;

    void Start()
    {
        sun = FindObjectOfType<SunSystem>();
        actions = FindObjectOfType<PlayerActions>();
    }

    void Update()
    {
        heatSlider.value = sun.heat;
        churrosText.text = "CHURROS: " + actions.churros;
        plataText.text = "PLATA: $" + actions.plata;
    }
}
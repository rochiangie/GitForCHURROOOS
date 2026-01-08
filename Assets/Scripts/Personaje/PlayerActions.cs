using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerHealthSystem health;
    private Animator anim;

    [Header("Configuración de Acciones")]
    public float drinkRecovery = 25f;
    public float drinkCost = 5f;

    private bool isDrunkEfecto = false;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        health = GetComponent<PlayerHealthSystem>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Tecla E para refrescarse con una bebida (si tiene dinero)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TomarBebida();
        }

        // Actualizar el parámetro en el Animator
        // Es "Drunk" si tiene poca hidratación O si activó el efecto por script
        bool currentDrunkState = (stats.hydration <= 20f || isDrunkEfecto);
        anim.SetBool("setdrunk", currentDrunkState);
    }

    public void TomarBebida()
    {
        if (stats.money >= drinkCost)
        {
            stats.money -= drinkCost;
            stats.hydration = Mathf.Min(stats.hydration + drinkRecovery, stats.maxStat);
            stats.temperature = Mathf.Max(stats.temperature - 15f, 0);

            Debug.Log("<color=blue>[ACCIÓN]</color> Tomaste una bebida fría. -$5");

            // Si quieres que la bebida dé un efecto de mareo temporal (como la birra)
            StartCoroutine(EfectoMareoTemporal(4f));
        }
        else
        {
            Debug.Log("<color=red>[ACCIÓN]</color> No tienes dinero para beber.");
        }
    }

    // Función pública que pueden llamar otros scripts (como el SunSystem que tenías)
    public void SetDrunk(bool state)
    {
        isDrunkEfecto = state;
        Debug.Log(state ? "Iniciando efecto mareo..." : "Efecto mareo terminado.");
    }

    // Corrutina para que el efecto se pase solo después de unos segundos
    private IEnumerator EfectoMareoTemporal(float tiempo)
    {
        isDrunkEfecto = true;
        yield return new WaitForSeconds(tiempo);
        isDrunkEfecto = false;
    }
}
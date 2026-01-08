using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Configuracion de Recuperacion")]
    public float recuperacionAguaBase = 30f;
    public float reduccionTemperaturaAgua = 20f;
    public float recuperacionStaminaChurro = 25f;

    private PlayerStats stats;
    private Animator anim;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
    }

    public void TomarAgua(float cantidad = 1f)
    {
        if (stats == null) return;
        stats.RecuperarHidratacion(recuperacionAguaBase * cantidad);
        stats.ReducirTemperatura(reduccionTemperaturaAgua * cantidad);
    }

    public void ComerChurro()
    {
        if (stats == null) return;
        if (stats.churrosCantidad > 0)
        {
            stats.churrosCantidad--;
            stats.RecuperarStamina(recuperacionStaminaChurro);
            if (anim != null) anim.SetTrigger("attack");
        }
    }

    public bool VenderChurro(float precio)
    {
        if (stats == null || stats.churrosCantidad <= 0) return false;
        
        stats.churrosCantidad--;
        stats.AgregarDinero(precio);
        if (anim != null) anim.SetTrigger("attack");
        GameEvents.OnChurroVendido?.Invoke();
        return true;
    }

    public void Descansar()
    {
        if (stats != null) stats.RecuperarStamina(50f * Time.deltaTime);
    }
}
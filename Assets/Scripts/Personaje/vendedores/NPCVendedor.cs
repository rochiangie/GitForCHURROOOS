using UnityEngine;

public class NPCVendedor : MonoBehaviour
{
    public enum Especialidad { RegalarBebida, DarInformacion, Sabotear, GuardarMercancia }
    public Especialidad miHabilidad;

    public string nombreVendedor;
    public Dialogo dialogoMision;
    public float afinidad = 0;

    private PlayerStats pStats;
    private PlayerHealthSystem pHealth;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            pStats = player.GetComponent<PlayerStats>();
            pHealth = player.GetComponent<PlayerHealthSystem>();
        }
    }

    public void AbrirConversacion()
    {
        DialogoManager.Instance.IniciarDialogo(dialogoMision, this);
    }

    public void EfectoDecision(bool acepto)
    {
        if (acepto)
        {
            EjecutarHabilidadEspecial();
            afinidad += 15;
        }
        else
        {
            afinidad -= 10;
            Debug.Log(nombreVendedor + " se molestó contigo.");
        }
    }

    void EjecutarHabilidadEspecial()
    {
        switch (miHabilidad)
        {
            case Especialidad.RegalarBebida:
                // Doña Rosa te quita el calor de golpe
                pStats.hydration = 100;
                pStats.temperature = 0;
                Debug.Log("¡Doña Rosa te refrescó gratis!");
                break;

            case Especialidad.DarInformacion:
                // El Rayo te dice dónde hay más dinero (Sube el precio del próximo churro)
                Debug.Log("El Rayo te pasó un dato: Próxima venta vale el doble.");
                // Aquí podrías activar un multiplicador de dinero
                break;

            case Especialidad.Sabotear:
                // Un rival te quita dinero o te empuja
                pStats.money -= 20;
                Debug.Log("¡Te robaron mientras hablabas!");
                break;
        }
    }
}
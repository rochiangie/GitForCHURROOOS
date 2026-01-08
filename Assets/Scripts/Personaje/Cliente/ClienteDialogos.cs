using UnityEngine;

public class ClienteDialogos : MonoBehaviour
{
    // Listas de frases para dar variedad
    private string[] frasesAmistoso = {
        "¡Qué grandes los churros de Mardel! Tomá una propina, pibe.",
        "¡Uff, con este calor vienen bárbaro! Gracias.",
        "¡Justo lo que necesitaba para el mate!"
    };

    private string[] frasesMolestos = {
        "¡Están re caros! En el centro los consigo a mitad de precio...",
        "¿Tanto vas a tardar? Ya se me enfrió el agua del termo.",
        "Me llenaste de arena la lona, nene. Tené más cuidado."
    };

    private string[] frasesAyudante = {
        "Te veo colorado, pibe. Tomá un poco de mi agua, no te me vayas a desmayar.",
        "Descansá un toque acá en la sombra si querés, está pegando fuerte el sol."
    };

    private string[] frasesIndiferente = {
        "Bueno, dame uno rápido que me quiero meter al agua.",
        "Sí, sí... dejalo por ahí. ¿Cuánto es?"
    };

    public string ObtenerFrase(ClientePersonalidad.TipoCliente tipo)
    {
        switch (tipo)
        {
            case ClientePersonalidad.TipoCliente.Amistoso:
                return frasesAmistoso[Random.Range(0, frasesAmistoso.Length)];
            case ClientePersonalidad.TipoCliente.Molesto:
                return frasesMolestos[Random.Range(0, frasesMolestos.Length)];
            case ClientePersonalidad.TipoCliente.Ayudante:
                return frasesAyudante[Random.Range(0, frasesAyudante.Length)];
            default:
                return frasesIndiferente[Random.Range(0, frasesIndiferente.Length)];
        }
    }
}
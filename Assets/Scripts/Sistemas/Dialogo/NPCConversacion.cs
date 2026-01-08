using UnityEngine;

public class NPCConversacion : MonoBehaviour
{
    public bool esCliente;
    public bool esVendedor;
    public Dialogo archivoDialogo;

    [Header("Si es Vendedor")]
    public float precioAgua = 10f;
    public float recuperacionHidratacion = 30f;

    public void Interactuar()
    {
        if (archivoDialogo != null)
            DialogoManager.Instance.AbrirPanel(this);
    }
}
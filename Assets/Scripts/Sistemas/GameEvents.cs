using UnityEngine;

public static class GameEvents
{
    // Usamos el namespace completo para evitar conflictos
    public static System.Action OnGameOver;
    public static System.Action OnChurroVendido;
    public static System.Action<int> OnDayChanged;
    public static System.Action<int> OnChurroCountChanged;
    public static System.Action<float> OnMoneyChanged;
}
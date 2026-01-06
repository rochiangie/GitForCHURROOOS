using System;
using UnityEngine;

public static class GameEvents
{
    // Evento cuando se vende un churro (pasa la cantidad restante y la plata actual)
    public static event Action<int, int> OnChurroSold;
    public static void TriggerChurroSold(int restantes, int totalPlata) => OnChurroSold?.Invoke(restantes, totalPlata);

    // Evento cuando se toma una birra
    public static event Action OnBeerDrunk;
    public static void TriggerBeerDrunk() => OnBeerDrunk?.Invoke();

    // Evento cuando peleamos con alguien
    public static event Action OnFight;
    public static void TriggerFight() => OnFight?.Invoke();

    // Evento de cambio de día
    public static event Action<int> OnDayChanged;
    public static void TriggerDayChanged(int nuevoDia) => OnDayChanged?.Invoke(nuevoDia);

    // Evento de Game Over
    public static event Action OnGameOver;
    public static void TriggerGameOver() => OnGameOver?.Invoke();
}
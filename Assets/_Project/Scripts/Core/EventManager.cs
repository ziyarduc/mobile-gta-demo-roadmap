using System;
using UnityEngine;

namespace GTAClone.Core
{
    /// <summary>
    /// Oyun içi sistemlerin (Oyuncu, Araç, Kamera, UI, Bölge) birbirine sıkı sıkıya bağlı olmadan
    /// haberleşmesini sağlayan EventBus / EventManager sınıfı.
    /// </summary>
    public static class EventManager
    {
        // Durum Değişikliği Olayı
        public static event Action<GameState> OnGameStateChanged;

        // Araç Etkileşim Olayları
        public static event Action<GameObject> OnPlayerEnteredVehicle; // Parametre: Binilen Araç GameObject'i
        public static event Action<GameObject> OnPlayerExitedVehicle;  // Parametre: İnilen Araç GameObject'i

        // Bölge Satın Alma Olayları
        public static event Action<string, int> OnZonePurchased;      // Bölge Adı, Ücret

        // Envanter Olayları
        public static event Action OnInventoryUpdated;

        // Para Değişimi Olayı
        public static event Action<int> OnMoneyChanged;                // Yeni bakiye

        #region Event Tetikleyicileri (Trigger Methods)

        public static void TriggerGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }

        public static void TriggerPlayerEnteredVehicle(GameObject vehicle)
        {
            OnPlayerEnteredVehicle?.Invoke(vehicle);
        }

        public static void TriggerPlayerExitedVehicle(GameObject vehicle)
        {
            OnPlayerExitedVehicle?.Invoke(vehicle);
        }

        public static void TriggerZonePurchased(string zoneName, int cost)
        {
            OnZonePurchased?.Invoke(zoneName, cost);
        }

        public static void TriggerInventoryUpdated()
        {
            OnInventoryUpdated?.Invoke();
        }

        public static void TriggerMoneyChanged(int newBalance)
        {
            OnMoneyChanged?.Invoke(newBalance);
        }

        #endregion
    }
}

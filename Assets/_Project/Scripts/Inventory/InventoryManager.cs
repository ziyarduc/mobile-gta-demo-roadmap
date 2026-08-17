using System.Collections.Generic;
using UnityEngine;
using GTAClone.Core;

namespace GTAClone.Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int quantity;

        public InventorySlot(ItemData item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }

    /// <summary>
    /// Oyuncunun taşıdığı eşyaları ve envanter slotlarını yöneten sistem.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Capacity")]
        [SerializeField] private int maxSlots = 8;
        [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>();

        public IReadOnlyList<InventorySlot> Slots => inventorySlots;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public bool AddItem(ItemData item, int amount = 1)
        {
            // Mevcut stacklenebilir slot kontrolü
            foreach (var slot in inventorySlots)
            {
                if (slot.item == item && slot.quantity < item.maxStack)
                {
                    slot.quantity += amount;
                    EventManager.TriggerInventoryUpdated();
                    return true;
                }
            }

            // Yeni slot ekleme
            if (inventorySlots.Count < maxSlots)
            {
                inventorySlots.Add(new InventorySlot(item, amount));
                EventManager.TriggerInventoryUpdated();
                return true;
            }

            Debug.LogWarning("[Inventory] Envanter dolu!");
            return false;
        }

        public bool RemoveItem(ItemData item, int amount = 1)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].item == item)
                {
                    inventorySlots[i].quantity -= amount;
                    if (inventorySlots[i].quantity <= 0)
                    {
                        inventorySlots.RemoveAt(i);
                    }
                    EventManager.TriggerInventoryUpdated();
                    return true;
                }
            }
            return false;
        }
    }
}

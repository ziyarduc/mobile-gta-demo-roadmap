using UnityEngine;
using GTAClone.Core;
using GTAClone.Player;

namespace GTAClone.Zone
{
    /// <summary>
    /// Haritada bir bölge satın alma noktasına girildiğinde çalışan tetikleyici bileşen.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ZoneTrigger : MonoBehaviour
    {
        [Header("Zone Configuration")]
        [SerializeField] private ZoneData zoneData;

        private bool isPlayerInTrigger = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInTrigger = true;
                Debug.Log($"[Zone] '{zoneData.zoneName}' bölgesine girildi. Fiyat: ₺{zoneData.purchasePrice}. Satın almak için E/F'ye basın.");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInTrigger = false;
            }
        }

        private void Update()
        {
            if (isPlayerInTrigger && !zoneData.isPurchased)
            {
                if (PlayerInputHandler.Instance != null && PlayerInputHandler.Instance.IsInteractPressed)
                {
                    TryPurchaseZone();
                }
            }
        }

        private void TryPurchaseZone()
        {
            if (GameManager.Instance != null && GameManager.Instance.TrySpendMoney(zoneData.purchasePrice))
            {
                zoneData.isPurchased = true;
                EventManager.TriggerZonePurchased(zoneData.zoneName, zoneData.purchasePrice);
                Debug.Log($"[Zone] Tebrikler! '{zoneData.zoneName}' bölgesi başarıyla satın alındı!");
            }
            else
            {
                Debug.LogWarning("[Zone] Yetersiz bakiye! Bu bölgeyi satın alamazsınız.");
            }
        }
    }
}

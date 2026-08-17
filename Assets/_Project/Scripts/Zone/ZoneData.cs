using UnityEngine;

namespace GTAClone.Zone
{
    /// <summary>
    /// Satın alınabilir bir bölgeyi tanımlayan ScriptableObject veri yapısı.
    /// </summary>
    [CreateAssetMenu(fileName = "NewZoneData", menuName = "GTA/Zone Data")]
    public class ZoneData : ScriptableObject
    {
        public string zoneName = "San Andreas Bölgesi";
        [TextArea] public string description = "Ticari ve yerleşim alanı.";
        public int purchasePrice = 10000;
        public int dailyRevenue = 500;
        public bool isPurchased = false;
        public Color zoneMapColor = Color.green;
    }
}

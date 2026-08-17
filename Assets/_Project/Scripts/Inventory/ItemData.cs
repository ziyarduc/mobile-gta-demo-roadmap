using UnityEngine;

namespace GTAClone.Inventory
{
    public enum ItemType
    {
        Weapon,
        Consumable,
        KeyItem,
        Collectible
    }

    /// <summary>
    /// GTA envanterinde taşınabilir bir eşyayı tanımlayan ScriptableObject.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItemData", menuName = "GTA/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string itemName = "Tabanca";
        [TextArea] public string itemDescription = "9mm standart beylik tabancası.";
        public Sprite itemIcon;
        public ItemType itemType = ItemType.Weapon;
        public int maxStack = 1;
        public GameObject worldPrefab;
    }
}

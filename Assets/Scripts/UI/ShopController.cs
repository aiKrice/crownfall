using UnityEngine;

namespace Crownfall.UI
{
    /// <summary>
    /// Controls the Shop tab - handles shop logic and purchases
    /// </summary>
    public class ShopController : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("🏪 ShopController initialized - empty for now");
            // Future: Handle shop items, purchases, currency, etc.
        }
        
        // Placeholder for future shop functionality
        public void OnItemPurchased(string itemId)
        {
            Debug.Log($"💰 Item purchased: {itemId}");
        }
        
        public void RefreshShopItems()
        {
            Debug.Log("🔄 Refreshing shop items...");
        }
    }
}
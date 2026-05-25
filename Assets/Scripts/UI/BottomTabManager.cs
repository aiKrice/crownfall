using UnityEngine;
using UnityEngine.UI;

namespace Crownfall.UI
{
    /// <summary>
    /// Manages bottom tab navigation only - no business logic
    /// </summary>
    public class BottomTabManager : MonoBehaviour
    {
        [Header("Tab Panels")]
        public GameObject shopPanel;
        public GameObject homePanel; 
        public GameObject libraryPanel;
        
        [Header("Tab Buttons")]
        public Button shopTabButton;
        public Button homeTabButton;
        public Button libraryTabButton;
        
        private string currentTab = "home";
        
        private void Start()
        {
            Debug.Log("📱 BottomTabManager initialized");
            
            // Show home tab by default
            ShowTab("home");
        }
        
        // ✅ APPELÉ DEPUIS L'INSPECTOR UNITY
        public void OnShopTabClicked()
        {
            if (currentTab == "shop")
            {
                Debug.Log("📱 Shop tab already selected");
                return;
            }
            ShowTab("shop");
        }
        
        // ✅ APPELÉ DEPUIS L'INSPECTOR UNITY
        public void OnHomeTabClicked()
        {
            if (currentTab == "home")
            {
                Debug.Log("🏠 Home tab already selected");
                return;
            }
            ShowTab("home");
        }
        
        // ✅ APPELÉ DEPUIS L'INSPECTOR UNITY
        public void OnLibraryTabClicked()
        {
            if (currentTab == "library")
            {
                Debug.Log("📚 Library tab already selected");
                return;
            }
            ShowTab("library");
        }
        
        private void ShowTab(string tabName)
        {
            currentTab = tabName.ToLower();
            
            // Hide all panels
            if (shopPanel != null) shopPanel.SetActive(false);
            if (homePanel != null) homePanel.SetActive(false);
            if (libraryPanel != null) libraryPanel.SetActive(false);
            
            // Reset all button colors
            if (shopTabButton != null) SetTabButtonColor(shopTabButton, false);
            if (homeTabButton != null) SetTabButtonColor(homeTabButton, false);
            if (libraryTabButton != null) SetTabButtonColor(libraryTabButton, false);
            
            // Show selected panel and highlight button
            switch (currentTab)
            {
                case "shop":
                    if (shopPanel != null) shopPanel.SetActive(true);
                    if (shopTabButton != null) SetTabButtonColor(shopTabButton, true);
                    Debug.Log("📱 Switched to Shop tab");
                    break;
                case "home":
                    if (homePanel != null) homePanel.SetActive(true);
                    if (homeTabButton != null) SetTabButtonColor(homeTabButton, true);
                    Debug.Log("🏠 Switched to Home tab");
                    break;
                case "library":
                    if (libraryPanel != null) libraryPanel.SetActive(true);
                    if (libraryTabButton != null) SetTabButtonColor(libraryTabButton, true);
                    Debug.Log("📚 Switched to Library tab");
                    break;
            }
        }
        
        private void SetTabButtonColor(Button button, bool selected)
        {
            if (button == null) return;
            
            var colors = button.colors;
            Color blueActive = new Color(0.2f, 0.6f, 1f, 1f);  // Bleu
            Color grayInactive = new Color(0.7f, 0.7f, 0.7f, 1f);  // Gris
            
            if (selected)
            {
                colors.normalColor = blueActive;
                colors.highlightedColor = blueActive;
                colors.pressedColor = blueActive;
                colors.selectedColor = blueActive;
            }
            else
            {
                colors.normalColor = grayInactive;
                colors.highlightedColor = grayInactive;
                colors.pressedColor = grayInactive;
                colors.selectedColor = grayInactive;
            }
            
            button.colors = colors;
        }
    }
}
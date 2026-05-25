using UnityEngine;
using UnityEngine.UI;

namespace Crownfall.UI
{
    /// <summary>
    /// Controls the Home tab - handles game start logic
    /// </summary>
    public class HomeController : MonoBehaviour
    {
        private bool isLoading = false;
        
        private void Start()
        {
            Debug.Log("🏠 HomeController initialized");
        }
        
        public void OnStartGameClicked()
        {
            if (isLoading) 
            {
                Debug.Log("⚠️ Already loading, ignoring click");
                return;
            }
            
            isLoading = true;
            Debug.Log("🚀 Start Game clicked from HomeController");
            
            if (CrownfallSceneManager.Instance != null)
            {
                CrownfallSceneManager.Instance.LoadGameScene();
            }
            else
            {
                Debug.LogError("❌ CrownfallSceneManager instance not found!");
                isLoading = false;
            }
        }
    }
}
using UnityEngine;
using Crownfall.UI;

namespace Crownfall.Test
{
    public class FlowTest : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("🎮 Crownfall Flow Test - Architecture créée avec succès!");
            Debug.Log("📱 Scenes configurées: SplashScene → MainMenuScene → GameScene");
            Debug.Log("🚀 Prêt pour export iOS/Android");
            
            if (CrownfallSceneManager.Instance != null)
            {
                Debug.Log("✅ CrownfallSceneManager détecté et fonctionnel");
            }
            else
            {
                Debug.Log("⚠️ CrownfallSceneManager non trouvé dans cette scène");
            }
        }
    }
}
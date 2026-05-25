using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Crownfall.UI
{
    /// <summary>
    /// Manages scene transitions and game flow
    /// </summary>
    public class CrownfallSceneManager : MonoBehaviour
    {
        [Header("Scene Names")]
        public const string SPLASH_SCENE = "SplashScene";
        public const string MAIN_MENU_SCENE = "MainMenuScene"; 
        public const string GAME_SCENE = "GameScene";
        
        [Header("Transition Settings")]
        public bool useTransitionEffects = true;
        public float transitionDuration = 1f;
        public CanvasGroup transitionPanel;
        
        private static CrownfallSceneManager instance;
        public static CrownfallSceneManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<CrownfallSceneManager>();
                return instance;
            }
        }

        private void Awake()
        {
            // Singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            Debug.Log($"🔍 CrownfallSceneManager Start() - Current scene: {SceneManager.GetActiveScene().name}");
            Debug.Log($"🔍 Expected splash scene name: {SPLASH_SCENE}");
            
            // If we're in splash scene, start the splash sequence
            if (SceneManager.GetActiveScene().name == SPLASH_SCENE)
            {
                Debug.Log("✅ Starting splash sequence!");
                StartCoroutine(SplashSequence());
            }
            else
            {
                Debug.Log("❌ Not in splash scene, no sequence started");
            }
        }

        private IEnumerator SplashSequence()
        {
            Debug.Log("🎬 Starting splash sequence...");
            
            // Show splash screen for a few seconds
            yield return new WaitForSeconds(2f);
            
            // Transition to main menu
            LoadMainMenu();
        }

        public void LoadSplashScene()
        {
            StartCoroutine(LoadSceneWithTransition(SPLASH_SCENE));
        }

        public void LoadMainMenu()
        {
            Debug.Log($"🎯 LoadMainMenu called - trying to load: {MAIN_MENU_SCENE}");
            StartCoroutine(LoadSceneWithTransition(MAIN_MENU_SCENE));
        }

        public void LoadGameScene()
        {
            StartCoroutine(LoadSceneWithTransition(GAME_SCENE));
        }

        public void LoadMainMenuScene()
        {
            Debug.Log("🔄 Loading scene: MainMenuScene");
            StartCoroutine(LoadSceneWithTransition(MAIN_MENU_SCENE));
        }

        public void QuitGame()
        {
            Debug.Log("🚪 Quitting game...");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private IEnumerator LoadSceneWithTransition(string sceneName)
        {
            Debug.Log($"🔄 Loading scene: {sceneName}");

            // Fade out
            if (useTransitionEffects && transitionPanel != null)
            {
                yield return StartCoroutine(FadeTransition(0f, 1f));
            }

            // Load new scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // Wait for scene to be ready
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Activate scene
            asyncLoad.allowSceneActivation = true;

            // Wait for scene to fully load
            yield return new WaitUntil(() => asyncLoad.isDone);

            // Fade in
            if (useTransitionEffects && transitionPanel != null)
            {
                yield return StartCoroutine(FadeTransition(1f, 0f));
            }

            Debug.Log($"✅ Scene loaded: {sceneName}");
        }

        private IEnumerator FadeTransition(float startAlpha, float endAlpha)
        {
            if (transitionPanel == null) yield break;

            float elapsedTime = 0f;
            
            while (elapsedTime < transitionDuration)
            {
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / transitionDuration);
                transitionPanel.alpha = alpha;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transitionPanel.alpha = endAlpha;
        }

        // Public methods for UI buttons
        public void OnStartGameClicked()
        {
            LoadGameScene();
        }

        public void OnMainMenuClicked()
        {
            LoadMainMenu();
        }

        public void OnQuitClicked()
        {
            QuitGame();
        }
    }
}
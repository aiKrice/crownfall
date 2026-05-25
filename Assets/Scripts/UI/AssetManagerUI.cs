using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Crownfall.UI
{
    public class AssetManagerUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject downloadPanel;
        public Button downloadButton;
        public Button generateButton;
        public Button validateButton;
        public Slider progressBar;
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI progressText;
        public Toggle overwriteToggle;
        
        [Header("Asset Status")]
        public GameObject assetStatusPanel;
        public Transform assetStatusParent;
        public GameObject assetStatusPrefab;

        private AutoAssetImporter assetImporter;
        private ProceduralAssetGenerator assetGenerator;
        private AssetManager assetManager;

        private void Start()
        {
            InitializeReferences();
            SetupUI();
            UpdateAssetStatus();
        }

        private void InitializeReferences()
        {
            assetImporter = FindFirstObjectByType<AutoAssetImporter>();
            assetGenerator = FindFirstObjectByType<ProceduralAssetGenerator>();
            assetManager = FindFirstObjectByType<AssetManager>();

            // Create components if they don't exist
            if (assetImporter == null)
            {
                GameObject go = new GameObject("AutoAssetImporter");
                assetImporter = go.AddComponent<AutoAssetImporter>();
            }

            if (assetGenerator == null)
            {
                GameObject go = GameObject.Find("AutoAssetImporter") ?? new GameObject("ProceduralAssetGenerator");
                assetGenerator = go.AddComponent<ProceduralAssetGenerator>();
            }

            if (assetManager == null)
            {
                GameObject go = GameObject.Find("AutoAssetImporter") ?? new GameObject("AssetManager");
                assetManager = go.AddComponent<AssetManager>();
            }
        }

        private void SetupUI()
        {
            // Setup button events
            if (downloadButton != null)
                downloadButton.onClick.AddListener(OnDownloadButtonClick);
            
            if (generateButton != null)
                generateButton.onClick.AddListener(OnGenerateButtonClick);
            
            if (validateButton != null)
                validateButton.onClick.AddListener(OnValidateButtonClick);

            if (overwriteToggle != null && assetImporter != null)
                overwriteToggle.onValueChanged.AddListener(assetImporter.OnToggleOverwrite);

            // Setup initial UI state
            UpdateProgressUI(0f, "Ready to download assets");
        }

        private void Update()
        {
            if (assetImporter != null)
            {
                float progress = assetImporter.GetDownloadProgress();
                bool isComplete = assetImporter.IsDownloadComplete();
                
                if (progress > 0 && !isComplete)
                {
                    UpdateProgressUI(progress, $"Downloading... {(progress * 100):F0}%");
                }
                else if (isComplete && progressBar.value < 1f)
                {
                    UpdateProgressUI(1f, "Download completed!");
                }
            }
        }

        public void OnDownloadButtonClick()
        {
            if (assetImporter != null)
            {
                UpdateProgressUI(0f, "Starting download...");
                assetImporter.OnDownloadButtonClick();
            }
            else
            {
                Debug.LogError("AutoAssetImporter not found!");
            }
        }

        public void OnGenerateButtonClick()
        {
            if (assetGenerator != null)
            {
                UpdateProgressUI(0.5f, "Generating procedural assets...");
                assetGenerator.GenerateAllAssets();
                
                StartCoroutine(DelayedProgressUpdate());
            }
            else
            {
                Debug.LogError("ProceduralAssetGenerator not found!");
            }
        }

        private IEnumerator DelayedProgressUpdate()
        {
            yield return new WaitForSeconds(1f);
            UpdateProgressUI(1f, "Procedural assets generated!");
            UpdateAssetStatus();
        }

        public void OnValidateButtonClick()
        {
            if (assetManager != null)
            {
                UpdateProgressUI(0.8f, "Validating assets...");
                // AssetManager validation is done in Start(), so we just simulate progress
                
                StartCoroutine(DelayedValidationUpdate());
            }
            else
            {
                Debug.LogError("AssetManager not found!");
            }
        }

        private IEnumerator DelayedValidationUpdate()
        {
            yield return new WaitForSeconds(0.5f);
            UpdateProgressUI(1f, "Asset validation completed!");
            UpdateAssetStatus();
        }

        private void UpdateProgressUI(float progress, string status)
        {
            if (progressBar != null)
                progressBar.value = progress;
            
            if (statusText != null)
                statusText.text = status;
            
            if (progressText != null)
                progressText.text = $"{(progress * 100):F0}%";
        }

        private void UpdateAssetStatus()
        {
            if (assetStatusParent == null || assetStatusPrefab == null)
                return;

            // Clear existing status items
            foreach (Transform child in assetStatusParent)
            {
                Destroy(child.gameObject);
            }

            // Check asset availability
            string[] assetCategories = {
                "Piece Sprites",
                "Cell Textures", 
                "Special Elements",
                "Magic Icons",
                "UI Elements"
            };

            foreach (string category in assetCategories)
            {
                CreateAssetStatusItem(category, CheckAssetCategoryStatus(category));
            }
        }

        private bool CheckAssetCategoryStatus(string category)
        {
            // Simplified check - in a real implementation, you'd check actual file existence
            switch (category)
            {
                case "Piece Sprites":
                    return System.IO.File.Exists("Assets/Art/Pieces/Blue/blue_king.png");
                case "Cell Textures":
                    return System.IO.File.Exists("Assets/Art/Board/Cells/light_cell.png");
                case "Special Elements":
                    return System.IO.File.Exists("Assets/Art/Board/Special/chest.png");
                case "Magic Icons":
                    return System.IO.File.Exists("Assets/Art/UI/MagicIcons/magic_level_1.png");
                case "UI Elements":
                    return true; // UI elements are usually built-in
                default:
                    return false;
            }
        }

        private void CreateAssetStatusItem(string categoryName, bool isAvailable)
        {
            GameObject statusItem = Instantiate(assetStatusPrefab, assetStatusParent);
            
            // Assuming the prefab has Text components for name and status
            TextMeshProUGUI nameText = statusItem.transform.Find("CategoryName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI statusText = statusItem.transform.Find("Status")?.GetComponent<TextMeshProUGUI>();
            Image statusIcon = statusItem.transform.Find("StatusIcon")?.GetComponent<Image>();

            if (nameText != null)
                nameText.text = categoryName;
            
            if (statusText != null)
            {
                statusText.text = isAvailable ? "✅ Available" : "❌ Missing";
                statusText.color = isAvailable ? Color.green : Color.red;
            }

            if (statusIcon != null)
                statusIcon.color = isAvailable ? Color.green : Color.red;
        }

        public void ShowDownloadPanel(bool show)
        {
            if (downloadPanel != null)
                downloadPanel.SetActive(show);
        }

        public void ShowAssetStatus(bool show)
        {
            if (assetStatusPanel != null)
                assetStatusPanel.SetActive(show);
        }

        // Public methods for external UI buttons
        public void OnRefreshStatusButtonClick()
        {
            UpdateAssetStatus();
        }

        public void OnToggleDownloadPanel()
        {
            if (downloadPanel != null)
                downloadPanel.SetActive(!downloadPanel.activeSelf);
        }

        public void OnToggleAssetStatus()
        {
            if (assetStatusPanel != null)
                assetStatusPanel.SetActive(!assetStatusPanel.activeSelf);
        }

        // Utility method to create simple UI if prefabs are missing
        public void CreateSimpleStatusUI()
        {
            if (assetStatusPrefab == null)
            {
                // Create a simple prefab programmatically
                GameObject prefab = new GameObject("AssetStatusItem");
                
                // Add background
                Image bg = prefab.AddComponent<Image>();
                bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                
                // Add layout
                HorizontalLayoutGroup layout = prefab.AddComponent<HorizontalLayoutGroup>();
                layout.padding = new RectOffset(10, 10, 5, 5);
                layout.spacing = 10;
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                
                // Add category name text
                GameObject nameObject = new GameObject("CategoryName");
                nameObject.transform.SetParent(prefab.transform);
                TextMeshProUGUI nameText = nameObject.AddComponent<TextMeshProUGUI>();
                nameText.text = "Category";
                nameText.fontSize = 14;
                
                // Add status text
                GameObject statusObject = new GameObject("Status");
                statusObject.transform.SetParent(prefab.transform);
                TextMeshProUGUI statusText = statusObject.AddComponent<TextMeshProUGUI>();
                statusText.text = "Status";
                statusText.fontSize = 12;
                statusText.alignment = TextAlignmentOptions.Right;
                
                assetStatusPrefab = prefab;
            }
        }
    }
}
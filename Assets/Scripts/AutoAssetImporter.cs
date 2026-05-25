using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AssetDownload
{
    public string name;
    public string url;
    public string localPath;
    public bool isDownloaded;
    public AssetType assetType;
}

public enum AssetType
{
    PieceSprite,
    CellTexture,
    UIElement,
    SpecialEffect,
    Audio
}

public class AutoAssetImporter : MonoBehaviour
{
    [Header("Download Settings")]
    public bool downloadOnStart = false;
    public bool overwriteExisting = false;
    
    [Header("Download Progress")]
    public UnityEngine.UI.Slider progressBar;
    public UnityEngine.UI.Text statusText;
    
    [Header("Asset URLs")]
    public List<AssetDownload> assetsToDownload = new List<AssetDownload>();

    private int currentDownloadIndex = 0;
    private bool isDownloading = false;

    // Free asset URLs (these are examples - you'll need to find actual working URLs)
    private readonly Dictionary<string, string> freeAssetUrls = new Dictionary<string, string>()
    {
        // Chess piece icons from game-icons.net (SVG format)
        ["blue_king"] = "https://game-icons.net/icons/ffffff/000000/1x1/delapouite/chess-king.svg",
        ["blue_queen"] = "https://game-icons.net/icons/ffffff/000000/1x1/delapouite/chess-queen.svg",
        ["blue_rook"] = "https://game-icons.net/icons/ffffff/000000/1x1/delapouite/chess-rook.svg",
        ["blue_bishop"] = "https://game-icons.net/icons/ffffff/000000/1x1/delapouite/chess-bishop.svg",
        ["blue_knight"] = "https://game-icons.net/icons/ffffff/000000/1x1/delapouite/chess-knight.svg",
        ["blue_pawn"] = "https://game-icons.net/icons/ffffff/000000/1x1/delapouite/chess-pawn.svg",
        
        // Red pieces (different color)
        ["red_king"] = "https://game-icons.net/icons/ff0000/000000/1x1/delapouite/chess-king.svg",
        ["red_queen"] = "https://game-icons.net/icons/ff0000/000000/1x1/delapouite/chess-queen.svg",
        ["red_rook"] = "https://game-icons.net/icons/ff0000/000000/1x1/delapouite/chess-rook.svg",
        ["red_bishop"] = "https://game-icons.net/icons/ff0000/000000/1x1/delapouite/chess-bishop.svg",
        ["red_knight"] = "https://game-icons.net/icons/ff0000/000000/1x1/delapouite/chess-knight.svg",
        ["red_pawn"] = "https://game-icons.net/icons/ff0000/000000/1x1/delapouite/chess-pawn.svg",
        
        // Special elements
        ["chest"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/chest.svg",
        ["seal"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/magic-swirl.svg",
        ["tomb"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/tombstone.svg",
        
        // Magic icons
        ["magic_level_1"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/fire-ball.svg",
        ["magic_level_2"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/ice-shard.svg",
        ["magic_level_3"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/lightning-bolt.svg",
        ["magic_level_4"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/earth-spit.svg",
        ["magic_level_5"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/wind-hole.svg",
        ["magic_level_6"] = "https://game-icons.net/icons/ffffff/000000/1x1/lorc/dark-squad.svg",
        
        // Procedural textures (using placeholder services)
        ["light_cell"] = "https://via.placeholder.com/256x256/FFFFFF/CCCCCC?text=Light",
        ["dark_cell"] = "https://via.placeholder.com/256x256/888888/444444?text=Dark",
        ["border_cell"] = "https://via.placeholder.com/256x256/000000/333333?text=Border",
        ["cemetery_cell"] = "https://via.placeholder.com/256x256/8B4513/654321?text=Cemetery",
        ["promotion_cell"] = "https://via.placeholder.com/256x256/FFD700/FFA500?text=Promotion"
    };

    private void Start()
    {
        InitializeAssetList();
        
        if (downloadOnStart)
        {
            StartDownload();
        }
    }

    private void InitializeAssetList()
    {
        if (assetsToDownload.Count == 0)
        {
            // Add all predefined assets
            foreach (var kvp in freeAssetUrls)
            {
                assetsToDownload.Add(new AssetDownload
                {
                    name = kvp.Key,
                    url = kvp.Value,
                    localPath = GetAssetPath(kvp.Key),
                    isDownloaded = false,
                    assetType = GetAssetType(kvp.Key)
                });
            }
        }
    }

    private string GetAssetPath(string assetName)
    {
        string folder = "";
        
        if (assetName.Contains("king") || assetName.Contains("queen") || assetName.Contains("rook") || 
            assetName.Contains("bishop") || assetName.Contains("knight") || assetName.Contains("pawn"))
        {
            folder = assetName.StartsWith("blue") ? "Art/Pieces/Blue/" : "Art/Pieces/Red/";
        }
        else if (assetName.Contains("cell"))
        {
            folder = "Art/Board/Cells/";
        }
        else if (assetName.Contains("magic"))
        {
            folder = "Art/UI/MagicIcons/";
        }
        else
        {
            folder = "Art/Board/Special/";
        }

        string extension = assetName.Contains("cell") ? ".png" : ".svg";
        return $"Assets/{folder}{assetName}{extension}";
    }

    private AssetType GetAssetType(string assetName)
    {
        if (assetName.Contains("king") || assetName.Contains("queen") || assetName.Contains("rook") || 
            assetName.Contains("bishop") || assetName.Contains("knight") || assetName.Contains("pawn"))
        {
            return AssetType.PieceSprite;
        }
        else if (assetName.Contains("cell"))
        {
            return AssetType.CellTexture;
        }
        else if (assetName.Contains("magic"))
        {
            return AssetType.UIElement;
        }
        else
        {
            return AssetType.SpecialEffect;
        }
    }

    public void StartDownload()
    {
        if (isDownloading)
        {
            Debug.LogWarning("Download already in progress!");
            return;
        }

        StartCoroutine(DownloadAllAssets());
    }

    private IEnumerator DownloadAllAssets()
    {
        isDownloading = true;
        currentDownloadIndex = 0;

        CreateDirectories();

        for (int i = 0; i < assetsToDownload.Count; i++)
        {
            currentDownloadIndex = i;
            var asset = assetsToDownload[i];

            UpdateStatus($"Downloading {asset.name}... ({i + 1}/{assetsToDownload.Count})");
            UpdateProgress((float)i / assetsToDownload.Count);

            // Check if file already exists
            if (File.Exists(asset.localPath) && !overwriteExisting)
            {
                Debug.Log($"Skipping {asset.name} - already exists");
                asset.isDownloaded = true;
                continue;
            }

            yield return StartCoroutine(DownloadAsset(asset));
            
            // Small delay between downloads to be respectful to servers
            yield return new WaitForSeconds(0.5f);
        }

        UpdateStatus("Download completed!");
        UpdateProgress(1f);
        
        isDownloading = false;

        #if UNITY_EDITOR
        // Refresh asset database in editor
        AssetDatabase.Refresh();
        ConfigureImportedAssets();
        #endif
    }

    private IEnumerator DownloadAsset(AssetDownload asset)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(asset.url))
        {
            // Set user agent to avoid blocking
            www.SetRequestHeader("User-Agent", "Unity-Game-Engine");
            
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    File.WriteAllBytes(asset.localPath, www.downloadHandler.data);
                    asset.isDownloaded = true;
                    Debug.Log($"✅ Downloaded: {asset.name}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"❌ Failed to save {asset.name}: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"❌ Failed to download {asset.name}: {www.error}");
                
                // Try to create a placeholder file
                CreatePlaceholderAsset(asset);
            }
        }
    }

    private void CreatePlaceholderAsset(AssetDownload asset)
    {
        try
        {
            if (asset.assetType == AssetType.CellTexture)
            {
                // Create a simple colored texture
                Texture2D placeholder = CreateColorTexture(256, 256, GetPlaceholderColor(asset.name));
                byte[] pngData = placeholder.EncodeToPNG();
                File.WriteAllBytes(asset.localPath, pngData);
                
                DestroyImmediate(placeholder);
                asset.isDownloaded = true;
                
                Debug.Log($"🎨 Created placeholder for: {asset.name}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create placeholder for {asset.name}: {e.Message}");
        }
    }

    private Color GetPlaceholderColor(string assetName)
    {
        switch (assetName)
        {
            case "light_cell": return Color.white;
            case "dark_cell": return Color.gray;
            case "border_cell": return Color.black;
            case "cemetery_cell": return new Color(0.5f, 0.3f, 0.1f); // Brown
            case "promotion_cell": return Color.yellow;
            default: return Color.magenta; // Error color
        }
    }

    private Texture2D CreateColorTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }

    private void CreateDirectories()
    {
        var directories = new HashSet<string>();
        
        foreach (var asset in assetsToDownload)
        {
            string dir = Path.GetDirectoryName(asset.localPath);
            directories.Add(dir);
        }
        
        foreach (string dir in directories)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Debug.Log($"📁 Created directory: {dir}");
            }
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        
        Debug.Log($"📦 {message}");
    }

    private void UpdateProgress(float progress)
    {
        if (progressBar != null)
            progressBar.value = progress;
    }

    #if UNITY_EDITOR
    private void ConfigureImportedAssets()
    {
        foreach (var asset in assetsToDownload)
        {
            if (!asset.isDownloaded) continue;

            string assetPath = asset.localPath;
            
            // Configure import settings based on asset type
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            
            if (importer is TextureImporter texImporter)
            {
                switch (asset.assetType)
                {
                    case AssetType.PieceSprite:
                    case AssetType.UIElement:
                        texImporter.textureType = TextureImporterType.Sprite;
                        texImporter.spriteImportMode = SpriteImportMode.Single;
                        texImporter.filterMode = FilterMode.Bilinear;
                        texImporter.maxTextureSize = 512;
                        break;
                        
                    case AssetType.CellTexture:
                        texImporter.textureType = TextureImporterType.Default;
                        texImporter.wrapMode = TextureWrapMode.Repeat;
                        texImporter.filterMode = FilterMode.Bilinear;
                        texImporter.maxTextureSize = 256;
                        break;
                }
                
                texImporter.SaveAndReimport();
            }
        }
        
        Debug.Log("🎯 Asset import settings configured!");
    }

    [MenuItem("Crownfall/Download Assets")]
    public static void MenuDownloadAssets()
    {
        var importer = FindFirstObjectByType<AutoAssetImporter>();
        if (importer != null)
        {
            importer.StartDownload();
        }
        else
        {
            Debug.LogError("No AutoAssetImporter found in scene!");
        }
    }

    [MenuItem("Crownfall/Create Asset Folders")]
    public static void MenuCreateFolders()
    {
        string[] folders = {
            "Assets/Art",
            "Assets/Art/Pieces",
            "Assets/Art/Pieces/Blue",
            "Assets/Art/Pieces/Red",
            "Assets/Art/Board",
            "Assets/Art/Board/Cells",
            "Assets/Art/Board/Special",
            "Assets/Art/UI",
            "Assets/Art/UI/MagicIcons",
            "Assets/Prefabs",
            "Assets/Materials",
            "Assets/Audio"
        };

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parentFolder = Path.GetDirectoryName(folder).Replace('\\', '/');
                string folderName = Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parentFolder, folderName);
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("📁 Asset folders created!");
    }
    #endif

    // Public methods for UI buttons
    public void OnDownloadButtonClick()
    {
        StartDownload();
    }

    public void OnToggleOverwrite(bool value)
    {
        overwriteExisting = value;
    }

    // Add custom asset download
    public void AddCustomAsset(string name, string url, AssetType type)
    {
        assetsToDownload.Add(new AssetDownload
        {
            name = name,
            url = url,
            localPath = GetAssetPath(name),
            isDownloaded = false,
            assetType = type
        });
    }

    // Check download status
    public float GetDownloadProgress()
    {
        if (assetsToDownload.Count == 0) return 1f;
        
        int downloadedCount = 0;
        foreach (var asset in assetsToDownload)
        {
            if (asset.isDownloaded) downloadedCount++;
        }
        
        return (float)downloadedCount / assetsToDownload.Count;
    }

    public bool IsDownloadComplete()
    {
        return GetDownloadProgress() >= 1f;
    }
}
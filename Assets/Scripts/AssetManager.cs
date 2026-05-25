using UnityEngine;
using System.Collections.Generic;
using Crownfall.Board;

[System.Serializable]
public class PieceAssets
{
    [Header("Blue Pieces")]
    public Sprite blueKing;
    public Sprite blueQueen;
    public Sprite blueOrkMan;
    public Sprite blueParrot;
    public Sprite blueKnight;
    public Sprite blueLion;
    public Sprite blueMagician;

    [Header("Red Pieces")]
    public Sprite redKing;
    public Sprite redQueen;
    public Sprite redOrkMan;
    public Sprite redParrot;
    public Sprite redKnight;
    public Sprite redLion;
    public Sprite redMagician;

    public Sprite GetPieceSprite(PieceType type, PlayerColor color)
    {
        switch (color)
        {
            case PlayerColor.Blue:
                switch (type)
                {
                    case PieceType.King: return blueKing;
                    case PieceType.Queen: return blueQueen;
                    case PieceType.OrkMan: return blueOrkMan;
                    case PieceType.Parrot: return blueParrot;
                    case PieceType.Knight: return blueKnight;
                    case PieceType.Lion: return blueLion;
                    case PieceType.Magician: return blueMagician;
                }
                break;
            case PlayerColor.Red:
                switch (type)
                {
                    case PieceType.King: return redKing;
                    case PieceType.Queen: return redQueen;
                    case PieceType.OrkMan: return redOrkMan;
                    case PieceType.Parrot: return redParrot;
                    case PieceType.Knight: return redKnight;
                    case PieceType.Lion: return redLion;
                    case PieceType.Magician: return redMagician;
                }
                break;
        }
        return null;
    }
}

[System.Serializable]
public class BoardAssets
{
    [Header("Cell Textures")]
    public Material lightCellMaterial;
    public Material darkCellMaterial;
    public Material borderCellMaterial;
    public Material cemeteryCellMaterial;
    public Material promotionCellMaterial;

    [Header("Special Elements")]
    public Sprite chestSprite;
    public Sprite sealSprite;
    public Sprite tombSprite;

    [Header("Magic Icons")]
    public Sprite[] magicIcons = new Sprite[7]; // Index 0-6 for magic levels
}

[System.Serializable]
public class UIAssets
{
    [Header("Health Bar")]
    public Sprite healthBarBackground;
    public Sprite healthBarFill;

    [Header("Buttons")]
    public Sprite buttonNormal;
    public Sprite buttonPressed;
    public Sprite buttonHighlight;

    [Header("Effects")]
    public Sprite promotionGlow;
    public Sprite selectionHighlight;
}

public class AssetManager : MonoBehaviour
{
    [Header("Asset Collections")]
    public PieceAssets pieces;
    public BoardAssets board;
    public UIAssets ui;

    [Header("Prefabs")]
    public GameObject cellPrefab;
    public GameObject piecePrefab;
    public GameObject chestPrefab;
    public GameObject sealPrefab;

    private static AssetManager instance;
    public static AssetManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<AssetManager>();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ValidateAssets();
    }

    private void ValidateAssets()
    {
        bool hasErrors = false;

        // Check piece sprites
        for (int i = 0; i < 6; i++)
        {
            PieceType type = (PieceType)i;
            if (pieces.GetPieceSprite(type, PlayerColor.Blue) == null)
            {
                Debug.LogWarning($"Missing blue {type} sprite!");
                hasErrors = true;
            }
            if (pieces.GetPieceSprite(type, PlayerColor.Red) == null)
            {
                Debug.LogWarning($"Missing red {type} sprite!");
                hasErrors = true;
            }
        }

        // Check board materials
        if (board.lightCellMaterial == null) { Debug.LogWarning("Missing light cell material!"); hasErrors = true; }
        if (board.darkCellMaterial == null) { Debug.LogWarning("Missing dark cell material!"); hasErrors = true; }
        if (board.borderCellMaterial == null) { Debug.LogWarning("Missing border cell material!"); hasErrors = true; }

        // Check prefabs
        if (cellPrefab == null) { Debug.LogWarning("Missing cell prefab!"); hasErrors = true; }
        if (piecePrefab == null) { Debug.LogWarning("Missing piece prefab!"); hasErrors = true; }

        if (!hasErrors)
        {
            Debug.Log("✅ All assets validated successfully!");
        }
        else
        {
            Debug.LogError("⚠️ Some assets are missing! Check the Asset Manager configuration.");
        }
    }

    public Sprite GetPieceSprite(PieceType type, PlayerColor color)
    {
        return pieces.GetPieceSprite(type, color);
    }

    public Material GetCellMaterial(CellType type, CellColor color)
    {
        switch (type)
        {
            case CellType.Border: return board.borderCellMaterial;
            case CellType.Cemetery: return board.cemeteryCellMaterial;
            case CellType.Promotion: return board.promotionCellMaterial;
            default:
                return color == CellColor.Light ? board.lightCellMaterial : board.darkCellMaterial;
        }
    }

    public Sprite GetMagicIcon(MagicLevel level)
    {
        int index = (int)level;
        if (index >= 0 && index < board.magicIcons.Length)
            return board.magicIcons[index];
        return null;
    }

    // Utility methods for creating assets programmatically if missing
    public void CreateBasicMaterials()
    {
        if (board.lightCellMaterial == null)
        {
            board.lightCellMaterial = new Material(Shader.Find("Standard"));
            board.lightCellMaterial.color = Color.white;
            board.lightCellMaterial.name = "LightCellMaterial";
        }

        if (board.darkCellMaterial == null)
        {
            board.darkCellMaterial = new Material(Shader.Find("Standard"));
            board.darkCellMaterial.color = new Color(0.3f, 0.3f, 0.3f);
            board.darkCellMaterial.name = "DarkCellMaterial";
        }

        if (board.borderCellMaterial == null)
        {
            board.borderCellMaterial = new Material(Shader.Find("Standard"));
            board.borderCellMaterial.color = Color.black;
            board.borderCellMaterial.name = "BorderCellMaterial";
        }

        Debug.Log("Basic materials created!");
    }

    public void CreateBasicPrefabs()
    {
        if (cellPrefab == null)
        {
            GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cell.transform.localScale = new Vector3(1f, 0.1f, 1f);
            cell.name = "CellPrefab";
            
            // Add CellDisplay component
            cell.AddComponent<CellDisplay>();
            
            // This would need to be saved as a prefab manually
            Debug.Log("Basic cell prefab created! Save it as a prefab in Assets/Prefabs/");
        }

        if (piecePrefab == null)
        {
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            piece.transform.localScale = new Vector3(0.8f, 0.3f, 0.8f);
            piece.name = "PiecePrefab";
            
            // Add PieceDisplay component
            piece.AddComponent<PieceDisplay>();
            
            Debug.Log("Basic piece prefab created! Save it as a prefab in Assets/Prefabs/");
        }
    }

    // Editor helper methods
    #if UNITY_EDITOR
    [ContextMenu("Create Basic Materials")]
    public void EditorCreateBasicMaterials()
    {
        CreateBasicMaterials();
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    [ContextMenu("Create Basic Prefabs")]
    public void EditorCreateBasicPrefabs()
    {
        CreateBasicPrefabs();
    }

    [ContextMenu("Validate All Assets")]
    public void EditorValidateAssets()
    {
        ValidateAssets();
    }
    #endif
}
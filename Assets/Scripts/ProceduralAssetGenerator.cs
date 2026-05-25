using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Generates procedural assets when downloads fail or as fallback options
/// </summary>
public class ProceduralAssetGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public int textureSize = 256;
    public int spriteSize = 512;
    
    [Header("Color Schemes")]
    public Color lightCellColor = Color.white;
    public Color darkCellColor = new Color(0.7f, 0.7f, 0.7f);
    public Color borderCellColor = Color.black;
    public Color cemeteryCellColor = new Color(0.5f, 0.3f, 0.1f);
    public Color promotionCellColor = Color.yellow;
    
    [Header("Piece Colors")]
    public Color bluePieceColor = Color.blue;
    public Color redPieceColor = Color.red;

    private void Start()
    {
        #if UNITY_EDITOR
        // Check if we need to generate assets
        if (!HasBasicAssets())
        {
            Debug.Log("🎨 Basic assets missing, generating procedural assets...");
            GenerateAllAssets();
        }
        #endif
    }

    private bool HasBasicAssets()
    {
        string[] requiredPaths = {
            "Assets/Art/Board/Cells/light_cell.png",
            "Assets/Art/Board/Cells/dark_cell.png",
            "Assets/Art/Pieces/Blue/blue_king.png",
            "Assets/Art/Pieces/Red/red_king.png"
        };

        foreach (string path in requiredPaths)
        {
            if (!File.Exists(path))
                return false;
        }
        return true;
    }

    public void GenerateAllAssets()
    {
        CreateDirectories();
        GenerateCellTextures();
        GeneratePieceSprites();
        GenerateSpecialElements();
        GenerateMagicIcons();
        
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        Debug.Log("✅ All procedural assets generated!");
        #endif
    }

    private void CreateDirectories()
    {
        string[] directories = {
            "Assets/Art/Board/Cells",
            "Assets/Art/Pieces/Blue",
            "Assets/Art/Pieces/Red",
            "Assets/Art/Board/Special",
            "Assets/Art/UI/MagicIcons"
        };

        foreach (string dir in directories)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }

    private void GenerateCellTextures()
    {
        // Light cell with subtle pattern
        var lightTexture = CreateCheckerboardTexture(textureSize, lightCellColor, Color.Lerp(lightCellColor, Color.gray, 0.1f), 32);
        SaveTexture(lightTexture, "Assets/Art/Board/Cells/light_cell.png");

        // Dark cell with subtle pattern
        var darkTexture = CreateCheckerboardTexture(textureSize, darkCellColor, Color.Lerp(darkCellColor, Color.black, 0.1f), 32);
        SaveTexture(darkTexture, "Assets/Art/Board/Cells/dark_cell.png");

        // Border cell - solid black with border
        var borderTexture = CreateBorderedTexture(textureSize, borderCellColor, Color.gray, 8);
        SaveTexture(borderTexture, "Assets/Art/Board/Cells/border_cell.png");

        // Cemetery cell - brown with cross pattern
        var cemeteryTexture = CreateCrossTexture(textureSize, cemeteryCellColor, Color.Lerp(cemeteryCellColor, Color.black, 0.3f));
        SaveTexture(cemeteryTexture, "Assets/Art/Board/Cells/cemetery_cell.png");

        // Promotion cell - golden with radial pattern
        var promotionTexture = CreateRadialTexture(textureSize, promotionCellColor, Color.Lerp(promotionCellColor, Color.white, 0.3f));
        SaveTexture(promotionTexture, "Assets/Art/Board/Cells/promotion_cell.png");

        Debug.Log("🎨 Cell textures generated");
    }

    private void GeneratePieceSprites()
    {
        string[] pieceNames = { "king", "queen", "rook", "bishop", "knight", "pawn" };
        
        foreach (string piece in pieceNames)
        {
            // Blue pieces
            var blueSprite = CreatePieceSprite(spriteSize, bluePieceColor, piece);
            SaveTexture(blueSprite, $"Assets/Art/Pieces/Blue/blue_{piece}.png");

            // Red pieces
            var redSprite = CreatePieceSprite(spriteSize, redPieceColor, piece);
            SaveTexture(redSprite, $"Assets/Art/Pieces/Red/red_{piece}.png");
        }

        Debug.Log("♟️ Piece sprites generated");
    }

    private void GenerateSpecialElements()
    {
        // Chest - golden square with shine
        var chestSprite = CreateChestSprite(spriteSize);
        SaveTexture(chestSprite, "Assets/Art/Board/Special/chest.png");

        // Seal - purple circle with mystical pattern
        var sealSprite = CreateSealSprite(spriteSize);
        SaveTexture(sealSprite, "Assets/Art/Board/Special/seal.png");

        // Tomb - gray rectangle with cross
        var tombSprite = CreateTombSprite(spriteSize);
        SaveTexture(tombSprite, "Assets/Art/Board/Special/tomb.png");

        Debug.Log("✨ Special elements generated");
    }

    private void GenerateMagicIcons()
    {
        Color[] magicColors = {
            Color.gray,      // Level 0
            Color.red,       // Level 1 - Fire
            Color.cyan,      // Level 2 - Ice
            Color.yellow,    // Level 3 - Lightning
            Color.green,     // Level 4 - Earth
            Color.white,     // Level 5 - Wind
            Color.magenta    // Level 6 - Dark
        };

        for (int i = 0; i < magicColors.Length; i++)
        {
            var magicIcon = CreateMagicIcon(128, magicColors[i], i);
            SaveTexture(magicIcon, $"Assets/Art/UI/MagicIcons/magic_level_{i}.png");
        }

        Debug.Log("🔮 Magic icons generated");
    }

    private Texture2D CreateCheckerboardTexture(int size, Color color1, Color color2, int checkerSize)
    {
        Texture2D texture = new Texture2D(size, size);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool isColor1 = (x / checkerSize + y / checkerSize) % 2 == 0;
                texture.SetPixel(x, y, isColor1 ? color1 : color2);
            }
        }
        
        texture.Apply();
        return texture;
    }

    private Texture2D CreateBorderedTexture(int size, Color fillColor, Color borderColor, int borderWidth)
    {
        Texture2D texture = new Texture2D(size, size);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool isBorder = x < borderWidth || x >= size - borderWidth || 
                               y < borderWidth || y >= size - borderWidth;
                texture.SetPixel(x, y, isBorder ? borderColor : fillColor);
            }
        }
        
        texture.Apply();
        return texture;
    }

    private Texture2D CreateCrossTexture(int size, Color baseColor, Color crossColor)
    {
        Texture2D texture = new Texture2D(size, size);
        int crossWidth = size / 16;
        int center = size / 2;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool isVerticalCross = Mathf.Abs(x - center) < crossWidth;
                bool isHorizontalCross = Mathf.Abs(y - center) < crossWidth;
                
                texture.SetPixel(x, y, (isVerticalCross || isHorizontalCross) ? crossColor : baseColor);
            }
        }
        
        texture.Apply();
        return texture;
    }

    private Texture2D CreateRadialTexture(int size, Color centerColor, Color edgeColor)
    {
        Texture2D texture = new Texture2D(size, size);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float maxDistance = size * 0.5f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float t = Mathf.Clamp01(distance / maxDistance);
                Color color = Color.Lerp(centerColor, edgeColor, t);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }

    private Texture2D CreatePieceSprite(int size, Color pieceColor, string pieceType)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        
        // Clear with transparent
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        
        texture.SetPixels(pixels);

        // Draw piece shape based on type
        switch (pieceType.ToLower())
        {
            case "king":
                DrawCrown(texture, center, size * 0.4f, pieceColor);
                break;
            case "queen":
                DrawDiamond(texture, center, size * 0.4f, pieceColor);
                break;
            case "rook":
                DrawCastle(texture, center, size * 0.4f, pieceColor);
                break;
            case "bishop":
                DrawMitre(texture, center, size * 0.4f, pieceColor);
                break;
            case "knight":
                DrawHorse(texture, center, size * 0.4f, pieceColor);
                break;
            case "pawn":
                DrawCircle(texture, center, size * 0.3f, pieceColor);
                break;
        }
        
        texture.Apply();
        return texture;
    }

    private Texture2D CreateChestSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color chestColor = new Color(1f, 0.8f, 0f); // Gold
        Color highlightColor = Color.yellow;
        
        // Create chest shape
        DrawRoundedRect(texture, new Rect(size * 0.2f, size * 0.3f, size * 0.6f, size * 0.4f), chestColor);
        DrawRoundedRect(texture, new Rect(size * 0.25f, size * 0.35f, size * 0.5f, size * 0.1f), highlightColor);
        
        texture.Apply();
        return texture;
    }

    private Texture2D CreateSealSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color sealColor = new Color(0.5f, 0f, 0.8f); // Purple
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        
        // Draw mystical circle
        DrawCircle(texture, center, size * 0.4f, sealColor);
        DrawCircle(texture, center, size * 0.3f, Color.Lerp(sealColor, Color.white, 0.3f));
        
        texture.Apply();
        return texture;
    }

    private Texture2D CreateTombSprite(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color tombColor = Color.gray;
        
        // Draw tombstone shape
        DrawRoundedRect(texture, new Rect(size * 0.3f, size * 0.2f, size * 0.4f, size * 0.6f), tombColor);
        
        texture.Apply();
        return texture;
    }

    private Texture2D CreateMagicIcon(int size, Color iconColor, int magicLevel)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        
        // Different shapes for different magic levels
        switch (magicLevel)
        {
            case 0: break; // Empty
            case 1: DrawTriangle(texture, center, size * 0.4f, iconColor); break; // Fire
            case 2: DrawDiamond(texture, center, size * 0.4f, iconColor); break; // Ice
            case 3: DrawZigzag(texture, center, size * 0.4f, iconColor); break; // Lightning
            case 4: DrawSquare(texture, center, size * 0.4f, iconColor); break; // Earth
            case 5: DrawSpiral(texture, center, size * 0.4f, iconColor); break; // Wind
            case 6: DrawStar(texture, center, size * 0.4f, iconColor); break; // Dark
        }
        
        texture.Apply();
        return texture;
    }

    // Helper drawing methods
    private void DrawCircle(Texture2D texture, Vector2 center, float radius, Color color)
    {
        int size = texture.width;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    texture.SetPixel(x, y, Color.Lerp(texture.GetPixel(x, y), color, color.a));
                }
            }
        }
    }

    private void DrawRoundedRect(Texture2D texture, Rect rect, Color color)
    {
        for (int x = (int)rect.x; x < rect.x + rect.width; x++)
        {
            for (int y = (int)rect.y; y < rect.y + rect.height; y++)
            {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private void DrawCrown(Texture2D texture, Vector2 center, float size, Color color)
    {
        DrawCircle(texture, center, size * 0.8f, color);
        DrawRoundedRect(texture, new Rect(center.x - size, center.y - size * 0.2f, size * 2, size * 0.4f), color);
    }

    private void DrawDiamond(Texture2D texture, Vector2 center, float size, Color color)
    {
        // Simple diamond shape
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float dx = Mathf.Abs(x - center.x);
                float dy = Mathf.Abs(y - center.y);
                if (dx + dy <= size)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private void DrawCastle(Texture2D texture, Vector2 center, float size, Color color)
    {
        DrawRoundedRect(texture, new Rect(center.x - size, center.y - size * 0.5f, size * 2, size), color);
    }

    private void DrawMitre(Texture2D texture, Vector2 center, float size, Color color)
    {
        DrawTriangle(texture, center, size, color);
    }

    private void DrawHorse(Texture2D texture, Vector2 center, float size, Color color)
    {
        // Simplified horse head shape
        DrawCircle(texture, center, size * 0.8f, color);
        DrawRoundedRect(texture, new Rect(center.x - size * 0.3f, center.y - size, size * 0.6f, size), color);
    }

    private void DrawTriangle(Texture2D texture, Vector2 center, float size, Color color)
    {
        // Simple triangle pointing up
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Vector2 point = new Vector2(x, y);
                if (IsPointInTriangle(point, 
                    new Vector2(center.x, center.y + size),
                    new Vector2(center.x - size, center.y - size),
                    new Vector2(center.x + size, center.y - size)))
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private void DrawSquare(Texture2D texture, Vector2 center, float size, Color color)
    {
        DrawRoundedRect(texture, new Rect(center.x - size, center.y - size, size * 2, size * 2), color);
    }

    private void DrawZigzag(Texture2D texture, Vector2 center, float size, Color color)
    {
        // Lightning bolt approximation
        DrawRoundedRect(texture, new Rect(center.x - size * 0.1f, center.y - size, size * 0.2f, size * 2), color);
        DrawRoundedRect(texture, new Rect(center.x - size * 0.5f, center.y - size * 0.2f, size, size * 0.4f), color);
    }

    private void DrawSpiral(Texture2D texture, Vector2 center, float size, Color color)
    {
        // Simple spiral approximation
        for (float angle = 0; angle < Mathf.PI * 6; angle += 0.1f)
        {
            float radius = (angle / (Mathf.PI * 6)) * size;
            int x = (int)(center.x + Mathf.Cos(angle) * radius);
            int y = (int)(center.y + Mathf.Sin(angle) * radius);
            
            if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
            {
                texture.SetPixel(x, y, color);
            }
        }
    }

    private void DrawStar(Texture2D texture, Vector2 center, float size, Color color)
    {
        // 5-pointed star
        for (int i = 0; i < 5; i++)
        {
            float angle = (i * Mathf.PI * 2 / 5) - Mathf.PI / 2;
            Vector2 outerPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * size;
            
            // Draw lines from center to outer points
            DrawLine(texture, center, outerPoint, color);
        }
    }

    private void DrawLine(Texture2D texture, Vector2 start, Vector2 end, Color color)
    {
        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);
        
        for (float t = 0; t <= distance; t += 0.5f)
        {
            Vector2 point = start + direction * t;
            int x = (int)point.x;
            int y = (int)point.y;
            
            if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
            {
                texture.SetPixel(x, y, color);
            }
        }
    }

    private bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
    {
        float denominator = ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
        float alpha = ((b.y - c.y) * (point.x - c.x) + (c.x - b.x) * (point.y - c.y)) / denominator;
        float beta = ((c.y - a.y) * (point.x - c.x) + (a.x - c.x) * (point.y - c.y)) / denominator;
        float gamma = 1 - alpha - beta;
        
        return alpha > 0 && beta > 0 && gamma > 0;
    }

    private void SaveTexture(Texture2D texture, string path)
    {
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(path, pngData);
        DestroyImmediate(texture);
    }

    #if UNITY_EDITOR
    [MenuItem("Crownfall/Generate Procedural Assets")]
    public static void MenuGenerateAssets()
    {
        var generator = FindFirstObjectByType<ProceduralAssetGenerator>();
        if (generator == null)
        {
            GameObject go = new GameObject("ProceduralAssetGenerator");
            generator = go.AddComponent<ProceduralAssetGenerator>();
        }
        
        generator.GenerateAllAssets();
    }
    #endif
}
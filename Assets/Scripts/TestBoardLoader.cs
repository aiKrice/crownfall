using UnityEngine;
using Crownfall.Board;

public class TestBoardLoader : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🧪 Testing BoardLoader...");
        
        var loader = gameObject.AddComponent<BoardLoader>();
        var board = loader.LoadBoard("chapter1_level1.json");
        
        if (board != null)
        {
            Debug.Log($"✅ Board loaded successfully!");
            Debug.Log($"📊 Chapter: {board.chapter}, Level: {board.level}");
            Debug.Log($"📐 Dimensions: {board.dimensions.width}x{board.dimensions.height}");
            Debug.Log($"🎲 Cells: {board.cells.Count}");
            
            var player1Cemetery = board.GetPlayerCemetery(1);
            var player2Cemetery = board.GetPlayerCemetery(2);
            
            Debug.Log($"⚰️ Player 1 cemetery: {player1Cemetery.Count} slots");
            Debug.Log($"⚰️ Player 2 cemetery: {player2Cemetery.Count} slots");
            Debug.Log($"👑 Player 1 cemetery full: {board.IsCemeteryFull(1)}");
            Debug.Log($"👑 Player 2 cemetery full: {board.IsCemeteryFull(2)}");
        }
        else
        {
            Debug.LogError("❌ Failed to load board!");
        }
    }
}
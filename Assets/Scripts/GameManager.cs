using UnityEngine;
using Crownfall.Board;
using Crownfall.UI;

public class GameManager : MonoBehaviour
{
    [Header("Board Setup")]
    public BoardManager boardManager;
    public string initialBoardFile = "default_board.json";

    [Header("Game State")]
    public PlayerColor currentPlayer = PlayerColor.Blue;
    public bool gameStarted = false;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("Crownfall Game Starting...");
        
        if (boardManager == null)
        {
            boardManager = FindFirstObjectByType<BoardManager>();
        }

        if (boardManager != null)
        {
            // Load board directly since OnBoardLoaded event might not exist
            boardManager.LoadBoard(initialBoardFile);
        }
        else
        {
            Debug.LogError("BoardManager not found! Please add a BoardManager to the scene.");
        }
    }

    public void OnBoardLoaded()
    {
        Debug.Log("Board loaded successfully!");
        gameStarted = true;
        
        // Initialize UI or other game systems here
        ShowGameStartMessage();
    }

    private void ShowGameStartMessage()
    {
        Debug.Log($"Game started! Current player: {currentPlayer}");
        Debug.Log("Click on pieces to select them, then click on valid cells to move.");
    }

    private void Update()
    {
        if (!gameStarted) return;

        // Handle input or game logic updates here
        HandleInput();
    }

    private void HandleInput()
    {
        // Basic input handling - can be expanded
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchPlayer();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBackToMenu();
        }
    }

    public void SwitchPlayer()
    {
        currentPlayer = currentPlayer == PlayerColor.Blue ? PlayerColor.Red : PlayerColor.Blue;
        Debug.Log($"Switched to player: {currentPlayer}");
    }

    // ✅ APPELÉ DEPUIS L'INSPECTOR UNITY
    public void GoBackToMenu()
    {
        Debug.Log("🔙 Going back to Main Menu...");
        
        if (CrownfallSceneManager.Instance != null)
        {
            CrownfallSceneManager.Instance.LoadMainMenuScene();
        }
        else
        {
            Debug.LogError("❌ CrownfallSceneManager not found!");
        }
    }

    private void OnGUI()
    {
        // Simple debug UI
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"Current Player: {currentPlayer}");
        GUILayout.Label($"Game Started: {gameStarted}");
        
        if (GUILayout.Button("Switch Player (Space)"))
        {
            SwitchPlayer();
        }
        
        if (GUILayout.Button("Back to Menu (Esc)"))
        {
            GoBackToMenu();
        }
        
        GUILayout.EndArea();
    }
}
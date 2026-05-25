using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Linq;

namespace Crownfall.Board
{
    // Classes pour parser le JSON (format temporaire)
    [Serializable]
    public class JsonBoardFormat
    {
        public string boardId;
        public string version;
        public int chapter;
        public int level;
        public string history;
        public BoardDimensions dimensions;
        public JsonCellData[] cells;
    }
    
    [Serializable]
    public class JsonCellData
    {
        public string id;
        public string type;
        public string subtype;
        public int playerId;
        public CellContent content;
        public bool promotion;
        public int promotionPlayerId;
        public string colorLayer;
    }

    public class BoardLoader : MonoBehaviour
    {
        [Header("Settings")]
        public bool loadFromStreamingAssets = false; // Load from Assets/Data/
        public string boardFileName = "chapter1_level1.json";
        
        [Header("Events")]
        public UnityEngine.Events.UnityEvent<BoardConfiguration> OnBoardLoaded;
        public UnityEngine.Events.UnityEvent<string> OnLoadError;

        private BoardConfiguration currentBoard;

        public BoardConfiguration CurrentBoard => currentBoard;

        public async Task<BoardConfiguration> LoadBoardAsync(string fileName = null)
        {
            string fileToLoad = fileName ?? boardFileName;
            string filePath = GetBoardFilePath(fileToLoad);

            try
            {
                string jsonContent;

                if (loadFromStreamingAssets)
                {
                    jsonContent = await LoadFromStreamingAssetsAsync(fileToLoad);
                }
                else
                {
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException($"Board file not found: {filePath}");
                    }
                    jsonContent = await File.ReadAllTextAsync(filePath);
                }

                currentBoard = ParseNewFormatJson(jsonContent);
                
                if (ValidateBoard(currentBoard))
                {
                    OnBoardLoaded?.Invoke(currentBoard);
                    Debug.Log($"Board '{currentBoard.boardId}' (Chapter {currentBoard.chapter} Level {currentBoard.level}) loaded successfully from {fileToLoad}");
                    return currentBoard;
                }
                else
                {
                    throw new InvalidDataException("Board validation failed");
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Failed to load board from {fileToLoad}: {e.Message}";
                Debug.LogError(errorMessage);
                OnLoadError?.Invoke(errorMessage);
                return null;
            }
        }

        public BoardConfiguration LoadBoard(string fileName = null)
        {
            string fileToLoad = fileName ?? boardFileName;
            string filePath = GetBoardFilePath(fileToLoad);

            try
            {
                string jsonContent;

                if (loadFromStreamingAssets)
                {
                    filePath = Path.Combine(Application.streamingAssetsPath, fileToLoad);
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException($"Board file not found in StreamingAssets: {fileToLoad}");
                    }
                    jsonContent = File.ReadAllText(filePath);
                }
                else
                {
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException($"Board file not found: {filePath}");
                    }
                    jsonContent = File.ReadAllText(filePath);
                }

                currentBoard = ParseNewFormatJson(jsonContent);
                
                if (ValidateBoard(currentBoard))
                {
                    OnBoardLoaded?.Invoke(currentBoard);
                    Debug.Log($"Board '{currentBoard.boardId}' (Chapter {currentBoard.chapter} Level {currentBoard.level}) loaded successfully from {fileToLoad}");
                    return currentBoard;
                }
                else
                {
                    throw new InvalidDataException("Board validation failed");
                }
            }
            catch (Exception e)
            {
                string errorMessage = $"Failed to load board from {fileToLoad}: {e.Message}";
                Debug.LogError(errorMessage);
                OnLoadError?.Invoke(errorMessage);
                return null;
            }
        }

        public void SaveBoard(BoardConfiguration board, string fileName = null)
        {
            string fileToSave = fileName ?? boardFileName;
            string filePath = GetBoardFilePath(fileToSave);

            try
            {
                string jsonContent = JsonUtility.ToJson(board, true);
                File.WriteAllText(filePath, jsonContent);
                Debug.Log($"Board '{board.boardId}' saved successfully to {fileToSave}");
            }
            catch (Exception e)
            {
                string errorMessage = $"Failed to save board to {fileToSave}: {e.Message}";
                Debug.LogError(errorMessage);
            }
        }

        private async Task<string> LoadFromStreamingAssetsAsync(string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            // On Android, StreamingAssets files are in a compressed format
            using (var www = UnityWebRequest.Get(filePath))
            {
                var operation = www.SendWebRequest();
                
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
                
                if (www.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Failed to load from StreamingAssets: {www.error}");
                }
                
                return www.downloadHandler.text;
            }
            #else
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Board file not found in StreamingAssets: {fileName}");
            }
            return await File.ReadAllTextAsync(filePath);
            #endif
        }

        private string GetBoardFilePath(string fileName)
        {
            if (loadFromStreamingAssets)
            {
                return Path.Combine(Application.streamingAssetsPath, fileName);
            }
            else
            {
                // Load from Assets/Data/ folder
                return Path.Combine(Application.dataPath, "Data", fileName);
            }
        }
        
        private BoardConfiguration ParseNewFormatJson(string jsonContent)
        {
            // Parse le JSON avec le nouveau format
            var jsonBoard = JsonUtility.FromJson<JsonBoardFormat>(jsonContent);
            
            // Convertit vers l'ancien format BoardConfiguration
            var board = new BoardConfiguration();
            board.boardId = jsonBoard.boardId;
            board.version = jsonBoard.version;
            board.chapter = jsonBoard.chapter;
            board.level = jsonBoard.level;
            board.history = jsonBoard.history;
            board.dimensions = jsonBoard.dimensions;
            board.cells = new List<CellData>();
            
            // Convertit chaque case JSON vers CellData
            foreach (var jsonCell in jsonBoard.cells)
            {
                var cellData = new CellData(jsonCell.id, ParseCellType(jsonCell.type), jsonCell.subtype);
                cellData.playerId = jsonCell.playerId;
                cellData.content = jsonCell.content;
                cellData.promotion = jsonCell.promotion;
                cellData.promotionPlayerId = jsonCell.promotionPlayerId;
                cellData.colorLayer = jsonCell.colorLayer;
                
                board.cells.Add(cellData);
            }
            
            Debug.Log($"📦 Parsed board: {board.cells.Count} cells, {board.dimensions.width}x{board.dimensions.height}");
            return board;
        }
        
        private CellType ParseCellType(string typeString)
        {
            return typeString switch
            {
                "border" => CellType.Border,
                "cemetery" => CellType.Cemetery,
                "playable" => CellType.Normal,
                _ => CellType.Normal
            };
        }

        private bool ValidateBoard(BoardConfiguration board)
        {
            if (board == null)
            {
                Debug.LogError("Board configuration is null");
                return false;
            }

            if (board.dimensions.width <= 0 || board.dimensions.height <= 0)
            {
                Debug.LogError("Board dimensions must be positive");
                return false;
            }

            if (board.cells == null || board.cells.Count == 0)
            {
                Debug.LogError("Board must have cells");
                return false;
            }

            // Validation basique pour le nouveau format
            foreach (var cell in board.cells)
            {
                if (!board.IsValidPosition(cell.Position))
                {
                    Debug.LogError($"Cell {cell.id} at position {cell.Position} is out of bounds");
                    return false;
                }
            }
            
            // Validate que chaque joueur a un roi
            var player1Pieces = board.GetPlayerPieceCells(1);
            var player2Pieces = board.GetPlayerPieceCells(2);
            
            int player1Kings = player1Pieces.Count(cell => cell.content.pieceType == "king");
            int player2Kings = player2Pieces.Count(cell => cell.content.pieceType == "king");
            
            if (player1Kings != 1 || player2Kings != 1)
            {
                Debug.LogError($"Each player must have exactly one king. Player1: {player1Kings}, Player2: {player2Kings}");
                return false;
            }

            Debug.Log("Board validation successful");
            return true;
        }

        // Utility methods for creating boards programmatically
        public BoardConfiguration CreateEmptyBoard(int width, int height, string name = "New Board")
        {
            var board = new BoardConfiguration
            {
                boardId = name,
                dimensions = new BoardDimensions { width = width, height = height }
            };

            // Create all cells
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    string cellId = $"{(char)('A' + x)}{y + 1}";
                    CellType cellType = (x == 0 || x == width - 1 || y == 0 || y == height - 1) ? 
                        CellType.Border : CellType.Normal;
                    string subtype = cellType == CellType.Border ? "side" : "normal";
                    
                    board.cells.Add(new CellData(cellId, cellType, subtype));
                }
            }

            return board;
        }
    }
}
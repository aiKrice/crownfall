using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Crownfall.Pieces;

namespace Crownfall.Board
{
    public class BoardManager : MonoBehaviour
    {
        [Header("Board Settings")]
        public Transform boardParent;
        public Transform piecesParent;
        
        [Header("Prefabs")]
        public GameObject cellPrefab;
        public GameObject genericPiecePrefab; // Un seul prefab générique pour toutes les pièces
        public GameObject chestPrefab;
        public GameObject sealPrefab;
        public GameObject tombPrefab;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent<PieceData, Vector2Int, Vector2Int> OnPieceMoved;
        public UnityEngine.Events.UnityEvent<PieceData> OnPieceDestroyed;
        public UnityEngine.Events.UnityEvent<PieceData> OnPiecePromoted;
        public UnityEngine.Events.UnityEvent<ChestReward, Vector2Int> OnChestOpened;
        public UnityEngine.Events.UnityEvent<PieceData> OnSealCreated;
        public UnityEngine.Events.UnityEvent<PieceData> OnSealInvoked;

        private BoardConfiguration currentBoard;
        private Dictionary<Vector2Int, GameObject> cellObjects = new Dictionary<Vector2Int, GameObject>();
        private Dictionary<string, GameObject> pieceObjects = new Dictionary<string, GameObject>();
        private Dictionary<Vector2Int, GameObject> chestObjects = new Dictionary<Vector2Int, GameObject>();
        private Dictionary<Vector2Int, GameObject> sealObjects = new Dictionary<Vector2Int, GameObject>();

        private void Awake()
        {
            var boardLoader = GetComponent<BoardLoader>();
            if (boardLoader != null)
            {
                boardLoader.OnBoardLoaded.AddListener(OnBoardLoaded);
            }
        }

        private void Start()
        {
            // Board will be loaded by GameManager
        }

        private void OnBoardLoaded(BoardConfiguration board)
        {
            currentBoard = board;
            BuildBoard();
        }

        public void LoadBoard(string fileName = null)
        {
            var boardLoader = GetComponent<BoardLoader>();
            if (boardLoader != null)
            {
                boardLoader.LoadBoard(fileName);
            }
        }

        private void BuildBoard()
        {
            if (currentBoard == null) return;

            ClearBoard();
            CreateCells();
            CreateChests();
            CreatePieces();
            CreateSeals();

            Debug.Log($"Board '{currentBoard.boardId}' (Chapter {currentBoard.chapter} Level {currentBoard.level}) built successfully");
        }

        private void ClearBoard()
        {
            // Clear existing objects
            foreach (var obj in cellObjects.Values)
                if (obj != null) DestroyImmediate(obj);
            foreach (var obj in pieceObjects.Values)
                if (obj != null) DestroyImmediate(obj);
            foreach (var obj in chestObjects.Values)
                if (obj != null) DestroyImmediate(obj);
            foreach (var obj in sealObjects.Values)
                if (obj != null) DestroyImmediate(obj);

            cellObjects.Clear();
            pieceObjects.Clear();
            chestObjects.Clear();
            sealObjects.Clear();
        }

        private void CreateCells()
        {
            if (cellPrefab == null || boardParent == null) return;

            foreach (var cell in currentBoard.cells)
            {
                GameObject cellObj = Instantiate(cellPrefab, boardParent);
                cellObj.transform.position = new Vector3(cell.Position.x, 0, cell.Position.y);
                cellObj.name = $"Cell_{cell.id}";

                // You can add cell-specific logic here (colors, materials, etc.)
                var cellComponent = cellObj.GetComponent<CellDisplay>();
                if (cellComponent != null)
                {
                    cellComponent.Initialize(cell);
                }

                cellObjects[cell.Position] = cellObj;
            }
        }

        private void CreatePieces()
        {
            if (piecePrefabs == null || piecesParent == null) return;

            // Create pieces from cell contents
            foreach (var cell in currentBoard.cells.Where(c => c.content?.type == "piece"))
            {
                CreatePieceFromCell(cell);
            }
        }

        private GameObject CreatePieceFromCell(CellData cell)
        {
            if (cell.content?.type != "piece") return null;
            
            // Récupérer le PieceModel via PieceModelManager
            var pieceModelManager = PieceModelManager.Instance;
            if (pieceModelManager == null)
            {
                Debug.LogError("PieceModelManager not found!");
                return null;
            }
            
            var pieceModel = pieceModelManager.GetPieceModelByName(cell.content.pieceType);
            if (pieceModel == null)
            {
                Debug.LogError($"No PieceModel found for {cell.content.pieceType}");
                return null;
            }

            if (genericPiecePrefab == null)
            {
                Debug.LogError("Generic piece prefab is missing!");
                return null;
            }

            GameObject pieceObj = Instantiate(genericPiecePrefab, piecesParent);
            pieceObj.transform.position = new Vector3(cell.Position.x, 0.5f, cell.Position.y);
            pieceObj.name = $"Piece_{pieceModel.pieceName}_{cell.id}";

            // Configurer l'apparence de la pièce avec le PieceModel
            ConfigurePieceVisuals(pieceObj, pieceModel, cell.content.playerId);

            var pieceComponent = pieceObj.GetComponent<PieceDisplay>();
            if (pieceComponent != null)
            {
                // Create temporary PieceData for compatibility (à remplacer plus tard)
                var pieceData = new PieceData(
                    cell.id + "_piece",
                    GetPieceTypeEnum(cell.content.pieceType),
                    cell.content.playerId == 1 ? PlayerColor.Blue : PlayerColor.Red,
                    cell.Position,
                    pieceModel.baseHp, pieceModel.baseDefense, new AttackRange(1, 3)
                );
                pieceComponent.Initialize(pieceData);
            }

            pieceObjects[cell.id] = pieceObj;
            return pieceObj;
        }
        
        private void ConfigurePieceVisuals(GameObject pieceObj, PieceModel model, int playerId)
        {
            // Appliquer le sprite de la pièce
            var spriteRenderer = pieceObj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && model.normalSkins.Count > 0)
            {
                spriteRenderer.sprite = model.normalSkins[0]; // Prendre le premier skin
            }
            
            // Appliquer la couleur selon le joueur
            var renderer = pieceObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Créer un matériau temporaire avec la couleur du modèle
                var material = new Material(Shader.Find("Standard"));
                material.color = playerId == 1 ? Color.blue : Color.red; // Couleur de base par joueur
                renderer.material = material;
            }
        }
        
        private int GetPieceTypeIndex(string pieceType)
        {
            return pieceType switch
            {
                "king" => 0,
                "queen" => 1,
                "orkman" => 2,
                "parrot" => 3,
                "lion" => 4,
                "magician" => 5,
                _ => 2 // Default to orkman
            };
        }
        
        private PieceType GetPieceTypeEnum(string pieceType)
        {
            return pieceType switch
            {
                "king" => PieceType.King,
                "queen" => PieceType.Queen,
                "orkman" => PieceType.OrkMan,
                "parrot" => PieceType.Parrot,
                "lion" => PieceType.Lion,
                "magician" => PieceType.Magician,
                "knight" => PieceType.Knight,
                _ => PieceType.OrkMan // Default to orkman
            };
        }

        private void CreateChests()
        {
            if (chestPrefab == null) return;

            // Create chests from cell contents
            foreach (var cell in currentBoard.cells.Where(c => c.content?.type == "chest"))
            {
                GameObject chestObj = Instantiate(chestPrefab, boardParent);
                chestObj.transform.position = new Vector3(cell.Position.x, 0.2f, cell.Position.y);
                chestObj.name = $"Chest_{cell.id}";

                var chestComponent = chestObj.GetComponent<ChestDisplay>();
                if (chestComponent != null)
                {
                    chestComponent.Initialize(cell.Position, this);
                }

                chestObjects[cell.Position] = chestObj;
            }
        }

        private void CreateSeals()
        {
            if (sealPrefab == null) return;

            // Create seals from cell contents
            foreach (var cell in currentBoard.cells.Where(c => c.content?.type == "seal"))
            {
                CreateSealObject(cell.Position, cell.content.invocationTurns);
            }
        }

        private GameObject CreateSealObject(Vector2Int position, int turnsRemaining)
        {
            if (sealPrefab == null) return null;

            GameObject sealObj = Instantiate(sealPrefab, boardParent);
            sealObj.transform.position = new Vector3(position.x, 0.3f, position.y);
            sealObj.name = $"Seal_{position.x}_{position.y}";

            var sealComponent = sealObj.GetComponent<SealDisplay>();
            if (sealComponent != null)
            {
                sealComponent.Initialize(position, turnsRemaining, this);
            }

            sealObjects[position] = sealObj;
            return sealObj;
        }

        // Game Logic Methods

        public bool MovePiece(string cellId, Vector2Int newPosition)
        {
            var sourceCell = currentBoard.GetCellById(cellId);
            if (sourceCell?.content?.type != "piece")
            {
                Debug.LogError($"No piece found at cell {cellId}");
                return false;
            }

            var targetCell = currentBoard.GetCell(newPosition);
            if (targetCell == null || !CanMoveTo(sourceCell, targetCell))
            {
                Debug.LogWarning($"Cannot move piece from {cellId} to {newPosition}");
                return false;
            }

            Vector2Int oldPosition = sourceCell.Position;
            
            // Handle chest interaction
            if (targetCell.content?.type == "chest")
            {
                HandleChestInteraction(sourceCell, targetCell);
            }

            // Move piece content
            targetCell.content = sourceCell.content;
            sourceCell.content = null;

            // Update visual
            if (pieceObjects.TryGetValue(cellId, out GameObject pieceObj))
            {
                pieceObj.transform.position = new Vector3(newPosition.x, 0.5f, newPosition.y);
                // Update piece object key
                pieceObjects.Remove(cellId);
                pieceObjects[targetCell.id] = pieceObj;
            }

            // Create temporary piece data for event
            var pieceData = new PieceData(
                targetCell.id + "_piece",
                GetPieceTypeEnum(targetCell.content.pieceType),
                targetCell.content.playerId == 1 ? PlayerColor.Blue : PlayerColor.Red,
                newPosition, 100, 5, new AttackRange(1, 3)
            );
            OnPieceMoved?.Invoke(pieceData, oldPosition, newPosition);
            return true;
        }

        public bool CanMoveTo(CellData sourceCell, CellData targetCell)
        {
            if (targetCell == null || targetCell.type == CellType.Border)
                return false;
            
            // Can't move to occupied cell (except chests)
            if (targetCell.content?.type == "piece")
                return false;

            // Add more movement validation logic here based on piece type
            return true;
        }

        public void HandleChestInteraction(CellData sourceCell, CellData chestCell)
        {
            // Simple chest rewards based on chest level
            var chestLevel = chestCell.content?.level ?? 1;
            ChestReward reward = ChestReward.Promotion; // Default reward
            
            // Random reward based on chest level
            var randomValue = UnityEngine.Random.Range(0, 3);
            reward = randomValue switch
            {
                0 => ChestReward.Death,
                1 => ChestReward.Promotion,
                _ => ChestReward.RandomSummon
            };

            switch (reward)
            {
                case ChestReward.Death:
                    CreateSeal(sourceCell, chestCell.Position);
                    break;
                case ChestReward.Promotion:
                    PromotePieceFromCell(sourceCell);
                    break;
                case ChestReward.RandomSummon:
                    Debug.Log($"Random summon triggered for piece at {sourceCell.id}");
                    break;
            }

            // Remove chest
            chestCell.content = null;
            if (chestObjects.TryGetValue(chestCell.Position, out GameObject chestObj))
            {
                DestroyImmediate(chestObj);
                chestObjects.Remove(chestCell.Position);
            }

            OnChestOpened?.Invoke(reward, chestCell.Position);
        }

        public void CreateSeal(CellData pieceCell, Vector2Int position)
        {
            // Remove piece from cell
            var pieceContent = pieceCell.content;
            pieceCell.content = null;
            
            // Remove piece visual
            if (pieceObjects.TryGetValue(pieceCell.id, out GameObject pieceObj))
            {
                DestroyImmediate(pieceObj);
                pieceObjects.Remove(pieceCell.id);
            }

            // Create seal at target position
            var cell = currentBoard.GetCell(position);
            if (cell != null)
            {
                cell.content = new CellContent
                {
                    type = "seal",
                    magicLevel = 1,
                    invocationTurns = currentBoard.maxTurnsForSealInvocation
                };
                CreateSealObject(position, currentBoard.maxTurnsForSealInvocation);
            }

            // Create temp piece data for event
            var tempPiece = new PieceData(
                pieceCell.id + "_piece",
                GetPieceTypeEnum(pieceContent?.pieceType ?? "orkman"),
                pieceContent?.playerId == 1 ? PlayerColor.Blue : PlayerColor.Red,
                pieceCell.Position, 100, 5, new AttackRange(1, 3)
            );
            OnSealCreated?.Invoke(tempPiece);
        }

        public bool TryInvokeSeal(Vector2Int sealPosition, PlayerColor invoker)
        {
            var cell = currentBoard.GetCell(sealPosition);
            if (cell?.content?.type != "seal") return false;

            float successRate = currentBoard.sealInvocationSuccessRate;
            if (cell.content.invocationTurns == 1)
                successRate = currentBoard.finalTurnSealSuccessRate;

            bool success = UnityEngine.Random.Range(0f, 1f) <= successRate;

            if (success)
            {
                Debug.Log($"Seal invocation successful at {sealPosition}");
                RemoveSeal(sealPosition);
                return true;
            }
            else
            {
                cell.content.invocationTurns--;
                if (cell.content.invocationTurns <= 0)
                {
                    // Send to cemetery - find empty cemetery slot for invoker
                    int playerId = invoker == PlayerColor.Blue ? 1 : 2;
                    var emptyCemetery = currentBoard.cells.FirstOrDefault(c => 
                        c.type == CellType.Cemetery && 
                        c.playerId == playerId && 
                        c.subtype == "empty");
                    
                    if (emptyCemetery != null)
                    {
                        emptyCemetery.subtype = "filled";
                        emptyCemetery.content = new CellContent { type = "tomb" };
                    }
                    
                    RemoveSeal(sealPosition);
                }
                else
                {
                    // Update seal display
                    if (sealObjects.TryGetValue(sealPosition, out GameObject sealObj))
                    {
                        var sealComponent = sealObj.GetComponent<SealDisplay>();
                        sealComponent?.UpdateTurns(cell.content.invocationTurns);
                    }
                }
            }

            return false;
        }

        public void PromotePieceFromCell(CellData cell)
        {
            if (cell.content?.type != "piece") return;

            // Promote piece to queen if not already
            if (cell.content.pieceType != "queen")
            {
                cell.content.pieceType = "queen";
            }

            // Update visual
            if (pieceObjects.TryGetValue(cell.id, out GameObject pieceObj))
            {
                var pieceComponent = pieceObj.GetComponent<PieceDisplay>();
                pieceComponent?.UpdatePromotion();
            }

            // Create temp piece data for event
            var tempPiece = new PieceData(
                cell.id + "_piece",
                PieceType.Queen,
                cell.content.playerId == 1 ? PlayerColor.Blue : PlayerColor.Red,
                cell.Position, 100, 5, new AttackRange(1, 3)
            );
            OnPiecePromoted?.Invoke(tempPiece);
        }

        public void RemovePiece(string cellId)
        {
            var cell = currentBoard.GetCellById(cellId);
            if (cell?.content?.type != "piece") return;

            var pieceContent = cell.content;
            cell.content = null;

            if (pieceObjects.TryGetValue(cellId, out GameObject pieceObj))
            {
                DestroyImmediate(pieceObj);
                pieceObjects.Remove(cellId);
            }

            // Create temp piece data for event
            var tempPiece = new PieceData(
                cellId + "_piece",
                GetPieceTypeEnum(pieceContent.pieceType),
                pieceContent.playerId == 1 ? PlayerColor.Blue : PlayerColor.Red,
                cell.Position, 100, 5, new AttackRange(1, 3)
            );
            OnPieceDestroyed?.Invoke(tempPiece);
        }

        public void RemoveSeal(Vector2Int position)
        {
            var cell = currentBoard.GetCell(position);
            if (cell?.content?.type == "seal")
            {
                cell.content = null;
            }

            if (sealObjects.TryGetValue(position, out GameObject sealObj))
            {
                DestroyImmediate(sealObj);
                sealObjects.Remove(position);
            }
        }

        // Utility Methods
        public CellData GetPieceCell(Vector2Int position)
        {
            return currentBoard?.GetPieceCell(position);
        }

        public CellData GetPieceCellById(string id)
        {
            return currentBoard?.GetCellById(id);
        }

        public List<CellData> GetPlayerPieces(PlayerColor player)
        {
            int playerId = player == PlayerColor.Blue ? 1 : 2;
            return currentBoard?.GetPlayerPieceCells(playerId) ?? new List<CellData>();
        }

        public BoardConfiguration GetCurrentBoard()
        {
            return currentBoard;
        }
    }
}
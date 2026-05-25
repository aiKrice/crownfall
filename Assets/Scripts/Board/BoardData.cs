using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crownfall.Board
{
    [Serializable]
    public struct AttackRange
    {
        public int min;
        public int max;

        public AttackRange(int minValue, int maxValue)
        {
            min = minValue;
            max = maxValue;
        }

        public int GetRandomAttack()
        {
            return UnityEngine.Random.Range(min, max + 1);
        }
    }

    [Serializable]
    public class CellData
    {
        public string id;
        public CellType type;
        public string subtype;
        public int playerId; // For cemetery ownership and promotions
        public CellContent content;
        public bool promotion;
        public int promotionPlayerId;
        public string colorLayer;
        
        // Position calculée depuis l'ID
        public Vector2Int Position
        {
            get
            {
                if (string.IsNullOrEmpty(id) || id.Length < 2)
                    return Vector2Int.zero;
                    
                int x = id[0] - 'A'; // A=0, B=1, etc.
                if (int.TryParse(id.Substring(1), out int y))
                    return new Vector2Int(x, y - 1); // JSON uses 1-based, Unity uses 0-based
                return Vector2Int.zero;
            }
        }
        
        public CellData(string cellId, CellType cellType, string cellSubtype)
        {
            id = cellId;
            type = cellType;
            subtype = cellSubtype;
            playerId = 0;
            content = null;
            promotion = false;
            promotionPlayerId = 0;
            colorLayer = "none";
        }
    }

    [Serializable]
    public class PieceData
    {
        [Header("Identity")]
        public string id;
        public PieceType type;
        public PlayerColor owner;
        public string skinId;
        
        [Header("Position")]
        public Vector2Int position;
        
        [Header("Stats")]
        public int healthPoints;
        public int maxHealthPoints;
        public int defense; // Range 3-7
        public AttackRange attackRange;
        public int level;
        public bool isPromoted;
        
        [Header("Magic")]
        public List<MagicLevel> magicPowers;

        public PieceData(string pieceId, PieceType pieceType, PlayerColor player, Vector2Int pos, int hp, int def, AttackRange attack, string skin = "default")
        {
            id = pieceId;
            type = pieceType;
            owner = player;
            position = pos;
            healthPoints = hp;
            maxHealthPoints = hp;
            defense = Mathf.Clamp(def, 3, 7);
            attackRange = attack;
            level = 1;
            isPromoted = false;
            skinId = skin;
            magicPowers = new List<MagicLevel>();
        }

        public bool HasMagicPower(MagicLevel magicLevel)
        {
            return magicPowers.Contains(magicLevel);
        }

        public void AddMagicPower(MagicLevel magicLevel)
        {
            if (!HasMagicPower(magicLevel))
            {
                magicPowers.Add(magicLevel);
            }
        }

        public int GetAttackDamage()
        {
            return attackRange.GetRandomAttack();
        }
    }

    [Serializable]
    public class CemeteryData
    {
        public PlayerColor owner;
        public List<Vector2Int> cemeteryPositions;
        public int filledTombs;
        public int maxTombs;

        public CemeteryData(PlayerColor player)
        {
            owner = player;
            cemeteryPositions = new List<Vector2Int>();
            filledTombs = 0;
            maxTombs = 0;
        }

        public bool IsFull()
        {
            return filledTombs >= maxTombs;
        }

        public void AddTomb()
        {
            if (filledTombs < maxTombs)
            {
                filledTombs++;
            }
        }
    }

    [Serializable]
    public class CellContent
    {
        public string type; // "piece", "chest", "seal", "tomb"
        public string pieceType; // "king", "queen", etc.
        public int playerId;
        public int level; // For chests
        public int magicLevel; // For seals
        public int invocationTurns; // For seals
    }
    
    [Serializable]
    public class BoardDimensions
    {
        public int width;
        public int height;
    }
    
    [Serializable]
    public class BoardConfiguration
    {
        [Header("Metadata")]
        public string boardId;
        public string version;
        public int chapter;
        public int level;
        public string history;
        
        [Header("Board")]
        public BoardDimensions dimensions;
        public List<CellData> cells;
        
        [Header("Legacy - Game Settings")]
        public int maxTurnsForSealInvocation = 3;
        public float sealInvocationSuccessRate = 1.0f;
        public float finalTurnSealSuccessRate = 0.5f;

        public BoardConfiguration()
        {
            cells = new List<CellData>();
            dimensions = new BoardDimensions();
        }

        public CellData GetCell(Vector2Int position)
        {
            return cells.Find(cell => cell.Position == position);
        }

        public CellData GetCell(int x, int y)
        {
            return GetCell(new Vector2Int(x, y));
        }
        
        public CellData GetCellById(string id)
        {
            return cells.Find(cell => cell.id == id);
        }

        public CellData GetPieceCell(Vector2Int position)
        {
            return cells.Find(cell => cell.Position == position && 
                             cell.content != null && cell.content.type == "piece");
        }
        
        public List<CellData> GetPlayerPieceCells(int playerId)
        {
            return cells.FindAll(cell => cell.content != null && 
                               cell.content.type == "piece" && 
                               cell.content.playerId == playerId);
        }

        public bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < dimensions.width && 
                   position.y >= 0 && position.y < dimensions.height;
        }

        public bool IsBorderCell(Vector2Int position)
        {
            var cell = GetCell(position);
            return cell != null && cell.type == CellType.Border;
        }
        
        public List<CellData> GetPlayerCemetery(int playerId)
        {
            return cells.FindAll(cell => cell.type == CellType.Cemetery && cell.playerId == playerId);
        }
        
        public bool IsCemeteryFull(int playerId)
        {
            // Performance optimisée : compte directement les empty restantes
            return !cells.Any(cell => cell.type == CellType.Cemetery && 
                                    cell.playerId == playerId && 
                                    cell.subtype == "empty");
        }
        
        public int GetCemeteryCapacity(int playerId)
        {
            return GetPlayerCemetery(playerId).Count;
        }
        
        public int GetFilledTombsCount(int playerId)
        {
            return cells.Count(cell => cell.type == CellType.Cemetery && 
                                     cell.playerId == playerId && 
                                     cell.subtype == "filled");
        }

        public bool CanPlacePiece(Vector2Int position)
        {
            if (!IsValidPosition(position) || IsBorderCell(position))
                return false;

            var cell = GetCell(position);
            return cell != null && cell.content == null;
        }
    }
}
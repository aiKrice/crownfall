using System.Collections.Generic;
using UnityEngine;

namespace Crownfall.Pieces
{
    [CreateAssetMenu(fileName = "New_PieceModel", menuName = "Crownfall/Piece Model")]
    public class PieceModel : ScriptableObject
    {
        [Header("🎯 Base Info")]
        public int pieceId = 1;
        public string pieceName = "King";
        public PieceRarity rarity = PieceRarity.Common;
        
        [Header("🔒 Sealed State")]
        public int sealedMaxTurns = 10;
        
        [Header("💪 Base Stats (Level 1)")]
        public int baseHp = 100;
        public int baseDefense = 5;
        public float levelStatBonus = 0.1f; // +10% HP/Attack par niveau
        
        [Header("⚔️ Combat")]
        [Tooltip("Protected from sudden death")]
        public bool suddenDeathProtection = false;
        [Tooltip("Can cause sudden death to others")]
        public bool canSuddenDeath = true;
        
        [Header("🎲 Battle Stats")]
        public Vector2Int strengthRange = new Vector2Int(10, 20);
        public int battleDefense = 4;
        public Vector2Int battleRange = new Vector2Int(1, 11);
        
        [Header("🚀 Movement")]
        public List<MovementData> normalMoves = new List<MovementData>();
        public List<MovementData> escapeMoves = new List<MovementData>();
        
        [Header("✨ Spells")]
        public List<SpellData> availableSpells = new List<SpellData>();
        
        [Header("⭐ PROMOTED Stats (In-Game Only)")]
        [Space(10)]
        public int promotedHp = 55;
        public int promotedDefense = 7;
        public Vector2Int promotedStrengthRange = new Vector2Int(15, 25);
        public int promotedBattleDefense = 6;
        
        [Header("⭐ Promoted Movement")]
        public List<MovementData> promotedNormalMoves = new List<MovementData>();
        public List<MovementData> promotedEscapeMoves = new List<MovementData>();
        
        [Header("⭐ Promoted Spells")]
        public List<SpellData> promotedSpells = new List<SpellData>();
        
        [Header("🎨 Base Visuals")]
        public List<Sprite> normalSkins = new List<Sprite>();
        public Color pieceColor = Color.white;
        
        [Header("🎨 Promoted Visuals")]
        public List<Sprite> promotedSkins = new List<Sprite>();
        public Color promotedPieceColor = Color.white;
        
        // 📊 Calculate stats at specific level
        public int GetHpAtLevel(int level)
        {
            return Mathf.RoundToInt(baseHp * (1 + (level - 1) * levelStatBonus));
        }
        
        public Vector2Int GetStrengthAtLevel(int level)
        {
            float bonus = (level - 1) * levelStatBonus;
            int minStr = Mathf.RoundToInt(strengthRange.x * (1 + bonus));
            int maxStr = Mathf.RoundToInt(strengthRange.y * (1 + bonus));
            return new Vector2Int(minStr, maxStr);
        }
        
        // 📊 Calculate PROMOTED stats at specific level
        public int GetPromotedHpAtLevel(int level)
        {
            return Mathf.RoundToInt(promotedHp * (1 + (level - 1) * levelStatBonus));
        }
        
        public Vector2Int GetPromotedStrengthAtLevel(int level)
        {
            float levelBonus = (level - 1) * levelStatBonus;
            int minStr = Mathf.RoundToInt(promotedStrengthRange.x * (1 + levelBonus));
            int maxStr = Mathf.RoundToInt(promotedStrengthRange.y * (1 + levelBonus));
            return new Vector2Int(minStr, maxStr);
        }
        
        public int GetSpellPowerAtLevel(int spellIndex, int level)
        {
            if (spellIndex >= availableSpells.Count) return 0;
            var spell = availableSpells[spellIndex];
            return Mathf.RoundToInt(spell.basePower * (1 + (level - 1) * spell.levelPowerBonus));
        }
        
        // 🎨 Get random skin for current state
        public Sprite GetRandomNormalSkin()
        {
            if (normalSkins.Count == 0) return null;
            return normalSkins[UnityEngine.Random.Range(0, normalSkins.Count)];
        }
        
        public Sprite GetRandomPromotedSkin()
        {
            if (promotedSkins.Count == 0) return null;
            return promotedSkins[UnityEngine.Random.Range(0, promotedSkins.Count)];
        }
        
        // 🔍 Validation
        private void OnValidate()
        {
            if (baseHp <= 0) baseHp = 1;
            if (levelStatBonus < 0) levelStatBonus = 0;
            if (strengthRange.x > strengthRange.y) strengthRange.y = strengthRange.x;
        }
    }
}
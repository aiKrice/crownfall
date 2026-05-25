using System;
using System.Collections.Generic;
using UnityEngine;

namespace Crownfall.Pieces
{
    [Serializable]
    public enum SpellType
    {
        Damage,    // Attaque
        Heal,      // Soins
        Buff,      // Amélioration temporaire
        Debuff,    // Malédiction temporaire
        Utility    // Téléportation, etc.
    }
    
    [Serializable]
    public enum SpellTargetingType
    {
        Linear,      // Ligne droite dans une direction
        Radial,      // Toutes cases dans un rayon (cercle)
        Pattern,     // Pattern personnalisé (ex: 3 cases devant)
        Cross,       // Forme en croix (+)
        Diagonal,    // Forme en X
        Adjacent     // Cases adjacentes uniquement
    }
    
    [Serializable]
    public enum ObstacleBehavior
    {
        Pierce,      // Traverse toutes les pièces
        Stop,        // S'arrête à la première pièce
        Friendly     // S'arrête aux alliés, traverse les ennemis
    }
    
    [CreateAssetMenu(fileName = "New_Spell", menuName = "Crownfall/Spell")]
    public class SpellData : ScriptableObject
    {
        [Header("🔮 Spell Info")]
        public int spellId;
        public string spellName;
        public SpellType type;
        
        [Header("⚡ Usage")]
        public int maxUses = 3;           // Utilisations par bataille
        public int cooldown = 0;          // Tours de recharge
        
        [Header("📊 Base Stats (Level 1)")]
        public int basePower = 10;        // Dégâts ou soins de base
        public float levelPowerBonus = 0.15f;  // +15% puissance par niveau
        
        [Header("🎯 Targeting")]
        public SpellTargetingType targetingType = SpellTargetingType.Linear;
        public Vector2Int rangeMinMax = new Vector2Int(1, 3);  // Min/Max portée
        public List<Direction> allowedDirections = new List<Direction> { Direction.N };
        public ObstacleBehavior obstacleBehavior = ObstacleBehavior.Stop;
        
        [Header("🎯 Custom Pattern (if Pattern type)")]
        [Tooltip("Positions relatives: (0,1)=devant, (1,0)=droite, (-1,1)=diagonal")]
        public List<Vector2Int> customPattern = new List<Vector2Int>();
        
        [Header("🎯 Effects")]
        public bool pierceArmor = false;  // Ignore la défense
        public bool selfCast = false;     // Peut cibler soi-même
        public bool areaOfEffect = false; // Effet de zone sur case cible
        public int aoeRadius = 1;         // Rayon effet de zone
        
        [Header("🎨 Visuals")]
        public Color spellColor = Color.white;
        public string animationTrigger;
        
        // 📊 Calculate power at specific level
        public int GetPowerAtLevel(int level)
        {
            return Mathf.RoundToInt(basePower * (1 + (level - 1) * levelPowerBonus));
        }
    }
}
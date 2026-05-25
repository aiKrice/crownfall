using System;
using System.Collections.Generic;
using UnityEngine;

namespace Crownfall.Pieces
{
    [Serializable]
    public enum MovementType
    {
        Linear,    // N, S, E, W
        Diagonal,  // NE, NW, SE, SW
        Jump,      // Knight-like L-shaped moves
        Teleport   // Direct teleport
    }
    
    [Serializable]
    public enum Direction
    {
        N, S, E, W,           // Linear
        NE, NW, SE, SW        // Diagonal
    }
    
    [Serializable]
    public class MovementData
    {
        [Header("Movement Type")]
        public MovementType type;
        
        [Header("Directions (for Linear/Diagonal)")]
        public List<Direction> directions = new List<Direction>();
        
        [Header("Range")]
        public int range = 1;
        
        [Header("Jump Pattern (for Knight-like moves)")]
        [Tooltip("For Jump type: (2,1) means 2 forward, 1 sideways")]
        public Vector2Int jumpPattern = new Vector2Int(2, 1);
        
        [Header("Special Properties")]
        public bool ignoresObstacles = false;
        public bool requiresLineOfSight = true;
    }
}
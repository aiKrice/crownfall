using System;

namespace Crownfall.Board
{
    [Serializable]
    public enum CellType
    {
        Border,      // Decorative border cells
        Normal,      // Normal light/dark cells
        Cemetery,    // Cemetery cells
        Promotion,   // Promotion cells
        Chest,       // Chest cells
        Seal         // Seal cells
    }

    [Serializable]
    public enum CellColor
    {
        Light,       // Light cells (even)
        Dark         // Dark cells (odd)
    }

    [Serializable]
    public enum PlayerColor
    {
        Blue,        // Active player (blue)
        Red          // Opponent (red)
    }

    [Serializable]
    public enum PieceType
    {
        King,
        Queen,
        OrkMan,
        Parrot,
        Lion,
        Magician,
        Knight
    }

    [Serializable]
    public enum ChestReward
    {
        Death,       // Instant death (becomes seal)
        Promotion,   // Unit promotion
        RandomSummon // Random summon nearby
    }

    [Serializable]
    public enum MagicLevel
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Level6 = 6
    }
}
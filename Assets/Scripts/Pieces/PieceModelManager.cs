using System.Collections.Generic;
using UnityEngine;

namespace Crownfall.Pieces
{
    public class PieceModelManager : MonoBehaviour
    {
        [Header("📦 Piece Models")]
        public PieceModel[] pieceModels = new PieceModel[0]; // Array libre, pas de taille fixe
        
        private Dictionary<int, PieceModel> pieceModelDict = new Dictionary<int, PieceModel>();
        
        private static PieceModelManager instance;
        public static PieceModelManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<PieceModelManager>();
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            BuildDictionary();
            ValidatePieceModels();
        }

        private void BuildDictionary()
        {
            pieceModelDict.Clear();
            
            foreach (var model in pieceModels)
            {
                if (model != null)
                {
                    if (pieceModelDict.ContainsKey(model.pieceId))
                    {
                        Debug.LogWarning($"Duplicate piece ID {model.pieceId}! Using latest one: {model.pieceName}");
                    }
                    pieceModelDict[model.pieceId] = model;
                }
            }
            
            Debug.Log($"Loaded {pieceModelDict.Count} piece models into dictionary");
        }

        private void ValidatePieceModels()
        {
            bool hasErrors = false;
            
            foreach (var model in pieceModels)
            {
                if (model == null)
                {
                    Debug.LogWarning("Null PieceModel found in array!");
                    hasErrors = true;
                }
                else if (model.pieceId <= 0)
                {
                    Debug.LogWarning($"Invalid piece ID {model.pieceId} for {model.pieceName}! ID must be > 0");
                    hasErrors = true;
                }
            }
            
            if (!hasErrors)
            {
                Debug.Log("✅ All PieceModels validated successfully!");
            }
        }

        public PieceModel GetPieceModel(int pieceId)
        {
            if (pieceModelDict.TryGetValue(pieceId, out PieceModel model))
            {
                return model;
            }
            Debug.LogError($"No PieceModel found with ID: {pieceId}");
            return null;
        }

        public PieceModel GetPieceModelByName(string pieceName)
        {
            foreach (var model in pieceModels)
            {
                if (model != null && model.pieceName.Equals(pieceName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return model;
                }
            }
            Debug.LogError($"No PieceModel found with name: {pieceName}");
            return null;
        }

        public Sprite GetPieceSprite(int pieceId, bool isPromoted, int playerColor)
        {
            var model = GetPieceModel(pieceId);
            if (model == null) return null;
            
            var skins = isPromoted ? model.promotedSkins : model.normalSkins;
            
            if (skins.Count == 0) return null;
            
            // For now, return first skin. Later can add player color logic or random selection
            return skins[0];
        }

        public Color GetPieceColor(int pieceId, bool isPromoted)
        {
            var model = GetPieceModel(pieceId);
            if (model == null) return Color.white;
            
            return isPromoted ? model.promotedPieceColor : model.pieceColor;
        }

        // Debug methods
        [ContextMenu("List All Piece Models")]
        public void ListAllPieceModels()
        {
            Debug.Log("=== PIECE MODELS ===");
            foreach (var kvp in pieceModelDict)
            {
                var model = kvp.Value;
                Debug.Log($"ID:{kvp.Key} {model.pieceName} ({model.rarity}) - Normal:{model.normalSkins.Count} Promoted:{model.promotedSkins.Count}");
            }
        }
        
        [ContextMenu("Rebuild Dictionary")]
        public void RebuildDictionary()
        {
            BuildDictionary();
            Debug.Log("Dictionary rebuilt!");
        }
    }
}
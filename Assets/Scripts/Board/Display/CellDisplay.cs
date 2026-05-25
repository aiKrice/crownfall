using UnityEngine;

namespace Crownfall.Board
{
    public class CellDisplay : MonoBehaviour
    {
        [Header("Visual Components")]
        public Renderer cellRenderer;
        public Material lightCellMaterial;
        public Material darkCellMaterial;
        public Material borderCellMaterial;
        public Material cemeteryCellMaterial;
        public Material promotionCellMaterial;

        [Header("Special Effects")]
        public GameObject highlightEffect;
        public GameObject cemeteryTombPrefab;

        private CellData cellData;
        private bool isHighlighted = false;

        public CellData CellData => cellData;

        public void Initialize(CellData data)
        {
            cellData = data;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (cellRenderer == null || cellData == null) return;

            Material materialToUse = null;

            switch (cellData.type)
            {
                case CellType.Border:
                    materialToUse = borderCellMaterial;
                    break;
                case CellType.Cemetery:
                    materialToUse = cemeteryCellMaterial;
                    break;
                default:
                    // Use color layer for material selection
                    if (cellData.promotion)
                    {
                        materialToUse = promotionCellMaterial;
                    }
                    else
                    {
                        materialToUse = cellData.colorLayer == "blue" ? 
                            lightCellMaterial : darkCellMaterial;
                    }
                    break;
            }

            if (materialToUse != null)
            {
                cellRenderer.material = materialToUse;
            }

            // Handle cemetery tomb
            if (cellData.type == CellType.Cemetery && cellData.subtype == "filled")
            {
                ShowCemeteryTomb();
            }
        }

        private void ShowCemeteryTomb()
        {
            if (cemeteryTombPrefab != null)
            {
                var tomb = Instantiate(cemeteryTombPrefab, transform);
                tomb.transform.localPosition = Vector3.up * 0.1f;
            }
        }

        public void SetHighlight(bool highlight)
        {
            isHighlighted = highlight;
            
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(highlight);
            }
        }

        public bool IsValidForMovement()
        {
            if (cellData == null) return false;
            
            return cellData.type != CellType.Border && 
                   (cellData.content == null || cellData.content.type != "seal");
        }

        public bool IsPromotionCell()
        {
            return cellData != null && cellData.promotion;
        }

        public bool IsCemeteryCell()
        {
            return cellData != null && cellData.type == CellType.Cemetery;
        }

        private void OnMouseEnter()
        {
            if (IsValidForMovement())
            {
                SetHighlight(true);
            }
        }

        private void OnMouseExit()
        {
            SetHighlight(false);
        }

        private void OnMouseDown()
        {
            // Handle cell click - can be used for piece movement
            var boardManager = FindFirstObjectByType<BoardManager>();
            if (boardManager != null)
            {
                // Notify board manager of cell click
                Debug.Log($"Cell clicked: {cellData.id} at {cellData.Position}");
            }
        }
    }
}
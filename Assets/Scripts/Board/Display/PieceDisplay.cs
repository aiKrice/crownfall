using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Crownfall.Board
{
    public class PieceDisplay : MonoBehaviour
    {
        [Header("Visual Components")]
        public Renderer pieceRenderer;
        public Material blueMaterial;
        public Material redMaterial;
        
        [Header("UI Elements")]
        public Canvas uiCanvas;
        public Slider healthBar;
        public TextMeshProUGUI levelText;
        public GameObject promotionIndicator;
        public GameObject[] magicPowerIndicators; // Max 7 indicators for magic levels 0-6

        [Header("Effects")]
        public ParticleSystem promotionEffect;
        public ParticleSystem magicEffect;
        public GameObject selectionHighlight;

        [Header("Animation")]
        public Animator pieceAnimator;

        private PieceData pieceData;
        private bool isSelected = false;
        private Vector3 originalScale;

        public PieceData PieceData => pieceData;

        private void Awake()
        {
            originalScale = transform.localScale;
            
            if (uiCanvas != null)
            {
                uiCanvas.worldCamera = Camera.main;
            }
        }

        public void Initialize(PieceData data)
        {
            pieceData = data;
            UpdateVisuals();
            UpdateUI();
            UpdateMagicIndicators();
        }

        private void UpdateVisuals()
        {
            if (pieceRenderer == null || pieceData == null) return;

            // Set material based on owner
            Material materialToUse = pieceData.owner == PlayerColor.Blue ? blueMaterial : redMaterial;
            if (materialToUse != null)
            {
                pieceRenderer.material = materialToUse;
            }

            // Handle promotion visual
            if (pieceData.isPromoted && promotionIndicator != null)
            {
                promotionIndicator.SetActive(true);
            }

            // Update animator if present
            if (pieceAnimator != null)
            {
                pieceAnimator.SetInteger("PieceType", (int)pieceData.type);
                pieceAnimator.SetBool("IsPromoted", pieceData.isPromoted);
                pieceAnimator.SetInteger("Level", pieceData.level);
            }
        }

        private void UpdateUI()
        {
            if (pieceData == null) return;

            // Update health bar
            if (healthBar != null)
            {
                healthBar.maxValue = pieceData.maxHealthPoints;
                healthBar.value = pieceData.healthPoints;
                
                // Color health bar based on health percentage
                var fillImage = healthBar.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    float healthPercent = (float)pieceData.healthPoints / pieceData.maxHealthPoints;
                    fillImage.color = Color.Lerp(Color.red, Color.green, healthPercent);
                }
            }

            // Update level text
            if (levelText != null)
            {
                levelText.text = pieceData.level.ToString();
                levelText.color = pieceData.isPromoted ? Color.gold : Color.white;
            }
        }

        private void UpdateMagicIndicators()
        {
            if (magicPowerIndicators == null || pieceData == null) return;

            for (int i = 0; i < magicPowerIndicators.Length; i++)
            {
                if (magicPowerIndicators[i] != null)
                {
                    MagicLevel magicLevel = (MagicLevel)i;
                    bool hasPower = pieceData.HasMagicPower(magicLevel);
                    magicPowerIndicators[i].SetActive(hasPower);
                }
            }
        }

        public void UpdatePromotion()
        {
            if (pieceData == null) return;

            UpdateVisuals();
            UpdateUI();

            // Play promotion effect
            if (promotionEffect != null)
            {
                promotionEffect.Play();
            }

            // Animate promotion
            if (pieceAnimator != null)
            {
                pieceAnimator.SetTrigger("Promote");
            }
        }

        public void TakeDamage(int damage)
        {
            if (pieceData == null) return;

            int actualDamage = Mathf.Max(0, damage - pieceData.defense);
            pieceData.healthPoints = Mathf.Max(0, pieceData.healthPoints - actualDamage);

            UpdateUI();

            // Play damage animation
            if (pieceAnimator != null)
            {
                pieceAnimator.SetTrigger("TakeDamage");
            }

            // Check if piece is destroyed
            if (pieceData.healthPoints <= 0)
            {
                OnPieceDestroyed();
            }
        }

        public void Heal(int amount)
        {
            if (pieceData == null) return;

            pieceData.healthPoints = Mathf.Min(pieceData.maxHealthPoints, pieceData.healthPoints + amount);
            UpdateUI();

            // Play heal effect
            if (magicEffect != null)
            {
                magicEffect.Play();
            }
        }

        public void AddMagicPower(MagicLevel magicLevel)
        {
            if (pieceData == null) return;

            pieceData.AddMagicPower(magicLevel);
            UpdateMagicIndicators();

            // Play magic acquisition effect
            if (magicEffect != null)
            {
                magicEffect.Play();
            }
        }

        private void OnPieceDestroyed()
        {
            // Play death animation
            if (pieceAnimator != null)
            {
                pieceAnimator.SetTrigger("Die");
            }

            // Notify board manager
            var boardManager = FindFirstObjectByType<BoardManager>();
            if (boardManager != null)
            {
                // Find the cell containing this piece to remove it
                var board = boardManager.GetCurrentBoard();
                var pieceCell = board?.cells?.Find(c => 
                    c.content?.type == "piece" && c.Position == pieceData.position);
                
                if (pieceCell != null)
                {
                    boardManager.RemovePiece(pieceCell.id);
                }
            }
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;

            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(selected);
            }

            // Scale animation for selection
            transform.localScale = selected ? originalScale * 1.1f : originalScale;
        }

        public Vector2Int GetBoardPosition()
        {
            return pieceData?.position ?? Vector2Int.zero;
        }

        public bool CanMoveTo(Vector2Int targetPosition)
        {
            if (pieceData == null) return false;

            var boardManager = FindFirstObjectByType<BoardManager>();
            if (boardManager == null) return false;

            // Find the source cell containing this piece
            var board = boardManager.GetCurrentBoard();
            if (board == null) return false;
            
            var sourceCell = board.cells?.Find(c => 
                c.content?.type == "piece" && c.Position == pieceData.position);
            
            var targetCell = board.GetCell(targetPosition);
            
            if (sourceCell == null || targetCell == null) return false;

            return boardManager.CanMoveTo(sourceCell, targetCell);
        }

        private void OnMouseDown()
        {
            if (pieceData == null) return;

            // Handle piece selection
            var boardManager = FindFirstObjectByType<BoardManager>();
            if (boardManager != null)
            {
                Debug.Log($"Piece selected: {pieceData.id} at {pieceData.position}");
                SetSelected(!isSelected);
            }
        }

        private void OnMouseEnter()
        {
            // Show hover effect
            transform.localScale = originalScale * 1.05f;
        }

        private void OnMouseExit()
        {
            if (!isSelected)
            {
                transform.localScale = originalScale;
            }
        }

        // Animation event callbacks
        public void OnPromotionAnimationComplete()
        {
            Debug.Log($"Promotion animation completed for {pieceData?.id}");
        }

        public void OnDeathAnimationComplete()
        {
            // Clean up the piece object
            Destroy(gameObject);
        }

        public void OnAttackAnimationComplete()
        {
            Debug.Log($"Attack animation completed for {pieceData?.id}");
        }
    }
}
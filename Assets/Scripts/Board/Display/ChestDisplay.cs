using UnityEngine;

namespace Crownfall.Board
{
    public class ChestDisplay : MonoBehaviour
    {
        [Header("Visual Components")]
        public Animator chestAnimator;
        public ParticleSystem openEffect;
        public ParticleSystem rewardEffect;
        
        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip openSound;
        public AudioClip rewardSound;

        private Vector2Int position;
        private BoardManager boardManager;
        private bool isOpened = false;

        public Vector2Int Position => position;
        public bool IsOpened => isOpened;

        public void Initialize(Vector2Int pos, BoardManager manager)
        {
            position = pos;
            boardManager = manager;
            
            // Set initial state
            if (chestAnimator != null)
            {
                chestAnimator.SetBool("IsOpened", false);
            }
        }

        public void OpenChest()
        {
            if (isOpened) return;

            isOpened = true;

            // Play opening animation
            if (chestAnimator != null)
            {
                chestAnimator.SetBool("IsOpened", true);
                chestAnimator.SetTrigger("Open");
            }

            // Play opening effect
            if (openEffect != null)
            {
                openEffect.Play();
            }

            // Play sound
            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }

            Debug.Log($"Chest opened at {position}");
        }

        public void ShowRewardEffect(ChestReward reward)
        {
            if (rewardEffect != null)
            {
                // Configure particle effect based on reward type
                var main = rewardEffect.main;
                switch (reward)
                {
                    case ChestReward.Death:
                        main.startColor = Color.black;
                        break;
                    case ChestReward.Promotion:
                        main.startColor = Color.gold;
                        break;
                    case ChestReward.RandomSummon:
                        main.startColor = Color.cyan;
                        break;
                }
                
                rewardEffect.Play();
            }

            // Play reward sound
            if (audioSource != null && rewardSound != null)
            {
                audioSource.PlayOneShot(rewardSound);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isOpened) return;

            var pieceDisplay = other.GetComponent<PieceDisplay>();
            if (pieceDisplay != null && boardManager != null)
            {
                var piece = pieceDisplay.PieceData;
                if (piece != null && piece.position == position)
                {
                    // Find the source cell containing this piece
                    var sourceCell = boardManager.GetCurrentBoard()?.cells?.Find(c => 
                        c.content?.type == "piece" && c.Position == piece.position);
                    
                    // Find the chest cell
                    var chestCell = boardManager.GetCurrentBoard()?.GetCell(position);
                    
                    if (sourceCell != null && chestCell != null)
                    {
                        OpenChest();
                        boardManager.HandleChestInteraction(sourceCell, chestCell);
                    }
                }
            }
        }

        private void OnMouseDown()
        {
            if (isOpened) return;

            Debug.Log($"Chest clicked at {position}");
            // Optional: Allow manual opening for testing
        }

        private void OnMouseEnter()
        {
            if (!isOpened)
            {
                // Add hover effect
                transform.localScale = Vector3.one * 1.1f;
            }
        }

        private void OnMouseExit()
        {
            if (!isOpened)
            {
                transform.localScale = Vector3.one;
            }
        }

        // Animation event callback
        public void OnOpenAnimationComplete()
        {
            Debug.Log($"Chest open animation completed at {position}");
        }

        // Called when the chest should be destroyed after opening
        public void DestroyChest()
        {
            // Add destruction effect if needed
            Destroy(gameObject, 1f); // Delay to allow effects to finish
        }
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Crownfall.Board
{
    public class SealDisplay : MonoBehaviour
    {
        [Header("Visual Components")]
        public Renderer sealRenderer;
        public Material sealMaterial;
        public ParticleSystem sealEffect;
        public ParticleSystem invocationEffect;
        
        [Header("UI Elements")]
        public Canvas uiCanvas;
        public TextMeshProUGUI turnsRemainingText;
        public Image invocationChanceBar;
        public Button invocationButton;
        
        [Header("Animation")]
        public Animator sealAnimator;
        
        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip sealCreateSound;
        public AudioClip invocationSuccessSound;
        public AudioClip invocationFailSound;

        private Vector2Int position;
        private int turnsRemaining;
        private BoardManager boardManager;
        private bool isInvoking = false;

        public Vector2Int Position => position;
        public int TurnsRemaining => turnsRemaining;

        private void Awake()
        {
            if (uiCanvas != null)
            {
                uiCanvas.worldCamera = Camera.main;
            }

            if (invocationButton != null)
            {
                invocationButton.onClick.AddListener(OnInvocationButtonClick);
            }
        }

        public void Initialize(Vector2Int pos, int turns, BoardManager manager)
        {
            position = pos;
            turnsRemaining = turns;
            boardManager = manager;
            
            UpdateVisuals();
            UpdateUI();

            // Play seal creation effect
            if (sealEffect != null)
            {
                sealEffect.Play();
            }

            // Play creation sound
            if (audioSource != null && sealCreateSound != null)
            {
                audioSource.PlayOneShot(sealCreateSound);
            }

            // Set initial animation state
            if (sealAnimator != null)
            {
                sealAnimator.SetInteger("TurnsRemaining", turnsRemaining);
            }
        }

        private void UpdateVisuals()
        {
            if (sealRenderer != null && sealMaterial != null)
            {
                sealRenderer.material = sealMaterial;
                
                // Adjust material properties based on remaining turns
                float urgency = 1f - (float)turnsRemaining / 3f; // Assuming max 3 turns
                Color sealColor = Color.Lerp(Color.blue, Color.red, urgency);
                sealRenderer.material.color = sealColor;
            }
        }

        private void UpdateUI()
        {
            if (turnsRemainingText != null)
            {
                turnsRemainingText.text = turnsRemaining.ToString();
                turnsRemainingText.color = turnsRemaining <= 1 ? Color.red : Color.white;
            }

            if (invocationChanceBar != null && boardManager != null)
            {
                var board = boardManager.GetCurrentBoard();
                if (board != null)
                {
                    float successRate = turnsRemaining == 1 ? 
                        board.finalTurnSealSuccessRate : 
                        board.sealInvocationSuccessRate;
                    
                    invocationChanceBar.fillAmount = successRate;
                    invocationChanceBar.color = Color.Lerp(Color.red, Color.green, successRate);
                }
            }

            if (invocationButton != null)
            {
                invocationButton.interactable = !isInvoking && turnsRemaining > 0;
            }
        }

        public void UpdateTurns(int newTurns)
        {
            turnsRemaining = newTurns;
            UpdateVisuals();
            UpdateUI();

            if (sealAnimator != null)
            {
                sealAnimator.SetInteger("TurnsRemaining", turnsRemaining);
                sealAnimator.SetTrigger("UpdateTurns");
            }

            if (turnsRemaining <= 0)
            {
                DestroySeal();
            }
        }

        public void AttemptInvocation(PlayerColor invoker)
        {
            if (isInvoking || boardManager == null) return;

            isInvoking = true;
            
            // Play invocation animation
            if (sealAnimator != null)
            {
                sealAnimator.SetTrigger("Invoke");
            }

            // Play invocation effect
            if (invocationEffect != null)
            {
                invocationEffect.Play();
            }

            bool success = boardManager.TryInvokeSeal(position, invoker);
            
            // Play appropriate sound
            if (audioSource != null)
            {
                AudioClip soundToPlay = success ? invocationSuccessSound : invocationFailSound;
                if (soundToPlay != null)
                {
                    audioSource.PlayOneShot(soundToPlay);
                }
            }

            if (success)
            {
                OnInvocationSuccess();
            }
            else
            {
                OnInvocationFailed();
            }

            isInvoking = false;
        }

        private void OnInvocationSuccess()
        {
            Debug.Log($"Seal invocation successful at {position}");
            
            // Play success effect
            if (invocationEffect != null)
            {
                var main = invocationEffect.main;
                main.startColor = Color.green;
                invocationEffect.Play();
            }

            // The seal will be destroyed by BoardManager
        }

        private void OnInvocationFailed()
        {
            Debug.Log($"Seal invocation failed at {position}. Turns remaining: {turnsRemaining}");
            
            // Play failure effect
            if (invocationEffect != null)
            {
                var main = invocationEffect.main;
                main.startColor = Color.red;
                invocationEffect.Play();
            }

            // Update UI will be called by BoardManager when it updates turns
        }

        private void OnInvocationButtonClick()
        {
            // For now, assume it's always the blue player trying to invoke
            // In a real game, you'd determine the current player
            AttemptInvocation(PlayerColor.Blue);
        }

        private void DestroySeal()
        {
            // Play destruction animation
            if (sealAnimator != null)
            {
                sealAnimator.SetTrigger("Destroy");
            }

            // Add destruction effect
            if (sealEffect != null)
            {
                var main = sealEffect.main;
                main.startColor = Color.gray;
                sealEffect.Play();
            }

            Debug.Log($"Seal destroyed at {position}");
            
            // Destroy after animation
            Destroy(gameObject, 1f);
        }

        private void OnMouseDown()
        {
            if (!isInvoking)
            {
                Debug.Log($"Seal clicked at {position}. Turns remaining: {turnsRemaining}");
                // Could open a context menu or attempt invocation
            }
        }

        private void OnMouseEnter()
        {
            // Add hover effect
            transform.localScale = Vector3.one * 1.1f;
        }

        private void OnMouseExit()
        {
            transform.localScale = Vector3.one;
        }

        // Animation event callbacks
        public void OnInvocationAnimationComplete()
        {
            Debug.Log($"Invocation animation completed for seal at {position}");
        }

        public void OnDestroyAnimationComplete()
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Crownfall.UI
{
    /// <summary>
    /// Controls the Library tab - handles character grid and selection
    /// </summary>
    public class LibraryController : MonoBehaviour
    {        
        private void Start()
        {
            Debug.Log("📚 LibraryController initialized");
        }
        
        // ✅ APPELÉ DEPUIS L'INSPECTOR UNITY - Ne pas ajouter AddListener()
        public void OnKingSelected() => OnCharacterSelected("Roi");
        public void OnQueenSelected() => OnCharacterSelected("Reine");
        public void OnRookSelected() => OnCharacterSelected("Tour");
        public void OnBishopSelected() => OnCharacterSelected("Fou");
        
        private void OnCharacterSelected(string characterName)
        {
            Debug.Log($"🎭 Character selected in LibraryController: {characterName}");
            ShowCharacterAlert(characterName);
        }
        
        private void ShowCharacterAlert(string characterName)
        {
            // For now just log - later we'll implement a proper modal/popup
            Debug.Log($"📋 Showing character details for: {characterName}");
            
            // Placeholder for future character details modal
            // This is where we'll show character stats, abilities, etc.
        }
    }
}
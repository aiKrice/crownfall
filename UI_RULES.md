# 🎯 RÈGLES UI CROWNFALL

## ✅ RÈGLE D'OR : INSPECTOR FIRST

### **📋 UI Statique (OBLIGATOIRE) :**
```csharp
// ✅ CORRECT - Méthodes publiques pour l'Inspector
public void OnStartGameClicked() { /* logic */ }
public void OnKingSelected() { /* logic */ }
```

**Dans Unity Inspector :**
- Button → OnClick() → Assigner directement la méthode
- **JAMAIS de AddListener() dans le code**

### **💻 UI Dynamique (EXCEPTION) :**
```csharp
// ⚠️ EXCEPTION - Seulement pour UI générée dynamiquement
// Exemples : inventaires, listes, menus créés au runtime
for (int i = 0; i < items.Count; i++)
{
    button.onClick.AddListener(() => OnItemClicked(i));
}
```

## 🚫 INTERDICTIONS

### **❌ JAMAIS FAIRE :**
```csharp
// ❌ INTERDIT - Cause des doubles listeners
startGameButton.onClick.AddListener(OnStartGameClicked);

// ❌ INTERDIT - GameObject.Find() au runtime
GameObject.Find("StartGameButton")?.GetComponent<Button>();

// ❌ INTERDIT - Références publiques pour UI statique
public Button startGameButton;
```

## 📖 JUSTIFICATIONS

### **Pourquoi Inspector > Code :**
1. **Visuel** : Connexions visibles dans l'interface Unity
2. **Sérialisé** : Sauvegardé avec la scène, pas de perte
3. **Debug** : Facile à voir si connecté dans l'Inspector
4. **Performance** : Pas de recherche GameObject au runtime
5. **Sécurité** : Évite les doubles listeners accidentels

### **Quand utiliser AddListener :**
- **UI générée dynamiquement** (listes d'items, inventaires)
- **Callbacks temporaires** (modals, popups avec logique custom)
- **Systèmes de données** (grids avec contenu variable)

## 🛠️ TEMPLATE CONTROLLER

```csharp
namespace Crownfall.UI
{
    public class MyController : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("🎮 MyController initialized");
        }
        
        // ✅ APPELÉ DEPUIS L'INSPECTOR UNITY
        public void OnButtonClicked()
        {
            Debug.Log("Button clicked!");
            // Logic here
        }
        
        // ✅ APPELÉ DEPUIS L'INSPECTOR UNITY
        public void OnSliderChanged(float value)
        {
            Debug.Log($"Slider value: {value}");
            // Logic here
        }
    }
}
```

## 📝 NOTES IMPORTANTES

- **Cette règle est OBLIGATOIRE** pour tout nouveau code UI
- **Exception uniquement** pour UI dynamique avec justification
- **Commentaires obligatoires** sur les méthodes publiques Inspector
- **Nettoyage progressif** de l'ancien code avec AddListener()

---
**🎯 Objectif : Code plus propre, debug plus facile, moins de bugs !**
# 📦 Système d'Assets Automatique - Mode d'Emploi

## 🚀 Mise en Route Rapide

### Option 1: Téléchargement Automatique
```csharp
// Dans l'éditeur Unity :
1. Window → Crownfall → Download Assets
2. Attendre le téléchargement
3. Assets automatiquement configurés
```

### Option 2: Génération Procédurale
```csharp
// Si le téléchargement échoue :
1. Window → Crownfall → Generate Procedural Assets  
2. Assets créés instantanément
3. Qualité basique mais fonctionnelle
```

### Option 3: Interface Utilisateur
```csharp
// Dans le jeu en cours d'exécution :
1. Ouvrir le panneau Asset Manager
2. Cliquer "Download" ou "Generate"  
3. Voir le progrès en temps réel
```

## 🔧 Configuration Détaillée

### 1. AutoAssetImporter
**Rôle** : Télécharge des images depuis des APIs gratuites

**Sources utilisées :**
- ✅ **game-icons.net** : Pièces d'échecs SVG
- ✅ **placeholder.com** : Textures de cases temporaires
- ⚠️ **APIs alternatives** si échec

**Configuration :**
```csharp
// Dans le script AutoAssetImporter
downloadOnStart = false;  // Auto-download au démarrage
overwriteExisting = false;  // Remplacer fichiers existants

// Ajouter des assets personnalisés
importer.AddCustomAsset("ma_piece", "https://mon-url.com/image.png", AssetType.PieceSprite);
```

### 2. ProceduralAssetGenerator  
**Rôle** : Crée des assets de base par code

**Assets générés :**
- 🎨 Textures de cases (damier, unis, bordures)
- ♟️ Sprites de pièces (formes géométriques colorées)  
- ✨ Éléments spéciaux (coffres, sceaux, tombes)
- 🔮 Icônes de magie (6 niveaux différents)

**Personnalisation :**
```csharp
// Modifier les couleurs
lightCellColor = Color.white;
bluePieceColor = Color.blue;

// Modifier les tailles
textureSize = 256;  // Textures de cases
spriteSize = 512;   // Sprites de pièces
```

### 3. AssetManager
**Rôle** : Gestion centralisée de tous les assets

**Fonctionnalités :**
- ✅ Validation automatique des assets
- 🔍 Recherche de sprites par type/couleur
- 🎯 Création de matériaux de base
- 🖼️ Interface d'assignment dans l'éditeur

## 📁 Structure de Fichiers Générée

```
Assets/Art/
├── Pieces/
│   ├── Blue/
│   │   ├── blue_king.png
│   │   ├── blue_queen.png
│   │   └── ... (toutes les pièces bleues)
│   └── Red/
│       └── ... (toutes les pièces rouges)
├── Board/
│   ├── Cells/
│   │   ├── light_cell.png
│   │   ├── dark_cell.png
│   │   ├── border_cell.png
│   │   ├── cemetery_cell.png
│   │   └── promotion_cell.png
│   └── Special/
│       ├── chest.png
│       ├── seal.png
│       └── tomb.png
└── UI/
    └── MagicIcons/
        ├── magic_level_0.png
        ├── magic_level_1.png
        └── ... (jusqu'à level_6)
```

## 🎮 Utilisation Dans le Jeu

### 1. Assignment Automatique
```csharp
// Le BoardManager utilise automatiquement les assets
boardManager.LoadBoard("default_board.json");
// → Toutes les pièces et cases sont automatiquement stylées
```

### 2. Changement de Skin
```csharp
// Dans PieceData
piece.skinId = "classic_king";  // Utilise le skin par défaut
piece.skinId = "medieval_king"; // Utilise un skin alternatif (si disponible)
```

### 3. Récupération Programmatique
```csharp
// Obtenir un sprite de pièce
Sprite kingSprite = AssetManager.Instance.GetPieceSprite(PieceType.King, PlayerColor.Blue);

// Obtenir un matériau de case
Material cellMat = AssetManager.Instance.GetCellMaterial(CellType.Normal, CellColor.Light);

// Obtenir une icône de magie
Sprite magicIcon = AssetManager.Instance.GetMagicIcon(MagicLevel.Level3);
```

## 🐛 Résolution de Problèmes

### Assets Manquants
```csharp
❌ Problème : "Missing blue king sprite!"
✅ Solution : 
1. Menu → Crownfall → Generate Procedural Assets
2. Ou réassigner manuellement dans AssetManager
```

### Téléchargement Échoué
```csharp
❌ Problème : "Failed to download [asset]: 404"
✅ Solutions :
1. Vérifier la connexion internet
2. URLs peut-être expirées → Utiliser génération procédurale
3. Créer les assets manuellement et les placer dans Art/
```

### Performance Lente
```csharp
❌ Problème : Téléchargement très lent
✅ Solutions :
1. downloadOnStart = false (télécharger manuellement)
2. Réduire textureSize/spriteSize dans ProceduralAssetGenerator
3. Utiliser assets locaux au lieu du téléchargement
```

## 🎨 Personnalisation Avancée

### Ajouter de Nouvelles Sources
```csharp
// Dans AutoAssetImporter, ajouter à freeAssetUrls :
["ma_texture"] = "https://mon-site.com/texture.png",

// Puis l'utiliser :
AddCustomAsset("ma_texture", "https://mon-site.com/texture.png", AssetType.CellTexture);
```

### Créer des Skins Personnalisés
```csharp
// 1. Télécharger/créer les images
// 2. Les placer dans Assets/Art/Pieces/[Color]/
// 3. Nommer avec convention : [color]_[piece]_[skin].png
//    Exemple : blue_king_medieval.png

// 4. Dans le code :
piece.skinId = "medieval";  // Utilisera blue_king_medieval.png
```

### Modifier les Shaders/Matériaux
```csharp
// Dans AssetManager, modifier :
board.lightCellMaterial = MonMateriauPersonnalise;

// Ou créer par code :
Material nouveauMateriau = new Material(Shader.Find("Universal Render Pipeline/Lit"));
nouveauMateriau.color = Color.red;
```

## 📊 État du Système

### ✅ Fonctionnalités Implémentées
- Téléchargement automatique d'assets
- Génération procédurale de fallback
- Validation et configuration automatique
- Interface utilisateur complète
- Support multi-skin
- Gestion centralisée des assets

### 🚧 Améliorations Futures
- Support d'animations de pièces
- Assets audio automatiques
- Compression d'images avancée
- Cache intelligent des téléchargements
- Skins premium/DLC

## 💡 Conseils d'Optimisation

1. **Mobile** : Utiliser textureSize=128 pour réduire la mémoire
2. **Qualité** : Télécharger des assets haute-res manuellement pour le produit final
3. **Performance** : Précharger tous les assets au démarrage du jeu
4. **Stockage** : Utiliser compression PNG pour réduire la taille des builds

---

**🎯 Résultat Final :** Un système complet qui télécharge, génère et gère automatiquement tous les assets visuels de ton jeu, avec des fallbacks robustes et une interface utilisateur intuitive !
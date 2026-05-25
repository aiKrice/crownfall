# Guide des Assets Gratuits pour Crownfall

## 🎨 Sources d'Images Gratuites

### Pour les Pièces d'Échecs :
1. **Kenney Assets** (kenney.nl)
   - Pack "Chess Pieces" 
   - Style cartoon parfait pour mobile
   - Format PNG avec transparence
   - Licence libre

2. **Freepik** (freepik.com)
   - Recherche "chess pieces icons"
   - Style flat design moderne
   - Formats vectoriels disponibles

3. **Game-icons.net**
   - Icons d'échecs variés
   - Style médiéval/fantasy
   - SVG haute qualité

4. **OpenGameArt.org**
   - Assets complets pour jeux d'échecs
   - Différents styles artistiques
   - Communauté de développeurs

### Pour les Cases du Plateau :
1. **Textures.com** (textures.com)
   - Textures de marbre, bois
   - Patterns géométriques
   - Pack "Game Board Textures"

2. **Freepik Textures**
   - Motifs damier personnalisables
   - Textures de pierre/métal
   - Effets lumineux

### Pour les Éléments Spéciaux :
1. **Coffres** : Kenney "RPG Pack"
2. **Sceaux Magiques** : Game-icons.net "Magic Seals"
3. **Particules** : Unity Particle Pack (Asset Store gratuit)

## 📁 Structure de Dossiers Recommandée

```
Assets/
├── Art/
│   ├── Pieces/
│   │   ├── Blue/
│   │   │   ├── king_blue.png
│   │   │   ├── queen_blue.png
│   │   │   └── ...
│   │   └── Red/
│   │       ├── king_red.png
│   │       ├── queen_red.png
│   │       └── ...
│   ├── Board/
│   │   ├── Cells/
│   │   │   ├── light_cell.png
│   │   │   ├── dark_cell.png
│   │   │   ├── border_cell.png
│   │   │   ├── cemetery_cell.png
│   │   │   └── promotion_cell.png
│   │   └── Special/
│   │       ├── chest.png
│   │       ├── seal.png
│   │       └── tomb.png
│   └── UI/
│       ├── health_bar.png
│       ├── magic_icons/
│       └── buttons/
```

## 🎯 Spécifications Techniques

### Tailles d'Images :
- **Pièces** : 512x512px minimum
- **Cases** : 256x256px
- **UI Elements** : 128x128px
- **Icons Magie** : 64x64px

### Formats :
- **PNG** avec transparence pour les pièces
- **JPG** pour les textures de fond
- **SVG** pour les UI (si Unity supporte)

## 🔧 Import Settings Unity

### Pour les Pièces :
```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Bilinear
Max Size: 512
Compression: High Quality
```

### Pour les Cases :
```
Texture Type: Default
Wrap Mode: Repeat
Filter Mode: Bilinear
Max Size: 256
Compression: Normal Quality
```

## 📋 Liste de Téléchargement

1. Télécharge depuis Kenney.nl :
   - "Chess Set" pack
   - "RPG Pack" (pour coffres)

2. Télécharge depuis Game-icons.net :
   - Magic seal icons (6 différents pour les niveaux)
   - Special effect icons

3. Textures de cases depuis Textures.com :
   - Marble texture (cases claires)
   - Dark stone texture (cases foncées)

4. Unity Asset Store (gratuit) :
   - "Particle Pack" 
   - "UI Pack"

## ⚡ Actions à Faire

1. Crée les dossiers dans Assets/Art/
2. Télécharge et importe les images
3. Configure les Import Settings
4. Crée les Materials et Prefabs
5. Assigne dans le BoardManager

Veux-tu que je crée des scripts pour automatiser l'import et la configuration ?
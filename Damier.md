# 🎲 Spécifications du Damier Crownfall

## 📋 Vue d'Ensemble

Le damier de Crownfall est un plateau de jeu **non rectangulaire** avec des zones spécialisées, inspiré des échecs mais adapté pour un gameplay RPG tactique mobile. Contrairement aux échecs traditionnels, le plateau intègre des éléments fantastiques et une progression narrative.

## 📐 Structure Générale

### Dimensions du Plateau
- **Largeur** : 7 colonnes (A-G) = 2 bordures + 5 cases jouables
- **Hauteur** : 10 lignes = Structure asymétrique avec zones spécialisées
- **Format** : Portrait mobile (adapté aux écrans tactiles)

### Zones du Plateau
1. **Zone de Cimetières** (L1, L10) : Repos des pièces détruites
2. **Zone de Combat** (L2-L3, L8-L9) : Placement initial des pièces
3. **Zone Neutre** (L4-L7) : Terrain de manœuvre avec objets spéciaux
4. **Bordures Décoratives** (Colonnes A et G) : Contours surélevés

## 🎯 Types de Cases

### 1. Cases Jouables (`playable`)
- **normal** : Case standard de jeu
- **Promotion** : Cases où les pièces peuvent être promues
- **Couleurs** : Alternance bleue/rouge pour la visibilité

### 2. Cases de Bordure (`border`)
- **corner_top_left/right** : Coins supérieurs arrondis
- **corner_bottom_left/right** : Coins inférieurs arrondis  
- **side_left/right** : Bordures latérales
- **Fonction** : Délimitation et esthétique du plateau

### 3. Cases de Cimetière (`cemetery`)
- **filled** : Contient une tombe (capacité occupée)
- **empty** : Emplacement libre pour nouvelles tombes
- **Position** : Lignes 1 et 10 (zones neutres)

## 🏆 Système de Promotion

### Principe
Les pièces peuvent être **promues** uniquement en atteignant les cases de **camp adverse**.

### Règles de Promotion
- **Joueur 1 (Bleu)** : Promotion sur cases `promotionPlayerId: 2` (L8-L9)  
- **Joueur 2 (Rouge)** : Promotion sur cases `promotionPlayerId: 1` (L2-L3)
- **Cases neutres** : Pas de promotion possible

### Zones de Promotion
- **L2-L3** : Promotion pour le camp rouge (cases du camp bleu)
- **L8-L9** : Promotion pour le camp bleu (cases du camp rouge)

## 🎨 Système de Couleurs

### Couches Visuelles (`colorLayer`)
- **blue** : Cases à fond bleu (alternance damier)
- **red** : Cases à fond rouge (alternance damier)  
- **none** : Cases sans couleur (bordures, cimetières)

### Application
Les couleurs sont **indépendantes** du type de case - c'est une **couche esthétique** appliquée par-dessus la géométrie de base.

## 📦 Contenus de Cases

### 1. Pièces (`piece`)
```json
{
  "type": "piece",
  "pieceType": "king|queen|rook|bishop|knight|pawn", 
  "playerId": 1|2
}
```

### 2. Coffres (`chest`)
```json
{
  "type": "chest",
  "level": 1-3
}
```

### 3. Sceaux Magiques (`seal`)
```json
{
  "type": "seal", 
  "magicLevel": 1-6,
  "invocationTurns": 1-5
}
```

### 4. Tombes (`tomb`)
```json
{
  "type": "tomb",
  "playerId": 1|2
}
```

## 🗺️ Disposition du Niveau 1-1

### Structure des Lignes
```
L1:  [Contour] [Cimetière Plein] [4x Cimetière Vide] [Contour]
L2:  [Contour] [5x Cases Jouables + Pièces Bleues] [Contour]  
L3:  [Contour] [5x Cases Jouables + Pions Bleus] [Contour]
L4-7: [Contour] [5x Cases Vides + Objets Spéciaux] [Contour]
L8:  [Contour] [5x Cases Jouables + Pions Rouges] [Contour]
L9:  [Contour] [5x Cases Jouables + Pièces Rouges] [Contour]  
L10: [Contour] [Cimetière Plein] [4x Cimetière Vide] [Contour]
```

### Placement Initial des Pièces

**Joueur 1 (Bleu) - Lignes 2-3:**
- **L2** : Tour, Cavalier, Fou, Reine, Roi (ligne arrière)
- **L3** : 5x Pions (ligne avant)

**Joueur 2 (Rouge) - Lignes 8-9:**  
- **L8** : 5x Pions (ligne avant)
- **L9** : Roi, Reine, Fou, Cavalier, Tour (ligne arrière)

## 📱 Optimisations Mobile

### Interface Tactile
- **Cases suffisamment grandes** : Minimum 64x64 pixels
- **Feedback visuel** : Surbrillance des mouvements possibles
- **Gestes intuitifs** : Tap pour sélectionner, drag pour déplacer

### Performance
- **Instanciation dynamique** : Cases créées par le BoardManager
- **Pooling des pièces** : Réutilisation des objets 3D
- **LOD (Level of Detail)** : Qualité adaptée selon la distance

## 🔄 Système de Chargement

### Format JSON
- **Structure modulaire** : Un fichier par niveau
- **Validation automatique** : Vérification de cohérence
- **Métadonnées** : chapter, level, history pour la progression

### Fichiers de Niveau  
- `chapter1_level1.json` : Niveau d'introduction
- `chapter1_level2.json` : Complexité croissante
- `chapterN_levelM.json` : Progression narrative

## 🛠️ Implémentation Technique

### Scripts Principaux
- **BoardLoader.cs** : Parser JSON → Données internes
- **BoardManager.cs** : Instanciation du plateau 3D  
- **CellDisplay.cs** : Rendu individuel des cases
- **PieceDisplay.cs** : Gestion visuelle des pièces

### Pipeline de Génération
1. **Chargement** : BoardLoader.LoadBoard("chapter1_level1.json")
2. **Validation** : Vérification dimensions et cohérence
3. **Instanciation** : Création des GameObjects 3D
4. **Stylisation** : Application textures et matériaux
5. **Activation** : Plateau prêt pour le gameplay

## 📊 Métriques et Équilibrage

### Capacités par Défaut
- **Cimetières** : 5 emplacements par joueur (cases vides L1/L10)
- **Objets spéciaux** : 2-4 par niveau (coffres + sceaux)  
- **Cases de promotion** : 5 par camp (toute la ligne adverse)

### Progression de Difficulté
- **Chapter 1** : Introduction des mécaniques de base
- **Chapters suivants** : Plateaux plus complexes, objets avancés

---

## 🎮 Exemple Complet : Chapter 1 Level 1

Le JSON complet du premier niveau suit ces spécifications et sert de **template** pour tous les niveaux suivants.

**🎯 Objectif :** Créer un système de plateau flexible, extensible et optimisé pour mobile, tout en conservant la profondeur tactique des jeux de plateau traditionnels.
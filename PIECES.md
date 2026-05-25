# Guide des Pieces
Listes des pieces du Roaster V1
King (1) 
Queen (2)
OrkMan (3)
Parrot(4)
Lion(5)
Magician(6)
Knight(7)
Mummy(8)
Cyclop(9)
Joker(10)
Jack(11)
Skeleton (12)
Fighter (13)
Spider(14)
Demon(15)
Dragon(16)


## Json de structures (partiel a terminer)
```json
[
    {
      "id": "11",
      "name": "Valet",
      "Rarity": "Epic"
      "states": {
        "scealed": {"max_turn": 10},
        "unpromoted": {
        "sudden_death_protection": false,
        "can_sudden_death": true,
          "hp": 40,
          "defense": 5,
          "spells": [1, 2],
          "moves": [
            { "type": "linear", "directions": ["N", "S"], "range": 3 },
            { "type": "diagonal", "directions": ["NE", "SW"], "range": 1 }
          ],
          "escape_moves": [
            { "type": "linear", "directions": ["E", "W"], "range": 2 }
          ],
          "battle": {
            "strength_range": [10, 20],
            "defense": 4,
            "battle_range": [1, 11]
          }
        },
        "promoted": {
        "sudden_death_protection": true,
        "can_sudden_death": true,
          "hp": 55,
          "defense": 7,
          "spells": [3, 5],
          "moves": [
            { "type": "linear", "directions": ["N", "S", "E", "W"], "range": 5 },
            { "type": "diagonal", "directions": ["NE", "NW", "SE", "SW"], "range": 2 }
          ],
          "escape_moves": [
            { "type": "teleport", "range": 3 }
          ],
          "battle": {
            "strength_range": [15, 25],
            "defense": 6,
            "battle_range": [1, 11]
          }
        }
      }
    }
  ]
```

## Pieces rules
Pour les stat par niveau on va faire un truc simple. On va juste garder un champ level et un champ %. Le % augmentera les stat d'atk et de point de vie. et pour les sort je veux aussi des donnes sur chaque sort. Une pieces    
  aura x sort de dispo mais le sort aura aussi un niveau, une ataque un typ (heal ou damage) une valeur de depart et un % qui sera modifie. En appliquant un % on se garde de specifie niveau par niveau dans un premier temps. ce  
   sera plus rappide en cas de reequilibrage. On part aussi sur un truc simple. Les mouvements seront toujours avec un range ainsi que les sort.
   Les sorts de magie ont un nombre d'utilisation.

## Système de Promotion
- **Promoted** = Boost temporaire IN-GAME uniquement
- Activable **une seule fois par partie**
- Perdu automatiquement à la fin de chaque partie
- Effets quand activé :
  - Max HP augmente (regain complet des PV sur le plateau)
  - Accès à de nouveaux sorts plus puissants
  - Range d'attaque augmente
  - Mouvements améliorés (range, nouveaux patterns)
- **Indépendant du Level** : Level = progression permanente monétisée via fragments
- **Calcul final** : (Stats_Base + Promotion_Bonus) * (1 + (Level-1) * LevelStatBonus)

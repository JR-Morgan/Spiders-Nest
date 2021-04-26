## Spider's Nest
Spider's Nest is a first-person 3D video game created for CS3450 Game Development coursework.
The game is inspired by tower defence genre.

*Warning: For those with arachnophobia, this game contains quite a few Spiders.*

## Gameplay
The player must explore one of two hexagonal levels to find and unlock all the chests.

Certain rooms (spawner rooms) will spawn spider enemies that the player must defeat to earn money.

The player can summon a black hole that will swallow up enemies. This attack costs 10% of the players money (so becomes very expensive compared with traps).

The player can also place one of two traps. These traps are a fixed cost, so become the players only option in late game.

The player must manage their money, while dodging and kiting spiders unlocking rooms and exploring the level.

## Technical features
The game supports the following features:
 - Multiplayer through Photon Networks servers.
 - A global leader board that connects to a [Backendless](https://backendless.com/) database.
 - Serialisation of game state, so the player can continue a previous game.
 - Enemy intelligence using a logic-based [subsumption architecture](https://en.wikipedia.org/wiki/Subsumption_architecture) where the AI will select from several behaviours based on conditional logic.
 - Animated enemy with several animation states (idle, walking, attacking, dying) with use of a separate animation layer for attacking.
 - Menus and UI using the new [UI Toolkit](https://docs.unity3d.com/Manual/UI-system-compare.html)

## Controls
Movement : W,S,A,D

Camera : Mouse

Use Action : Right click

Interact : E

Pause : Escape


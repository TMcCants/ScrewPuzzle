# V0.1 Architecture Guide

This guide explains who owns each rule and where Tamika can safely change it.

## The Complete Flow

1. The player taps a `Screw`.
2. The screw asks its `ScrewDependency` whether all blockers are gone.
3. If legal, the screw asks `TrayManager` for the next open slot.
4. `TrayManager` stores the screw and checks `TrayRules` for three matching colors.
5. A matching set clears. A full tray without a match loses the level.
6. `GameManager` counts cleared screws and decides when the level is won or lost.
7. `RadioRestoration` loosens radio parts during play and runs the final restoration effect after the win.

The important design principle is that no individual screw decides the whole game.

## Script Ownership

### `LevelDefinition.cs`

Contains the changeable data for one puzzle level:

- level name and completion messages
- level number, next scene, and win-button wording
- tray capacity and match size
- every screw's color and position
- blocker indexes that point to other screws in the same level

`LevelDefinition` does not build visuals or run gameplay. That separation lets a future level
reuse the same screw, tray, and game-state systems with a different layout. For now, the radio
definition is created in code by `RadioLevelBootstrap`; a later content pass can move these
definitions into Unity assets after the level format is proven.

Safe changes: wording, tray capacity, match size, screw positions, colors, and blocker indexes.
After changing level data, always test both a winning route and a full-tray losing route.

### `ProgressManager.cs`

Stores one integer through Unity `PlayerPrefs`: the highest unlocked level. Level 1 is the default.
Completing Level 1 stores Level 2 as unlocked, and `PlayerPrefs.Save()` writes the change to the
local device.

This is intentionally not a general save-data system. It does not store scores, stars, settings,
currencies, boosters, or player accounts.

### `FeedbackAudio.cs`

Generates the prototype's short interaction cues at runtime and plays them through one persistent
2D audio source. A successful screw selection, a blocked screw, a cleared match, Game Over, and
full restoration each have a distinct cue. No imported audio clips or scene assignments are
required for this prototype pass.

The player's sound preference is stored as a single `PlayerPrefs` value. The gameplay UI toggles
that value and updates the shared audio source immediately; the saved choice persists across
levels and future app sessions.

This is deliberately a feedback service, not a gameplay-rule owner. `Screw` reports blocked
input and `TrayManager` reports accepted selections and matches; neither system waits for audio
before continuing. Final recorded sound effects can replace the generated clips without changing
the puzzle rules.

### `Screw.cs`

Owns one screw.

- receives a click or tap through `OnMouseDown`
- refuses input when already removed or moving
- consults its dependency component
- shakes briefly when the player taps it while blocked
- asks the tray to accept it
- animates itself into and within the tray
- gives a small settle pulse when it reaches a tray slot
- brightens and pops before shrinking away when its set clears

Safe changes: screw movement speed, blocked visual alpha, blocked-feedback duration,
and blocked-feedback shake distance.

Do not add match or win logic here. A screw should not know what every other screw is doing.

### `ScrewDependency.cs`

Owns the blocking list for one screw. `AreAllBlockersRemoved()` returns true only when every
referenced blocker has left the puzzle object.

Safe change: update the blocker indexes in a level's `ScrewDefinition` entries. The shared
`PrototypeLevelBuilder` converts those indexes into screw references.

An empty blocker list means the screw is open immediately.

### `TrayManager.cs`

Owns the live tray.

- stores screws in selection order
- assigns each screw to the next slot
- asks `TrayRules` whether a matching set exists
- removes a matching set
- compacts the remaining screws
- reports an unusable full tray to `GameManager`
- plays the tray's damped Game Over shake before the result overlay appears

Safe changes:

- tray capacity: change the number of generated slot transforms
- match size: change the `3` passed to `trayManager.Configure(...)`
- timing: adjust `matchPause`

The current capacity is five. A newly added screw is checked for a match before the tray is declared full. This prevents a valid third match from causing a false loss.

### `TrayRules.cs`

Contains only the pure match-finding rule. It has no Unity scene references, animations, or UI. This separation makes the most important puzzle rule easy to unit test.

### `GameManager.cs`

Owns the level state.

- `Playing`: screws may be selected
- `Won`: gameplay input stops and restoration begins
- `Lost`: gameplay input stops and the restart overlay appears

It counts screws that have cleared from the tray, not merely screws that have moved off the radio. The win requires every required screw to be cleared and the tray to be empty.

Safe change: result text or the required screw count supplied by the level builder.

### `RadioRestoration.cs`

Owns the connection between puzzle progress and the radio visuals.

Each `RadioPart` contains:

- the part's transform
- the screws holding that part
- the position offset used when it loosens
- the rotation used when it loosens

When all of a part's holding screws leave the radio, the part shifts. When the whole puzzle is won,
every part returns to its original position and the radio runs its power-on sequence.

Safe changes: release offsets, release rotations, restoration duration, power-on flicker timing,
completion-pop size, and glow color. The final effect deliberately uses three readable stages:
display flicker, one scale pop, and a short hold before the result overlay appears.

### `RestorationController.cs`

Defines the small contract shared by every restorable object: respond when a screw leaves and
play a final restoration sequence. `GameManager` talks to this general controller instead of
depending directly on the radio. `RadioRestoration` supplies the radio-specific animation; a toy
car can later supply a different animation without changing the game-state rules.

### `RadioLevelBootstrap.cs`

Builds this temporary prototype level from simple shapes at runtime. It creates the camera,
radio, screws, tray, UI, dependencies, and restoration-part assignments. It now reads the
radio's puzzle layout and rule settings from `LevelDefinition` instead of embedding those values
throughout the builder.

This is assembly code, not a gameplay-rule owner. It exists so V0.1 is immediately playable without permanent art or prefab work. When real art arrives, replace this builder with scene objects and prefabs while retaining the five gameplay systems above.

### `PrototypeLevelBuilder.cs`

Creates the temporary pieces every prototype level shares: camera, background, tray, UI, screws,
blocker references, and simple rectangle/circle visuals. Object bootstraps call these helpers so
they do not copy the puzzle setup code.

This remains prototype infrastructure. Final art can replace the generated shapes without
changing `Screw`, `TrayManager`, `GameManager`, or the level definitions.

### `ToyCarLevelBootstrap.cs` and `ToyCarRestoration.cs`

`ToyCarLevelBootstrap` owns only the toy car's level data, shapes, and part assignments. Its
restoration controller loosens the roof, hood, and wheel assembly during play. On completion, it
reassembles the car, flashes the headlights, and plays a short forward movement before displaying
the result overlay.

### `LevelSelectBootstrap.cs`

Builds the temporary level-selection screen. It always enables the radio button and asks
`ProgressManager` whether the toy-car button should be enabled. Scene names remain explicit and
beginner-readable while the game contains only two levels.

## V0.1 Level Data

The radio has nine screws:

| Set | Count | Role |
|---|---:|---|
| Red | 3 | Faceplate |
| Blue | 3 | Speaker grille |
| Yellow | 3 | Radio case |

Five screws begin selectable. That is intentional: the player can make a poor mixed-color sequence and fill the five-slot tray. The simplest winning route is red, blue, yellow.

## V0.2 Level Data

The toy car also uses nine screws, a five-slot tray, and match-three clearing. It deliberately
keeps those rules constant so the second-level test isolates whether the reusable architecture,
new object visuals, blockers, and car-specific restoration work correctly.

## Deliberately Absent

There is no score/star system, currency, advertising, purchase system, collection, booster,
analytics, achievement, account, or cloud-save integration. V0.2 saves only the highest unlocked
level locally. Adding the larger systems before the multi-level loop is validated would make the
learning build harder to reason about without improving the current alpha.

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

### `Screw.cs`

Owns one screw.

- receives a click or tap through `OnMouseDown`
- refuses input when already removed or moving
- consults its dependency component
- asks the tray to accept it
- animates itself into and within the tray
- shrinks away when its set clears

Safe changes: screw movement speed and the blocked visual alpha.

Do not add match or win logic here. A screw should not know what every other screw is doing.

### `ScrewDependency.cs`

Owns the blocking list for one screw. `AreAllBlockersRemoved()` returns true only when every referenced blocker has left the radio.

Safe change: update the blocker references for a screw. In the prototype, those references are assigned inside `RadioLevelBootstrap.ConfigureBlockers()`.

An empty blocker list means the screw is open immediately.

### `TrayManager.cs`

Owns the live tray.

- stores screws in selection order
- assigns each screw to the next slot
- asks `TrayRules` whether a matching set exists
- removes a matching set
- compacts the remaining screws
- reports an unusable full tray to `GameManager`

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

When all of a part's holding screws leave the radio, the part shifts. When the whole puzzle is won, every part returns to its original position and the radio display pulses.

Safe changes: release offsets, release rotations, restoration duration, pulse size, and glow color.

### `RadioLevelBootstrap.cs`

Builds this temporary prototype level from simple shapes at runtime. It creates the camera, radio, screws, tray, UI, dependencies, and restoration-part assignments.

This is assembly code, not a gameplay-rule owner. It exists so V0.1 is immediately playable without permanent art or prefab work. When real art arrives, replace this builder with scene objects and prefabs while retaining the five gameplay systems above.

## V0.1 Level Data

The radio has nine screws:

| Set | Count | Role |
|---|---:|---|
| Red | 3 | Faceplate |
| Blue | 3 | Speaker grille |
| Yellow | 3 | Radio case |

Five screws begin selectable. That is intentional: the player can make a poor mixed-color sequence and fill the five-slot tray. The simplest winning route is red, blue, yellow.

## Deliberately Absent

There is no save file, level map, currency, advertising, purchase system, collection, booster, analytics, achievement, or cloud integration in this architecture. Adding any of those before the core loop is validated would make this learning build harder to reason about without improving V0.1.

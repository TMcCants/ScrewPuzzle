# ScrewPuzzle

The locked V0.5 art direction is documented in
[`docs/VISUAL_DIRECTION.md`](docs/VISUAL_DIRECTION.md).

A small, expandable Unity puzzle game built as both a commercial learning project and a complete game-shipping exercise.

## Current Product Direction

The game begins with a simple screw/sort puzzle core, uses restoration as the immediate payoff, and deliberately postpones the larger collection/meta layer until later iterations.

**Core progression philosophy:**

> Puzzle first → restoration payoff second → collection/meta layer later.

## Core Level Sequence

1. The player sees a broken object.
2. The player solves the screw/sort puzzle by removing legal screws, managing limited tray space, and clearing matching sets.
3. Panels or components release as progress is made so the object visibly responds to the player's actions.
4. The puzzle reaches completion when all required screws/components are cleared.
5. The object restores or reassembles.
6. The restored object briefly comes to life for a satisfying completion moment.

Examples of completion beats:

- Vintage radio lights up and plays a short sound/static cue.
- Toy robot wakes up.
- Old camera flashes.
- Small CRT television flickers on.
- Toy car rolls forward.

## Starter Object Family

The first object set is intentionally compact and mechanically readable:

- Vintage radio — planned first object / Level 1
- Toy robot
- Old camera
- Small CRT television
- Toy car

## Development Philosophy

This project is intended to teach game development by ownership rather than by requiring every line of code to be written manually.

The working rhythm is:

**Vale builds → Vale explains → Tamika documents → Tamika modifies → review together.**

The goal is for Tamika to understand the codebase well enough to explain its systems, safely modify them, and eventually extend the game independently.

## V0.1 Technical Foundation

The first playable vertical slice is now implemented for Unity 6. It includes:

- one portrait radio level
- nine colored screws
- per-screw blocking dependencies
- a five-slot tray
- match-three clearing
- a full-tray loss state
- restart controls
- staged radio-part release
- a final restoration and radio pulse effect

The level uses simple generated shapes so gameplay can be tested before permanent art exists.

## V0.2 Multi-Level Foundation — Complete

The radio and toy-car levels now share the same prototype builder and gameplay systems. A reusable
`LevelDefinition` owns tray capacity, match size, screw colors, screw positions, blocker indexes,
and object-specific completion messages. Each object keeps its own visual construction and final
restoration effect. A level-selection screen and one-value local save unlock the toy car after the
radio is completed.

## V0.5 Production Visual Pass — Complete

V0.5 replaced the prototype presentation with the locked warm-workshop art direction. It includes
production radio and toy-car artwork, reusable hardware and interface styling, level-select cards,
the Lunyx app icon and loading screen, gameplay visual feedback, audio controls, and Android-safe
portrait presentation. All Android test scenarios passed on September 21, 2026.

## V0.6 Toy Robot Level — Complete

The production toy robot is integrated as Level 3 with twelve screws, four repair stages, two
dependency tiers, persistent local progression, and a three-card level-select flow. The complete
Radio → Toy Car → Toy Robot journey and both win/loss paths have passed Unity regression testing.
Android version `0.6.0` (version code `3`) passed all functional and visual scenarios on a physical device on September 22, 2026. See
[`docs/V06_ROADMAP.md`](docs/V06_ROADMAP.md) for the locked scope and acceptance criteria.

## V0.7 Navigation and First-Time Experience

Phase 1 adds shared in-level navigation across all three restorations: Resume, Restart, confirmed
return to Level Select, result-screen escape routes, and Android Back-button handling. Version
`0.7.0` (Android version code `4`) is ready for Unity and physical-device validation. First-play
guidance is now implemented for genuinely new Level 1 players and awaits Unity and Android validation. See
[`docs/V07_ROADMAP.md`](docs/V07_ROADMAP.md) for the locked behavior map and acceptance criteria.

## Open and Play

1. Add this repository folder as a project in Unity Hub.
2. Open it with Unity `6000.3.23f1` or a compatible Unity 6 editor.
3. Open `Assets/Scenes/Loading.unity` to test the complete startup flow, or open
   `Assets/Scenes/LevelSelect.unity` to skip directly to level selection.
4. Set the Game view to a portrait ratio such as `9:16`.
5. Press Play.

Level 1 begins unlocked. Completing the radio unlocks Toy Car; completing Toy Car unlocks Toy
Robot. Both unlocks save on the local device. Completing Radio opens Toy Car directly, completing
Toy Car opens Toy Robot directly, and completing Toy Robot returns to Level Select.

Radio and Toy Car use the safe red → blue → yellow route. Toy Robot uses first red → blue or yellow
→ the remaining middle color → final red. Mixed-color choices can fill the tray and demonstrate the
loss state.

Read [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) before changing gameplay rules and [`docs/UNITY_SETUP.md`](docs/UNITY_SETUP.md) before replacing the generated prototype visuals.

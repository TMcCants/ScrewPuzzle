# ScrewPuzzle

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

## Open and Play

1. Add this repository folder as a project in Unity Hub.
2. Open it with Unity `6000.3.23f1` or a compatible Unity 6 editor.
3. Open `Assets/Scenes/Level01_Radio.unity`.
4. Set the Game view to a portrait ratio such as `9:16`.
5. Press Play.

For the intended safe solution, clear the three red screws, then the three blue screws, then the three yellow screws. Some mixed-color choices will fill the tray and demonstrate the loss state.

Read [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) before changing gameplay rules and [`docs/UNITY_SETUP.md`](docs/UNITY_SETUP.md) before replacing the generated prototype visuals.

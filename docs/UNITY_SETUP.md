# Unity Setup and Modification Guide

## First Launch

1. Install Unity `6000.3.23f1` through Unity Hub. A later compatible Unity 6 patch should also work; let Unity make a local backup before upgrading.
2. In Unity Hub, choose **Add** and select the repository folder.
3. Let Unity import packages and generate its local `Library` folder.
4. Open `Assets/Scenes/LevelSelect.unity`.
5. Choose a portrait Game view such as `9:16`.
6. Press Play.

## Opening the Toy-Car Level

1. Stop Play Mode.
2. In the Project window, open `Assets/Scenes/Level02_ToyCar.unity`.
3. Press Play.
4. Use red, blue, then yellow for the intended winning route.

Opening the toy-car scene directly remains useful for isolated testing, but normal play now begins
from the level-selection scene.

Each saved scene contains one bootstrap object. All visible prototype objects are generated when
Play begins.

## Testing the Level Flow

1. Open `Assets/Scenes/LevelSelect.unity` and press Play.
2. Confirm that Level 1 is available.
3. If this is the first run, confirm that Level 2 is locked.
4. Complete the radio level and click **Next Level**.
5. Complete the toy-car level and click **Level Select**.
6. Confirm that the toy-car button is now unlocked.
7. Stop and start Play Mode again; confirm that Level 2 remains unlocked.

## Controls

- Click a bright screw with a mouse or tap it on a touch device.
- Dim screws are blocked.
- Clicking a blocked screw shows a short explanation.
- Three screws of the same color clear from the tray.
- Five unmatched screws fill the tray and lose the level.
- **Restart** or **Play Again** reloads the single scene.

## Quick Verification

### Win path

1. Select all three red screws.
2. Select all three blue screws.
3. Select all three yellow screws.
4. Confirm that each set clears and the radio parts loosen.
5. On the final match, confirm that the radio reassembles, its display flickers three times,
   the radio performs one scale pop, and the restored overlay appears after a short hold.

### Loss path

Select open screws in this order by color: red, blue, yellow, red, blue. Confirm that the full-tray overlay appears and selection stops.

### Blocking path

At the start, click any dim screw. Confirm that it shakes briefly, stays attached to the radio,
and the status text says it is blocked. Click it rapidly several times and confirm that the screw
always returns to its original position.

## Tamika's First Controlled Change

Start with a configuration change, not a rewrite.

Recommended exercise: open `PrototypeLevelBuilder.cs`, find `ColorFor`, change the blue screw
color slightly, save, and return to Unity. After compilation, play a level and confirm the change.

Then try one rule change: in a level's `LevelDefinition`, change its match size from `3` to `2`.
This intentionally makes the current nine-screw level impossible to finish cleanly because nine is
not divisible by two. That is useful evidence that match size and level color counts must be
designed together. Change it back to `3` afterward.

## Replacing Prototype Art Later

The runtime-generated shapes are not the long-term content pipeline. When the core loop feels correct:

1. Create radio and screw prefabs from final sprites.
2. Place the radio, tray slots, UI, and systems in the scene.
3. Assign references in the Inspector instead of through `RadioLevelBootstrap`.
4. Remove the bootstrap component only after the hand-authored scene completes both win and loss tests.

The core scripts were designed to survive that change.

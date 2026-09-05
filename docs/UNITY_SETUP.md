# Unity Setup and Modification Guide

## First Launch

1. Install Unity `6000.3.23f1` through Unity Hub. A later compatible Unity 6 patch should also work; let Unity make a local backup before upgrading.
2. In Unity Hub, choose **Add** and select the repository folder.
3. Let Unity import packages and generate its local `Library` folder.
4. Open `Assets/Scenes/Level01_Radio.unity`.
5. Choose a portrait Game view such as `9:16`.
6. Press Play.

The saved scene contains one bootstrap object. All visible prototype objects are generated when Play begins.

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
4. Confirm that each set clears, radio parts loosen, the radio reassembles, its display pulses, and the restored overlay appears.

### Loss path

Select open screws in this order by color: red, blue, yellow, red, blue. Confirm that the full-tray overlay appears and selection stops.

### Blocking path

At the start, click any dim screw. Confirm that it does not move and the status text says it is blocked.

## Tamika's First Controlled Change

Start with a configuration change, not a rewrite.

Recommended exercise: open `RadioLevelBootstrap.cs`, find `ColorFor`, change the blue screw color slightly, save, and return to Unity. After compilation, play the scene and confirm the change.

Then try one rule change: in `BuildLevel`, change the `3` passed to `trayManager.Configure` to `2`. This intentionally makes the current nine-screw level impossible to finish cleanly because nine is not divisible by two. That is useful evidence that match size and level color counts must be designed together. Change it back to `3` afterward.

## Replacing Prototype Art Later

The runtime-generated shapes are not the long-term content pipeline. When the core loop feels correct:

1. Create radio and screw prefabs from final sprites.
2. Place the radio, tray slots, UI, and systems in the scene.
3. Assign references in the Inspector instead of through `RadioLevelBootstrap`.
4. Remove the bootstrap component only after the hand-authored scene completes both win and loss tests.

The core scripts were designed to survive that change.

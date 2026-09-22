# ScrewPuzzle V0.7 Roadmap — Navigation and First-Time Experience

Status: **Phase 1 validated; Phase 2 first-play guidance implemented and awaiting Unity and Android validation**

## Goal

Make every puzzle easy to leave, restart, or resume without weakening the completed V0.6 gameplay,
art, sound, restoration, or progression systems.

## Phase 1 — Navigation

- Add a shared `MENU` button to Radio, Toy Car, and Toy Robot.
- Provide `RESUME`, `RESTART`, and `LEVEL SELECT` actions.
- Warn the player that unfinished puzzle progress will be lost before returning to Level Select.
- Add a direct `LEVEL SELECT` option to Game Over and eligible victory screens.
- Avoid duplicating the Level Select action after the final Toy Robot victory.
- Map the Android Back button to close confirmation, close the menu, open the menu, or return from a
  finished result screen according to the current UI state.
- Block screw input while the navigation menu is open.

## Locked Behavior Map

| Current state | Player action | Expected result |
|---|---|---|
| Active puzzle | Tap MENU | Navigation overlay opens and puzzle input stops |
| Navigation menu | Tap RESUME | Overlay closes and the same puzzle continues |
| Navigation menu | Tap RESTART | Current level reloads |
| Navigation menu | Tap LEVEL SELECT | Leave confirmation opens |
| Leave confirmation | Tap CANCEL | Confirmation closes and menu remains open |
| Leave confirmation | Tap LEAVE | Level Select opens; saved unlocks remain unchanged |
| Active puzzle | Android Back | Navigation overlay opens |
| Navigation menu | Android Back | Navigation overlay closes |
| Leave confirmation | Android Back | Confirmation closes and menu remains open |
| Game Over | Tap LEVEL SELECT | Level Select opens |
| Radio or Toy Car victory | Tap LEVEL SELECT | Level Select opens instead of continuing |
| Toy Robot victory | View actions | Only the existing Level Select action appears |

## Phase 2 — First-Play Guidance

Implemented after Phase 1 passed Unity and physical-device testing on September 22, 2026:

- First radio interaction explains that bright screws are available.
- The first successful selection advances the prompt to match-three and the five-slot limit.
- A blocked tap temporarily explains that dim screws must be unlocked.
- The first cleared match completes and locally saves the tutorial.
- A SKIP action immediately completes and locally saves the tutorial.
- Returning players and Levels 2–3 do not receive tutorial prompts.

## Protected V0.6 Behavior

- All puzzle layouts, dependencies, and solutions.
- Restoration animations and audio.
- Win and Game Over detection.
- Persistent unlock and sound preferences.
- Loading, level-select, and Android-safe portrait presentation.
- The preserved `release/v0.6.0` branch.

## Phase 1 Acceptance Criteria

Phase 1 passes only when every behavior in the locked behavior map works in all three levels, no
button or message clips on the phone, saved unlocks remain correct, and the Unity Console contains
no red errors.

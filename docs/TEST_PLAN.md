# V0.1 Test Plan

## Automated Edit Mode Tests

Open **Window → General → Test Runner**, select **EditMode**, and run all tests.

`TrayRulesTests` verifies:

- matching colors are found even when separated in the tray
- no match is returned for a full mixed tray
- the configured match size is honored

## Manual Play Tests

| Test | Expected result |
|---|---|
| Tap a bright screw | It moves to the next tray slot |
| Tap a dim screw | It shakes, remains on the radio, and a blocked message appears |
| Place three same-color screws | The three shrink away and remaining screws compact |
| Remove a part's final holding screw | That radio part visibly loosens |
| Fill all five slots without a match | Input stops and the loss overlay appears |
| Clear all nine screws | Radio parts return, the power-on effect plays, and the win overlay appears |
| Press Restart during play | Level reloads cleanly |
| Press Play Again after win/loss | Level reloads cleanly |

## V0.4 Interaction Audio

Run the following checks in both the radio and toy-car levels with the device volume audible:

| Test | Expected result |
|---|---|
| Tap a bright screw | A short mechanical selection tick plays once |
| Watch a screw reach the tray | It gives one small settle pulse without changing slots |
| Tap a dim screw | A distinct low blocked cue plays with the shake |
| Clear three same-color screws | They briefly brighten and pop while the match cue plays |
| Fill the tray without a match | The tray gives one damped shake before the Game Over overlay |
| Clear the final matching set | A warm four-note victory cue begins with restoration |
| Tap a blocked screw rapidly | Each accepted tap responds without changing the screw position |
| Replay either level | Sounds remain restrained and do not delay input or animation |
| Tap SOUND: ON | Label changes to SOUND: OFF and all feedback becomes silent |
| Change levels while muted | SOUND: OFF remains visible and the next level stays silent |
| Stop and restart Play Mode while muted | SOUND: OFF remains saved and gameplay stays silent |
| Tap SOUND: OFF | Label changes to SOUND: ON and one confirmation tick plays |

This slice passes only when all five feedback states are distinguishable by sound, the saved
sound preference behaves correctly, both levels retain their original gameplay behavior, and the
Unity Console contains no audio-listener warnings or red errors.

## Exit Requirement

V0.1 passes only when both the win path and the intentional loss path work from a clean scene reload and the Unity Console contains no red errors.

## V0.2 Refactor Regression Check

The radio level is the safety net while reusable level data is introduced. After any level-system
refactor, rerun the complete win path and intentional loss path. The screen layout, blockers,
tray capacity, match size, restoration sequence, and result messages must behave exactly as they
did before the refactor.

## V0.2 Toy-Car Level

Open `Assets/Scenes/Level02_ToyCar.unity` and run these checks:

| Test | Expected result |
|---|---|
| Start the scene | `TOY CAR` appears and all nine screws are visible |
| Tap a dim screw | It shakes, stays attached, and shows the blocked message |
| Select red, blue, yellow, red, blue | The five-slot tray fills and Game Over appears |
| Select red set, blue set, then yellow set | All three sets clear and the car parts loosen |
| Finish the winning route | The car reassembles, headlights flash, the car moves, and the restored overlay appears |
| Press Play Again | The toy-car scene reloads cleanly |

The toy-car level passes only when both outcome paths work and the Console contains no red errors.

## V0.2 Level Selection and Local Unlock

Start from `Assets/Scenes/LevelSelect.unity`.

| Test | Expected result |
|---|---|
| First run | Radio is available and Toy Car is locked |
| Lose the radio level | Play Again reloads the radio; Toy Car remains locked |
| Win the radio level | Next Level opens the toy-car scene and saves the unlock |
| Lose the toy-car level | Play Again reloads the toy car |
| Win the toy-car level | Level Select returns to the selection screen |
| Return to level selection | Both Radio and Toy Car are available |
| Stop and restart Play Mode | Toy Car remains unlocked |

The flow passes only when scene navigation, button wording, and the persisted unlock all behave
correctly with no Console errors.

## V0.4 Android Device Validation

Build and install version `0.4.0` with Android version code `2`, then verify on a physical phone:

- both levels accept comfortable touch input
- all five feedback sounds remain pleasant through the phone speaker
- SOUND ON/OFF persists after fully closing and reopening the app
- tray-arrival, match-clear, and full-tray visual effects remain readable at phone scale
- radio-to-toy-car unlocking remains saved
- no content is clipped by the camera cutout or bottom navigation area

## V0.5 Workshop Background

Run both gameplay scenes and confirm:

- the warm workshop background fills the portrait camera with no exposed edges
- the radio, toy car, screws, tray, instructions, and controls remain easy to read
- no sharp background prop appears behind an interactive screw
- the background remains stationary through match, loss, restoration, and scene transitions
- the Unity Console contains no missing-resource, texture, or rendering errors

## V0.5 Gameplay Hardware

Run both gameplay scenes and confirm:

- every screw uses the new dimensional metal head while remaining clearly red, blue, or yellow
- blocked screws are visibly dim but their slot and silhouette stay readable
- the tray shows exactly five evenly spaced recessed slots
- each arriving screw settles in the center of its matching tray recess
- screw tap areas still feel comfortable and no nearby screw is selected accidentally
- arrival pulse, blocked shake, match pop, and full-tray shake still play without visual clipping
- match clearing, tray compaction, win, and loss behavior are unchanged
- the Unity Console contains no missing-resource, texture, alpha, or rendering errors

## V0.5 Buttons and Typography

Run Level Select and both gameplay scenes, then confirm:

- the title, headings, instructions, status, and controls form a clear hierarchy at phone scale
- all button labels remain centered and comfortably inside the brass frame
- level-select buttons, Restart, Continue, and Play Again respond visually when pressed
- the locked Level 2 button is visibly disabled without becoming unreadable
- the sound control remains readable but does not compete with the puzzle object
- long result messages wrap cleanly without clipping
- all touch targets remain comfortable near device cutouts and navigation areas
- the Unity Console contains no missing-font, missing-resource, or sliced-sprite errors

## V0.5 Vintage Radio Artwork

Run `Level01_Radio` and confirm:

- the walnut cabinet, brass faceplate, speaker grille, display, and two knobs align cleanly
- all nine gameplay screws retain their exact positions and remain obvious at phone scale
- clearing the three red screws loosens the faceplate together with its display and knobs
- clearing the three blue screws loosens only the speaker grille
- clearing the three yellow screws loosens only the outer cabinet layer
- the final restoration returns every layer to its original alignment
- the display indicator flickers amber and the radio performs its final scale pop
- win, loss, Restart, sound, and progression behavior remain unchanged
- the Unity Console contains no missing-resource, texture, alpha, or rendering errors

## V0.5 Toy Car Artwork

Run `Level02_ToyCar` and confirm:

- the roof pillars, hood, and headlights sit against the body with no unintended floating gaps
- the teal body, cream roof and windows, brass details, and rubber wheels align cleanly
- all nine gameplay screws remain visible, readable, and comfortably tappable at phone scale
- clearing the three red screws loosens the complete roof and window assembly
- clearing the three blue screws loosens only the hood
- clearing the three yellow screws loosens both wheels together
- the final restoration returns every layer to its original alignment
- both headlights flash three times and the car performs its short forward movement
- win, loss, Restart, sound, and progression behavior remain unchanged
- the Unity Console contains no missing-resource, texture, alpha, or rendering errors

## V0.5 Level Select Artwork

Run `LevelSelect` and confirm:

- the title, subtitle, two walnut-and-brass cards, progress message, and save note fit the portrait screen
- the radio card uses the production radio artwork and opens `Level01_Radio`
- an unlocked toy-car card uses the production car artwork and opens `Level02_ToyCar`
- both cards give visible pressed/highlighted feedback without washing out their labels
- when Level 2 is locked, its card, thumbnail, status, and `LOCKED` label are visibly disabled
- the locked toy-car card cannot be opened and the progress message explains how to unlock it
- completing the radio unlocks the toy-car card and the unlocked state persists after restarting Play Mode
- no card, label, or thumbnail is clipped by a device cutout or bottom navigation area
- the Unity Console contains no missing-resource, texture, font, or rendering errors

## V0.5 App Icon and Loading Screen

Open `Loading` and confirm:

- the approved brass-screw icon is sharp, centered, and fully visible inside the portrait screen
- `SCREWPUZZLE`, the Lunyx studio credit, and `RESTORING...` remain readable at phone scale
- the icon breathes subtly and the red, blue, and yellow loading dots pulse in sequence
- the loading screen remains visible briefly, then opens Level Select without a flash or pause
- opening Level Select directly still works for quick gameplay testing
- Android Player settings show the approved art for adaptive, round, and legacy icon sizes
- an installed Android build uses the approved icon in the launcher and recent-apps view
- the Unity Console contains no missing-scene, missing-resource, font, texture, or loading errors

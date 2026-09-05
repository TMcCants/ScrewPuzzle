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
| Tap a dim screw | It remains on the radio and a blocked message appears |
| Place three same-color screws | The three shrink away and remaining screws compact |
| Remove a part's final holding screw | That radio part visibly loosens |
| Fill all five slots without a match | Input stops and the loss overlay appears |
| Clear all nine screws | Radio parts return, display pulses, win overlay appears |
| Press Restart during play | Level reloads cleanly |
| Press Play Again after win/loss | Level reloads cleanly |

## Exit Requirement

V0.1 passes only when both the win path and the intentional loss path work from a clean scene reload and the Unity Console contains no red errors.

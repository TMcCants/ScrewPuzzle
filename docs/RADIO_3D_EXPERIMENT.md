# 3D Radio color-tray experiment

Separate from the existing 2D levels and saved progression.

## Rules

Rotate the radio to reach twelve screws: six red, three blue, and three yellow.
Four trays contain three holes each. Red and blue trays start open; two bonus trays
start locked. Each open tray accepts only its assigned color. A complete set of three
clears and the tray receives the next pending color (yellow, then red).
A screw without an available matching tray stays on the radio and displays guidance.
The board is fully solvable using only the two free trays. There is no overflow loss
in this version. Clearing all twelve screws wins.

Bonus trays use clearly marked TEST UNLOCK buttons. These consume the next pending
color order early; they do not add duplicate orders. No ads, billing, or saved purchases
are connected. Restart relocks both bonus trays and restores all screws and rotation.
Actual rewarded-ad and purchase integration remains future work.

## Open and test

PC: Tools → ScrewPuzzle → Open 3D Radio Test, then Play. Try portrait and Free Aspect.
Android: switch Build Profiles to Android, then Tools → ScrewPuzzle → Build 3D Radio
Android Test. This builds a separate .radio3dtest application while preserving normal
build settings. A new APK must be built to test these changes on the phone.

- Confirm four trays, three holes each, two initially open and two locked.
- Tap yellow initially: it stays on the radio and guidance appears.
- Collect red from front/back/left: the red tray clears and becomes yellow.
- Collect yellow from front/back/left, then blue, then the three right-side red screws:
  the board completes without opening a bonus tray.
- Restart and open both test trays: yellow and a second red order become available.
  Complete all four orders; every screw should be accounted for exactly once.
- Restart during screw flight and after completion: all screws return and bonus trays relock.
- Drag away and back starting on a screw: rotation must not become a tap.
- Hidden screws remain blocked by the cabinet. Canceled/multitouch gestures do not remove screws.
- Check that tray controls do not select screws behind them and the radio remains clear of the UI.

## Implementation

Radio3DPrototypeBootstrap builds the cabinet and responsive portrait UI fitted within the
screen safe area. Trays are UI cards with circular holes, replacing oversized world-space
blocks. Radio3DPuzzle owns assigned-color orders, flight, clearing, unlock simulation,
and restart. Radio3DInteraction owns gesture handling and nearest-hit raycasts.
Radio3DScrew caches its original transform and collider state.

The previous five-slot version passed user PC/phone testing. The new four-tray version
needs a fresh visual and device check. Automated checks cover both free-only and unlocked
completion, unavailable-color rejection, color replacement, occlusion, gesture intent,
and restart during flight.

September 30, 2026: all 16 Unity EditMode tests passed in the isolated validation copy after the color-tray rewrite. Manual PC/phone verification of this new layout remains pending.
Final spacing follow-up: the interaction regression and preview capture both passed. Portrait (540 x 960) and wide (1000 x 700) renders were inspected.

![Color trays](art/Radio3D_Color_Trays_Preview.png)


User acceptance: the user confirmed that the four-tray layout looks good and all test scenarios passed. Approved saving this milestone before detachable-plate work.

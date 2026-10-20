# Lunyx home and collections

The refined, user-supplied Lunyx logo opens the app on a black splash. It remains visible for four seconds including fades and cannot be skipped. The first loading frame is excluded, large frame delays are capped, and timing pauses when the app loses focus. It appears once per app session.

Startup flow: Lunyx → Home → Collections → Toy Shop → puzzle.

Home provides Play, Continue, and Settings. Player Name is inside Settings; Save and Cancel return there. Continue opens the last unfinished puzzle; if that run has been completed or cleared, it falls back to another unfinished toy. Nicknames are optional, limited to 20 characters in the input, and saved on this device. This is not an online account or cloud save.

Settings expose the existing independent music and effects preferences plus How to Play. The Toy Shop footer returns to Collections. Existing puzzle Boards buttons still return to the Toy Shop. Android Back returns from subpages and does nothing during the splash.

The normal workshop build and shared scene list now start at Workshop_Home. Direct single-board test menu commands retain their requested starting board. All seven scenes remain included.

No store, coin purchases, or real rewarded ads are added in this update. Existing bonus-tray preview behavior remains in place.

## Validation

Unity 6000.3.23f1: five automated tests passed (WorkshopHomeTests and ThreeDBoardNavigationTests). Covered splash timeout and input blocking, nickname saving, music toggle, guide navigation, Home–Collections–Toy Shop return flow, continuation preference/fallback, scene inclusion, and existing board switch/resume behavior.

## Phone check

1. In Unity choose Tools → ScrewPuzzle → Build and Run 3D Workshop on Phone.
2. Confirm the Lunyx splash, then Home. Fully close and relaunch the app to check the four-second logo display. Tap and press Back during it; neither should skip it. Returning Home within the same session should not replay it.
3. Open Settings → Player Name and save a nickname; close/reopen the app and confirm it is remembered. Check blank and Cancel behavior.
4. Toggle music and effects independently. Open How to Play and return Home.
5. Play → Toy Shop → any toy. Remove a screw, return through Boards → Back to Collections → Back to Home, then Continue. Confirm the unfinished puzzle resumes.
6. Check portrait/landscape and phone safe areas. Confirm all five toys remain accessible.

Changes are local until separately committed and pushed.

Rendered previews: docs/art/Lunyx_Home_Phone.png, Lunyx_Collections_Phone.png, Lunyx_Settings_Phone.png, and Lunyx_Home_Wide.png. Isolated Unity preview capture passed; portrait and landscape home layouts were visually inspected.

Revision validation: all three WorkshopHomeTests passed with the fixed non-skippable splash and Player Name inside Settings. Unity preview capture also passed; updated Settings layout visually inspected. Phone confirmation remains required.

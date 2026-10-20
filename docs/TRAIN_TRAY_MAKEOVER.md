# Train sorting boxes and screw availability

This design has now been extended to every toy; see [All toys rotation and trays](ALL_TOYS_ROTATION_AND_TRAYS.md). The notes below record the original train-first pass.

Train-only presentation pass:

- Cream rounded sorting boxes with colored rims, recessed sockets, collected plus heads and compact color/count labels.
- Bonus trays have closed cream lids, a padlock and Unlock button. Lids lift/fade away only after a successful unlock. Restart closes them; resume preserves an unlocked tray without replaying the lift.
- Unlock opens a clearly labeled preview dialog. Cancel does nothing. Preview Open calls the existing free test unlock; no ad is watched, no credits are spent, and no payment integration exists. When the queue is empty, unopened bonus trays say Not needed and cannot be opened.
- Available screws keep their bright original color/material, even if no current tray matches. Assembly-blocked screws are muted with their color still identifiable.
- A high-contrast padlock badge appears only for a visible, front-facing blocked screw. Markers have no colliders and do not intercept UI input; occluded screws are not revealed through the toy.
- Tapping a visible blocked screw names the blocking assembly (or asks to wait for its release animation). Tapping an available screw without a matching tray explains the tray mismatch. Neither action removes a screw or spends anything.
- Hints still filter to accessible screws. Ordinary picking retains its accessible-only API; train taps use a separate nearest-visible-screw query for explanations.

The airplane and other boards retain their existing tray appearance. Puzzle colors, randomized layouts, save identities, two-free-tray rules and touch target sizes are unchanged. The design uses procedural UI sprites, not newly generated artwork.

## Phone acceptance

1. Check rounded trays, sockets, counts and lid labels against the station background in portrait and landscape.
2. Tilt to the cab roof early: its blocked screws should be muted with padlock badges. Tap one and confirm the explanation names the drive assembly.
3. Tap an available color without a matching tray: it stays bright and explains the tray mismatch.
4. Cancel a bonus-tray unlock; nothing should change. Confirm Preview Open and check the lid lift. No real payment or ad occurs.
5. Resume with an opened bonus tray, then restart: the lid should respectively remain open and return closed.
6. Complete the train using only the two free trays; screws should brighten when their blocker finishes coming away.

## Validation

Unity EditMode play-session checks passed for the train full solve/restart, train resume, and hints across all five toys. The new presentation test passed for blocked-screw explanations, unmatched colors, cancel/open, lid animation, resume/restart, becoming available, and exhausted tray queues (Logs/train-lids-verified-results.xml). Portrait and landscape previews were inspected. git diff --check passed. Physical phone testing remains pending.

Previews: [Train trays](art/Train_Tray_Makeover_Phone.png), [Locked screws](art/Train_Locked_Screws.png).

## Open station background

Removed the train's large lower gray backing. The station now shows around the individually shadowed trays and bottom buttons. A compact translucent rounded backing keeps progress and feedback readable. Unity preview capture passed (Logs/train-backing-results.xml); portrait and landscape renders were visually inspected. Phone review remains pending.

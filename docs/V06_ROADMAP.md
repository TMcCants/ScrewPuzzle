# ScrewPuzzle V0.6 Roadmap — Toy Robot Level

Status: **Complete — Unity regression and physical Android-device validation passed on September 22, 2026**

## Goal

Add the toy robot as Level 3 and use it to prove that ScrewPuzzle's reusable content pipeline can
support another polished object without rewriting the working puzzle rules or destabilizing the
completed radio and toy-car levels.

## Product Principle

> Extend the content, prove the architecture, and preserve what already works.

V0.6 is a focused content milestone. The toy robot should feel new because of its object,
restoration sequence, and completion personality—not because the game suddenly introduces an
unrelated rule set.

## Included Scope

- A third playable toy-robot level.
- A robot-specific level definition using the existing screw colors, blockers, tray, and
  match-three rules.
- Production toy-robot artwork in the locked warm-workshop material language.
- Separately releasable robot components for visible restoration progress.
- A robot-specific final restoration animation and completion sound.
- A third level-select card with clear locked and unlocked states.
- Progression that unlocks the toy robot after the toy car is completed.
- Android validation covering gameplay, visuals, sound, progression, restart, win, and loss.

## Locked Puzzle Structure

Approved on September 21, 2026; arm distribution revised with Tamika's approval:

- Twelve screws, producing four match-three clears. The count remains locked for V0.6 even though the final artwork has room for expansion.
- Five tray slots and the existing match size of three.
- The existing red, blue, and yellow screw palette.
- Color distribution of six red, three blue, and three yellow screws.
- Two layers of blocker dependencies so the additional screws add planning rather than only length.
- No fourth screw color in V0.6.
- Four readable repair stages:
  1. Head housing
  2. Chest access plate
  3. Left arm assembly
  4. Right forearm casing
- The yellow screw group is placed on the left arm and the final red group on the right arm so both
  sides participate visibly in the repair.
- Arm assemblies may shift or tilt when released but remain in frame and return during restoration.
- The right forearm casing restores before the robot uses that arm for its awkward completion wave.

## Locked Dependency Map

Approved on September 21, 2026:

| Repair stage | Screw group | Access rule |
|---|---|---|
| Head housing | First three red screws | Initially exposed |
| Chest access plate | Three blue screws | One exposed; two blocked by head screws |
| Left arm assembly | Three yellow screws | One exposed; two blocked by head screws |
| Right forearm casing | Final three red screws | Each blocked by a paired blue and yellow screw |

The intended safe solution is:

> First red set → blue or yellow → remaining middle color → final red set

This creates a real middle choice while retaining a fair solution. Exposed blue and yellow screws
provide visible risk if the player mixes colors carelessly. The final red screws unlock gradually
through blue-and-yellow blocker pairs, creating two dependency tiers without introducing a new
mechanic.

## Locked Level-Select Direction

Approved on September 21, 2026:

- Level cards use the walnut-and-brass board without object thumbnails.
- Each card shows its level number above a large centered restoration name.
- Unlocked cards omit redundant status text; locked cards add a clear `LOCKED` line and use muted typography.
- The text-first system applies to every level so the selector remains readable and scalable as
  the game grows.

## Protected Existing Behavior

V0.6 must preserve:

- Radio and toy-car puzzle layouts and solutions.
- Blocked-screw dimming, tap message, shake, and sound.
- Screw tray-entry movement and settle pulse.
- Three-screw match glow, clearing, and tray compaction.
- Five-slot full-tray loss detection and Game Over reaction.
- Restoration animations and completion flow.
- Persistent sound on/off behavior.
- Portrait safe-area handling and live Unity text.
- The locked warm-workshop visual direction.

## Deliberately Deferred

- New puzzle-rule types.
- Power-ups, boosters, hints, or consumable items.
- Monetization.
- Collection or room-decoration meta systems.
- Accounts, cloud saves, analytics, or online services.
- Additional levels beyond the toy robot.

These ideas may be evaluated later, but they do not belong in the V0.6 implementation.

## Locked Character Direction

Approved on September 21, 2026:

- Balanced humanoid proportions.
- A vintage workshop-helper identity.
- Painted metal body, brushed-brass joints, and restrained surface wear.
- A moderately sized, expressive head without childish proportions.
- Clear shoulders, torso, forearms, thighs, and sturdy feet.
- Friendly illuminated eyes without a cartoon face.
- Playful amusement expressed through movement rather than an exaggerated design.

The final activation beat is:

1. The eyes flicker on unevenly.
2. The head turns slightly as the robot gets its bearings.
3. One arm rises with a small mechanical stutter.
4. The robot gives the player a quick, awkward wave.
5. The chest light flashes with misplaced mechanical pride.

Avoid oversized eyes, a giant head, exaggerated skinny limbs, a permanent grin, or other choices
that would weaken the premium warm-workshop direction.

## Production Decisions Locked

- Final screw positions are integrated across the head, chest, left arm, and right arm.
- Completion sound and activation timing are integrated and passed Unity testing.

## Implementation Order

1. Completed — inspect reusable level and progression code for two-level assumptions.
2. Completed — define the twelve-screw layout and fair safe solution.
3. Completed — add the robot level definition, scene, bootstrap, and restoration controller.
4. Completed — add Level 3 progression and the third level-select card.
5. Completed — validate both successful and full-tray-loss scenarios with generated-shape visuals.
6. Completed — produce and integrate the final layered robot artwork.
7. Completed — validate artwork alignment, screw placement, cover releases, activation, successful play, and Game Over behavior.
8. Completed — run regression tests on Radio, Toy Car, and Toy Robot, including continuous progression.
9. Completed — built version `0.6.0` (Android version code `3`) and passed all functional and visual scenarios on a physical Android device.

## Acceptance Criteria

V0.6 is complete when:

- A new player can complete Radio, unlock and complete Toy Car, then unlock Toy Robot.
- Closing and reopening the app preserves the correct highest unlocked level.
- The toy robot supports both a successful solution and a legitimate full-tray loss.
- Its blockers, screw colors, tray contents, progress, and controls remain readable on a phone.
- The restoration sequence clearly responds to player progress and ends with a satisfying robot
  activation.
- Sound-off remains persistent and suppresses all robot-level effects.
- Restart works from both active play and Game Over.
- Radio and Toy Car pass their existing regression scenarios unchanged.
- The Unity Console is clean.
- The Android build passes all functional and visual scenarios.

# ScrewPuzzle V0.6 Roadmap — Toy Robot Level

Status: **Planned and approved on September 21, 2026**

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

Approved on September 21, 2026:

- Twelve screws, producing four match-three clears.
- Five tray slots and the existing match size of three.
- The existing red, blue, and yellow screw palette.
- Color distribution of six red, three blue, and three yellow screws.
- Two layers of blocker dependencies so the additional screws add planning rather than only length.
- No fourth screw color in V0.6.
- Four readable repair stages:
  1. Head housing
  2. Chest access plate
  3. Lower-torso service panel
  4. Right forearm casing
- Robot limbs remain structurally attached. Repair panels may shift, tilt, or loosen but must not
  make the robot appear dismembered.
- The right forearm casing restores before the robot uses that arm for its awkward completion wave.

## Locked Dependency Map

Approved on September 21, 2026:

| Repair stage | Screw group | Access rule |
|---|---|---|
| Head housing | First three red screws | Initially exposed |
| Chest access plate | Three blue screws | One exposed; two blocked by head screws |
| Lower-torso service panel | Three yellow screws | One exposed; two blocked by head screws |
| Right forearm casing | Final three red screws | Each blocked by a paired blue and yellow screw |

The intended safe solution is:

> First red set → blue or yellow → remaining middle color → final red set

This creates a real middle choice while retaining a fair solution. Exposed blue and yellow screws
provide visible risk if the player mixes colors carelessly. The final red screws unlock gradually
through blue-and-yellow blocker pairs, creating two dependency tiers without introducing a new
mechanic.

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

## Decisions to Lock Before Art Production

1. Exact screw positions on the final artwork.
2. Completion sound and exact animation timing.
3. Final robot preview composition for the level-select card.

## Implementation Order

1. Inspect the reusable level and progression code for assumptions limited to two levels.
2. Define the robot's puzzle layout and validate that it has a fair safe solution.
3. Add the robot level definition, scene, bootstrap, and restoration controller.
4. Add Level 3 progression and the third level-select card.
5. Test the complete level with temporary visuals.
6. Produce and integrate the final robot art layers.
7. Add the robot completion animation and sound.
8. Run regression tests on Radio, Toy Car, and Toy Robot.
9. Build and validate on Android.

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

Train follow-up: the train now uses the airplane's .2352-unit visible heads with original touch targets. See [Train upgrade](TRAIN_UPGRADE.md).

Airplane follow-up: its visible heads are now 20% smaller again, with full rotation and an airport backdrop. See [Airplane airport](AIRPLANE_AIRPORT.md). Other toys retain the sizes described below.

# Shared recessed plus-head screws

All five Toy Shop boards now use the airplane's approved screw design: visible
head diameter reduced from 0.42 to 0.294 world units, seated 0.04 units inward,
with a beveled rim and a real plus-shaped cavity. Sloped walls and a floor below
the face use the screw's color, rather than a painted dark cross.

The shared Radio3DPrototypeBootstrap screw builder creates the visual mesh once
per board and releases it with the board's other meshes. Airplane-only code was
removed so there is one source of truth for all toys.

Original Head and Slot colliders remain unchanged. Screw roots, positions, colors,
plate ownership, tray sequences and save signatures are unchanged. This preserves
existing tap areas, restart/resume behavior and puzzle solutions.

Phone acceptance: rebuild, inspect every toy and hidden layer, try off-center taps,
then rotate/restart/resume to confirm the smaller visual targets remain comfortable.

Validation: eight selected Unity checks passed, covering every board's complete hint-guided solve, shared visible-head dimensions and original colliders, airplane off-center tapping/restart, cavity depth and five toy thumbnail captures. A corrected radio thumbnail capture also passed. All updated thumbnails visually reviewed. Phone acceptance of the all-toy rollout remains pending.

# Toy Shop progression review

Recommended learning order: Radio (rotation and tray matching), Car (different
color positions around the object), Robot (practice following layered assemblies).
Cards now number that order and explain the focus on unfinished boards. All boards
remain freely selectable; completed badges and resume/replay behavior remain.

## Findings

| Board | Screws | Plates | Inner layers | Reachable logical states | Dead ends |
| --- | ---: | ---: | ---: | ---: | ---: |
| Radio | 18 | 6 | 2 | 5098 | 0 |
| Car | 15 | 5 | 1 | 4559 | 0 |
| Robot | 18 | 6 | 2 | 5098 | 0 |

A depth-first enumeration used the current screw colors, covering-plate rules,
three-hole tray refill sequence and two free trays. It explored every accepted
removal order, deduplicating by removed screws, tray contents and queue position.
All terminal states were wins. This is a logical audit, not a phone interaction
or player difficulty study; rotation effort, visibility and animations are excluded.

Radio and Robot are logically equivalent under renamed faces. Car changes color
positions but has fewer layers. Therefore the current set is a learning sequence,
not a verified easy/medium/hard progression. Preserve the recently accepted layouts.
Before adding harder boards, observe unaided completion time, rotations, hint use
and points of confusion on these three. Use that evidence to design a new challenge;
do not create difficulty by requiring bonus trays or purchased hints.

Validation: Unity batch compilation succeeded. Manual menu review and player difficulty observations remain pending.

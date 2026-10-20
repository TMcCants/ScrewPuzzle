# Car planning pass

The car keeps 15 screws and five plates. Colors by panel are now Near R/B/B,
Far B/Y/R, Rear Y/R/B, Nose R/R/B, Motor R/B/Y. Tray jobs are R/B/R/B/Y.
The motor carrier must be uncovered before all exterior screws can be cleared:
one red and one blue needed to finish those color jobs are inside. Yellow comes
last, so the player revisits exterior panels after uncovering the motor.

This is a modest planning increase, not a validated difficulty rating. The free
tray rules still prevent wrong-color moves. A logical enumeration of all accepted
removal orders reached 2,404 distinct states and zero non-winning terminal states
with two free trays. It excludes animation and physical screw picking, which are
covered separately by Unity tests.

Old unfinished car saves reset because the signature includes colors and tray
order. Completed badges remain. Radio and Robot layouts are unchanged.

Device acceptance: solve without hints or bonus trays; note whether returning to
outer panels feels more interesting; check motor access, restart and resume.

Validation: updated direct-picking car solve/restart test passed. Hint solving, three resume tests and thumbnail capture also passed. The original route test failed because it used the previous colors; its replacement route passed. Manual PC and phone acceptance pending.

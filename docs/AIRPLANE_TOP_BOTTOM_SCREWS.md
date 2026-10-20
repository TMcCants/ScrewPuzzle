# Airplane top and underside fasteners

Six existing screws moved: the three Cockpit screws sit on top-mounted seats around the canopy; the three Landing Gear screws attach a belly bracket underneath. There are still 27 screws, nine assemblies, and two free trays. Screw colors, random socket permutations, tray queue, identities and dependency graph are unchanged. The existing save signature and seed remain compatible, so an unfinished run resumes with the revised physical mounts.

Cockpit fasteners retain the existing unlock after the gearbox; landing gear retains its unlock after the cockpit. This preserves the validated two-tray puzzle. Release directions now follow the new faces: cockpit upward, landing gear downward.

Hints on the airplane now orient toward a screw's full 3D outward normal, rather than searching only horizontal angles. Other toys retain yaw-only hint behavior. Small visual heads and generous colliders are unchanged.

Validation includes the full tap-through solution, top and bottom hint raycasts, complete disassembly/restart, resume at the deeper stage, 1,000 random seeds, and exhaustive no-dead-end checks for the layout family.

Phone check: rotate above and below, reach the cockpit and landing-gear stages, tap the six relocated screws, try Hint from an awkward angle, and finish with the two free trays.

Results: initial Unity run 7 passed / 0 failed; final seat-geometry and all-board hint run 5 passed / 0 failed. Top and underside renders inspected; shelf thumbnail refreshed. git diff --check passed. No Android device test or GitHub push performed in this pass.

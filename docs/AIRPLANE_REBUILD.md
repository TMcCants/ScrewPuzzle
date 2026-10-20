Latest visual/control follow-up: [Airport backdrop, smaller heads, and full rotation](AIRPLANE_AIRPORT.md). The disassembly and randomized-layout behavior below is unchanged.

# Airplane rebuild and randomized layouts

The airplane now has nine screw-fastened assemblies (27 screws): near wing and side shell, far wing and side shell, tail, propeller/nose, spring carrier, gearbox, cockpit/upper shell, landing gear, and final chassis/wind-up key. Every visible toy part belongs to a removable assembly. Completion leaves no fixed toy body behind.

The visual pass adds tapered wings, roundels and seams, rounded fuselage shells, a glazed canopy, metal wheel hubs/axles, and toothed gears. It retains the shared small recessed plus heads and original larger invisible touch targets. Other toys and the Toy Shop layout are unchanged.

## Randomization

`PlaneScrewLayout` generates a deterministic member of a validated layout family. A new run chooses a positive seed, permutes the three color labels (including the matching tray queue), and shuffles the three physical sockets on each assembly. These are intentionally constrained random layouts, not arbitrary scattered screws. The cover graph and clearable color totals are preserved. The first six assemblies retain the existing planning puzzle; three final removable assemblies add nine screws.

Only two trays are needed. The generator contains a solution witness; tests exercise 1,000 seeds and exhaustively enumerate reachable logical states for one template. Socket permutations and color renaming preserve that graph. Runtime tests separately verify raycast reachability and complete disassembly.

The run save stores `layoutSeed` alongside the action history. Reopening an unfinished airplane reconstructs its colors and socket positions before replaying its moves. Restart retries the same layout. Leaving a completed board and selecting Replay creates a fresh randomized run. Old airplane saves have a different signature and safely reset; completed badges remain. Other boards use seed zero and their existing save behavior.

## Phone acceptance

Build with Tools > ScrewPuzzle > Build and Run 3D Workshop on Phone.

- Rotate the airplane and check the new shape, smaller plus heads, and easy tapping.
- Finish with only the two free trays; the cockpit, wheels and chassis must also come away.
- Leave midway through the inner layers and return: colors, placements, and progress should match.
- Restart midway through a release: the entire airplane should return with the same layout.
- Complete, return to the shop and Replay: check the new screw arrangement.

Automated validation results are recorded in the task response. Phone visual quality/performance still needs device confirmation. No ads or purchases were added.

## Automated results (2026-10-20)

- Layout audit passed: 1,000 deterministic seeds plus exhaustive reachable-state enumeration with no dead ends using two trays.
- Airplane resume test passed, including leaving during removal and restoring the saved seed/positions before completing.
- Regression run: 8 passed, 0 failed (airplane tap-through solve/restart, all-five-board hints, existing resume tests, captures).
- Final fuselage/inner-mount polish: 4 passed, 0 failed (full tap-through solve, mid-release restart, shelf and portrait/wide captures).
- Final renders: `art/Airplane_Rebuild.png` and `art/Airplane_Rebuild_Phone.png`. Shelf-Plane.png updated without changing its importer metadata.
- Changes remain local; no GitHub push or Android device test was performed in this pass.

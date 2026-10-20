Latest presentation pass: [Rounded trays, covered bonus boxes, and screw availability](TRAIN_TRAY_MAKEOVER.md).

# Train upgrade: station, full rotation, and complete disassembly

The train now matches the airplane's controls and puzzle structure: smaller .2352-unit recessed plus heads with the original .42-unit collider, full horizontal/vertical rotation, Reset View without progress loss, and hints that orient toward top and underside screws.

Nine removable assemblies / 27 screws:
- Engine shell and chimney
- Far shell
- Tender body and coal
- Nose
- Boiler carrier and copper motor
- Drive gears
- Cab, windows and roof (three roof screws)
- All wheels, axles and drive rods (three underside screws)
- Final chassis, tender frame and coupling

No rendered toy part is left on a permanent frame. Forest-green enamel, cream trim, brass hubs/spokes, copper motor and toothed gears distinguish the train from the airplane.

## Randomization and saving

The train deliberately shares the airplane's validated nine-assembly layout family (`PlaneScrewLayout`). Color-label permutations, matching tray queues, and shuffled sockets vary each fresh run while preserving solvability with two free trays. This is constrained randomization, not arbitrary screw scattering. The common generator is audited across 1,000 seeds and all reachable logical template states. Train runtime tests independently validate its actual geometry, taps and hints.

The train stores its own seed and action history under its existing board key. Resume reconstructs the same arrangement; Restart retries it. Completing and replaying from the shop creates a new run. Existing unfinished 18-screw train runs reset safely because the assembly signature changed; completion badges remain. Airplane behavior and saved layouts are unchanged.

## Station artwork

Saved project asset: Assets/Resources/Art/Workshop/Station-Background.png.
Generated with the built-in image_gen tool via the imagegen skill, using this prompt:

> Create a polished portrait 2:3 background illustration for a cozy mobile 3D toy train screw puzzle. A cute miniature countryside railway station in a warm handcrafted storybook 3D style: honey-colored timber platform canopy framing upper corners, a small cream station house with muted forest-green trim near one edge, a charming round station clock without readable lettering, distant railway tracks, soft trees and gentle hills. Warm morning light, rounded toy-like architecture, restrained pastel colors, softly blurred distant details. Keep the central area and lower half spacious and uncluttered: broad pale warm stone platform where a separate interactive train will be rendered. Decorative details toward edges, clear central silhouette space, low contrast background. No train or foreground vehicle, no people, no text, no logos, no UI, no screws. Match a cozy toy-shop world and a toy airport hangar with soft cinematic daylight.

The camera backdrop component now accepts either airport or station artwork. It retains aspect-fill framing, a build-included unlit shader, no input collider, and separate UI contrast backings. The Toy Shop layout is unchanged; only the train thumbnail is refreshed.

## Phone acceptance

Use Tools > ScrewPuzzle > Build and Run 3D Workshop on Phone.

1. Rotate through the roof and underside; verify small heads are still easy to tap.
2. Complete all nine assemblies using two free trays. The wheels, cab and final chassis must disappear too.
3. Try hints from a tilted view; Reset View must preserve removed screws and trays.
4. Leave during a partial run and return: arrangement and progress should match.
5. Check station clarity in portrait and landscape, then replay from the shop for another arrangement.

The roof and underside fasteners unlock at the cab and wheel stages, following the internal motor and gear stages. Actual Android rendering/performance needs phone acceptance. No ads or purchases are added.

## Automated results

2026-10-20: 9 initial checks passed / 0 failed (train tap-through completion/restart, seeded resume, rotation/reset view, shared layout audit, airplane backdrop/rotation regression, and captures). The final hint-driven solve test passed across all five toys. Portrait and wide renders inspected; shelf thumbnail updated. git diff --check passed. Changes are local; no GitHub push or Android device run was performed.

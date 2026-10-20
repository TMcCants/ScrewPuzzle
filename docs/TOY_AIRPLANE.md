Current implementation: see [Airplane rebuild and randomized layouts](AIRPLANE_REBUILD.md) for the nine-assembly, 27-screw version. The notes below describe the original prototype and earlier visual trials.

# Wind-up toy airplane

The fifth Toy Shop board has a blue fuselage, cream wing stripes, gold tail and
wind-up key. Near and far wings remove independently; the near wing and root
cover reveal a spring carrier, then the propeller gearbox. The nose cowl and
propeller leave together. The tailplane and fin belong to the rear assembly.

18 screws and six assemblies use the accepted robot/train color/dependency pattern
and two free trays. This adds a different shape, not a claimed difficulty increase.

Independent toy-plane save/completion keys preserve the other boards. Next Toy
now goes Train -> Airplane -> Radio. Both Android build paths include the new
scene; make a full build, not a scripts-only patch. Music remains continuous and
quieter on the airplane just like the other boards.

Acceptance: scroll to Toy Airplane, rotate and select all faces, solve without
bonus trays, inspect both inner mechanisms, restart during removal, resume a
partial board, verify completion/Next Toy and independent Music/SFX controls.
Manual PC and Android acceptance is pending.

Validation: nine initial checks passed for direct picking, two-free-tray solving, restart, resume, hints across five boards, build lists and model capture. Four final checks passed for the focused airplane resume test, music lifecycle and board/shop preview captures. Portrait and landscape previews reviewed. Android device acceptance remains pending.

## Smaller screw trial

Airplane-only: visible heads and slots scale to 70% of their former size and move
0.04 world units toward their mounting surfaces. The existing Head/Slot objects
retain their original colliders, transforms and selection rules; their renderers
are hidden and separate mesh-only children supply the smaller visuals. No new
raycast priority or click-through behavior is introduced. Restart, detachment and
resume still manage the same screw roots and colliders. Save signatures are unchanged.

Phone acceptance: compare proportions and try taps slightly off-center on wings,
tail, nose and the hidden layers. The other four toys retain their current screws.

Smaller-screw validation: all five selected checks passed, including a tap outside the new visible head radius, full solve, restart, saved-game recovery and render captures. Thumbnail and portrait preview visually reviewed. Phone tap comfort remains for user acceptance.

Phillips trial: airplane screw slots now use two shallow dark diagonal strips, forming an X. These are render-only geometry; original tap colliders remain unchanged. Both render checks passed and thumbnail/portrait previews were visually reviewed. Phone appearance acceptance pending.

## Geometric Phillips recess

Supersedes the flat cross-strip trial: the airplane head is now a shared custom
mesh with a cross-shaped opening, sloped walls and a floor 0.0327 world units
below the top face. The outer rim is beveled. All faces use the screw's colored
material; the cavity's highlights and dark areas come from its surface normals.
Original invisible selection colliders remain unchanged. The shared runtime mesh
is released when the board is destroyed.

Validation: four gameplay/render checks passed (including off-center selection,
full solve and restart). Two additional checks passed: raycasts against the actual
head mesh verify a lower center floor and raised outer face, and a close-up render
was visually reviewed. Phone appearance acceptance is still pending.

Orientation refinement: removed the 45-degree recess rotation so the mounted screw shows a plus-shaped opening. Recess depth, head dimensions and tap colliders are unchanged. Four geometry/render checks passed; updated model preview visually reviewed.

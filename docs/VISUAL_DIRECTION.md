# ScrewPuzzle V0.5 Visual Direction

Status: **Locked on September 19, 2026**

![Approved warm-workshop concept](art/ScrewPuzzle_V05_Visual_Direction.png)

## North Star

ScrewPuzzle uses a warm, tactile workshop-restoration aesthetic. The game should feel like the
player is repairing a desirable physical object on a cared-for workbench—not manipulating flat
prototype shapes and not playing inside a bright generic mobile-game template.

The approved concept is the style anchor. It establishes mood, materials, depth, palette, and
visual hierarchy; it is not a pixel-perfect gameplay screenshot.

## Style

- Premium stylized 2.5D mobile-game art
- Handcrafted, dimensional materials with clean silhouettes
- Cozy workshop light and restrained surface wear
- Mature and approachable rather than childish, grimy, or photorealistic
- Modern restoration-puzzle identity; avoid exaggerated steampunk decoration

## Core Palette

| Role | Direction | Reference color |
|---|---|---|
| Background | Deep warm charcoal | `#151210` |
| Primary wood | Aged walnut | `#6B3215` |
| Metal accent | Brushed brass | `#C98A3B` |
| Highlight | Warm amber | `#E3A84A` |
| Primary text | Warm cream | `#F2E3C3` |
| Red screw | Saturated tool red | `#E63A32` |
| Blue screw | Clear cobalt blue | `#1689E5` |
| Yellow screw | Golden yellow | `#F4B916` |
| Blocked state | Desaturated gray at reduced opacity | `#858585` |

Exact values may be tuned for contrast on a physical phone, but the relationships must remain.
Red, blue, and yellow are the only bright gameplay colors.

## Environment

- Use a dark workshop surface with subtle worn texture.
- Add restrained amber edge lighting and depth behind the object.
- Suggest tools or shelving only as soft, low-contrast atmosphere.
- Never let background detail compete with screws, tray slots, instructions, or buttons.

## Puzzle Objects

- Objects use believable materials: walnut, painted metal, rubber, glass, and brushed brass.
- Each object needs a strong readable silhouette and clearly separable restoration parts.
- Interactive screws sit above the object visually and remain obvious at phone scale.
- The radio retains all nine gameplay screws: three red, three blue, and three yellow.
- Blocked screws remain visibly dim without becoming invisible.

## Tray and Screws

- The tray is dark metal with five clearly recessed circular slots.
- Filled and empty slots must be distinguishable at a glance.
- Screws have dimensional rims, a readable slot, and strong color separation.
- Existing settle, match-pop, and full-tray feedback must remain readable against the new art.

## Interface

- Preserve the current hierarchy: title, level name, instruction, object, tray, progress, restart.
- Use a confident display face for the title with less ornament than the concept rendering.
- Use a clean, highly legible face for instructions, status, and controls.
- Keep touch targets generous and protect controls from camera cutouts and navigation areas.
- The sound control is secondary and must not compete with the puzzle object.
- Render live text in Unity; do not bake instructional or status text into raster artwork.

## Production Corrections to the Concept

The approved image intentionally locks the direction, not every generated detail. Production must:

1. Restore the radio's exact nine-screw layout.
2. Reduce title ornament and decorative metalwork by roughly 15 percent.
3. Reduce the visual prominence of the sound control.
4. Keep interactive regions cleaner than the atmospheric outer background.
5. Preserve all tested puzzle rules, animations, sound behavior, and accessibility contrast.

## Implementation Order

1. Reusable workshop background — implemented
2. Reusable screw, tray, button, and typography system
3. Radio artwork and restoration-part integration
4. Toy-car artwork in the same material language
5. Level-select screen
6. Lunyx app icon and loading screen
7. Android visual-regression pass

The production background lives at
`Assets/Resources/Art/Workshop_Background.png`. Both gameplay bootstraps load it through the
shared `PrototypeLevelBuilder`; a plain-color fallback protects level startup if the resource is
missing.

## Avoid

- Candy-like rainbow interfaces
- Sci-fi neon
- Busy tool clutter
- Heavy grime or horror lighting
- Childish cartoon proportions
- Excessive steampunk gears, rivets, or ornamental framing
- Art details that obscure blockers, screw colors, tray capacity, or tap targets

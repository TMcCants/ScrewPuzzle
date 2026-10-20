Follow-up: [Top and underside fasteners](AIRPLANE_TOP_BOTTOM_SCREWS.md) relocates six screws without changing the total or tray logic.

# Airplane airport and full rotation

The airplane now uses .2352-unit visible screw heads (20% smaller than its previous .294 heads). The original .42-unit head collider and slot collider remain unchanged. Other toys retain their accepted screw sizes.

Horizontal drag turns the airplane; vertical drag pitches it about the camera's right axis. Both allow full revolutions using quaternions. The existing drag threshold, stationary-release selection, UI exclusion, cancellation, and multi-touch handling remain shared. Reset View restores orientation without touching puzzle progress; Restart resets both. Hints can find a reachable screw after arbitrary tilt.

Airport-Background.png is a project-owned generated raster in Assets/Resources/Art/Workshop. A camera-mounted backdrop quad renders before geometry with a Resources shader, aspect-fill cropping and no collider. It follows camera framing in portrait and landscape. Dark translucent UI backings preserve text contrast. The Toy Shop and other boards are unchanged.

## Artwork provenance

Generated with the built-in image_gen tool through the imagegen skill (not the CLI). Final prompt:

> Create a polished background illustration for a cozy mobile 3D toy airplane screw puzzle. Portrait 2:3 composition. A cute miniature airport viewed from inside a warm cream-colored toy hangar, looking out toward a tiny pastel control tower and distant runway, gentle blue sky and fluffy clouds, soft golden daylight. Rounded handcrafted toy-like architecture, charming sophisticated storybook 3D rendering. Hangar framing and small airport details near edges; large uncluttered muted warm blue-gray central area and simple cream floor in lower half where a separate interactive airplane will be rendered. Soft low-contrast detail, no people, no foreground aircraft, no text, no letters, no logos, no UI, no screws. Must work as a subtle game backdrop with an independently rendered toy over the center; background should feel distant and gently out of focus.

## Phone acceptance

Use Tools > ScrewPuzzle > Build and Run 3D Workshop on Phone.

- Check screw visibility and off-center tapping with the smaller heads.
- Drag vertically to inspect top and underside; drag horizontally through a full turn. Drags must never collect a screw.
- Try Hint after tilting; Reset View should return upright without changing trays or removed screws.
- Complete the airplane using two free trays and inspect the airport/text in portrait and landscape.

Runtime tests cover the full rotation, reset view preserving progress, hint recovery, background resource/shader availability, complete disassembly, and original generous tap targets. Device rendering/performance remains for phone acceptance.

Validation: first Unity run 5 passed / 0 failed; final Reset View/shared interaction run 5 passed / 0 failed. Portrait and wide renders inspected. git diff --check passed. Phone testing and GitHub publishing were not performed.

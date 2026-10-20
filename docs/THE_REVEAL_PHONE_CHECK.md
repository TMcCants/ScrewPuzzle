# The Reveal phone icon check

The Reveal is configured for the next Android build. Unscrew Wonders remains provisional; product name and application identifiers are unchanged.

## Artwork and exports

- Assets/Resources/Art/Branding/Reveal_Foreground.png: transparent generated artwork mechanically fitted into a radius of 298 pixels on a 1024-pixel canvas, inside Android's 66/108 safe diameter.
- Reveal_Background.png: opaque deep teal background.
- Reveal_Legacy.png: composed icon for legacy and round slots.
- docs/art/unscrew-wonders-concepts/05-phone-icon-preview.png: simulated circular and rounded-square launcher viewports at 48, 64, 96 and 180 pixels.
- The original icons remain available for rollback. Android slots reference the new assets at all configured densities.

The source was edited with built-in imagegen. Export-Reveal.ps1 performs mechanical sizing, compositing and preview rendering; it does not redraw the generated artwork. All textures use uncompressed import settings to preserve the recessed head and alpha edges.

## Validation

2026-10-20: User confirmed successful phone testing of the new icon, startup, saved completion/progress and replay checks. This is user-reported device validation, in addition to the agent's static and visual checks below.

Visual review: screw, star and panel remain visible at the tested sizes and both masks. Preview uses a centered 72/108 viewport; this is an approximation, not a screenshot from a phone. No themed monochrome artwork is included in this pass.

ADB could not start its local server, so no phone installation or on-device inspection was completed.

Static verification passed: three 1024x1024 exports; foreground corner alpha 0, background/legacy corner alpha 255; six adaptive background and six foreground assignments, twelve legacy/round assignments; git diff whitespace check clean. A Unity batch import launch produced no log, so Unity import/build validation is unconfirmed.

## On your phone

1. In Unity use Tools > ScrewPuzzle > Build and Run 3D Workshop on Phone.
2. Find ScrewPuzzle 3D Workshop in the app drawer and on the home screen. The provisional name will not appear as the app label.
3. Check that the coral screw and golden star are visible, with no clipped edges, then launch it and confirm the Lunyx splash and game still open.
4. If the old icon remains, first remove only the home-screen shortcut and add it again from the app drawer. Avoid uninstalling, which can erase local progress.

Android design reference: https://developer.android.com/develop/ui/compose/system/icon_design_adaptive

## Generation prompt

Edit this approved app icon into an Android adaptive FOREGROUND LAYER with TRUE TRANSPARENT BACKGROUND. Preserve coral red threaded Phillips plus-head screw lifting diagonally upward-right out of a teal toy panel and revealing one gold star. CRITICAL: zoom the entire subject WAY OUT. All artwork including teal panel, glow and screw must fit inside a central circle of diameter 58% of the square canvas, with wide completely transparent margins around all four sides. Turn the cropped teal panel into a complete small rounded chunky teal tile with a circular hole containing the gold star. Screw above hole, distinct star below-left. Compact composition; smooth polished toy plastic, clean deep recessed PLUS, 3 clear thread ridges, one tiny sparkle only. No shadows or glow outside central circle. No background color, no frame, no letters, no mockup. The transparent margins are essential for Android launcher masking.

# Toy race car and racetrack

The car is now a red number-seven toy racer with a lower rounded glass cockpit, cream twin racing stripes, sport wheels, beveled wheel openings, a tapered nose, and a rear spoiler. Hood stripes and nose trim leave with the Nose Assembly; the spoiler, its supports, and trunk stripes leave with the Rear Bumper Assembly. Side number badges and stripes belong to the side assemblies. The wheel openings use matching mesh colliders, so the visible openings are also open to raycasts.

The existing 15 screws, five assemblies, color pattern, dependencies, tray sequence, screw identities, and save signature are preserved. Full pitch/yaw rotation, top and underside fasteners, smaller recessed plus heads, generous tap proxies, lock indicators, covered bonus trays, and the two-free-tray solution remain in place. The car's shop card retains its internal identity and now displays Race Car.

Visibility follow-up: the hood now has two screws (Nose 1/red and Nose 2/blue) on cream seats, aligned with the sloping hood. The underside has two screws (Rear 1/red and Rear 2/blue) on silver seats along an extended cream mounting strip. The heads project clearly beyond their seats. These are relocated existing screws, so the total remains 15 and the same assembly/solution/save identities apply.

The racetrack uses the same camera-mounted, aspect-fill artwork and mobile-compatible background shader as the airport and station. It has no collider. The compact header backing keeps controls readable; the racetrack remains visible behind and around the trays.

## Background artwork

Saved asset: [Racetrack-Background.png](../Assets/Resources/Art/Workshop/Racetrack-Background.png).

Generated with the built-in image-generation tool, not the fallback CLI.

Final prompt:

> Use case: stylized-concept. Asset type: portrait background illustration for a cozy 3D toy screw-puzzle mobile game. Create a cute miniature racetrack environment, polished softly rendered 3D storybook toy aesthetic, warm afternoon sunlight, rounded shapes, playful welcoming mood. Empty broad smooth pale warm-gray pit lane in the foreground and center, with a gently curving racetrack behind it, pastel red-and-white safety barriers, a little pit garage and checkered pennants toward the edges, distant small grandstand, rounded green hills and fluffy clouds. View from slightly above toy-car height. Portrait composition, approximately 2:3 aspect ratio. Keep the central middle area spacious and visually quiet for a separately rendered toy race car, and the lower third uncluttered for game trays. Details concentrated toward the sides and distant upper background. No cars, no people, no characters, no readable text, no logos, no interface, no foreground obstacles. This is environment artwork only.

## Phone acceptance

1. Open Race Car from the Toy Shop and check its new appearance and background.
2. Turn all the way around, above, and underneath; tap the hood and underside screws.
3. Complete the board with only the two free trays. The spoiler and racing details should leave with their assemblies.
4. Restart and confirm all pieces return. Leave a partly completed board and return to check resume.
5. Check portrait and landscape framing, tray readability, Reset View, and the sound/music controls.

Use **Tools → ScrewPuzzle → Build and Run 3D Workshop on Phone**.

## Previews and validation

- [Portrait](art/Race_Car_Phone.png)
- [Top screws](art/Race_Car_Top.png)
- [Underside](art/Race_Car_Underside.png)
- [Landscape](art/Race_Car_Wide.png)

Unity validation: the initial six checks passed, including resume; the final three checks passed after sculpting the body (`Logs/race-car-final-results.xml`). The final run verifies full car completion with only free trays, screw reachability, spoiler/hood release and restart, background shader/texture loading and scene cleanup, plus all-board orbit and tray controls. Rendered portrait, landscape, top and underside views were reviewed.

Generated background: `Assets/Resources/Art/Workshop/Racetrack-Background.png`. The Toy Shop thumbnail was rendered from the actual model into `Assets/Resources/Art/Workshop/Shelf-Car.png`.

Physical phone testing remains pending. Changes are saved locally, not pushed to GitHub.

Toy Shop render check passed (Logs/race-car-shop-results.xml); the updated thumbnail and Race Car label were visually verified. [Shop preview](art/Race_Car_Shop.png). git diff --check passed.

Vertical-screw follow-up validation: all three checks passed in Logs/race-car-visible-final-results.xml. Tests require exactly two top and two underside screws, verify unobstructed rays to the visible head geometry as well as taps, and complete the original two-free-tray solution. Decorative seats have no colliders, preventing primitive cylinder collision approximations from intercepting taps. Fresh top/underside previews were reviewed; phone testing remains pending. Rebuild and press Restart on the car to view all four screws in a fresh puzzle.

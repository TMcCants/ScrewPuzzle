# Full rotation and sorting trays on every toy

All five boards now use continuous horizontal and vertical rotation, Reset View, smaller recessed plus-head visuals with the existing larger touch colliders, and the shared cream sorting trays. Available screws stay bright; blocked screws are muted and show a padlock when visible. Tapping a blocked screw explains its assembly dependency.

The robot is framed slightly smaller and higher to leave room below its boots for Reset View; its head tap proxies retain their original world size. Lock-badge visibility checks the rendered head's location against covering geometry, so the larger invisible tap proxy cannot reveal an inner screw through an outer panel. First-play coaching now introduces looking above and underneath.

The two bonus trays have covered lids. Cancel keeps them closed; Preview Open uses the existing free test unlock and lifts the lid. Ads and purchased credits are not connected. Restart closes the lids and restores the puzzle. There is no large lower gray backing; only compact text backings and individual tray shadows.

## Screw placement

- Radio: Left 2 moves to a top mounting tab; Right 0 moves to an underside tab. Both tabs belong to their side-panel assemblies.
- Car: Nose 1 and Nose 2 sit on contrasting hood seats; Rear 1 and Rear 2 sit on an extended underside mount. All remain part of their original removable assemblies. See [race-car follow-up](RACE_CAR_REDESIGN.md).
- Robot: Left 2 moves to a top mounting tab; Right 0 moves to an underside tab, attached to their respective side assemblies.
- Train and airplane retain their existing top, underside, side, and layered screw layouts.

Counts, colors, screw identities, assembly dependencies, and tray sequences are preserved. Randomized train and airplane layouts retain their existing rules. The radio, car, and robot retain their existing fixed puzzles.

## Phone checks

1. Open each toy and rotate sideways, over the top, and underneath. Reset View should preserve puzzle progress.
2. Check that the new top/underside screws are easy to tap and release their original assemblies.
3. Complete each board with only the two free trays.
4. Check the bright/blocked screw states and tap a visible blocked screw for its explanation.
5. Cancel a bonus unlock, then try Preview Open. Restart should restore the lid.
6. Leave a partly completed board and return; progress should resume.

Build with **Tools → ScrewPuzzle → Build and Run 3D Workshop on Phone**.

## Previews

| Toy | Front | Top | Underside |
| --- | --- | --- | --- |
| Radio | [View](art/All_Toys_Radio_Front.png) | [View](art/All_Toys_Radio_Top.png) | [View](art/All_Toys_Radio_Underside.png) |
| Car | [View](art/All_Toys_Car_Front.png) | [View](art/All_Toys_Car_Top.png) | [View](art/All_Toys_Car_Underside.png) |
| Robot | [View](art/All_Toys_Robot_Front.png) | [View](art/All_Toys_Robot_Top.png) | [View](art/All_Toys_Robot_Underside.png) |
| Train | [View](art/All_Toys_Train_Front.png) | [View](art/All_Toys_Train_Top.png) | [View](art/All_Toys_Train_Underside.png) |
| Airplane | [View](art/All_Toys_Airplane_Front.png) | [View](art/All_Toys_Airplane_Top.png) | [View](art/All_Toys_Airplane_Underside.png) |

## Validation

- Initial Unity regression run: 13 passed, 0 failed (`Logs/all-toys-upgrade-results.xml`). Covers complete free-tray solves, input gestures, release/restart, resume, full orbit, hint selection, and shared tray controls.
- Tutorial and preview run: 3 passed, 0 failed (`Logs/all-toys-preview-results.xml`).
- Final run after the robot framing and hidden-head visibility corrections: 6 passed, 0 failed (`Logs/all-toys-final-results.xml`). Rechecks complete hint-driven solves on all five boards, robot solve/restart, covered-head occlusion, all-board orbit/tray controls, train lock-state transitions and lid persistence, and preview generation.
- Rendered previews inspected from the front, top, and underside of the newly updated toys. `git diff --check` passed.
- This update still needs physical phone testing. Changes are local, not pushed to GitHub.

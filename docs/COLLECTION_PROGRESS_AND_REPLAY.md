# Collection progression and replay

Toy Shop and its collection card show a count of the five distinct completed toys. Existing completion preferences remain authoritative; repeat wins never inflate the count.

Toy cards show NEW, IN PROGRESS, COMPLETED, or REPLAY IN PROGRESS. An unfinished replay retains its completion credit and RESUME button. Finishing all five displays a collection-complete invitation to replay.

Play Again loads a fresh board. Radio, race car, and robot select a different color permutation from the previous replay, changing screw and tray colors together. Physical screw sockets and dependency order remain unchanged on these three boards. Train and airplane keep their randomized socket/color layout generator. Restart during a puzzle retains that puzzle's layout.

Unfinished replays save their variant with their action history. Older unfinished games use variant zero and retain their original colors. All variants of the first three toys preserve every color match, tray count, and dependency in their proven two-free-tray solution.

## Phone checks

- Verify existing completed toys remain completed and the collection count is correct.
- Start a new toy, remove a screw, and return to Toy Shop: IN PROGRESS / RESUME.
- Finish a toy: count increases once and its card becomes COMPLETED / REPLAY.
- Replay a completed toy: its colors change; after a move the card reads REPLAY IN PROGRESS and completion count stays unchanged.
- Close/reopen the app and Continue: colors and removed screws should match the saved replay.
- Finish all five: the collection card acknowledges completion. Every toy remains replayable.

Publication scope: the accumulated game changes are submitted on a review branch as a draft PR. Progression still requires phone testing before merge.

Validation: 13 automated checks passed across completion/replay, progress, resume, home, and randomized layout suites. Two additional Unity preview captures passed. Collection-complete and Toy Shop replay-status renders were visually inspected. Preview progress is test data; user saves were not changed by the isolated validation copy.

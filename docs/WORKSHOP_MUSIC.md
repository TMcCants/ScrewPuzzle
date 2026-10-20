# Toy Shop music

An original 48-second synthesized score, Little Clockwork, uses piano-like plucks,
a low accompaniment and sparse bell tones at 80 BPM. This is procedural music,
not a recording of acoustic instruments. Note tails wrap across the loop seam.

WorkshopMusic creates one persistent non-spatial AudioSource and a 22.05 kHz mono
clip once per session. The shop volume is 0.28; puzzles fade to 0.14. Leaving the
workshop fades to silence. Pause/focus loss pauses playback; returning resumes it
with a fade. Music toggle is saved separately from the existing effects preference.
SFX labels control the existing screw/tray/celebration effects. Music defaults on.

Phone checks: listen through a full loop, move between shop and all four toys,
toggle Music and SFX independently, close/reopen the app, background/return, and
confirm screw cues and celebrations remain audible. Listen for repetition, harsh
high notes and the loop seam. Artistic balance and actual device audio need manual
acceptance; automated sample checks cannot establish that music sounds pleasant.

Validation: music lifecycle test passed (single player, independent setting, bounded sample peak, loop boundary, scene volumes, pause/resume). Two UI capture tests passed; portrait shop and board controls were visually reviewed. Manual phone listening remains pending.

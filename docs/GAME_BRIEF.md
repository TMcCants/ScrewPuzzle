# ScrewPuzzle — Game Brief

## Working Title

ScrewPuzzle

## Core Fantasy

Bring broken objects back to life by carefully removing, sorting, and clearing screws.

## Player Goal

Clear all required screws without overfilling the holding tray, then complete the restoration of the object.

## Core Gameplay Loop

See broken object → select legal screw → move screw to tray → match and clear sets → release object components → finish puzzle → watch object restore → see a short “come to life” payoff.

## First Level Object

Vintage radio.

## Starter Object Family

- Vintage radio
- Toy robot
- Old camera
- Small CRT television
- Toy car

## Core Rules

- Screws have colors.
- Only eligible screws can be selected.
- Selected screws move into a limited holding tray.
- Matching sets of screws clear from the tray.
- The tray has limited capacity, so careless choices can create a fail state.
- Some screws may be blocked by object layers or other screws.
- Removing screws causes visible parts of the object to loosen or release.
- The level is won when all required screws are cleared.
- The object then restores and performs a short completion animation.

## Version 0.1 Scope

Version 0.1 is intentionally small and is designed to prove the complete game loop.

### Included

- One playable level
- One vintage radio
- Clickable screws
- Screw colors
- Legal/blocked screw logic
- Limited tray
- Match-and-clear behavior
- Lose state when the tray becomes unusable/full
- Win condition
- Restart
- Basic restoration sequence
- Short radio completion animation or effect

### Explicitly Not Included

The following are deferred until later iterations:

- Level map
- Currencies
- Ads
- In-app purchases
- Collections
- Cosmetics
- Daily rewards
- Boosters
- Multiple themes
- Save progression
- Multiple object families
- Fancy menus
- Analytics
- Achievements
- Cloud saves

## Learning Goal

The first milestone is complete only when Tamika can explain:

- Which script owns screw behavior
- Which system decides whether a screw can move
- How the tray tracks screws
- How matches are detected
- How win/loss states are triggered
- How the radio restoration is connected to puzzle progress
- Where each rule can be modified safely

Tamika will then make at least one controlled code or configuration change herself.

## Success Criterion for Version 0.1

The goal is not for Version 0.1 to look like a finished mobile game.

The vertical slice is successful when:

> One complete level works from first tap to restored radio, and Tamika understands how the system is put together.

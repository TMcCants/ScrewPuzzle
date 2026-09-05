# ScrewPuzzle

A small, expandable Unity puzzle game built as both a commercial learning project and a complete game-shipping exercise.

## Current Product Direction

The game begins with a simple screw/sort puzzle core, uses restoration as the immediate payoff, and deliberately postpones the larger collection/meta layer until later iterations.

**Core progression philosophy:**

> Puzzle first → restoration payoff second → collection/meta layer later.

## Core Level Sequence

1. The player sees a broken object.
2. The player solves the screw/sort puzzle by removing legal screws, managing limited tray space, and clearing matching sets.
3. Panels or components release as progress is made so the object visibly responds to the player's actions.
4. The puzzle reaches completion when all required screws/components are cleared.
5. The object restores or reassembles.
6. The restored object briefly comes to life for a satisfying completion moment.

Examples of completion beats:

- Vintage radio lights up and plays a short sound/static cue.
- Toy robot wakes up.
- Old camera flashes.
- Small CRT television flickers on.
- Toy car rolls forward.

## Starter Object Family

The first object set is intentionally compact and mechanically readable:

- Vintage radio — planned first object / Level 1
- Toy robot
- Old camera
- Small CRT television
- Toy car

## Development Philosophy

This project is intended to teach game development by ownership rather than by requiring every line of code to be written manually.

The working rhythm is:

**Vale builds → Vale explains → Tamika documents → Tamika modifies → review together.**

The goal is for Tamika to understand the codebase well enough to explain its systems, safely modify them, and eventually extend the game independently.

## Status

**Product definition in progress.**

Unity implementation has not started yet. The next step is to lock the one-page Game Brief and exact Version 0.1 scope before beginning implementation.

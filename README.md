## Overview
A data‑driven, high‑performance match/blast puzzle framework using a **single Game partial‑class architecture**.  
All gameplay systems (board, fall, boosters, action‑queue, replay, rendering, gizmos) live inside partial files of the `Game` class.

## Performance Features
- **Procedural instanced rendering** (no SpriteRenderers, no per‑item GameObjects).
- **Central ActionQueue**: deterministic execution of clicks, blasts, boosters, gravity.
- **Seed‑based randomness** enabling fully reproducible sessions.
- **Replay System**: records all actions; can rebuild entire gameplay deterministically.
- **Optimized Fall Simulation**: batched transforms, easing motion, zero per‑item Update.

## Debug Tools
- Optional cell gizmos with indices & color overlay.
- Config‑driven: fall speeds, match thresholds, busy durations, debug toggles.

## Summary
Fast, deterministic, replayable, and extremely modular.
Ideal as a foundation for any modern match/blast puzzle game.

# Ki Skill Architecture

This document tracks the current skill framework and the planned lore-inspired roster. It should stay honest: names here are planning labels unless the technique appears in the implemented table.

## Implemented Metadata

Each implemented ki technique now carries:

- display name
- category
- source/inspiration label
- required form
- required ki power
- initial ki cost
- held beam drain, if applicable
- projectile behavior
- collision style
- pierce/tracking/terrain flags derived from behavior
- color and progression note

The current metadata lives in `Common/KiTechniques.cs` and is exposed in item tooltips and the stats panel.

Projectile behavior now reads this metadata for terrain collision and impact feel. Sustained beams trace forward until solid terrain blocks them and use a shared renderer with charge orb, core, glow, animated stream segments, head, and impact flare. Terrain-passing ultimates keep their special behavior, and guided piercing techniques remain terrain-free for now.

## Implemented Techniques

| Technique | Category | Source label | Collision style | Notes |
| --- | --- | --- | --- | --- |
| Basic Ki Blast | Basic ki blast | General ki | Terrain blocked | Cheap small pop-shot for constant starter pressure. |
| Ki Barrage | Barrage | General ki | Terrain blocked | Four small pressure shots with spread and lighter per-shot damage. |
| Masenko | Heavy blast | Gohan line | Heavy impact | Fast yellow palm-wave/lance burst after basic control. |
| Kamehameha | Continuous beam | Turtle School | Sustained line | Stable blue-white held beam with short charge and long reach. |
| Death Beam | Basic ki blast | Frieza line | Terrain blocked | Very fast thin precision shot with small bright impact. |
| Destructo Disk | Cutting disk | Earthling tactical | Guided piercing | Cursor-guided spinning cutting disk with piercing identity. |
| Galick Gun | Continuous beam | Vegeta line | Sustained line | Purple held beam with more aggressive flicker and higher strain. |
| Special Beam Cannon | Continuous beam | Namekian | Sustained line | Narrow drill-like beam with spiral visual accents. |
| Big Bang Attack | Heavy blast | Vegeta line | Heavy impact | Large compact orb that swells briefly and detonates harder. |
| Final Flash | Continuous beam | Vegeta line | Sustained line | Long-charge huge yellow beam with high drain, long reach, and shake feedback. |
| Spirit Bomb | Ultimate | Turtle School | Terrain-passing ultimate | Slow charge above the player, heavy ki drain, long terrain-passing travel, huge impact. |
| God Kamehameha | Continuous beam | God ki | Sustained line | Cleaner high-tier god-ki beam with smoother charge/release profile. |
| Ultra Instinct Barrage | Barrage | Ultra Instinct | Terrain blocked | Seven fast clean shots with afterimage-like streaks and lighter per-shot damage. |

## Planned Roster Buckets

Add future techniques by choosing a bucket first, then tuning the Terraria behavior:

- Basic ki blasts: Ki Blast, Charged Ki Blast, Energy Wave, Eye Laser, Finger Beam.
- Beams: Kamehameha variants, Galick Gun variants, Final Flash, Special Beam Cannon, Death Beam.
- Cutting disks: Destructo Disk, Multiple Destructo Disk, tracking saucer variants.
- Barrages: Continuous Energy Bullets, Die Die Missile-style barrages, Finger Barrage.
- Heavy blasts and bombs: Big Bang Attack, Death Ball, Supernova, Eraser Cannon, Spirit Bomb.
- Utility and defense: Solar Flare, Energy Barrier, binding/trap techniques.
- Advanced/endgame: God Kamehameha, Hakai-style late/postgame destruction, Ultra Instinct Barrage.

## Implementation Rules

- Do not add a technique without a clear category and collision style.
- Beams should use held/channel behavior, charge briefly when appropriate, and drain ki continuously after release.
- Standard sustained beams should stop at solid terrain unless explicitly marked as terrain-passing or piercing.
- Disks should not behave like normal balls; basic disks pierce, special variants may track.
- Ultimate attacks should have charge, commitment, high ki cost, and longer cooldown/fatigue later.
- Mark game-only or uncertain moves separately before putting them in progression.

## Sound Hooks

`Systems/KiSoundSystem.cs` is the central sound helper. It currently maps events to temporary CC0 energy/electric sounds and keeps vanilla Terraria fallback sounds inside the helper:

- power-up start
- transformation complete
- power-down
- Kaio-Ken activation
- low-ki fizzle
- technique charge start
- technique charge/release
- technique sustain
- technique fire
- technique impact
- melee impact

Future original audio should swap through this helper instead of scattering sound calls across items, projectiles, and player code.

The current pass also maps each implemented technique to a distinct temporary sound profile and fallback group. These are still placeholder sounds, but the charge/release/sustain/impact hooks no longer all advertise the same profile during dev inspection.

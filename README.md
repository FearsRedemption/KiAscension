# Ki Ascension

A DBZ fan-based Terraria mod for tModLoader: train from a base Saiyan, build physical power and ki power, unlock iconic transformations, throw ki techniques, push through boss gates, and break major limits through dramatic breakthrough moments.

The design target is a Terraria progression overhaul with Dragon Ball Z style combat fantasy: ki blasts, beams, transformations, Kaio-Ken strain, aura escalation, training arcs, and eventually DBZ-flavored original bosses and mobs.

## Current Version Context

Checked on May 28-29, 2026 before scaffolding:

- Terraria's latest released PC line is `1.4.5`, with `1.4.5.6` listed on the Official Terraria Wiki and May news still describing `1.4.5.7` as in progress.
- tModLoader stable is still on the `1.4.4-stable` line; the latest GitHub stable release shown during setup was `v2026.03.3.0`, with newer preview builds also still labeled `1.4.4`.
- This mod is scaffolded for current stable tModLoader so it can be developed today, with a planned update pass when stable tModLoader catches Terraria `1.4.5`.

Sources:

- Terraria May 2026 news: https://store.steampowered.com/app/105600/Terraria/
- Terraria `1.4.5.6`: https://terraria.wiki.gg/wiki/1.4.5.6
- tModLoader releases: https://github.com/tModLoader/tModLoader/releases
- tModLoader mod development docs: https://www.tmodloader.app/docs/mod-development.html

## Current Status

The current build is playable prototype gameplay, not final content. Core progression, ki resource behavior, transformations, starter training, starter enemy pressure, and representative techniques exist. Final art, final audio, full boss content, higher training tiers, advanced melee, and final UI polish are still planned.

### Implemented

- Starting items: `Ki Training Focus`, `Saiyan Strike`, and `Basic Ki Blast`
- Separate ki spell items, from basic blasts through Masenko, Kamehameha, Death Beam, Destructo Disk, Galick Gun, Special Beam Cannon, Big Bang Attack, Final Flash, Spirit Bomb, God Kamehameha, and Ultra Instinct Barrage
- Held beam behavior for Kamehameha-style techniques, using temporary charge orbs, animated beam streams, beam heads, and impact flares instead of plain bars
- Terrain-aware beam range for sustained beams, while ultimate terrain-passing attacks remain supported
- Guided/piercing Destructo Disk behavior
- Kai Level, physical power, and ki power tracking per player
- Compact scouter-style ki/power/form/held-spell/next-ceiling HUD foundation with smaller text, wrapped long lines, badges, and locked-spell status
- Power experience from combat, enemy kills, weight training, meditation, and gravity room training
- Hotkeys:
  - tap `Z`: ascend one unlocked Saiyan form
  - hold `Z`: charge up to the highest unlocked Saiyan form
  - tap `X`: descend one Saiyan form
  - hold `X`: controlled power-down to Base Saiyan
  - tap/hold `C`: raise Kaio-Ken one level or charge to the highest unlocked Kaio-Ken level
  - tap/hold `V`: lower Kaio-Ken one level or release it fully
- Kaio-Ken is a separate parallel amplifier with HP/ki strain
- Saiyan forms provide max ki, ki regeneration, defense, movement, damage, and light health regeneration hooks
- Ki technique costs and beam drains scale through a shared ki resource system, with ki-control efficiency from ki power and advanced forms
- Spell tooltips show current ki costs, locked requirements, and beam sustain drain
- HUD ki readout shows net ki per second after transformation, flight, and active held-technique drain
- Styled read-only inspection panels:
  - tap `B`: toggle detailed ki stats, active drains, gates, and held-spell costs
  - tap `N`: toggle a dev inspection panel for synced player state plus temporary feel-debug readouts
- Form-gated ki flight starts at Super Saiyan and scales through later forms with different control and ki drain
- Hair/form visuals are routed through dedicated hair profiles separate from stat logic
- Ki techniques have metadata for category, source label, collision style, terrain behavior, pierce, held-beam, and cursor-tracking behavior
- Central ki sound helper wired to transformations, technique charge/release/sustain, fire/impact, beam fizzle, and strain events using imported CC0 temporary sounds with Terraria fallback
- Aura profiles plus a temporary animated aura draw layer, electric arc overlay, charge-up flare, and power-down collapse per Saiyan form/Kaio-Ken overlay
- Enemy and boss stat scaling by world progression
- Vanilla and non-ki weapons remain usable while the custom ki/melee systems are still growing, but Ki Ascension progression is intended to outscale them over time
- Boss and witness-loss gates for major emotional breakthroughs, with auto-breakthrough only when already at the previous form ceiling
- Multiplayer-safe per-player power, ki power, Kai Level, Saiyan form unlocks, Kaio-Ken level, and ki
- Custom Terraria-style `ModHair` sprites for Super Saiyan-style forms, with matching `_Alt` textures and natural-hair restore handling
- `Weighted Training Bands`, wooden/copper placeable weight benches, wooden training bag, meditation mat, capped training sources, and `Gravity Room Core` training tools
- `Saiba Sprout`, an early hostile ki-trained plant enemy that appears on the surface and fires simple ki shots
- Starter item/crafting framework docs, `Ki Fragment` material drops, and implementation folders for future bosses and mobs

### Partially Implemented

- UI polish: the main HUD and B/N panels are now styled and wrapped, but final UI art, advanced combat UI, and editable/config-gated dev controls are still planned.
- Aura visuals: animated aura overlay support exists with temporary generated sheets, but final aura art/shaders are not complete.
- Hair visuals: custom `ModHair` placeholders and `_Alt` textures exist with cleaner silhouettes/face cutouts, but final animated transformation hair needs an art pass.
- Melee: `Saiyan Strike` is playable and scales with physical power, but full punch/kick animations, charged strikes, dash strikes, and combo visuals are not complete.
- Training: focus meditation, weighted gear, wooden/copper benches, wooden training bag, meditation mat, and gravity training exist; higher station tiers and full gravity chamber progression are still in progress.
- Sounds: a central sound helper is wired with temporary legally usable energy sounds; final DBZ-style original audio is planned.
- Bosses/mobs: `Saiba Sprout` is implemented as the first enemy; custom boss encounters are still planned.

### Planned

- Higher-tier placeable training stations and upgraded gravity chamber path.
- Expanded melee combo inputs and clearer punch/kick presentation.
- More high-quality representative ki techniques and final per-technique visual/audio identity.
- More DBZ-flavored original bosses and mobs, building outward from the first implemented early enemy.
- Final sprite, aura, sound, and UI art pass.

## Progression

Implemented Saiyan form path:

1. Base Saiyan
2. Awakened State
3. Super Saiyan, witness-loss gated
4. Super Saiyan 2, witness-loss gated
5. Super Saiyan 3
6. Super Saiyan God
7. Super Saiyan Blue
8. Ultra Instinct Sign, witness-loss gated
9. Ultra Instinct

Kaio-Ken is implemented as a separate parallel track from `Off` through late-game fantasy levels such as `x20`, `x100`, and `x200`. The display names keep the anime flavor, while the actual multipliers are tuned for Terraria instead of literal numeric scaling.

See [docs/progression.md](docs/progression.md) for the design pass that keeps the DBZ progression fantasy aligned with Terraria pacing. See [docs/current-behavior.md](docs/current-behavior.md) for what the current prototype should feel like in game. See [docs/phase-audit.md](docs/phase-audit.md) for the phase-by-phase implementation audit. See [docs/ki-skills.md](docs/ki-skills.md) for implemented technique metadata and planned roster buckets. See [docs/asset-sources.md](docs/asset-sources.md) for imported/placeholder asset notes. See [docs/items-framework.md](docs/items-framework.md) and [docs/bosses-and-mobs.md](docs/bosses-and-mobs.md) for expansion planning.

## Development Setup

1. Install Terraria and tModLoader through Steam.
2. Launch tModLoader once.
3. Download or clone this repo folder into:

   ```text
   Documents/My Games/Terraria/tModLoader/ModSources/KiAscension
   ```

4. In tModLoader, open `Workshop > Develop Mods`.
5. Build and reload `Ki Ascension`.

This repo does not vendor Terraria or tModLoader binaries. tModLoader supplies the required `tModLoader.targets` and Terraria assemblies from the `ModSources` parent folder. See [docs/dependencies.md](docs/dependencies.md).

## Roadmap

- Expand the starter training stations into higher tiers.
- Expand melee combo inputs beyond the starter `Saiyan Strike`.
- Add a mentor/trainer town NPC for form tutorials and optional rituals.
- Add branching late-game paths: Ultra Instinct, Ultra Ego, and Legendary Wrath.
- Add final original sprites, sounds, aura assets, and UI art.
- Implement original DBZ-flavored boss encounters that fit the ascension ladder.
- Rebalance enemy scaling with real playtest data.
- Update target compatibility once tModLoader stable supports Terraria `1.4.5`.

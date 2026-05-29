# Ki Ascension

A Dragon Ball Z style Terraria mod concept built as an original tModLoader mod foundation: train from a base ki fighter, gain power through combat, transform with hotkeys, and break major limits when an ally dies nearby.

The repo intentionally uses original placeholder implementation and vanilla Terraria visuals only. Do not ship copyrighted Dragon Ball assets, music, sprites, or ripped names/logos without permission.

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

## Implemented Foundation

- Starting items: `Ki Training Focus` and `Basic Ki Blast`
- Separate ki spell items, from basic blasts through Kamehameha, Destructo Disk, Galick Gun, Big Bang Attack, Final Flash, Spirit Bomb, God Kamehameha, and Ultra Instinct Barrage
- Held beam behavior for Kamehameha-style techniques
- Guided/piercing Destructo Disk behavior
- Kai Level, physical power, and ki power tracking per player
- On-screen ki/power/form/held-spell/next-ceiling HUD
- Power experience from combat, enemy kills, weight training, meditation, and gravity room training
- Hotkeys:
  - `Z`: ascend one unlocked form
  - hold `Z`: power directly up to the highest unlocked form
  - `X`: descend one form
- Enemy and boss stat scaling by world progression
- Vanilla and non-ki weapons are heavily downscaled so Ki Ascension replaces normal Terraria weapon progression
- Boss and witness-loss gates for major emotional breakthroughs
- Multiplayer-safe per-player power, ki power, Kai Level, form unlocks, and ki
- Custom Terraria-style `ModHair` sprites for Super Saiyan-style forms
- `Weighted Training Bands` and `Gravity Room Core` training tools

## Progression

The first implementation is a straight path:

1. Base Saiyan
2. Awakened State
3. Kaio-Ken
4. Super Saiyan, witness-loss gated
5. Super Saiyan 2, witness-loss gated
6. Super Saiyan 3
7. Super Saiyan God
8. Super Saiyan Blue
9. Ultra Instinct Sign, witness-loss gated
10. Ultra Instinct

See [docs/progression.md](docs/progression.md) for the design pass that keeps it lore-inspired while still fitting Terraria pacing. See [docs/current-behavior.md](docs/current-behavior.md) for what the current prototype should feel like in game.

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

- Polish the current ki/EXP HUD into a themed full UI.
- Add melee combo inputs for punch/kick strings.
- Add a mentor/trainer town NPC for form tutorials and optional rituals.
- Add branching late-game paths: Ultra Instinct, Ultra Ego, and Legendary Wrath.
- Add proper original sprites, sounds, and aura shaders.
- Add original DBZ-flavored boss encounters that fit the ascension ladder.
- Rebalance enemy scaling with real playtest data.
- Update target compatibility once tModLoader stable supports Terraria `1.4.5`.

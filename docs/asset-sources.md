# Asset Sources And Status

This file tracks art and audio source status separately from gameplay design docs.

No official Dragon Ball, Dragon Ball Z, Dragon Ball Super, Terraria, or third-party copyrighted fan assets are intentionally bundled here. Current visual assets are either original temporary placeholders generated for this mod or imported CC0 audio files listed below.

## Hair Sprites

Status: improved original temporary placeholder, currently not swapped in during normal gameplay.

The current `ModHair` sheets in `Hairs/` were generated for this mod as simple Terraria-format placeholders. They are 40x784 PNGs, matching 14 frames of 40x56 hair data. Each hair has a matching `_Alt.png` file because tModLoader expects alternate hair textures when head equipment changes how hair is drawn. Every base sheet and `_Alt` sheet is kept paired so the assets can be re-enabled safely after the art improves.

These are not final art. Because the placeholder sprites still read as too rough in play, normal gameplay currently disables custom hair style swapping and instead keeps the player's natural hair style with form-specific tint plus aura. The sheets remain in the repo for tModLoader asset completeness and future replacement testing. ModHair itself remains static; the animation energy is handled through aura overlays for now.

Replacement TODOs:

- Draw or commission final Terraria-proportioned form hair sheets.
- Keep every final sheet paired with a matching `_Alt.png`.
- Re-enable the custom hair swap only after face alignment, head equipment behavior, power-down restore, and save/reload are re-tested.

Researched but not imported:

- OpenGameArt/LPC hair sets, such as `[LPC] Long Straight Hair with 12 Colors`, are useful references but are built for different top-down RPG proportions, not Terraria player hair frames.

## Aura Visuals

Status: improved original temporary placeholder.

`Assets/Effects/KiAura.png` is an original eight-frame white aura sheet generated for this mod and tinted in code per form. `Assets/Effects/KiAuraElectric.png` is an original eight-frame electric arc sheet used for Super Saiyan 2-style arcs, unstable breakthroughs, and Kaio-Ken aggression. The draw layer now stacks multiple tinted, offset, and pulsing passes to make power-up, power-down, Kaio-Ken, Super Saiyan, and electric forms feel more alive while still using placeholder sheets.

Replacement TODOs:

- Replace the generic white-tinted sheet with form-specific aura sheets or a stronger procedural renderer.
- Add dedicated flight trails once the flight pass gets a visual polish pass.
- Re-test visibility behind the player so aura art does not hide the character silhouette.

Researched but not imported:

- OpenGameArt `Auras` by Kutejnikov is listed as CC0 and contains animated 512x512 aura renders, but the archives are large and not Terraria-proportioned: https://opengameart.org/content/auras
- OpenGameArt `Energy Sprite Sheets` by fzeeshan is listed as CC0 and contains transparent energy effects, but the sheets are much larger than needed for this pass: https://opengameart.org/content/energy-sprite-sheets

## Beam And Projectile Effects

Status: original temporary placeholder.

`Assets/Effects/KiBeamSegment.png`, `KiBeamHead.png`, `KiBeamImpact.png`, `KiChargeOrb.png`, `KiOrbProjectile.png`, and `KiDisk.png` were generated for this mod as small grayscale/white-alpha effect textures. The renderer tints them per technique, so the same legal placeholder sheets can support Kamehameha blue, Galick Gun purple, Final Flash yellow, Special Beam Cannon drill accents, Spirit Bomb white/blue, and Destructo Disk cutting visuals.

These are not final art. They exist to replace plain rectangle beams and generic projectiles with readable gameplay silhouettes while the final projectile art direction is developed.

Replacement TODOs:

- Give Kamehameha, Galick Gun, Final Flash, Special Beam Cannon, Death Beam, Destructo Disk, Big Bang Attack, Spirit Bomb, and God Kamehameha their own final silhouettes.
- Keep shared renderer support so final art can still use charge orbs, stream segments, heads, impact flares, and projectile bodies.
- Re-test terrain impact, beam length, disk steering, and Spirit Bomb charge size after replacing sheets.

Researched but not imported:

- OpenGameArt searches for CC0 energy beams, laser sheets, projectile sheets, and anime-style energy effects found useful references, but nothing was imported in this pass because the best matches were either not Terraria-proportioned, visually inconsistent with the current placeholder style, or required more license/source review before bundling.
- Existing researched aura packs remain possible later reference material, but the current build uses original generated textures to avoid asset uncertainty.

## Sound Effects

Status: imported CC0 temporary sounds.

All imported sounds are routed through `Systems/KiSoundSystem.cs`. If a custom sound cannot be played, the helper falls back to a vanilla Terraria sound instead of scattering fallback behavior across gameplay files. Technique charge start, charge loop, release, sustain, impact, and fizzle hooks now have per-technique mappings even where the current sound is still a temporary shared placeholder.

| File | Source | Author | License | Use |
| --- | --- | --- | --- | --- |
| `Assets/Sounds/PowerUp.ogg` | OpenGameArt `Power-Up Sound Effects` | Spring Spring | CC0 | Power-up start |
| `Assets/Sounds/Charge.wav` | OpenGameArt `Electricity Game Sound Pack` | faxcorp | CC0 | Transformation/heavy charge |
| `Assets/Sounds/ChargeStart.wav` | OpenGameArt `Electricity Game Sound Pack` | faxcorp | CC0 | Kaio-Ken and beam start |
| `Assets/Sounds/ElectricLoop.wav` | OpenGameArt `Electricity Game Sound Pack` | faxcorp | CC0 | Electric/disk/beam impact texture |
| `Assets/Sounds/PowerDown.wav` | OpenGameArt `Electricity Game Sound Pack` | faxcorp | CC0 | Power-down and low-ki fizzle |
| `Assets/Sounds/KiBlast.wav` | OpenGameArt `Electricity Game Sound Pack` | faxcorp | CC0 | Ki blasts and light impacts |
| `Assets/Sounds/HeavyImpact.wav` | OpenGameArt `Electricity Game Sound Pack` | faxcorp | CC0 | Heavy blast impacts |
| `Assets/Sounds/MeleeImpact.wav` | OpenGameArt `Electricity Game Sound Pack` | faxcorp | CC0 | Punch/kick impact |

Source URLs:

- https://opengameart.org/content/power-up-sound-effects
- https://opengameart.org/content/electricity-game-sound-pack

These are not final DBZ-style audio. They are legally usable temporary energy/electric sound effects to move away from pure built-in Terraria placeholders while the final sound direction is developed.

Replacement TODOs:

- Produce or source final original anime-ki-style sounds for each major technique and transformation event.
- Keep all calls routed through `KiSoundSystem` so replacement assets remain centralized.
- Re-test missing-asset fallback behavior after any rename or asset swap.

## Training Station Sprites

Status: original temporary placeholder.

`Items/Training/WoodenWeightBench.png`, `Items/Training/CopperWeightBench.png`, `Items/Training/WoodenTrainingBag.png`, `Items/Training/MeditationMat.png`, `Tiles/WoodenWeightBenchTile.png`, `Tiles/CopperWeightBenchTile.png`, `Tiles/WoodenTrainingBagTile.png`, and `Tiles/MeditationMatTile.png` were generated as original simple pixel placeholders for the first training station passes. The tile sheets are 36x36 to match tModLoader 2x2 tile framing, while the item icons are 32x32.

These are not final furniture sprites. Later training stations should get a cohesive art pass once the station roster settles.

## Technique Item Sprites

Status: original temporary placeholder.

The added technique icons, including `MasenkoSpell.png`, `DeathBeamSpell.png`, and `SpecialBeamCannonSpell.png`, are original simple 28x28 placeholder item sprites generated for this mod. They are visual identifiers for gameplay testing, not final spell art.

## Enemy Sprites

Status: original temporary placeholder.

`NPCs/Enemies/SaibaSprout.png` is an original simple placeholder sprite for the first implemented early enemy. It is meant to prove the spawn/combat/drop loop before a cohesive enemy art pass.

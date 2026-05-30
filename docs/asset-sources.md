# Asset Sources And Status

This file tracks art and audio source status separately from gameplay design docs.

## Hair Sprites

Status: original temporary placeholder.

The current `ModHair` sheets in `Hairs/` were generated for this mod as simple Terraria-format placeholders. They are 40x784 PNGs, matching 14 frames of 40x56 hair data. Each hair has a matching `_Alt.png` file because tModLoader expects alternate hair textures when head equipment changes how hair is drawn.

These are not final art. They are tighter around the top and sides of the player head than the earliest prototype sheets so they should not cover the face as aggressively. True transformation hair animation remains a later art pass.

Researched but not imported:

- OpenGameArt/LPC hair sets, such as `[LPC] Long Straight Hair with 12 Colors`, are useful references but are built for different top-down RPG proportions, not Terraria player hair frames.

## Aura Visuals

Status: original temporary placeholder.

`Assets/Effects/KiAura.png` is an original six-frame white aura sheet generated for this mod and tinted in code per form. It gives the mod a real animated aura foundation without depending on unverified anime assets.

Researched but not imported:

- OpenGameArt `Auras` by Kutejnikov is listed as CC0 and contains animated 512x512 aura renders, but the archive is large and not Terraria-proportioned.
- OpenGameArt `Energy Sprite Sheets` by fzeeshan is listed as CC0 and contains transparent energy effects, but the sheets are much larger than needed for this pass.

## Beam And Projectile Effects

Status: original temporary placeholder.

`Assets/Effects/KiBeamSegment.png`, `KiBeamHead.png`, `KiBeamImpact.png`, `KiChargeOrb.png`, `KiOrbProjectile.png`, and `KiDisk.png` were generated for this mod as small grayscale/white-alpha effect textures. The renderer tints them per technique, so the same legal placeholder sheets can support Kamehameha blue, Galick Gun purple, Final Flash yellow, Special Beam Cannon drill accents, Spirit Bomb white/blue, and Destructo Disk cutting visuals.

These are not final art. They exist to replace plain rectangle beams and generic projectiles with readable gameplay silhouettes while the final projectile art direction is developed.

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

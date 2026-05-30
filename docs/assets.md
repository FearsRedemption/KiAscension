# Asset Sources And Status

This project should not ship ripped Dragon Ball, Dragon Ball Z, Terraria, anime, game, movie, or fan-pack assets unless their license and permission are explicit.

## Hair Sprites

Status: original temporary placeholder.

The current `ModHair` sheets in `Hairs/` were generated for this mod as simple Terraria-format placeholders. They are 40x784 PNGs, matching 14 frames of 40x56 hair data, and each has a matching `_Alt.png` file because tModLoader uses alternate hair textures when head equipment covers hair.

These are not final art. They are intentionally tighter around the top and sides of the head so they do not cover the player's face as aggressively as the first prototype sheets. True transformation hair animation is still planned as a later art pass.

Researched but not imported:

- OpenGameArt/LPC hair sets, such as `[LPC] Long Straight Hair with 12 Colors`, are useful references but are built for different top-down RPG proportions, not Terraria player hair frames.

## Aura Visuals

Status: original temporary placeholder.

`Assets/Effects/KiAura.png` is an original six-frame white aura sheet generated for this mod and tinted in code per form. It is intentionally lightweight and procedural-looking so the mod has an animated aura foundation without depending on unverified anime assets.

Researched but not imported:

- OpenGameArt `Auras` by Kutejnikov is listed as CC0 and contains animated 512x512 aura renders, but the archive is large and not Terraria-proportioned.
- OpenGameArt `Energy Sprite Sheets` by fzeeshan is listed as CC0 and contains transparent energy effects, but the sheets are much larger than needed for this pass.

## Sound Effects

Status: imported CC0 temporary sounds.

All imported sounds are routed through `Systems/KiSoundSystem.cs`. If a custom sound cannot be played, the helper falls back to a vanilla Terraria sound instead of scattering fallback behavior across gameplay files.

Imported files:

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

These are not final DBZ-style audio. They are legally usable temporary energy/electric sound effects to move away from pure built-in Terraria placeholders.

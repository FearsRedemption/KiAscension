# Phase Audit

This audit maps the large phase request to the current repo state. It separates implemented gameplay from placeholder/framework work so future passes do not accidentally treat planned systems as finished.

## Summary

The mod is buildable and now has foundations for every requested phase. Phases 1-8, 11-14, and 19 have concrete implementation. Phases 9-10 and 15-18 have working starter hooks plus planning/framework docs, but they are not full content-complete expansions yet.

## Phase Status

| Phase | Status | Current repo state |
| --- | --- | --- |
| 1. Audit current behavior vs docs | Implemented | `README.md`, `docs/current-behavior.md`, and this audit describe actual behavior and remaining gaps. |
| 2. Charged Saiyan power-up/down | Implemented | `Z`/`X` support tap stepping and held charge/power-down behavior in `KiPlayer`. |
| 3. Separate Kaio-Ken | Implemented | Kaio-Ken is separate from Saiyan forms, has own keybinds, levels, drain, and sync. |
| 4. Core transformation stats/hooks | Implemented | Saiyan forms and Kaio-Ken feed max ki, regen, drain, damage, defense, speed, HP regen, and flight hooks. |
| 5. Ki mana/spell economy | Implemented | Ki costs, beam sustain drain, fizzle behavior, ki-control discounts, and tooltip costs are centralized. |
| 6. Ki UI/stats/dev inspection | Partially implemented | HUD plus read-only stats/dev panels exist. Full themed UI and editable dev tools are planned. |
| 7. Breakthrough behavior | Implemented | Unlocks auto-transform only when the player was already in their previous highest form. |
| 8. Flight progression | Implemented foundation | Super Saiyan+ ki flight exists with form-scaled control and drain. Full flight polish/trails remain planned. |
| 9. Ki skill architecture | Implemented foundation | Techniques carry category, source, collision style, terrain, pierce, tracking, and held-beam metadata. |
| 10. Ki skill roster planning | Implemented as planning | `docs/ki-skills.md` lists implemented examples and future lore-inspired buckets. Full roster is not implemented. |
| 11. Skill feel/terrain/sound/visual behavior | Partially implemented | Beams, disks, barrages, heavy blasts, terrain blocking, terrain-passing ultimates, and impact hooks exist. Advanced charge/cooldown/fatigue remains planned. |
| 12. Sound architecture | Implemented foundation | `KiSoundSystem` centralizes placeholder sounds for transformations, fire, impact, fizzle, and melee. Original sound assets are not present. |
| 13. Aura visuals by form | Implemented foundation | `AscensionAuraProfiles` drives form colors, dust, light, intensity, and electric arcs. Shader-level aura polish remains planned. |
| 14. Hair transformation cleanup | Implemented foundation | Hair profiles are separated from stats, natural hair is restored for base, and tModLoader `_Alt` hair textures are tracked. |
| 15. Training/progression expansion | Partially implemented | Focus, weighted gear, gravity room, training caps, and outgrown messaging exist. Tiered stations/gravity upgrades are planned. |
| 16. Items/accessories/crafting/training furniture | Partially implemented | Starter combat item, training items, `Ki Fragment`, and item framework docs exist. Full accessory/armor/furniture set is planned. |
| 17. Melee punch/kick foundation | Implemented foundation | `Saiyan Strike` gives a starter no-graphic physical strike with scaling and combo hook. Full animations/hitboxes/chains are planned. |
| 18. Future bosses/mobs | Implemented as planning | Placeholder folders and `docs/bosses-and-mobs.md` exist. No custom NPCs are implemented yet. |
| 19. Documentation update | Implemented | Docs were updated to distinguish implemented, partial, placeholder, and planned behavior. |

## Highest Priority Remaining Work

- Replace placeholder sounds with original audio assets through `KiSoundSystem`.
- Expand training into tiered placeable stations and gravity chamber upgrades.
- Build real melee animations/hitboxes for `Saiyan Strike`.
- Add the first original DBZ-inspired enemy or boss and wire its drops into progression.
- Add more high-quality representative ki techniques before expanding the full roster.

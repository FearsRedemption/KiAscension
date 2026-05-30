# Phase Audit

This audit maps the large phase request to the current repo state. It separates implemented gameplay from placeholder/framework work so future passes do not accidentally treat planned systems as finished.

## Summary

The mod is buildable and now has foundations for every requested phase. Phases 1-8, 11-14, and 19 have concrete implementation. Phases 9-10 and 15-17 have working starter hooks plus planning/framework docs, but they are not full content-complete expansions yet. Phase 18 remains planning only.

## Phase Status

| Phase | Status | Current repo state |
| --- | --- | --- |
| 1. Audit current behavior vs docs | Implemented | `README.md`, `docs/current-behavior.md`, and this audit describe actual behavior and remaining gaps. |
| 2. Charged Saiyan power-up/down | Implemented | `Z`/`X` support tap stepping and held charge/power-down behavior in `KiPlayer`. |
| 3. Separate Kaio-Ken | Implemented | Kaio-Ken is separate from Saiyan forms, has own keybinds, levels, drain, and sync. |
| 4. Core transformation stats/hooks | Implemented | Saiyan forms and Kaio-Ken feed max ki, regen, drain, damage, defense, speed, HP regen, and flight hooks. |
| 5. Ki mana/spell economy | Implemented | Ki costs, beam sustain drain, fizzle behavior, ki-control discounts, and tooltip costs are centralized. |
| 6. Ki UI/stats/dev inspection | Implemented foundation | Styled HUD plus read-only stats/dev panels exist, including wrapped long lines and locked-spell reasons. Final UI art and editable dev tools are planned. |
| 7. Breakthrough behavior | Implemented | Unlocks auto-transform only when the player was already in their previous highest form. |
| 8. Flight progression | Implemented foundation | Super Saiyan+ ki flight exists with form-scaled control and drain. Full flight polish/trails remain planned. |
| 9. Ki skill architecture | Implemented foundation | Techniques carry category, source, collision style, terrain, pierce, tracking, and held-beam metadata. |
| 10. Ki skill roster planning | Implemented as planning | `docs/ki-skills.md` lists implemented examples and future lore-inspired buckets. Full roster is not implemented. |
| 11. Skill feel/terrain/sound/visual behavior | Partially implemented | Beams, disks, barrages, heavy blasts, terrain blocking, terrain-passing ultimates, and impact hooks exist. Advanced charge/cooldown/fatigue remains planned. |
| 12. Sound architecture | Implemented foundation | `KiSoundSystem` centralizes temporary CC0 sounds for transformations, fire, impact, fizzle, and melee, with vanilla fallback. Final original audio remains planned. |
| 13. Aura visuals by form | Implemented foundation | `AscensionAuraProfiles` drives form colors, dust, light, intensity, electric arcs, and an animated aura draw layer. Shader-level aura polish remains planned. |
| 14. Hair transformation cleanup | Implemented foundation | Hair profiles are separated from stats, natural hair is saved/restored for base, and tModLoader `_Alt` hair textures are explicit. |
| 15. Training/progression expansion | Partially implemented | Focus, weighted gear, wooden/copper weight benches, gravity room, training caps, and outgrown messaging exist. More station types and gravity upgrades are planned. |
| 16. Items/accessories/crafting/training furniture | Partially implemented | Starter combat item, training items, wooden/copper placeable benches, `Ki Fragment`, and item framework docs exist. Full accessory/armor/furniture set is planned. |
| 17. Melee punch/kick foundation | Implemented foundation | `Saiyan Strike` gives a starter quick punch, heavy punch, rising kick combo with scaling, hit effects, and sound hooks. Full animations, custom hitboxes, charged strikes, and dash strikes are planned. |
| 18. Future bosses/mobs | Planning only | Placeholder folders and `docs/bosses-and-mobs.md` exist. No custom NPCs are implemented yet. |
| 19. Documentation update | Implemented | Docs were updated to distinguish implemented, partial, placeholder, and planned behavior. |

## Highest Priority Remaining Work

- Replace temporary CC0 sounds with final original audio assets through `KiSoundSystem`.
- Expand training beyond the starter wooden/copper benches into more station types and gravity chamber upgrades.
- Build real melee animations, custom hitboxes, charged strikes, and dash strikes for `Saiyan Strike`.
- Add the first original DBZ-inspired enemy or boss and wire its drops into progression.
- Add more high-quality representative ki techniques before expanding the full roster.

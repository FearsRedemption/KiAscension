# Sprite Art Review

This pass reviewed every current PNG in the mod and replaced the roughest DBZ-style placeholders with original Terraria-scale pixel art. No official Dragon Ball, Dragon Ball Z, Dragon Ball Super, or ripped Terraria sprites are bundled.

## Review Criteria

- Readable at 1x inventory or NPC scale.
- Transparent background with clean alpha.
- Strong silhouette and dark outline where appropriate.
- Limited palettes that match Terraria-style item readability.
- DBZ-inspired colors and energy shapes without copying protected artwork.
- Correct tModLoader dimensions for hair, effects, items, tiles, NPCs, and mod icons.
- Matching `_Alt` hair sheets for every `ModHair`.

## Updated Sprite Groups

- Technique icons: all current spell icons were regenerated as 32x32 original sprites with distinct beam, orb, barrage, disk, drill, and ultimate identities.
- Combat/material icons: `Saiyan Strike` and `Ki Fragment` now use custom sprites instead of vanilla fallback textures.
- Hair sheets: all Saiyan/Ultra Instinct hair sheets and `_Alt` sheets were regenerated with clearer silhouettes and face openings.
- Effects: aura, electric arcs, beam segments, beam head, impact flare, charge orb, projectile orb, and disk textures were regenerated as stronger tintable grayscale sheets.
- Training items/tiles: benches, training bag, meditation mat, weighted bands, and gravity core sprites were cleaned up for better object silhouettes.
- Enemy: `Saiba Sprout` was redrawn as an original plant-warrior placeholder with a clearer Terraria-scale body.
- Mod icon: `icon.png` now uses a ki-orb/aura motif instead of the default `MOD` placeholder. The optional `icon_small.png` was removed because it triggered an FNA3D image-load warning during packaging.

## QA Artifacts

Temporary contact sheets were generated during this pass for visual review, then removed from the mod source tree because tModLoader scans PNGs as loadable assets. Future art passes should generate contact sheets outside the mod source folder, or use a non-PNG/non-bundled location, so documentation images do not affect Build + Reload.

## Validation

Automated validation checked:

- Hair sheets are 40x784 RGBA.
- Every hair sheet has a matching `_Alt.png`.
- `_Alt` hair sheets currently match their base sheets exactly.
- Aura sheets are 768x128 RGBA.
- Beam/effect textures keep their expected dimensions.
- Item and tile sprites stay within small Terraria-scale dimensions.
- Mod icon is 80x80.

These sprites are improved placeholders, not final commissioned art. They should be good enough to stop the mod looking like a janky debug prototype, while leaving room for a later dedicated art pass.

# Current Gameplay Behavior

This is the intended behavior of the current prototype when loaded through tModLoader.

## On Install

Ki Ascension is meant to become the main combat progression path, but vanilla and non-ki weapons currently remain usable while the custom melee system grows. The player should understand quickly that the `Ki Training Focus`, EXP, Kai Level, forms, `Saiyan Strike`, and ki techniques are the main long-term progression path.

## Starting A Character

New characters start with `Ki Training Focus`, `Saiyan Strike`, and `Basic Ki Blast`. The focus is now a training tool, not a cycling weapon: using it meditates for a small amount of physical power and ki power. `Saiyan Strike` is a starter punch/kick item that scales with physical power and follows a simple quick punch, heavy punch, rising kick combo. If the focus or starter combat tools are lost, the mod tries to grant them again when entering a world.

Early mobs are scaled up compared to vanilla, but the starter spell, `Saiyan Strike`, normal weapons, and early ki regeneration are tuned so the first slimes and zombies are not a wall.

## Core Loop

1. Fight enemies with separate ki technique items.
2. Gain physical power from combat/training and ki power from ki technique hits.
3. Raise Kai Level as combined power increases.
4. Unlock new technique items and forms when power, ki power, boss gates, and breakthrough requirements are met.
5. Tap `Z` and `X` to step up or down through unlocked Saiyan forms.
6. Hold `Z` to charge up into the highest unlocked Saiyan form; hold `X` to power down to Base Saiyan.
7. Tap or hold `C`/`V` to raise or lower the separate Kaio-Ken level.

The HUD is now a compact DBZ-style panel instead of plain debug text. It shows Kai Level, ki, net ki regeneration after active drains, active Saiyan form, active Kaio-Ken state, held spell, active training station, and the next ceiling. Long gate and spell lines wrap or clip inside the panel so they should not run across the screen. Locked held spells display their locked state and a readable requirement, for example `Held Spell: Spirit Bomb (Locked)` and `Reason: Requires Super Saiyan 3 + 14500 Ki Power`.

Tap `B` to toggle a styled read-only stats panel with current form bonuses, Kaio-Ken strain, final damage/speed multipliers, ki economy, next gates, and held-spell cost details. Tap `N` to toggle a styled read-only dev inspection panel for synced player state while testing. These panels are intentionally still read-only; editable dev tools are planned later.

## Transformations

Saiyan forms and Kaio-Ken are separate systems. Saiyan forms provide the main transformation bonuses: max ki, ki regeneration, damage, defense, movement, light health regeneration, ki drain, and a placeholder flight-control multiplier for later flight work.

Saiyan form ceilings are revalidated against the current world's boss flags. Super Saiyan requires enough total power, Eye of Cthulhu defeated in the current world, and the witness-loss breakthrough. If the player loads the same character into a fresh world where Eye of Cthulhu has not been defeated, Super Saiyan is not available until that world's gate is satisfied. The HUD/dev panel shows the current blocker, such as `Super Saiyan locked: defeat the Eye of Cthulhu`.

Kaio-Ken can run alongside the current Saiyan form. It adds extra damage and movement, but it primarily strains HP/body endurance while adding only light early ki pressure. Releasing Kaio-Ken returns only the Kaio-Ken level to `Off`; it does not change the active Saiyan form.

True ki flight is available only while an active Saiyan form is Super Saiyan or higher. Hold jump/up/down to engage flight movement. Early forms drain more ki and feel heavier, while God and Ultra Instinct tiers are more efficient and responsive. Flight does not replace wings or mounts permanently; it is a transformation movement layer.

## Technique Feel

Techniques are now separate spell items. Basic blasts and barrages behave like fast projectile pressure. Masenko is an early focused impact blast. Kamehameha, Galick Gun, Special Beam Cannon, Final Flash, and God Kamehameha are held beams that drain ki over time. Death Beam is a fast precision shot. Destructo Disk pierces multiple enemies and steers toward the mouse while it flies. Big Bang Attack and Spirit Bomb are heavier projectile attacks.

Every ki technique has an initial ki cost, and beam techniques also have a per-second sustain drain. Current ki power and advanced forms apply a ki-control discount through shared resource math, so training can make lower techniques smoother without making high forms free too early. Spell tooltips show the current effective costs for the local player and explain locked spell requirements. Sustained beams now stop at solid terrain, while techniques marked as terrain-passing, such as Spirit Bomb, can still use special terrain behavior.

Enemy kills now grant modest training based on the last hit source. Ki technique kills grant mostly Ki Power and some Physical Power, Saiyan Strike or melee kills grant mostly Physical Power and some Ki Power, and vanilla weapon kills grant smaller Physical Power. Boss kills give a noticeable training bump, but they do not bypass form gates.

Technique definitions now include metadata for category, source label, collision style, held behavior, terrain behavior, piercing, and cursor tracking. This is the foundation for a larger lore-inspired roster without forcing every technique into custom one-off logic.

Technique fire, impact, beam fizzle, transformations, Kaio-Ken changes, and ki strain now route through a central sound helper. The current build uses temporary CC0 energy/electric sounds where available and falls back to vanilla Terraria sounds through the helper if a custom sound cannot play.

## Training

Training sources now have simple caps so starter tools cannot carry the whole game. `Ki Training Focus` helps early physical power and ki power, `Weighted Training Bands` slow the player while equipped but build physical power while moving, and `Gravity Room Core` is a placeable 2x2 tile that creates a training field nearby for mixed physical/ki training.

The first placeable station set is implemented with `Wooden Weight Bench`, `Copper Weight Bench`, `Wooden Training Bag`, and `Meditation Mat`. Place a station and right-click it to begin a short slow training window. Benches train physical power, the training bag trains striking power plus a little ki control, and the meditation mat trains ki power. If the player has outgrown a station, it says `This equipment can no longer push your limits.` Any multiplayer player can use a placed higher-tier station; progression remains owned by that player, not the station owner.

The training numbers are intentionally small so training helps without replacing boss progression.

Enemies and bosses can drop `Ki Fragment`, a starter material hook for later ascension crafting. The current build does not yet consume fragments in many recipes; it is a framework piece for the next item expansion.

## Multiplayer

Each player has their own saved and synced:

- EXP
- Ki power EXP
- Kai Level
- unlocked form ceiling
- current form
- unlocked Kaio-Ken level
- current Kaio-Ken level
- unlocked technique ceiling
- ki

In multiplayer, the server owns EXP, ki power, training, and unlock progression so clients should not double-award themselves. Clients send selected Saiyan form and Kaio-Ken level to the server; the server clamps those choices to what that player has unlocked and broadcasts the result.

Saved form and Kaio-Ken unlocks are clamped by the current world's boss progression flags when loaded or synced, so a character cannot carry a boss-gated form into a world where that boss gate is still false.

Nearby players can still trigger witness-loss breakthroughs for each other. If one player has enough EXP for a witness-gated form and another player dies close enough, the waiting player should break through.

Automatic breakthrough follows the current-form rule: if the player is already in their highest previously unlocked Saiyan form, unlocking the next form immediately transforms them into it with a stronger aura burst. If they are powered down or in a lower form, the ceiling unlocks but their current form does not jump.

## Forms And Visuals

When the player transforms, the mod stores the player's natural hair once, then applies custom Terraria-style `ModHair` sprites and stage colors:

- Kaio-Ken: separate red aura overlay, no Saiyan hair change
- Awakened State: mostly natural hair with a light tint
- Super Saiyan forms: custom spiky gold hair
- Super Saiyan 3: longer custom gold hair
- Super Saiyan God: custom red-tinted compact hair
- Super Saiyan Blue: custom spiky blue hair
- Ultra Instinct Sign: custom darker/incomplete silver hair
- Ultra Instinct: custom silver/white hair

Powering down to Base restores the stored natural hair style and color. Kaio-Ken does not change hair style; it only adds a red aura layer. Aura behavior is profile-driven per form, with separate dust type, light strength, dust intensity, secondary color, electric-arc settings, and an animated aura draw-layer texture. Hair behavior is also profile-driven so form hair selection stays separate from stat logic.

## Enemies And Boss Direction

The current repo now includes `Saiba Sprout`, an early surface enemy that chases the player and fires a simple hostile ki shot. It gives the early world a small DBZ-flavored combat pressure source instead of keeping mobs entirely theoretical.

The current repo does not add DBZ-flavored bosses yet. The progression is ready for them: future bosses should be original, Dragon Ball-inspired encounters that test the current ascension tier and reward meaningful EXP or breakthrough materials.

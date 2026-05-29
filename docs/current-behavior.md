# Current Gameplay Behavior

This is the intended behavior of the current prototype when loaded through tModLoader.

## On Install

Ki Ascension is meant to replace normal Terraria weapon progression. Vanilla and non-ki weapons still exist for utility, mining, collection, and experimentation, but their combat damage is heavily reduced. The player should understand quickly that the `Ki Training Focus`, EXP, Kai Level, forms, and ki techniques are the main progression path.

## Starting A Character

New characters start with `Ki Training Focus` and `Basic Ki Blast`. The focus is now a training tool, not a cycling weapon: using it meditates for a small amount of physical power and ki power. If it is lost, it can be remade from one Dirt Block as a safety valve.

Early mobs are scaled up heavily compared to vanilla, but the starter spell and slow ki regeneration are tuned so the first slimes and zombies are not a wall.

## Core Loop

1. Fight enemies with separate ki technique items.
2. Gain physical power from combat/training and ki power from ki technique hits.
3. Raise Kai Level as combined power increases.
4. Unlock new technique items and forms when power, ki power, boss gates, and breakthrough requirements are met.
5. Tap `Z` and `X` to step up or down through unlocked Saiyan forms.
6. Hold `Z` to charge up into the highest unlocked Saiyan form; hold `X` to power down to Base Saiyan.
7. Tap or hold `C`/`V` to raise or lower the separate Kaio-Ken level.

The HUD shows Kai Level, physical power, ki power, ki, net ki regeneration after active drains, active Saiyan form, active Kaio-Ken state, held spell, and the next ceiling. Chat messages call out Kai Level increases, new techniques, unlocked forms, boss gates, Kaio-Ken unlocks, and witness-loss gates.

Tap `B` to toggle a read-only stats panel with current form bonuses, Kaio-Ken strain, final damage/speed multipliers, ki economy, next gates, and held-spell cost details. Tap `N` to toggle a read-only dev inspection panel for synced player state while testing.

## Transformations

Saiyan forms and Kaio-Ken are separate systems. Saiyan forms provide the main transformation bonuses: max ki, ki regeneration, damage, defense, movement, light health regeneration, ki drain, and a placeholder flight-control multiplier for later flight work.

Kaio-Ken can run alongside the current Saiyan form. It adds extra damage and movement, but it drains ki and slowly strains HP while active. Releasing Kaio-Ken returns only the Kaio-Ken level to `Off`; it does not change the active Saiyan form.

## Technique Feel

Techniques are now separate spell items. Basic blasts and barrages behave like fast projectile pressure. Kamehameha, Galick Gun, Final Flash, and God Kamehameha are held beams that drain ki over time. Destructo Disk pierces multiple enemies and steers toward the mouse while it flies. Big Bang Attack and Spirit Bomb are heavier projectile attacks.

Every ki technique has an initial ki cost, and beam techniques also have a per-second sustain drain. Current ki power and advanced forms apply a modest ki-control discount through shared resource math, so training can make lower techniques smoother without making high forms free too early. Spell tooltips show the current effective costs for the local player.

## Training

`Weighted Training Bands` slow the player while equipped, but moving and jumping under the extra load builds physical power. `Gravity Room Core` is a placeable 2x2 tile that creates a training field nearby; moving in its radius builds physical power and ki power. The training numbers are intentionally small so training helps without replacing boss progression.

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

Nearby players can still trigger witness-loss breakthroughs for each other. If one player has enough EXP for a witness-gated form and another player dies close enough, the waiting player should break through.

Automatic breakthrough follows the current-form rule: if the player is already in their highest previously unlocked Saiyan form, unlocking the next form immediately transforms them into it with a stronger aura burst. If they are powered down or in a lower form, the ceiling unlocks but their current form does not jump.

## Forms And Visuals

When the player transforms, the mod applies custom Terraria-style `ModHair` sprites and stage colors:

- Kaio-Ken: separate red aura overlay, no Saiyan hair change
- Awakened State: mostly natural hair with a light tint
- Super Saiyan forms: custom spiky gold hair
- Super Saiyan 3: longer custom gold hair
- Super Saiyan God: custom red-tinted compact hair
- Super Saiyan Blue: custom spiky blue hair
- Ultra Instinct: custom silver/white hair

## Boss Direction

The current repo does not add DBZ-flavored bosses yet. The progression is ready for them: future bosses should be original, Dragon Ball-inspired encounters that test the current ascension tier and reward meaningful EXP or breakthrough materials.

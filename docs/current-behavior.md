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
5. Use `Z` and `X` to move up or down through unlocked forms.
6. Hold `Z` to power directly up to the highest unlocked form.

The HUD shows Kai Level, physical power, ki power, ki, ki regeneration, active form, held spell, and the next ceiling. Chat messages call out Kai Level increases, new techniques, unlocked forms, boss gates, and witness-loss gates.

## Technique Feel

Techniques are now separate spell items. Basic blasts and barrages behave like fast projectile pressure. Kamehameha, Galick Gun, Final Flash, and God Kamehameha are held beams that drain ki over time. Destructo Disk pierces multiple enemies and steers toward the mouse while it flies. Big Bang Attack and Spirit Bomb are heavier projectile attacks.

## Training

`Weighted Training Bands` slow the player while equipped, but moving and jumping under the extra load builds physical power. `Gravity Room Core` is a placeable 2x2 tile that creates a training field nearby; moving in its radius builds physical power and ki power. The training numbers are intentionally small so training helps without replacing boss progression.

## Multiplayer

Each player has their own saved and synced:

- EXP
- Ki power EXP
- Kai Level
- unlocked form ceiling
- current form
- unlocked technique ceiling
- ki

In multiplayer, the server owns EXP, ki power, training, and unlock progression so clients should not double-award themselves. Clients send only their selected form to the server; the server clamps that choice to what that player has unlocked and broadcasts the result.

Nearby players can still trigger witness-loss breakthroughs for each other. If one player has enough EXP for a witness-gated form and another player dies close enough, the waiting player should break through.

## Forms And Visuals

When the player transforms, the mod applies custom Terraria-style `ModHair` sprites and stage colors:

- Kaio-Ken: red hair tint
- Super Saiyan forms: custom spiky gold hair
- Super Saiyan 3: longer custom gold hair
- Super Saiyan God: custom red-tinted compact hair
- Super Saiyan Blue: custom spiky blue hair
- Ultra Instinct: custom silver/white hair

## Boss Direction

The current repo does not add DBZ-flavored bosses yet. The progression is ready for them: future bosses should be original, Dragon Ball-inspired encounters that test the current ascension tier and reward meaningful EXP or breakthrough materials.

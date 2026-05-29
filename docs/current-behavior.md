# Current Gameplay Behavior

This is the intended behavior of the current prototype when loaded through tModLoader.

## On Install

Ki Ascension is meant to replace normal Terraria weapon progression. Vanilla and non-ki weapons still exist for utility, mining, collection, and experimentation, but their combat damage is heavily reduced. The player should understand quickly that the `Ki Training Focus`, EXP, Kai Level, forms, and ki techniques are the main progression path.

## Starting A Character

New characters start with `Ki Training Focus`. It fires Basic Ki Blast at a low ki cost, and the starter ki pool was raised so early enemies are not a wall. If the item is lost, it can be remade from one Dirt Block as a safety valve.

Early mobs are scaled up only lightly. They should take more attention than vanilla, but the first slimes and zombies should still be killable with Basic Ki Blast.

## Core Loop

1. Fight enemies with ki techniques.
2. Gain EXP from hits and kills.
3. Raise Kai Level as total EXP increases.
4. Unlock new techniques and forms when EXP and form requirements are met.
5. Use `Z` and `X` to move up or down through unlocked forms.
6. Use `C` and `V` to cycle unlocked ki techniques.

The HUD shows Kai Level, EXP, ki, active form, active technique, and the next ceiling. Chat messages also call out Kai Level increases, new techniques, unlocked forms, and witness-loss gates.

## Multiplayer

Each player has their own saved and synced:

- EXP
- Kai Level
- unlocked form ceiling
- current form
- unlocked technique ceiling
- selected technique
- ki

In multiplayer, the server owns EXP and unlock progression so clients should not double-award themselves. Clients send only their selected form and selected technique to the server; the server clamps those choices to what that player has unlocked and broadcasts the result.

Nearby players can still trigger witness-loss breakthroughs for each other. If one player has enough EXP for a witness-gated form and another player dies close enough, the waiting player should break through.

## Forms And Visuals

When the player transforms, the mod applies a Terraria-style hair color and available vanilla hairstyle intended to suggest that form:

- Kaio-Ken: red hair tint
- Super Saiyan forms: gold hair tint and spikier/longer style picks
- Super Saiyan God: red hair tint
- Super Saiyan Blue: blue hair tint
- Ultra Instinct: silver/white tint

These are runtime visuals. Proper custom `ModHair` sprites are a later art pass.

## Boss Direction

The current repo does not add DBZ-flavored bosses yet. The progression is ready for them: future bosses should be original, Dragon Ball-inspired encounters that test the current ascension tier and reward meaningful EXP or breakthrough materials.

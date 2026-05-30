# Progression Design

The design goal is lore-inspired escalation without letting the player skip Terraria's exploration, boss checks, and gear chase. Forms should feel like training milestones, not a replacement for playing the game.

## Core Loops

- Fight enemies to gain physical power and ki power.
- Use `Saiyan Strike` for starter punch/kick combat while physical power grows.
- Use separate ki spell items for attacks; use the `Ki Training Focus` for light meditation training.
- Scale all combat around higher enemy health and damage, so training matters.
- Unlock low forms through practice, then lock Super Saiyan-style jumps behind Terraria boss milestones.
- Unlock major Super Saiyan-style breakthroughs by witnessing a nearby town NPC or player death after meeting the experience threshold.
- Tap hotkeys to step up or down through Saiyan forms and Kaio-Ken levels.
- Hold `Z`/`X` to charge into the highest unlocked Saiyan form or power down to base.
- Hold `C`/`V` to charge Kaio-Ken to the highest unlocked level or release it fully.
- Trigger a dramatic automatic breakthrough only when already fighting at the highest previously unlocked Saiyan form.

## Stage Table

| Stage | Total power | Gate | Terraria pacing |
| --- | ---: | --- | --- |
| Base Saiyan | 0 | Starting state | New character |
| Awakened State | 500 | Training | Early surface/cavern |
| Super Saiyan | 3000 | Eye of Cthulhu plus witness loss | Early boss breakthrough |
| Super Saiyan 2 | 6500 | Eater of Worlds or Brain of Cthulhu plus witness loss | Evil boss tier |
| Super Saiyan 3 | 12000 | Skeletron | Dungeon/pre-hardmode mastery |
| Super Saiyan God | 22000 | Wall of Flesh | Hardmode entry |
| Super Saiyan Blue | 34000 | Any mechanical boss | Mechanical boss tier |
| Ultra Instinct Sign | 50000 | Plantera plus witness loss | Jungle temple lead-in |
| Ultra Instinct | 72000 | Moon Lord | Endgame |

## Kaio-Ken Table

Kaio-Ken is now parallel to Saiyan forms. Its display names keep the anime fantasy, but the actual stat multipliers are softened for Terraria balance. It can stack with Saiyan forms, but drains ki and HP while active.

| Kaio-Ken level | Total power | Gate | Role |
| --- | ---: | --- | --- |
| Off | 0 | Starting state | No strain |
| Kaio-Ken | 700 | Training | First risky burst |
| Kaio-Ken x2 | 1100 | Training | Early combat amplifier |
| Kaio-Ken x3 | 1800 | Eye of Cthulhu | Pre-boss to early boss tier |
| Kaio-Ken x5 | 3400 | Eater of Worlds or Brain of Cthulhu | Evil boss tier |
| Kaio-Ken x10 | 6500 | Skeletron | Late pre-hardmode |
| Kaio-Ken x20 | 12000 | Wall of Flesh | Early hardmode |
| Kaio-Ken x50 | 24000 | Any mechanical boss | Mechanical tier fantasy |
| Kaio-Ken x100 | 42000 | Plantera | Late hardmode strain |
| Kaio-Ken x200 | 72000 | Moon Lord | Endgame burst fantasy |

## Technique Order

The technique ladder is ordered by a mix of lore chronology and Terraria combat pacing. Big Bang Attack intentionally comes before Final Flash, and Destructo Disk comes before the heavier Vegeta-style finishers.

| Technique | Ki power | Form gate | Feel |
| --- | ---: | --- | --- |
| Basic Ki Blast | 0 | Base Saiyan | Starter projectile pressure |
| Ki Barrage | 260 | Base Saiyan | Multiple quick blasts |
| Kamehameha | 800 | Awakened State | Held blue beam that drains ki |
| Destructo Disk | 1600 | Awakened State | Guided piercing disk |
| Galick Gun | 2200 | Awakened State | Held purple rival beam |
| Big Bang Attack | 3600 | Super Saiyan | Heavy compact burst |
| Final Flash | 7600 | Super Saiyan 2 | Expensive long golden beam |
| Spirit Bomb | 14500 | Super Saiyan 3 | Slow boss-scale projectile |
| God Kamehameha | 26000 | Super Saiyan God | God-ki held beam |
| Ultra Instinct Barrage | 54000 | Ultra Instinct Sign | Endgame rapid instinct pressure |

## Power Types

Physical power is earned from combat and weight training. Ki power is earned from ki technique hits, meditation, and gravity room training. Kai Level is based on combined power, while ki technique unlocks care about ki power specifically.

Max ki and ki regeneration both rise with Kai Level, ki power, and later forms. Early regeneration is intentionally slow so the starter loop feels closer to weak Terraria gear, then gets smoother as training pays off.

Effective ki costs use a shared resource formula. Ki power grants a modest control discount, and God/Ultra Instinct-style forms add a small efficiency bonus. The discount is capped so strong attacks and high transformation drains remain meaningful.

## Flight Progression

True ki flight starts at Super Saiyan. It is active only while holding jump/up/down in an eligible form, and it consumes ki while active. Later forms increase responsiveness and efficiency so flight grows from a difficult burst tool into an endgame movement identity.

| Stage | Flight behavior |
| --- | --- |
| Base Saiyan | No true flight. |
| Awakened State | Movement boost only; true flight is still locked. |
| Super Saiyan | First true flight, powerful but ki hungry. |
| Super Saiyan 2 | Sharper control with more strain. |
| Super Saiyan 3 | Strong lift and speed, high ki drain. |
| Super Saiyan God | Better efficiency and calmer control. |
| Super Saiyan Blue | Fast, controlled, still expensive. |
| Ultra Instinct Sign | Highly responsive evasive flight. |
| Ultra Instinct | Efficient endgame flight. |

## Aura Progression

Each Saiyan form has a profile for primary color, secondary color, light strength, dust type, dust intensity, and electric arcs. Kaio-Ken is not a Saiyan hair/form profile; it remains a red overlay and HP-strain state that can stack on top of the active Saiyan form.

Super Saiyan 2 and higher high-energy forms can emit electric arcs. God forms use cleaner red/blue aura profiles, while Ultra Instinct profiles lean silver-white and more controlled.

## Training Rooms

The current training pass has three capped sources:

- `Ki Training Focus`: light starter meditation for early physical power and ki control.
- `Weighted Training Bands`: accessory-based weight training for physical power.
- `Gravity Room Core`: placeable training tile that rewards moving nearby with physical power and ki power.

Caps are intentionally conservative. If a source stops helping, the player needs harder training, higher-tier future equipment, boss progression, or a gravity-room upgrade.

## Starter Melee

`Saiyan Strike` is the current melee foundation. It is a no-graphic punch/kick item, scales with physical power, and tracks a simple three-step combo counter for future animation and hitbox work. It is not a full combo system yet.

## Materials

`Ki Fragment` is a starter material hook dropped by enemies and bosses. It gives future crafting work a place to attach technique upgrades, accessories, training equipment, and boss materials without inventing the material layer later.

## Branches To Add

Ultra Ego and Legendary Wrath should become late-game branches rather than simple linear upgrades.

- Ultra Instinct: dodge, speed, reduced contact damage, precise ki usage.
- Ultra Ego: higher damage after taking hits, risk/reward endurance.
- Legendary Wrath: huge ki pool, high damage, harder ki control, aggression rewards.

## Witness-Loss Gate

The current prototype triggers a pending breakthrough if these are true:

- The player has enough total power for the next Saiyan form.
- The next form requires witness loss.
- A town/friendly NPC or another player dies within 1200 pixels.

This is intentionally dramatic but still Terraria-friendly. Later versions can make the moment cleaner with a custom event, a mentor NPC, or a consent-safe multiplayer toggle.

When a Saiyan form unlocks, the current form changes only if the player was already in their highest previously unlocked form. For example, a player in Super Saiyan who meets every Super Saiyan 2 requirement will break through into Super Saiyan 2. A player who meets the same requirements while in Base Saiyan unlocks the Super Saiyan 2 ceiling but stays in Base until they power up manually.

## Balance Notes

- Saiyan forms multiply final NPC hit damage, increase movement, add defense, add max ki, improve ki regeneration, and add light health regeneration.
- Kaio-Ken adds a parallel damage/speed multiplier, but drains ki and HP while active.
- Strong forms drain ki so the best play is charging up for bursts and powering down to recover.
- Enemy scaling starts immediately and rises with boss progression.
- Boss rewards are intentionally high because boss fights should feel like training arcs.

# Progression Design

The design goal is lore-inspired escalation without letting the player skip Terraria's exploration, boss checks, and gear chase. Forms should feel like training milestones, not a replacement for playing the game.

## Core Loops

- Fight enemies to gain physical power and ki power.
- Use separate ki spell items for attacks; use the `Ki Training Focus` for light meditation training.
- Scale all combat around higher enemy health and damage, so training matters.
- Unlock low forms through practice, then lock Super Saiyan-style jumps behind Terraria boss milestones.
- Unlock major Super Saiyan-style breakthroughs by witnessing a nearby town NPC or player death after meeting the experience threshold.
- Use hotkeys to move up or down through forms as ki allows.
- Hold `Z` to jump directly to the highest unlocked form.

## Stage Table

| Stage | Total power | Gate | Terraria pacing |
| --- | ---: | --- | --- |
| Base Saiyan | 0 | Starting state | New character |
| Awakened State | 500 | Training | Early surface/cavern |
| Kaio-Ken | 1400 | Training, drains ki | Pre-boss pressure tool |
| Super Saiyan | 3000 | Eye of Cthulhu plus witness loss | Early boss breakthrough |
| Super Saiyan 2 | 6500 | Eater of Worlds or Brain of Cthulhu plus witness loss | Evil boss tier |
| Super Saiyan 3 | 12000 | Skeletron | Dungeon/pre-hardmode mastery |
| Super Saiyan God | 22000 | Wall of Flesh | Hardmode entry |
| Super Saiyan Blue | 34000 | Any mechanical boss | Mechanical boss tier |
| Ultra Instinct Sign | 50000 | Plantera plus witness loss | Jungle temple lead-in |
| Ultra Instinct | 72000 | Moon Lord | Endgame |

## Technique Order

The technique ladder is ordered by a mix of lore chronology and Terraria combat pacing. Big Bang Attack intentionally comes before Final Flash, and Destructo Disk comes before the heavier Vegeta-style finishers.

| Technique | Ki power | Form gate | Feel |
| --- | ---: | --- | --- |
| Basic Ki Blast | 0 | Base Saiyan | Starter projectile pressure |
| Ki Barrage | 260 | Base Saiyan | Multiple quick blasts |
| Kamehameha | 800 | Awakened State | Held blue beam that drains ki |
| Destructo Disk | 1600 | Kaio-Ken | Guided piercing disk |
| Galick Gun | 2200 | Kaio-Ken | Held purple rival beam |
| Big Bang Attack | 3600 | Super Saiyan | Heavy compact burst |
| Final Flash | 7600 | Super Saiyan 2 | Expensive long golden beam |
| Spirit Bomb | 14500 | Super Saiyan 3 | Slow boss-scale projectile |
| God Kamehameha | 26000 | Super Saiyan God | God-ki held beam |
| Ultra Instinct Barrage | 54000 | Ultra Instinct Sign | Endgame rapid instinct pressure |

## Power Types

Physical power is earned from combat and weight training. Ki power is earned from ki technique hits, meditation, and gravity room training. Kai Level is based on combined power, while ki technique unlocks care about ki power specifically.

Max ki and ki regeneration both rise with Kai Level, ki power, and later forms. Early regeneration is intentionally slow so the starter loop feels closer to weak Terraria gear, then gets smoother as training pays off.

## Training Rooms

The current training pass has two pieces:

- `Weighted Training Bands`: accessory-based weight training for physical power.
- `Gravity Room Core`: placeable training tile that rewards moving nearby with physical power and ki power.

## Branches To Add

Ultra Ego and Legendary Wrath should become late-game branches rather than simple linear upgrades.

- Ultra Instinct: dodge, speed, reduced contact damage, precise ki usage.
- Ultra Ego: higher damage after taking hits, risk/reward endurance.
- Legendary Wrath: huge ki pool, high damage, harder ki control, aggression rewards.

## Witness-Loss Gate

The current prototype triggers a pending breakthrough if these are true:

- The player has enough power experience for the next form.
- The next form requires witness loss.
- A town/friendly NPC or another player dies within 1200 pixels.

This is intentionally dramatic but still Terraria-friendly. Later versions can make the moment cleaner with a custom event, a mentor NPC, or a consent-safe multiplayer toggle.

## Balance Notes

- Forms multiply final NPC hit damage, increase movement, add defense, and add max ki.
- Strong forms drain ki so the best play is cycling up for bursts and powering down to recover.
- Enemy scaling starts immediately and rises with boss progression.
- Boss rewards are intentionally high because boss fights should feel like training arcs.

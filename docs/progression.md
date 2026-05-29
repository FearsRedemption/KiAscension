# Progression Design

The design goal is lore-inspired escalation without letting the player skip Terraria's exploration, boss checks, and gear chase. Forms should feel like training milestones, not a replacement for playing the game.

## Core Loops

- Fight enemies to gain power experience.
- Use the `Ki Training Focus` early for basic ranged ki pressure.
- Scale all combat around higher enemy health and damage, so training matters.
- Unlock low forms through practice.
- Unlock major Super Saiyan-style breakthroughs by witnessing a nearby town NPC or player death after meeting the experience threshold.
- Use hotkeys to move up or down through forms as ki allows.

## Stage Table

| Stage | XP | Gate | Terraria pacing |
| --- | ---: | --- | --- |
| Base Saiyan | 0 | Starting state | New character |
| Awakened State | 150 | Training | Early surface/cavern |
| Kaio-Ken | 450 | Training, drains ki | Pre-boss pressure tool |
| Super Saiyan | 1000 | Witness loss | Around Eye/Eater/Brain/Skeletron |
| Super Saiyan 2 | 2200 | Witness loss | Late pre-hardmode into early hardmode |
| Super Saiyan 3 | 4200 | Training, heavy drain | Mechanical boss tier |
| Super Saiyan God | 7000 | Future ritual/trainer gate | Plantera/Golem tier |
| Super Saiyan Blue | 10000 | God form mastery | Cultist/Pillars |
| Ultra Instinct Sign | 14000 | Witness loss | Moon Lord and post-Moon Lord |
| Ultra Instinct | 19000 | Mastery | Endgame |

## Technique Order

The technique ladder is ordered by a mix of lore chronology and Terraria combat pacing. Big Bang Attack intentionally comes before Final Flash, and Destructo Disk comes before the heavier Vegeta-style finishers.

| Technique | XP | Form gate | Why it sits here |
| --- | ---: | --- | --- |
| Basic Ki Blast | 0 | Base Saiyan | Starter projectile and training tool |
| Ki Barrage | 120 | Base Saiyan | Faster fundamentals before true signature attacks |
| Kamehameha | 300 | Awakened State | First controlled signature beam |
| Destructo Disk | 650 | Kaio-Ken | Early precision/cutting skill, strong but not a late finisher |
| Galick Gun | 900 | Kaio-Ken | Rival beam before the Super Saiyan finisher tier |
| Big Bang Attack | 1400 | Super Saiyan | First major Super Saiyan finisher |
| Final Flash | 2600 | Super Saiyan 2 | Bigger late-Cell-saga-style beam, after Big Bang |
| Spirit Bomb | 4600 | Super Saiyan 3 | Lore-early technique moved later because it plays like a boss nuke |
| God Kamehameha | 7600 | Super Saiyan God | God-ki signature beam upgrade |
| Ultra Instinct Barrage | 14000 | Ultra Instinct Sign | Endgame rapid instinct pressure |

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

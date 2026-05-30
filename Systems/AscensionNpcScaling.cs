using System;
using KiAscension.Items.Materials;
using KiAscension.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Systems;

public class AscensionNpcScaling : GlobalNPC
{
    private const int RewardRange = 2400;
    private const int WitnessRange = 1200;

    public override void SetDefaults(NPC npc)
    {
        if (npc.townNPC || npc.friendly || npc.lifeMax <= 5 || npc.damage <= 0)
        {
            return;
        }

        float multiplier = GetWorldScale(npc);
        npc.lifeMax = Math.Max(npc.lifeMax, (int)(npc.lifeMax * multiplier));
        npc.damage = Math.Max(npc.damage, (int)(npc.damage * (1f + (multiplier - 1f) * 0.65f)));
        npc.defense = Math.Max(npc.defense, (int)(npc.defense * (1f + (multiplier - 1f) * 0.35f)));
    }

    public override void OnKill(NPC npc)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            return;
        }

        if (npc.townNPC || npc.friendly)
        {
            NotifyWitnesses(npc.Center, "NPC");
            return;
        }

        if (npc.lifeMax <= 5 || npc.value <= 0f)
        {
            return;
        }

        int reward = CalculateReward(npc);
        TryDropKiFragment(npc);

        for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
        {
            Player player = Main.player[playerIndex];

            if (player is null || !player.active || player.dead)
            {
                continue;
            }

            if (Vector2.DistanceSquared(player.Center, npc.Center) > RewardRange * RewardRange)
            {
                continue;
            }

            KiPlayer kiPlayer = player.GetModPlayer<KiPlayer>();
            kiPlayer.AddPowerExperience(reward, npc.boss);
            kiPlayer.AddKiExperience(Math.Max(1, reward / 2), false);
        }
    }

    private static void TryDropKiFragment(NPC npc)
    {
        int dropChance = npc.boss ? 1 : 9;

        if (!npc.boss && !Main.rand.NextBool(dropChance))
        {
            return;
        }

        int stack = npc.boss ? Math.Clamp(npc.lifeMax / 900, 3, 14) : 1;
        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<KiFragment>(), stack);
    }

    private static float GetWorldScale(NPC npc)
    {
        float scale = 2f;

        if (NPC.downedBoss1)
        {
            scale += 0.15f;
        }

        if (NPC.downedBoss2)
        {
            scale += 0.15f;
        }

        if (NPC.downedBoss3)
        {
            scale += 0.2f;
        }

        if (Main.hardMode)
        {
            scale += 0.45f;
        }

        if (NPC.downedMechBossAny)
        {
            scale += 0.25f;
        }

        if (NPC.downedPlantBoss)
        {
            scale += 0.3f;
        }

        if (NPC.downedMoonlord)
        {
            scale += 0.35f;
        }

        if (npc.boss)
        {
            scale += 0.65f;
        }

        return scale;
    }

    private static int CalculateReward(NPC npc)
    {
        int reward = Math.Max(10, npc.lifeMax / 12);

        if (npc.boss)
        {
            reward = Math.Max(300, reward * 8);
        }
        else if (npc.damage > 80)
        {
            reward *= 2;
        }

        return Math.Clamp(reward, 10, npc.boss ? 1800 : 220);
    }

    private static void NotifyWitnesses(Vector2 lossPosition, string source)
    {
        for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
        {
            Player player = Main.player[playerIndex];

            if (player is null || !player.active || player.dead)
            {
                continue;
            }

            if (Vector2.DistanceSquared(player.Center, lossPosition) > WitnessRange * WitnessRange)
            {
                continue;
            }

            player.GetModPlayer<KiPlayer>().WitnessLoss(lossPosition, source);
        }
    }
}

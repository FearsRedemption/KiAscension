using System;
using KiAscension.Items.Combat;
using KiAscension.Items.Materials;
using KiAscension.Items.Techniques;
using KiAscension.Players;
using KiAscension.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Systems;

public class AscensionNpcScaling : GlobalNPC
{
    private const int RewardRange = 2400;
    private const int WitnessRange = 1200;
    private KillTrainingSource lastTrainingSource = KillTrainingSource.Environment;
    private int lastPlayerHitIndex = -1;

    public override bool InstancePerEntity => true;

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
        GetTrainingRewardSplit(npc, reward, out int physicalReward, out int kiReward);

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
            bool primaryKiller = playerIndex == lastPlayerHitIndex || lastPlayerHitIndex < 0 || npc.boss;
            int playerPhysicalReward = primaryKiller ? physicalReward : Math.Max(0, physicalReward / 2);
            int playerKiReward = primaryKiller ? kiReward : Math.Max(0, kiReward / 2);
            kiPlayer.AddKillTrainingExperience(playerPhysicalReward, playerKiReward, npc.boss && primaryKiller, GetRewardSourceLabel());
        }
    }

    public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        if (player is null || !player.active || damageDone <= 0)
        {
            return;
        }

        lastPlayerHitIndex = player.whoAmI;
        lastTrainingSource = item.ModItem is SaiyanStrike
            ? KillTrainingSource.SaiyanStrike
            : item.ModItem is KiTechniqueItem
                ? KillTrainingSource.KiTechnique
                : IsMeleeDamage(item.DamageType)
                    ? KillTrainingSource.Melee
                    : KillTrainingSource.VanillaWeapon;
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (damageDone <= 0 || projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
        {
            return;
        }

        Player player = Main.player[projectile.owner];

        if (player is null || !player.active)
        {
            return;
        }

        lastPlayerHitIndex = player.whoAmI;
        lastTrainingSource = projectile.ModProjectile is KiTechniqueProjectile
            ? KillTrainingSource.KiTechnique
            : IsMeleeDamage(projectile.DamageType)
                ? KillTrainingSource.Melee
                : KillTrainingSource.VanillaWeapon;
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
        if (!npc.boss && npc.lifeMax < 25 && npc.value <= 0f)
        {
            return 0;
        }

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

    private void GetTrainingRewardSplit(NPC npc, int reward, out int physicalReward, out int kiReward)
    {
        physicalReward = 0;
        kiReward = 0;

        if (reward <= 0)
        {
            return;
        }

        switch (lastTrainingSource)
        {
            case KillTrainingSource.KiTechnique:
                kiReward = reward;
                physicalReward = Math.Max(1, reward / 5);
                break;
            case KillTrainingSource.SaiyanStrike:
            case KillTrainingSource.Melee:
                physicalReward = reward;
                kiReward = Math.Max(1, reward / 5);
                break;
            case KillTrainingSource.VanillaWeapon:
                physicalReward = Math.Max(1, reward / 3);
                break;
            default:
                physicalReward = Math.Max(1, reward / 4);
                kiReward = Math.Max(1, reward / 4);
                break;
        }

        if (npc.boss)
        {
            physicalReward += lastTrainingSource == KillTrainingSource.KiTechnique ? reward / 4 : reward / 2;
            kiReward += lastTrainingSource is KillTrainingSource.SaiyanStrike or KillTrainingSource.Melee ? reward / 4 : reward / 2;
        }
    }

    private string GetRewardSourceLabel()
    {
        return lastTrainingSource switch
        {
            KillTrainingSource.KiTechnique => "ki kill",
            KillTrainingSource.SaiyanStrike => "strike kill",
            KillTrainingSource.Melee => "melee kill",
            KillTrainingSource.VanillaWeapon => "weapon kill",
            _ => "combat kill"
        };
    }

    private static bool IsMeleeDamage(DamageClass damageClass)
    {
        return damageClass == DamageClass.Melee || damageClass.CountsAsClass(DamageClass.Melee);
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

    private enum KillTrainingSource
    {
        Environment,
        VanillaWeapon,
        Melee,
        SaiyanStrike,
        KiTechnique
    }
}

using System;
using System.Collections.Generic;
using KiAscension.Common;
using KiAscension.Players;
using KiAscension.Projectiles;
using KiAscension.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Techniques;

public abstract class KiTechniqueItem : ModItem
{
    protected abstract KiTechnique Technique { get; }

    protected KiTechniqueDefinition Definition => KiTechniques.Get((int)Technique);

    public KiTechniqueDefinition TechniqueDefinition => Definition;

    public override void SetDefaults()
    {
        KiTechniqueDefinition technique = Definition;
        Item.damage = technique.BaseDamage;
        Item.DamageType = DamageClass.Generic;
        Item.width = 28;
        Item.height = 28;
        Item.useTime = technique.UseTime;
        Item.useAnimation = technique.UseTime;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = technique.Knockback;
        Item.value = Item.buyPrice(silver: 1);
        Item.rare = GetRarity(technique.RequiredStage);
        Item.UseSound = null;
        Item.autoReuse = technique.Behavior != KiTechniqueBehavior.Beam && technique.Technique != KiTechnique.SpiritBomb;
        Item.channel = technique.Behavior == KiTechniqueBehavior.Beam || technique.Technique == KiTechnique.SpiritBomb;
        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<KiTechniqueProjectile>();
        Item.shootSpeed = technique.ShootSpeed;
    }

    public override bool CanUseItem(Player player)
    {
        KiPlayer kiPlayer = player.GetModPlayer<KiPlayer>();
        KiTechniqueDefinition technique = Definition;

        if (!kiPlayer.IsTechniqueUnlocked(technique))
        {
            return false;
        }

        if (!kiPlayer.HasKiForTechnique(technique))
        {
            return false;
        }

        return (technique.Behavior != KiTechniqueBehavior.Beam && technique.Technique != KiTechnique.SpiritBomb)
            || !HasOwnedTechniqueProjectile(player, technique.Technique);
    }

    public override float UseSpeedMultiplier(Player player)
    {
        KiTechniqueDefinition technique = Definition;
        return Item.useTime / (float)Math.Max(1, technique.UseTime);
    }

    public override void ModifyShootStats(
        Player player,
        ref Vector2 position,
        ref Vector2 velocity,
        ref int type,
        ref int damage,
        ref float knockback)
    {
        KiTechniqueDefinition technique = Definition;

        if (velocity.LengthSquared() > 0f)
        {
            velocity = Vector2.Normalize(velocity) * technique.ShootSpeed;
        }

        type = ModContent.ProjectileType<KiTechniqueProjectile>();
        damage = player.GetModPlayer<KiPlayer>().GetKiTechniqueDamage(technique);
        knockback = technique.Knockback;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        KiPlayer kiPlayer = Main.LocalPlayer.GetModPlayer<KiPlayer>();
        KiTechniqueDefinition technique = Definition;
        int initialCost = kiPlayer.GetTechniqueInitialKiCost(technique);

        string text = technique.Behavior == KiTechniqueBehavior.Beam || technique.Technique == KiTechnique.SpiritBomb
            ? $"Ki cost: {initialCost} start, {kiPlayer.GetTechniqueChannelKiCostPerSecond(technique)}/s while held"
            : $"Ki cost: {initialCost}";

        tooltips.Add(new TooltipLine(Mod, "KiAscensionKiCost", text));
        tooltips.Add(new TooltipLine(Mod, "KiAscensionTechniqueType", $"Type: {technique.CategoryLabel} | {technique.SourceLabel}"));
        tooltips.Add(new TooltipLine(Mod, "KiAscensionTechniqueCollision", $"Behavior: {technique.CollisionLabel}"));

        if (!kiPlayer.IsTechniqueUnlocked(technique))
        {
            tooltips.Add(new TooltipLine(Mod, "KiAscensionTechniqueLocked", $"Locked: {kiPlayer.GetTechniqueLockReason(technique)}")
            {
                OverrideColor = new Color(255, 150, 130)
            });
        }
        else if (!kiPlayer.HasKiForTechnique(technique))
        {
            tooltips.Add(new TooltipLine(Mod, "KiAscensionTechniqueLowKi", "Not enough ki to cast yet.")
            {
                OverrideColor = new Color(255, 190, 120)
            });
        }
    }

    public override bool Shoot(
        Player player,
        EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback)
    {
        KiPlayer kiPlayer = player.GetModPlayer<KiPlayer>();
        KiTechniqueDefinition technique = Definition;

        if (!kiPlayer.TryConsumeTechniqueInitialKi(technique))
        {
            return false;
        }

        if (technique.Behavior == KiTechniqueBehavior.Beam || technique.Technique == KiTechnique.SpiritBomb)
        {
            KiSoundSystem.PlayTechniqueChargeStart(position, technique);
        }
        else
        {
            KiSoundSystem.PlayTechniqueFire(position, technique);
        }

        if (technique.Behavior == KiTechniqueBehavior.Barrage)
        {
            ShootBarrage(player, source, position, velocity, type, damage, knockback, technique);
            return false;
        }

        float extraAi = technique.Technique == KiTechnique.SpiritBomb ? velocity.ToRotation() : 0f;
        SpawnTechniqueProjectile(player, source, position, velocity, type, damage, knockback, technique, extraAi);
        return false;
    }

    protected virtual void AddTechniqueRecipe(Recipe recipe)
    {
    }

    public override void AddRecipes()
    {
        if (Technique == KiTechnique.BasicKiBlast)
        {
            return;
        }

        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.FallenStar, Math.Max(1, (int)Definition.RequiredStage + 1));
        recipe.AddTile(TileID.WorkBenches);
        AddTechniqueRecipe(recipe);
        recipe.Register();
    }

    private static void ShootBarrage(
        Player player,
        IEntitySource source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback,
        KiTechniqueDefinition technique)
    {
        int shotCount = technique.Technique == KiTechnique.UltraInstinctBarrage ? 5 : 3;
        float spread = technique.Technique == KiTechnique.UltraInstinctBarrage ? 0.26f : 0.18f;

        for (int i = 0; i < shotCount; i++)
        {
            float offset = shotCount == 1 ? 0f : MathHelper.Lerp(-spread, spread, i / (float)(shotCount - 1));
            SpawnTechniqueProjectile(player, source, position, velocity.RotatedBy(offset), type, damage, knockback, technique, i);
        }
    }

    private static void SpawnTechniqueProjectile(
        Player player,
        IEntitySource source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback,
        KiTechniqueDefinition technique,
        float extraAi)
    {
        int projectileIndex = Projectile.NewProjectile(
            source,
            position,
            velocity,
            type,
            damage,
            knockback,
            player.whoAmI,
            (float)technique.Technique,
            extraAi);

        if (projectileIndex < 0 || projectileIndex >= Main.maxProjectiles)
        {
            return;
        }

        Projectile projectile = Main.projectile[projectileIndex];
        projectile.penetrate = technique.Penetration;
        projectile.timeLeft = technique.TimeLeft;
        projectile.scale = technique.ProjectileScale;
        projectile.width = Math.Max(8, (int)(14 * technique.ProjectileScale));
        projectile.height = Math.Max(8, (int)(14 * technique.ProjectileScale));

        if (technique.Behavior == KiTechniqueBehavior.Beam)
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
        }

        if (technique.Technique == KiTechnique.SpiritBomb)
        {
            projectile.tileCollide = false;
            projectile.penetrate = 3;
            projectile.width = 28;
            projectile.height = 28;
        }

        if (technique.IgnoresTerrain)
        {
            projectile.tileCollide = false;
        }
    }

    private bool HasOwnedTechniqueProjectile(Player player, KiTechnique technique)
    {
        int projectileType = ModContent.ProjectileType<KiTechniqueProjectile>();

        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile projectile = Main.projectile[i];

            if (!projectile.active || projectile.owner != player.whoAmI || projectile.type != projectileType)
            {
                continue;
            }

            if ((KiTechnique)(int)projectile.ai[0] == technique)
            {
                return true;
            }
        }

        return false;
    }

    private static int GetRarity(AscensionStage stage)
    {
        return stage switch
        {
            AscensionStage.Base => ItemRarityID.White,
            AscensionStage.Awakened => ItemRarityID.Blue,
            AscensionStage.SuperSaiyan => ItemRarityID.Orange,
            AscensionStage.SuperSaiyan2 => ItemRarityID.LightRed,
            AscensionStage.SuperSaiyan3 => ItemRarityID.Pink,
            AscensionStage.SuperSaiyanGod => ItemRarityID.LightPurple,
            AscensionStage.SuperSaiyanBlue => ItemRarityID.Lime,
            _ => ItemRarityID.Cyan
        };
    }
}

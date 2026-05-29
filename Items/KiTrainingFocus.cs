using System;
using KiAscension.Common;
using KiAscension.Players;
using KiAscension.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items;

public class KiTrainingFocus : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.FallenStar}";

    public override void SetDefaults()
    {
        Item.damage = 12;
        Item.DamageType = DamageClass.Generic;
        Item.width = 20;
        Item.height = 20;
        Item.useTime = 24;
        Item.useAnimation = 24;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2f;
        Item.value = 0;
        Item.rare = ItemRarityID.White;
        Item.UseSound = SoundID.Item20;
        Item.autoReuse = true;
        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<KiTechniqueProjectile>();
        Item.shootSpeed = 9f;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.DirtBlock)
            .Register();
    }

    public override void HoldItem(Player player)
    {
        KiTechniqueDefinition technique = player.GetModPlayer<KiPlayer>().CurrentTechnique;
        Item.damage = technique.BaseDamage;
        Item.useTime = technique.UseTime;
        Item.useAnimation = technique.UseTime;
        Item.knockBack = technique.Knockback;
        Item.shootSpeed = technique.ShootSpeed;
    }

    public override bool CanUseItem(Player player)
    {
        KiTechniqueDefinition technique = player.GetModPlayer<KiPlayer>().CurrentTechnique;
        return player.GetModPlayer<KiPlayer>().TryConsumeKi(technique.KiCost);
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
        KiTechniqueDefinition technique = kiPlayer.CurrentTechnique;

        if (velocity.LengthSquared() > 0f)
        {
            velocity = Vector2.Normalize(velocity) * technique.ShootSpeed;
        }

        int projectileIndex = Projectile.NewProjectile(
            source,
            position,
            velocity,
            ModContent.ProjectileType<KiTechniqueProjectile>(),
            Math.Max(damage, technique.BaseDamage),
            technique.Knockback,
            player.whoAmI,
            (float)kiPlayer.SelectedTechniqueIndex);

        if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles)
        {
            Projectile projectile = Main.projectile[projectileIndex];
            projectile.penetrate = technique.Penetration;
            projectile.timeLeft = technique.TimeLeft;
            projectile.scale = technique.ProjectileScale;
            projectile.width = Math.Max(8, (int)(14 * technique.ProjectileScale));
            projectile.height = Math.Max(8, (int)(14 * technique.ProjectileScale));
        }

        return false;
    }
}

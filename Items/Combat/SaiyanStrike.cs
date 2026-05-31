using System.Collections.Generic;
using KiAscension.Players;
using KiAscension.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Combat;

public class SaiyanStrike : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.FeralClaws}";

    public override void SetDefaults()
    {
        Item.damage = 20;
        Item.DamageType = DamageClass.Melee;
        Item.width = 24;
        Item.height = 24;
        Item.useTime = 16;
        Item.useAnimation = 16;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 4f;
        Item.value = 0;
        Item.rare = ItemRarityID.White;
        Item.noUseGraphic = true;
        Item.autoReuse = true;
        Item.UseSound = null;
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        KiPlayer kiPlayer = player.GetModPlayer<KiPlayer>();
        damage *= kiPlayer.PhysicalDamageMultiplier * kiPlayer.GetSaiyanStrikeComboDamageMultiplier();
    }

    public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
    {
        knockback *= player.GetModPlayer<KiPlayer>().GetSaiyanStrikeComboKnockbackMultiplier();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        KiPlayer kiPlayer = Main.LocalPlayer.GetModPlayer<KiPlayer>();
        tooltips.Add(new TooltipLine(Mod, "KiAscensionCombo", $"Combo: {kiPlayer.NextMeleeComboName} ({kiPlayer.NextMeleeComboStep}/3)"));
        tooltips.Add(new TooltipLine(Mod, "KiAscensionScaling", $"Physical power scaling: x{kiPlayer.PhysicalDamageMultiplier:0.00}"));
        tooltips.Add(new TooltipLine(Mod, "KiAscensionRole", "Starter punch/kick weapon. Vanilla weapons remain usable while melee expands."));
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        int comboStep = player.GetModPlayer<KiPlayer>().RegisterSaiyanStrikeHit(damageDone);
        SpawnStrikeEffects(player, target, comboStep);
        KiSoundSystem.PlayMeleeImpact(target.Center, comboStep);
    }

    private static void SpawnStrikeEffects(Player player, NPC target, int comboStep)
    {
        if (Main.dedServ)
        {
            return;
        }

        int dustType = comboStep switch
        {
            2 => DustID.Torch,
            3 => DustID.GoldFlame,
            _ => DustID.Smoke
        };

        int dustCount = comboStep == 3 ? 12 : 7;
        Lighting.AddLight(target.Center, comboStep == 3 ? new Vector3(0.75f, 0.52f, 0.12f) : new Vector3(0.32f, 0.24f, 0.16f));

        for (int i = 0; i < dustCount; i++)
        {
            Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, dustType);
            dust.noGravity = comboStep >= 2;
            dust.velocity = new Vector2(player.direction * Main.rand.NextFloat(1.2f, 3.6f), Main.rand.NextFloat(-2.6f, 0.4f));
            dust.scale = comboStep == 3 ? 1.25f : 0.9f;
        }

        if (comboStep == 3)
        {
            KiFeedbackSystem.RequestScreenShake(target.Center, 2.4f, 8);

            for (int i = 0; i < 6; i++)
            {
                Dust burst = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Electric);
                burst.noGravity = true;
                burst.velocity = Main.rand.NextVector2Circular(2.8f, 2.1f);
                burst.scale = 0.82f;
            }
        }
    }
}

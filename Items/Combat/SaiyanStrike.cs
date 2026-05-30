using KiAscension.Players;
using KiAscension.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Combat;

public class SaiyanStrike : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.FeralClaws}";

    public override void SetDefaults()
    {
        Item.damage = 18;
        Item.DamageType = DamageClass.Melee;
        Item.width = 24;
        Item.height = 24;
        Item.useTime = 18;
        Item.useAnimation = 18;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 4f;
        Item.value = 0;
        Item.rare = ItemRarityID.White;
        Item.noUseGraphic = true;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item1;
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        damage *= player.GetModPlayer<KiPlayer>().PhysicalDamageMultiplier;
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        player.GetModPlayer<KiPlayer>().RegisterSaiyanStrikeHit(damageDone);
        KiSoundSystem.PlayMeleeImpact(target.Center);
    }
}

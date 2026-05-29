using System;
using KiAscension.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items;

public class KiTrainingFocus : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.useTime = 90;
        Item.useAnimation = 90;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.value = 0;
        Item.rare = ItemRarityID.White;
        Item.UseSound = SoundID.Item4;
        Item.autoReuse = false;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.DirtBlock)
            .Register();
    }

    public override bool? UseItem(Player player)
    {
        KiPlayer kiPlayer = player.GetModPlayer<KiPlayer>();
        kiPlayer.AddTrainingExperience(4, 3, true);

        if (!Main.dedServ)
        {
            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(
                    player.position,
                    player.width,
                    player.height,
                    DustID.GemTopaz,
                    Main.rand.NextFloat(-1.2f, 1.2f),
                    Main.rand.NextFloat(-2f, -0.4f),
                    140,
                    new Color(255, 230, 120),
                    0.9f);

                Main.dust[dust].noGravity = true;
            }
        }

        return true;
    }
}

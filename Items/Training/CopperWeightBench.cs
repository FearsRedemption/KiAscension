using KiAscension.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Training;

public class CopperWeightBench : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<CopperWeightBenchTile>());
        Item.width = 30;
        Item.height = 22;
        Item.value = Item.buyPrice(silver: 22);
        Item.rare = ItemRarityID.Blue;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<WoodenWeightBench>()
            .AddIngredient(ItemID.CopperBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

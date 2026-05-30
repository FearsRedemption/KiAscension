using KiAscension.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Training;

public class WoodenWeightBench : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<WoodenWeightBenchTile>());
        Item.width = 30;
        Item.height = 22;
        Item.value = Item.buyPrice(silver: 8);
        Item.rare = ItemRarityID.White;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Wood, 20)
            .Register();
    }
}

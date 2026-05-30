using KiAscension.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Training;

public class WoodenTrainingBag : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<WoodenTrainingBagTile>());
        Item.width = 26;
        Item.height = 32;
        Item.value = Item.buyPrice(silver: 10);
        Item.rare = ItemRarityID.White;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Wood, 12)
            .AddIngredient(ItemID.Cobweb, 10)
            .Register();
    }
}

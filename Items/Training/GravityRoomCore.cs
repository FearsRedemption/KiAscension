using KiAscension.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Training;

public class GravityRoomCore : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<GravityRoomCoreTile>());
        Item.width = 28;
        Item.height = 28;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.MeteoriteBar, 8)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

using KiAscension.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Training;

public class WeightedTrainingBands : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 24;
        Item.accessory = true;
        Item.value = Item.buyPrice(silver: 25);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<KiPlayer>().IsWeightTraining = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.IronBar, 8)
            .AddIngredient(ItemID.StoneBlock, 25)
            .AddTile(TileID.Anvils)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.LeadBar, 8)
            .AddIngredient(ItemID.StoneBlock, 25)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

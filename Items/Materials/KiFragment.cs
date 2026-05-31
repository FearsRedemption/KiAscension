using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Items.Materials;

public class KiFragment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(copper: 50);
        Item.rare = ItemRarityID.Blue;
    }
}

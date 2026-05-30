using KiAscension.Common;
using KiAscension.Items.Training;
using KiAscension.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace KiAscension.Tiles;

public class WoodenWeightBenchTile : ModTile
{
    public override void SetStaticDefaults()
    {
        WeightBenchTileHelper.SetStaticDefaults(this, new Color(136, 92, 54), "Mods.KiAscension.Tiles.WoodenWeightBenchTile.MapEntry");
        DustType = DustID.WoodFurniture;
        HitSound = SoundID.Dig;
        MineResist = 1.2f;
    }

    public override bool RightClick(int i, int j)
    {
        return WeightBenchTileHelper.StartTraining(i, j, TrainingSource.WoodenWeightBench);
    }

    public override void MouseOver(int i, int j)
    {
        WeightBenchTileHelper.ShowMouseOver(ModContent.ItemType<WoodenWeightBench>());
    }
}

public class CopperWeightBenchTile : ModTile
{
    public override void SetStaticDefaults()
    {
        WeightBenchTileHelper.SetStaticDefaults(this, new Color(185, 104, 66), "Mods.KiAscension.Tiles.CopperWeightBenchTile.MapEntry");
        DustType = DustID.CopperCoin;
        HitSound = SoundID.Tink;
        MineResist = 1.5f;
    }

    public override bool RightClick(int i, int j)
    {
        return WeightBenchTileHelper.StartTraining(i, j, TrainingSource.CopperWeightBench);
    }

    public override void MouseOver(int i, int j)
    {
        WeightBenchTileHelper.ShowMouseOver(ModContent.ItemType<CopperWeightBench>());
    }
}

internal static class WeightBenchTileHelper
{
    public static void SetStaticDefaults(ModTile tile, Color mapColor, string mapKey)
    {
        int type = tile.Type;
        Main.tileFrameImportant[type] = true;
        Main.tileNoAttach[type] = true;
        Main.tileTable[type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.addTile(type);

        tile.AddMapEntry(mapColor, Language.GetText(mapKey));
    }

    public static bool StartTraining(int i, int j, TrainingSource source)
    {
        Point topLeft = GetTopLeft(i, j);
        Main.LocalPlayer.GetModPlayer<KiPlayer>().RequestTrainingStationUse(source, topLeft.X, topLeft.Y);
        return true;
    }

    public static void ShowMouseOver(int itemType)
    {
        Player player = Main.LocalPlayer;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = itemType;
        player.noThrow = 2;
    }

    private static Point GetTopLeft(int i, int j)
    {
        Tile tile = Framing.GetTileSafely(i, j);
        int frameX = tile.TileFrameX / 18;
        int frameY = tile.TileFrameY / 18;
        return new Point(i - frameX % 2, j - frameY % 2);
    }
}

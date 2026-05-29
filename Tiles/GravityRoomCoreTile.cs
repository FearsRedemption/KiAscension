using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace KiAscension.Tiles;

public class GravityRoomCoreTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(120, 75, 210), Language.GetText("Mods.KiAscension.Tiles.GravityRoomCoreTile.MapEntry"));
        DustType = DustID.Electric;
        HitSound = SoundID.Item93;
        MineResist = 2f;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.22f;
        g = 0.08f;
        b = 0.45f;
    }
}

using KiAscension.Hairs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KiAscension.Common;

public static class AscensionVisuals
{
    public static int GetHairStyle(AscensionStage stage, int naturalHairStyle)
    {
        return stage switch
        {
            AscensionStage.SuperSaiyan => ModContent.GetInstance<SuperSaiyanHair>().Type,
            AscensionStage.SuperSaiyan2 => ModContent.GetInstance<SuperSaiyan2Hair>().Type,
            AscensionStage.SuperSaiyan3 => ModContent.GetInstance<SuperSaiyan3Hair>().Type,
            AscensionStage.SuperSaiyanGod => ModContent.GetInstance<GodHair>().Type,
            AscensionStage.SuperSaiyanBlue => ModContent.GetInstance<SuperSaiyan2Hair>().Type,
            AscensionStage.UltraInstinctSign => ModContent.GetInstance<UltraInstinctHair>().Type,
            AscensionStage.UltraInstinct => ModContent.GetInstance<UltraInstinctHair>().Type,
            _ => naturalHairStyle
        };
    }

    public static Color GetHairColor(StageDefinition stage, Color naturalHairColor)
    {
        return stage.Stage switch
        {
            AscensionStage.Base => naturalHairColor,
            AscensionStage.Awakened => new Color(255, 239, 125),
            AscensionStage.SuperSaiyan => new Color(255, 226, 70),
            AscensionStage.SuperSaiyan2 => new Color(255, 242, 110),
            AscensionStage.SuperSaiyan3 => new Color(255, 205, 55),
            AscensionStage.SuperSaiyanGod => new Color(235, 55, 85),
            AscensionStage.SuperSaiyanBlue => new Color(65, 215, 255),
            AscensionStage.UltraInstinctSign => new Color(205, 220, 255),
            AscensionStage.UltraInstinct => new Color(238, 245, 255),
            _ => stage.AuraColor
        };
    }

    public static void Apply(Player player, StageDefinition stage, int naturalHairStyle, Color naturalHairColor)
    {
        player.hair = GetHairStyle(stage.Stage, naturalHairStyle);
        player.hairColor = GetHairColor(stage, naturalHairColor);
    }
}

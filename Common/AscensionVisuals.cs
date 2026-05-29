using Microsoft.Xna.Framework;
using Terraria;

namespace KiAscension.Common;

public static class AscensionVisuals
{
    public static int GetHairStyle(AscensionStage stage, int naturalHairStyle)
    {
        return stage switch
        {
            AscensionStage.SuperSaiyan => 7,
            AscensionStage.SuperSaiyan2 => 10,
            AscensionStage.SuperSaiyan3 => 27,
            AscensionStage.SuperSaiyanBlue => 10,
            AscensionStage.UltraInstinctSign => 7,
            AscensionStage.UltraInstinct => 10,
            _ => naturalHairStyle
        };
    }

    public static Color GetHairColor(StageDefinition stage, Color naturalHairColor)
    {
        return stage.Stage switch
        {
            AscensionStage.Base => naturalHairColor,
            AscensionStage.Awakened => new Color(255, 239, 125),
            AscensionStage.KaioKen => new Color(255, 70, 65),
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

using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public static class AscensionHairProfiles
{
    public static AscensionHairProfile Get(AscensionStage stage)
    {
        return stage switch
        {
            AscensionStage.Awakened => new(AscensionHairStyle.Natural, new Color(255, 239, 125), false, "Natural hair with a faint awakened tint."),
            AscensionStage.SuperSaiyan => new(AscensionHairStyle.SuperSaiyan, new Color(255, 226, 70), false, "Spiky gold Super Saiyan hair."),
            AscensionStage.SuperSaiyan2 => new(AscensionHairStyle.SuperSaiyan2, new Color(255, 242, 110), false, "Sharper gold Super Saiyan 2 hair."),
            AscensionStage.SuperSaiyan3 => new(AscensionHairStyle.SuperSaiyan3, new Color(255, 205, 55), false, "Longer flowing Super Saiyan 3 hair."),
            AscensionStage.SuperSaiyanGod => new(AscensionHairStyle.SuperSaiyanGod, new Color(235, 55, 85), false, "Compact red divine hair."),
            AscensionStage.SuperSaiyanBlue => new(AscensionHairStyle.SuperSaiyanBlue, new Color(65, 215, 255), false, "Blue Super Saiyan-style hair."),
            AscensionStage.UltraInstinctSign => new(AscensionHairStyle.UltraInstinctSign, new Color(205, 220, 255), false, "Incomplete silver-blue instinct hair."),
            AscensionStage.UltraInstinct => new(AscensionHairStyle.UltraInstinct, new Color(238, 245, 255), false, "Mastered silver-white instinct hair."),
            _ => new(AscensionHairStyle.Natural, Color.White, true, "Natural player hair.")
        };
    }
}

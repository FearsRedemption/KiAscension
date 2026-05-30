using KiAscension.Hairs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KiAscension.Common;

public static class AscensionVisuals
{
    public static int GetHairStyle(AscensionStage stage, int naturalHairStyle)
    {
        AscensionHairProfile profile = AscensionHairProfiles.Get(stage);

        return profile.HairStyle switch
        {
            AscensionHairStyle.SuperSaiyan => ModContent.GetInstance<SuperSaiyanHair>().Type,
            AscensionHairStyle.SuperSaiyan2 => ModContent.GetInstance<SuperSaiyan2Hair>().Type,
            AscensionHairStyle.SuperSaiyan3 => ModContent.GetInstance<SuperSaiyan3Hair>().Type,
            AscensionHairStyle.SuperSaiyanGod => ModContent.GetInstance<GodHair>().Type,
            AscensionHairStyle.SuperSaiyanBlue => ModContent.GetInstance<SuperSaiyanBlueHair>().Type,
            AscensionHairStyle.UltraInstinctSign => ModContent.GetInstance<UltraInstinctSignHair>().Type,
            AscensionHairStyle.UltraInstinct => ModContent.GetInstance<UltraInstinctHair>().Type,
            _ => naturalHairStyle
        };
    }

    public static bool IsAscensionHairStyle(int hairStyle)
    {
        return hairStyle == ModContent.GetInstance<SuperSaiyanHair>().Type
            || hairStyle == ModContent.GetInstance<SuperSaiyan2Hair>().Type
            || hairStyle == ModContent.GetInstance<SuperSaiyan3Hair>().Type
            || hairStyle == ModContent.GetInstance<GodHair>().Type
            || hairStyle == ModContent.GetInstance<SuperSaiyanBlueHair>().Type
            || hairStyle == ModContent.GetInstance<UltraInstinctSignHair>().Type
            || hairStyle == ModContent.GetInstance<UltraInstinctHair>().Type;
    }

    public static Color GetHairColor(StageDefinition stage, Color naturalHairColor)
    {
        AscensionHairProfile profile = AscensionHairProfiles.Get(stage.Stage);
        return profile.UseNaturalColor ? naturalHairColor : profile.Tint;
    }

    public static void Apply(Player player, StageDefinition stage, int naturalHairStyle, Color naturalHairColor)
    {
        player.hair = GetHairStyle(stage.Stage, naturalHairStyle);
        player.hairColor = GetHairColor(stage, naturalHairColor);
    }
}

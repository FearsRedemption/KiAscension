using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace KiAscension.Common;

public static class AscensionStages
{
    public static readonly StageDefinition[] Definitions =
    {
        new(AscensionStage.Base, "Base Saiyan", 0, AscensionProgressionGate.None, false, 1f, 1f, 0, 0, 0, 0, 0, 1f, new Color(255, 244, 175)),
        new(AscensionStage.Awakened, "Awakened State", 500, AscensionProgressionGate.None, false, 1.25f, 1.12f, 3, 70, 5, 0, 1, 1.1f, new Color(255, 244, 120)),
        new(AscensionStage.SuperSaiyan, "Super Saiyan", 3000, AscensionProgressionGate.EyeOfCthulhu, true, 1.75f, 1.28f, 7, 140, 7, 2, 2, 1.18f, new Color(255, 224, 80)),
        new(AscensionStage.SuperSaiyan2, "Super Saiyan 2", 6500, AscensionProgressionGate.WorldEvilBoss, true, 2.25f, 1.39f, 12, 205, 8, 4, 3, 1.3f, new Color(255, 240, 110)),
        new(AscensionStage.SuperSaiyan3, "Super Saiyan 3", 12000, AscensionProgressionGate.Skeletron, false, 2.9f, 1.5f, 15, 290, 7, 8, 2, 1.4f, new Color(255, 200, 60)),
        new(AscensionStage.SuperSaiyanGod, "Super Saiyan God", 22000, AscensionProgressionGate.WallOfFlesh, false, 3.45f, 1.6f, 20, 390, 11, 3, 5, 1.52f, new Color(245, 55, 80)),
        new(AscensionStage.SuperSaiyanBlue, "Super Saiyan Blue", 34000, AscensionProgressionGate.MechanicalBoss, false, 4.3f, 1.76f, 27, 500, 11, 7, 5, 1.64f, new Color(70, 210, 255)),
        new(AscensionStage.UltraInstinctSign, "Ultra Instinct Sign", 50000, AscensionProgressionGate.Plantera, true, 5f, 1.96f, 31, 610, 13, 5, 6, 1.84f, new Color(200, 220, 255)),
        new(AscensionStage.UltraInstinct, "Ultra Instinct", 72000, AscensionProgressionGate.MoonLord, false, 6f, 2.18f, 38, 760, 16, 3, 9, 2.1f, new Color(235, 245, 255))
    };

    public static int MaxStageIndex => Definitions.Length - 1;

    public static StageDefinition Get(int stageIndex)
    {
        return Definitions[Math.Clamp(stageIndex, 0, MaxStageIndex)];
    }

    public static bool IsGateSatisfied(StageDefinition stage)
    {
        return IsGateSatisfied(stage.RequiredGate);
    }

    public static bool IsGateSatisfied(AscensionProgressionGate gate)
    {
        return gate switch
        {
            AscensionProgressionGate.None => true,
            AscensionProgressionGate.EyeOfCthulhu => NPC.downedBoss1,
            AscensionProgressionGate.WorldEvilBoss => NPC.downedBoss2,
            AscensionProgressionGate.Skeletron => NPC.downedBoss3,
            AscensionProgressionGate.WallOfFlesh => Main.hardMode,
            AscensionProgressionGate.MechanicalBoss => NPC.downedMechBossAny,
            AscensionProgressionGate.Plantera => NPC.downedPlantBoss,
            AscensionProgressionGate.Golem => NPC.downedGolemBoss,
            AscensionProgressionGate.MoonLord => NPC.downedMoonlord,
            _ => true
        };
    }

    public static string GetGateText(AscensionProgressionGate gate)
    {
        return gate switch
        {
            AscensionProgressionGate.EyeOfCthulhu => "defeat the Eye of Cthulhu",
            AscensionProgressionGate.WorldEvilBoss => "defeat the Eater of Worlds or Brain of Cthulhu",
            AscensionProgressionGate.Skeletron => "defeat Skeletron",
            AscensionProgressionGate.WallOfFlesh => "defeat the Wall of Flesh",
            AscensionProgressionGate.MechanicalBoss => "defeat a mechanical boss",
            AscensionProgressionGate.Plantera => "defeat Plantera",
            AscensionProgressionGate.Golem => "defeat Golem",
            AscensionProgressionGate.MoonLord => "defeat the Moon Lord",
            _ => "train"
        };
    }
}

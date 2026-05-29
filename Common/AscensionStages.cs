using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace KiAscension.Common;

public static class AscensionStages
{
    public static readonly StageDefinition[] Definitions =
    {
        new(AscensionStage.Base, "Base Saiyan", 0, AscensionProgressionGate.None, false, 1f, 1f, 0, 0, 0, new Color(255, 244, 175)),
        new(AscensionStage.Awakened, "Awakened State", 500, AscensionProgressionGate.None, false, 1.12f, 1.06f, 2, 15, 0, new Color(255, 244, 120)),
        new(AscensionStage.KaioKen, "Kaio-Ken", 1400, AscensionProgressionGate.None, false, 1.28f, 1.14f, 0, 30, 3, new Color(255, 80, 60)),
        new(AscensionStage.SuperSaiyan, "Super Saiyan", 3000, AscensionProgressionGate.EyeOfCthulhu, true, 1.6f, 1.24f, 6, 70, 2, new Color(255, 224, 80)),
        new(AscensionStage.SuperSaiyan2, "Super Saiyan 2", 6500, AscensionProgressionGate.WorldEvilBoss, true, 2.05f, 1.34f, 10, 110, 3, new Color(255, 240, 110)),
        new(AscensionStage.SuperSaiyan3, "Super Saiyan 3", 12000, AscensionProgressionGate.Skeletron, false, 2.6f, 1.44f, 14, 160, 7, new Color(255, 200, 60)),
        new(AscensionStage.SuperSaiyanGod, "Super Saiyan God", 22000, AscensionProgressionGate.WallOfFlesh, false, 3.2f, 1.55f, 18, 230, 5, new Color(245, 55, 80)),
        new(AscensionStage.SuperSaiyanBlue, "Super Saiyan Blue", 34000, AscensionProgressionGate.MechanicalBoss, false, 4f, 1.7f, 24, 310, 7, new Color(70, 210, 255)),
        new(AscensionStage.UltraInstinctSign, "Ultra Instinct Sign", 50000, AscensionProgressionGate.Plantera, true, 4.7f, 1.9f, 28, 390, 9, new Color(200, 220, 255)),
        new(AscensionStage.UltraInstinct, "Ultra Instinct", 72000, AscensionProgressionGate.MoonLord, false, 5.6f, 2.1f, 34, 500, 10, new Color(235, 245, 255))
    };

    public static int MaxStageIndex => Definitions.Length - 1;

    public static StageDefinition Get(int stageIndex)
    {
        return Definitions[Math.Clamp(stageIndex, 0, MaxStageIndex)];
    }

    public static bool IsGateSatisfied(StageDefinition stage)
    {
        return stage.RequiredGate switch
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

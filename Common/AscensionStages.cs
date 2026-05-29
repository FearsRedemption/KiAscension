using System;
using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public static class AscensionStages
{
    public static readonly StageDefinition[] Definitions =
    {
        new(AscensionStage.Base, "Base Saiyan", 0, false, 1f, 1f, 0, 0, 0, new Color(255, 244, 175)),
        new(AscensionStage.Awakened, "Awakened State", 150, false, 1.15f, 1.08f, 2, 20, 0, new Color(255, 244, 120)),
        new(AscensionStage.KaioKen, "Kaio-Ken", 450, false, 1.35f, 1.18f, 0, 35, 2, new Color(255, 80, 60)),
        new(AscensionStage.SuperSaiyan, "Super Saiyan", 1000, true, 1.65f, 1.25f, 6, 60, 1, new Color(255, 224, 80)),
        new(AscensionStage.SuperSaiyan2, "Super Saiyan 2", 2200, true, 2.1f, 1.35f, 10, 90, 2, new Color(255, 240, 110)),
        new(AscensionStage.SuperSaiyan3, "Super Saiyan 3", 4200, false, 2.7f, 1.45f, 14, 130, 5, new Color(255, 200, 60)),
        new(AscensionStage.SuperSaiyanGod, "Super Saiyan God", 7000, false, 3.25f, 1.55f, 18, 180, 3, new Color(245, 55, 80)),
        new(AscensionStage.SuperSaiyanBlue, "Super Saiyan Blue", 10000, false, 4f, 1.7f, 24, 230, 4, new Color(70, 210, 255)),
        new(AscensionStage.UltraInstinctSign, "Ultra Instinct Sign", 14000, true, 4.75f, 1.9f, 28, 280, 6, new Color(200, 220, 255)),
        new(AscensionStage.UltraInstinct, "Ultra Instinct", 19000, false, 5.6f, 2.1f, 34, 350, 8, new Color(235, 245, 255))
    };

    public static int MaxStageIndex => Definitions.Length - 1;

    public static StageDefinition Get(int stageIndex)
    {
        return Definitions[Math.Clamp(stageIndex, 0, MaxStageIndex)];
    }
}

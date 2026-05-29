using System;
using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public static class KiTechniques
{
    public static readonly KiTechniqueDefinition[] Definitions =
    {
        new(
            KiTechnique.BasicKiBlast,
            "Basic Ki Blast",
            AscensionStage.Base,
            0,
            12,
            4,
            22,
            10f,
            2f,
            2,
            90,
            0.9f,
            new Color(255, 225, 95),
            "Starter ranged pressure."),
        new(
            KiTechnique.KiBarrage,
            "Ki Barrage",
            AscensionStage.Base,
            120,
            9,
            4,
            12,
            11f,
            1.2f,
            1,
            70,
            0.75f,
            new Color(255, 235, 120),
            "Fast training volley before signature beams."),
        new(
            KiTechnique.Kamehameha,
            "Kamehameha",
            AscensionStage.Awakened,
            300,
            28,
            16,
            38,
            14f,
            3.2f,
            3,
            115,
            1.25f,
            new Color(90, 190, 255),
            "First signature beam after the player learns controlled output."),
        new(
            KiTechnique.DestructoDisk,
            "Destructo Disk",
            AscensionStage.KaioKen,
            650,
            38,
            22,
            44,
            10f,
            1.5f,
            5,
            135,
            1.15f,
            new Color(255, 245, 150),
            "Precision cutter unlocked before the heavier Vegeta-style finishers."),
        new(
            KiTechnique.GalickGun,
            "Galick Gun",
            AscensionStage.KaioKen,
            900,
            48,
            28,
            48,
            14.5f,
            4f,
            4,
            120,
            1.35f,
            new Color(185, 95, 255),
            "Rival beam tier, placed before Super Saiyan-exclusive finishers."),
        new(
            KiTechnique.BigBangAttack,
            "Big Bang Attack",
            AscensionStage.SuperSaiyan,
            1400,
            58,
            42,
            56,
            9f,
            6f,
            2,
            100,
            1.75f,
            new Color(110, 235, 255),
            "First Super Saiyan finisher; lore-wise before Final Flash."),
        new(
            KiTechnique.FinalFlash,
            "Final Flash",
            AscensionStage.SuperSaiyan2,
            2600,
            82,
            58,
            66,
            16f,
            7f,
            6,
            135,
            1.8f,
            new Color(255, 220, 70),
            "Heavier late-Cell-saga beam after Big Bang Attack."),
        new(
            KiTechnique.SpiritBomb,
            "Spirit Bomb",
            AscensionStage.SuperSaiyan3,
            4600,
            115,
            85,
            82,
            7f,
            8f,
            3,
            150,
            2.4f,
            new Color(180, 235, 255),
            "Moved later than its lore introduction because it is a boss-scale Terraria nuke."),
        new(
            KiTechnique.GodKamehameha,
            "God Kamehameha",
            AscensionStage.SuperSaiyanGod,
            7600,
            145,
            95,
            58,
            17f,
            7.5f,
            7,
            145,
            2f,
            new Color(245, 80, 105),
            "God-ki version of the signature beam."),
        new(
            KiTechnique.UltraInstinctBarrage,
            "Ultra Instinct Barrage",
            AscensionStage.UltraInstinctSign,
            14000,
            190,
            120,
            20,
            18f,
            3f,
            2,
            80,
            1.25f,
            new Color(235, 245, 255),
            "Endgame instinctive rapid-fire pressure.")
    };

    public static int MaxTechniqueIndex => Definitions.Length - 1;

    public static KiTechniqueDefinition Get(int techniqueIndex)
    {
        return Definitions[Math.Clamp(techniqueIndex, 0, MaxTechniqueIndex)];
    }

    public static bool IsUnlocked(KiTechniqueDefinition definition, int powerExperience, int unlockedStageIndex)
    {
        return powerExperience >= definition.RequiredExperience
            && unlockedStageIndex >= (int)definition.RequiredStage;
    }

    public static int GetHighestUnlockedIndex(int powerExperience, int unlockedStageIndex)
    {
        int highestUnlockedIndex = 0;

        for (int i = 1; i < Definitions.Length; i++)
        {
            if (!IsUnlocked(Definitions[i], powerExperience, unlockedStageIndex))
            {
                continue;
            }

            highestUnlockedIndex = i;
        }

        return highestUnlockedIndex;
    }
}

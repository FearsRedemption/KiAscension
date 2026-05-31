using System;
using KiAscension.Items.Techniques;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace KiAscension.Common;

public static class KiTechniques
{
    public static readonly KiTechniqueDefinition[] Definitions =
    {
        new(
            KiTechnique.BasicKiBlast,
            KiTechniqueBehavior.Bolt,
            KiTechniqueCategory.BasicBlast,
            KiTechniqueSourceType.GeneralKi,
            KiTechniqueCollisionStyle.TerrainBlocked,
            "Basic Ki Blast",
            AscensionStage.Base,
            0,
            17,
            1,
            0,
            14,
            13.5f,
            1.6f,
            1,
            76,
            0.75f,
            new Color(255, 225, 95),
            "Cheap starter pop-shot for constant early pressure."),
        new(
            KiTechnique.KiBarrage,
            KiTechniqueBehavior.Barrage,
            KiTechniqueCategory.Barrage,
            KiTechniqueSourceType.GeneralKi,
            KiTechniqueCollisionStyle.TerrainBlocked,
            "Ki Barrage",
            AscensionStage.Base,
            260,
            10,
            5,
            0,
            13,
            13.5f,
            1.2f,
            1,
            70,
            0.62f,
            new Color(255, 235, 120),
            "Rapid small-shot pressure before signature beams."),
        new(
            KiTechnique.Masenko,
            KiTechniqueBehavior.HeavyBlast,
            KiTechniqueCategory.HeavyBlast,
            KiTechniqueSourceType.GohanLine,
            KiTechniqueCollisionStyle.HeavyImpact,
            "Masenko",
            AscensionStage.Awakened,
            520,
            30,
            8,
            0,
            18,
            15f,
            3.1f,
            1,
            92,
            1f,
            new Color(255, 232, 80),
            "Fast yellow palm-wave burst after basic ki control."),
        new(
            KiTechnique.Kamehameha,
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.TurtleSchool,
            KiTechniqueCollisionStyle.SustainedLine,
            "Kamehameha",
            AscensionStage.Awakened,
            800,
            17,
            8,
            13,
            22,
            14.5f,
            3.2f,
            -1,
            2,
            1.2f,
            new Color(90, 190, 255),
            "Stable blue-white signature beam after controlled output."),
        new(
            KiTechnique.DeathBeam,
            KiTechniqueBehavior.Bolt,
            KiTechniqueCategory.BasicBlast,
            KiTechniqueSourceType.FriezaLine,
            KiTechniqueCollisionStyle.TerrainBlocked,
            "Death Beam",
            AscensionStage.Awakened,
            1150,
            34,
            10,
            0,
            15,
            24f,
            1.25f,
            1,
            58,
            0.55f,
            new Color(255, 96, 170),
            "Very fast thin precision poke."),
        new(
            KiTechnique.DestructoDisk,
            KiTechniqueBehavior.SteeringDisk,
            KiTechniqueCategory.CuttingDisk,
            KiTechniqueSourceType.EarthlingTactical,
            KiTechniqueCollisionStyle.GuidedPiercing,
            "Destructo Disk",
            AscensionStage.Awakened,
            1600,
            38,
            24,
            0,
            40,
            11.5f,
            1.5f,
            10,
            240,
            1.05f,
            new Color(255, 245, 150),
            "Spinning guided cutter unlocked before heavier finishers."),
        new(
            KiTechnique.GalickGun,
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.VegetaLine,
            KiTechniqueCollisionStyle.SustainedLine,
            "Galick Gun",
            AscensionStage.Awakened,
            2200,
            25,
            12,
            23,
            26,
            15f,
            4f,
            -1,
            2,
            1.4f,
            new Color(185, 95, 255),
            "Aggressive purple rival beam with higher strain."),
        new(
            KiTechnique.SpecialBeamCannon,
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.Namekian,
            KiTechniqueCollisionStyle.SustainedLine,
            "Special Beam Cannon",
            AscensionStage.Awakened,
            2900,
            36,
            26,
            23,
            34,
            16f,
            2.2f,
            -1,
            2,
            0.78f,
            new Color(190, 255, 95),
            "Narrow drill-like beam for committed piercing pressure."),
        new(
            KiTechnique.BigBangAttack,
            KiTechniqueBehavior.HeavyBlast,
            KiTechniqueCategory.HeavyBlast,
            KiTechniqueSourceType.VegetaLine,
            KiTechniqueCollisionStyle.HeavyImpact,
            "Big Bang Attack",
            AscensionStage.SuperSaiyan,
            3600,
            66,
            46,
            0,
            50,
            8.4f,
            7f,
            2,
            130,
            1.95f,
            new Color(110, 235, 255),
            "Large compact Super Saiyan burst before Final Flash."),
        new(
            KiTechnique.FinalFlash,
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.VegetaLine,
            KiTechniqueCollisionStyle.SustainedLine,
            "Final Flash",
            AscensionStage.SuperSaiyan2,
            7600,
            44,
            26,
            46,
            48,
            16f,
            7f,
            -1,
            2,
            2.15f,
            new Color(255, 220, 70),
            "Huge high-drain golden beam with long commitment."),
        new(
            KiTechnique.SpiritBomb,
            KiTechniqueBehavior.HeavyBlast,
            KiTechniqueCategory.Ultimate,
            KiTechniqueSourceType.TurtleSchool,
            KiTechniqueCollisionStyle.TerrainPassingUltimate,
            "Spirit Bomb",
            AscensionStage.SuperSaiyan3,
            14500,
            132,
            92,
            82,
            96,
            6.2f,
            8f,
            3,
            420,
            2.55f,
            new Color(180, 235, 255),
            "Slow last-resort charge ultimate with terrain-passing travel."),
        new(
            KiTechnique.GodKamehameha,
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.GodKi,
            KiTechniqueCollisionStyle.SustainedLine,
            "God Kamehameha",
            AscensionStage.SuperSaiyanGod,
            26000,
            54,
            32,
            46,
            40,
            18f,
            7.5f,
            -1,
            2,
            2.05f,
            new Color(245, 80, 105),
            "Clean high-tier god-ki signature beam."),
        new(
            KiTechnique.UltraInstinctBarrage,
            KiTechniqueBehavior.Barrage,
            KiTechniqueCategory.Barrage,
            KiTechniqueSourceType.UltraInstinct,
            KiTechniqueCollisionStyle.TerrainBlocked,
            "Ultra Instinct Barrage",
            AscensionStage.UltraInstinctSign,
            54000,
            155,
            105,
            0,
            12,
            22f,
            2.2f,
            3,
            64,
            0.82f,
            new Color(235, 245, 255),
            "Clean high-speed instinctive pressure.")
    };

    public static int MaxTechniqueIndex => Definitions.Length - 1;

    public static KiTechniqueDefinition Get(int techniqueIndex)
    {
        return Definitions[Math.Clamp(techniqueIndex, 0, MaxTechniqueIndex)];
    }

    public static bool IsUnlocked(KiTechniqueDefinition definition, int kiPowerExperience, int unlockedStageIndex)
    {
        return kiPowerExperience >= definition.RequiredKiPower
            && unlockedStageIndex >= (int)definition.RequiredStage;
    }

    public static int GetHighestUnlockedIndex(int kiPowerExperience, int unlockedStageIndex)
    {
        int highestUnlockedIndex = 0;

        for (int i = 1; i < Definitions.Length; i++)
        {
            if (!IsUnlocked(Definitions[i], kiPowerExperience, unlockedStageIndex))
            {
                continue;
            }

            highestUnlockedIndex = i;
        }

        return highestUnlockedIndex;
    }

    public static int GetItemType(KiTechnique technique)
    {
        return technique switch
        {
            KiTechnique.BasicKiBlast => ModContent.ItemType<BasicKiBlastSpell>(),
            KiTechnique.KiBarrage => ModContent.ItemType<KiBarrageSpell>(),
            KiTechnique.Masenko => ModContent.ItemType<MasenkoSpell>(),
            KiTechnique.Kamehameha => ModContent.ItemType<KamehamehaSpell>(),
            KiTechnique.DeathBeam => ModContent.ItemType<DeathBeamSpell>(),
            KiTechnique.DestructoDisk => ModContent.ItemType<DestructoDiskSpell>(),
            KiTechnique.GalickGun => ModContent.ItemType<GalickGunSpell>(),
            KiTechnique.SpecialBeamCannon => ModContent.ItemType<SpecialBeamCannonSpell>(),
            KiTechnique.BigBangAttack => ModContent.ItemType<BigBangAttackSpell>(),
            KiTechnique.FinalFlash => ModContent.ItemType<FinalFlashSpell>(),
            KiTechnique.SpiritBomb => ModContent.ItemType<SpiritBombSpell>(),
            KiTechnique.GodKamehameha => ModContent.ItemType<GodKamehamehaSpell>(),
            KiTechnique.UltraInstinctBarrage => ModContent.ItemType<UltraInstinctBarrageSpell>(),
            _ => ModContent.ItemType<BasicKiBlastSpell>()
        };
    }
}

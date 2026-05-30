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
            18,
            2,
            0,
            18,
            11f,
            2f,
            2,
            90,
            1f,
            new Color(255, 225, 95),
            "Starter ranged pressure."),
        new(
            KiTechnique.KiBarrage,
            KiTechniqueBehavior.Barrage,
            KiTechniqueCategory.Barrage,
            KiTechniqueSourceType.GeneralKi,
            KiTechniqueCollisionStyle.TerrainBlocked,
            "Ki Barrage",
            AscensionStage.Base,
            260,
            12,
            3,
            0,
            14,
            12f,
            1.2f,
            1,
            70,
            0.85f,
            new Color(255, 235, 120),
            "Fast training volley before signature beams."),
        new(
            KiTechnique.Masenko,
            KiTechniqueBehavior.HeavyBlast,
            KiTechniqueCategory.HeavyBlast,
            KiTechniqueSourceType.GohanLine,
            KiTechniqueCollisionStyle.HeavyImpact,
            "Masenko",
            AscensionStage.Awakened,
            520,
            24,
            6,
            0,
            22,
            12.5f,
            2.6f,
            1,
            92,
            1.12f,
            new Color(255, 232, 80),
            "First focused palm-wave after basic ki control."),
        new(
            KiTechnique.Kamehameha,
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.TurtleSchool,
            KiTechniqueCollisionStyle.SustainedLine,
            "Kamehameha",
            AscensionStage.Awakened,
            800,
            16,
            7,
            14,
            24,
            14f,
            3.2f,
            -1,
            2,
            1.25f,
            new Color(90, 190, 255),
            "First signature beam after the player learns controlled output."),
        new(
            KiTechnique.DeathBeam,
            KiTechniqueBehavior.Bolt,
            KiTechniqueCategory.BasicBlast,
            KiTechniqueSourceType.FriezaLine,
            KiTechniqueCollisionStyle.TerrainBlocked,
            "Death Beam",
            AscensionStage.Awakened,
            1150,
            30,
            8,
            0,
            18,
            19f,
            1.6f,
            1,
            70,
            0.72f,
            new Color(255, 96, 170),
            "Fast precision poke for players who want a sharp single-target shot."),
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
            22,
            0,
            44,
            10f,
            1.5f,
            8,
            210,
            1.15f,
            new Color(255, 245, 150),
            "Precision cutter unlocked before the heavier Vegeta-style finishers."),
        new(
            KiTechnique.GalickGun,
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.VegetaLine,
            KiTechniqueCollisionStyle.SustainedLine,
            "Galick Gun",
            AscensionStage.Awakened,
            2200,
            22,
            10,
            20,
            28,
            14.5f,
            4f,
            -1,
            2,
            1.35f,
            new Color(185, 95, 255),
            "Rival beam tier, placed before Super Saiyan-exclusive finishers."),
        new(
            KiTechnique.SpecialBeamCannon,
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.Namekian,
            KiTechniqueCollisionStyle.SustainedLine,
            "Special Beam Cannon",
            AscensionStage.Awakened,
            2900,
            32,
            24,
            24,
            36,
            16f,
            2.2f,
            -1,
            2,
            0.82f,
            new Color(190, 255, 95),
            "Narrow committed beam for boss pressure before Super Saiyan finishers."),
        new(
            KiTechnique.BigBangAttack,
            KiTechniqueBehavior.HeavyBlast,
            KiTechniqueCategory.HeavyBlast,
            KiTechniqueSourceType.VegetaLine,
            KiTechniqueCollisionStyle.HeavyImpact,
            "Big Bang Attack",
            AscensionStage.SuperSaiyan,
            3600,
            58,
            42,
            0,
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
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.VegetaLine,
            KiTechniqueCollisionStyle.SustainedLine,
            "Final Flash",
            AscensionStage.SuperSaiyan2,
            7600,
            34,
            20,
            36,
            40,
            16f,
            7f,
            -1,
            2,
            1.8f,
            new Color(255, 220, 70),
            "Heavier late-Cell-saga beam after Big Bang Attack."),
        new(
            KiTechnique.SpiritBomb,
            KiTechniqueBehavior.HeavyBlast,
            KiTechniqueCategory.Ultimate,
            KiTechniqueSourceType.TurtleSchool,
            KiTechniqueCollisionStyle.TerrainPassingUltimate,
            "Spirit Bomb",
            AscensionStage.SuperSaiyan3,
            14500,
            115,
            75,
            68,
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
            KiTechniqueBehavior.Beam,
            KiTechniqueCategory.ContinuousBeam,
            KiTechniqueSourceType.GodKi,
            KiTechniqueCollisionStyle.SustainedLine,
            "God Kamehameha",
            AscensionStage.SuperSaiyanGod,
            26000,
            48,
            28,
            52,
            44,
            17f,
            7.5f,
            -1,
            2,
            2f,
            new Color(245, 80, 105),
            "God-ki version of the signature beam."),
        new(
            KiTechnique.UltraInstinctBarrage,
            KiTechniqueBehavior.Barrage,
            KiTechniqueCategory.Barrage,
            KiTechniqueSourceType.UltraInstinct,
            KiTechniqueCollisionStyle.TerrainBlocked,
            "Ultra Instinct Barrage",
            AscensionStage.UltraInstinctSign,
            54000,
            190,
            120,
            0,
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

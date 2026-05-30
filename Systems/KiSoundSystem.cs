using System;
using KiAscension.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace KiAscension.Systems;

public static class KiSoundSystem
{
    private static readonly SoundStyle PowerUpStartSound = CustomSound("PowerUp", 0.86f, 0.08f, 2);
    private static readonly SoundStyle TransformationCompleteSound = CustomSound("Charge", 0.72f, 0.04f, 2);
    private static readonly SoundStyle PowerDownSound = CustomSound("PowerDown", 0.74f, 0.05f, 2);
    private static readonly SoundStyle KaioKenActivationSound = CustomSound("ChargeStart", 0.68f, 0.08f, 2);
    private static readonly SoundStyle KiBlastSound = CustomSound("KiBlast", 0.55f, 0.16f, 4);
    private static readonly SoundStyle ElectricSustainSound = CustomSound("ElectricLoop", 0.42f, 0.1f, 3);
    private static readonly SoundStyle HeavyImpactSound = CustomSound("HeavyImpact", 0.7f, 0.08f, 3);
    private static readonly SoundStyle MeleeImpactSound = CustomSound("MeleeImpact", 0.62f, 0.12f, 4);

    public static void PlayPowerUpStart(Vector2 position)
    {
        Play(PowerUpStartSound, SoundID.Item29, position);
    }

    public static void PlayTransformationComplete(Vector2 position)
    {
        Play(TransformationCompleteSound, SoundID.Item14, position);
    }

    public static void PlayPowerDown(Vector2 position)
    {
        Play(PowerDownSound, SoundID.Item8, position);
    }

    public static void PlayKaioKenActivation(Vector2 position)
    {
        Play(KaioKenActivationSound, SoundID.Item29, position);
    }

    public static void PlayLowKiFizzle(Vector2 position)
    {
        Play(PowerDownSound, SoundID.Item16, position);
    }

    public static void PlayTechniqueFire(Vector2 position, KiTechniqueDefinition technique)
    {
        Play(technique.Behavior switch
        {
            KiTechniqueBehavior.Beam => KaioKenActivationSound,
            KiTechniqueBehavior.SteeringDisk => ElectricSustainSound,
            KiTechniqueBehavior.HeavyBlast => TransformationCompleteSound,
            KiTechniqueBehavior.Barrage => KiBlastSound,
            _ => KiBlastSound
        }, GetTechniqueFireFallback(technique), position);
    }

    public static void PlayTechniqueImpact(Vector2 position, KiTechniqueDefinition technique)
    {
        Play(technique.Category switch
        {
            KiTechniqueCategory.Ultimate => HeavyImpactSound,
            KiTechniqueCategory.HeavyBlast => HeavyImpactSound,
            KiTechniqueCategory.CuttingDisk => ElectricSustainSound,
            KiTechniqueCategory.ContinuousBeam => ElectricSustainSound,
            _ => KiBlastSound
        }, GetTechniqueImpactFallback(technique), position);
    }

    public static void PlayMeleeImpact(Vector2 position)
    {
        Play(MeleeImpactSound, SoundID.Item10, position);
    }

    private static SoundStyle CustomSound(string assetName, float volume, float pitchVariance, int maxInstances)
    {
        return new SoundStyle($"KiAscension/Assets/Sounds/{assetName}")
        {
            Volume = volume,
            PitchVariance = pitchVariance,
            MaxInstances = maxInstances,
            SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
        };
    }

    private static SoundStyle GetTechniqueFireFallback(KiTechniqueDefinition technique)
    {
        return technique.Behavior switch
        {
            KiTechniqueBehavior.Beam => SoundID.Item13,
            KiTechniqueBehavior.SteeringDisk => SoundID.Item1,
            KiTechniqueBehavior.HeavyBlast => SoundID.Item20,
            KiTechniqueBehavior.Barrage => SoundID.Item33,
            _ => SoundID.Item20
        };
    }

    private static SoundStyle GetTechniqueImpactFallback(KiTechniqueDefinition technique)
    {
        return technique.Category switch
        {
            KiTechniqueCategory.Ultimate => SoundID.Item14,
            KiTechniqueCategory.HeavyBlast => SoundID.Item14,
            KiTechniqueCategory.CuttingDisk => SoundID.Item10,
            KiTechniqueCategory.ContinuousBeam => SoundID.Item93,
            _ => SoundID.Item10
        };
    }

    private static void Play(SoundStyle sound, SoundStyle fallbackSound, Vector2 position)
    {
        if (Main.dedServ)
        {
            return;
        }

        try
        {
            SoundEngine.PlaySound(sound, position);
        }
        catch (Exception)
        {
            SoundEngine.PlaySound(fallbackSound, position);
        }
    }
}

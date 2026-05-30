using KiAscension.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace KiAscension.Systems;

public static class KiSoundSystem
{
    public static void PlayPowerUpStart(Vector2 position)
    {
        Play(SoundID.Item29, position);
    }

    public static void PlayTransformationComplete(Vector2 position)
    {
        Play(SoundID.Item14, position);
    }

    public static void PlayPowerDown(Vector2 position)
    {
        Play(SoundID.Item8, position);
    }

    public static void PlayLowKiFizzle(Vector2 position)
    {
        Play(SoundID.Item16, position);
    }

    public static void PlayTechniqueFire(Vector2 position, KiTechniqueDefinition technique)
    {
        Play(technique.Behavior switch
        {
            KiTechniqueBehavior.Beam => SoundID.Item13,
            KiTechniqueBehavior.SteeringDisk => SoundID.Item1,
            KiTechniqueBehavior.HeavyBlast => SoundID.Item20,
            KiTechniqueBehavior.Barrage => SoundID.Item33,
            _ => SoundID.Item20
        }, position);
    }

    public static void PlayTechniqueImpact(Vector2 position, KiTechniqueDefinition technique)
    {
        Play(technique.Category switch
        {
            KiTechniqueCategory.Ultimate => SoundID.Item14,
            KiTechniqueCategory.HeavyBlast => SoundID.Item14,
            KiTechniqueCategory.CuttingDisk => SoundID.Item10,
            KiTechniqueCategory.ContinuousBeam => SoundID.Item93,
            _ => SoundID.Item10
        }, position);
    }

    public static void PlayMeleeImpact(Vector2 position)
    {
        Play(SoundID.Item10, position);
    }

    private static void Play(SoundStyle sound, Vector2 position)
    {
        if (Main.dedServ)
        {
            return;
        }

        SoundEngine.PlaySound(sound, position);
    }
}

using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public readonly struct KiTechniqueDefinition
{
    public KiTechniqueDefinition(
        KiTechnique technique,
        string displayName,
        AscensionStage requiredStage,
        int requiredExperience,
        int baseDamage,
        int kiCost,
        int useTime,
        float shootSpeed,
        float knockback,
        int penetration,
        int timeLeft,
        float projectileScale,
        Color color,
        string progressionNote)
    {
        Technique = technique;
        DisplayName = displayName;
        RequiredStage = requiredStage;
        RequiredExperience = requiredExperience;
        BaseDamage = baseDamage;
        KiCost = kiCost;
        UseTime = useTime;
        ShootSpeed = shootSpeed;
        Knockback = knockback;
        Penetration = penetration;
        TimeLeft = timeLeft;
        ProjectileScale = projectileScale;
        Color = color;
        ProgressionNote = progressionNote;
    }

    public KiTechnique Technique { get; }

    public string DisplayName { get; }

    public AscensionStage RequiredStage { get; }

    public int RequiredExperience { get; }

    public int BaseDamage { get; }

    public int KiCost { get; }

    public int UseTime { get; }

    public float ShootSpeed { get; }

    public float Knockback { get; }

    public int Penetration { get; }

    public int TimeLeft { get; }

    public float ProjectileScale { get; }

    public Color Color { get; }

    public string ProgressionNote { get; }
}

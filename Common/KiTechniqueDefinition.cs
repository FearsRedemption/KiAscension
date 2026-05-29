using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public readonly struct KiTechniqueDefinition
{
    public KiTechniqueDefinition(
        KiTechnique technique,
        KiTechniqueBehavior behavior,
        string displayName,
        AscensionStage requiredStage,
        int requiredKiPower,
        int baseDamage,
        int initialKiCost,
        int channelKiCostPerSecond,
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
        Behavior = behavior;
        DisplayName = displayName;
        RequiredStage = requiredStage;
        RequiredKiPower = requiredKiPower;
        BaseDamage = baseDamage;
        InitialKiCost = initialKiCost;
        ChannelKiCostPerSecond = channelKiCostPerSecond;
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

    public KiTechniqueBehavior Behavior { get; }

    public string DisplayName { get; }

    public AscensionStage RequiredStage { get; }

    public int RequiredKiPower { get; }

    public int BaseDamage { get; }

    public int InitialKiCost { get; }

    public int ChannelKiCostPerSecond { get; }

    public int UseTime { get; }

    public float ShootSpeed { get; }

    public float Knockback { get; }

    public int Penetration { get; }

    public int TimeLeft { get; }

    public float ProjectileScale { get; }

    public Color Color { get; }

    public string ProgressionNote { get; }
}

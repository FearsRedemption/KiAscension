using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public readonly struct StageDefinition
{
    public StageDefinition(
        AscensionStage stage,
        string displayName,
        int requiredExperience,
        AscensionProgressionGate requiredGate,
        bool requiresWitnessLoss,
        float damageMultiplier,
        float speedMultiplier,
        int defenseBonus,
        int maxKiBonus,
        int kiRegenBonus,
        int kiDrainPerSecond,
        int lifeRegenBonus,
        float flightControlMultiplier,
        Color auraColor)
    {
        Stage = stage;
        DisplayName = displayName;
        RequiredExperience = requiredExperience;
        RequiredGate = requiredGate;
        RequiresWitnessLoss = requiresWitnessLoss;
        DamageMultiplier = damageMultiplier;
        SpeedMultiplier = speedMultiplier;
        DefenseBonus = defenseBonus;
        MaxKiBonus = maxKiBonus;
        KiRegenBonus = kiRegenBonus;
        KiDrainPerSecond = kiDrainPerSecond;
        LifeRegenBonus = lifeRegenBonus;
        FlightControlMultiplier = flightControlMultiplier;
        AuraColor = auraColor;
    }

    public AscensionStage Stage { get; }

    public string DisplayName { get; }

    public int RequiredExperience { get; }

    public AscensionProgressionGate RequiredGate { get; }

    public bool RequiresWitnessLoss { get; }

    public float DamageMultiplier { get; }

    public float SpeedMultiplier { get; }

    public int DefenseBonus { get; }

    public int MaxKiBonus { get; }

    public int KiRegenBonus { get; }

    public int KiDrainPerSecond { get; }

    public int LifeRegenBonus { get; }

    public float FlightControlMultiplier { get; }

    public Color AuraColor { get; }
}


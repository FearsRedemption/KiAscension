using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public readonly struct StageDefinition
{
    public StageDefinition(
        AscensionStage stage,
        string displayName,
        int requiredExperience,
        bool requiresWitnessLoss,
        float damageMultiplier,
        float speedMultiplier,
        int defenseBonus,
        int maxKiBonus,
        int kiDrainPerSecond,
        Color auraColor)
    {
        Stage = stage;
        DisplayName = displayName;
        RequiredExperience = requiredExperience;
        RequiresWitnessLoss = requiresWitnessLoss;
        DamageMultiplier = damageMultiplier;
        SpeedMultiplier = speedMultiplier;
        DefenseBonus = defenseBonus;
        MaxKiBonus = maxKiBonus;
        KiDrainPerSecond = kiDrainPerSecond;
        AuraColor = auraColor;
    }

    public AscensionStage Stage { get; }

    public string DisplayName { get; }

    public int RequiredExperience { get; }

    public bool RequiresWitnessLoss { get; }

    public float DamageMultiplier { get; }

    public float SpeedMultiplier { get; }

    public int DefenseBonus { get; }

    public int MaxKiBonus { get; }

    public int KiDrainPerSecond { get; }

    public Color AuraColor { get; }
}


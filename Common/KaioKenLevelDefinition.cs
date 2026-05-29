using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public readonly struct KaioKenLevelDefinition
{
    public KaioKenLevelDefinition(
        string displayName,
        int requiredPower,
        AscensionProgressionGate requiredGate,
        float damageMultiplier,
        float speedMultiplier,
        int kiDrainPerSecond,
        int lifeDrainPerSecond,
        Color auraColor)
    {
        DisplayName = displayName;
        RequiredPower = requiredPower;
        RequiredGate = requiredGate;
        DamageMultiplier = damageMultiplier;
        SpeedMultiplier = speedMultiplier;
        KiDrainPerSecond = kiDrainPerSecond;
        LifeDrainPerSecond = lifeDrainPerSecond;
        AuraColor = auraColor;
    }

    public string DisplayName { get; }

    public int RequiredPower { get; }

    public AscensionProgressionGate RequiredGate { get; }

    public float DamageMultiplier { get; }

    public float SpeedMultiplier { get; }

    public int KiDrainPerSecond { get; }

    public int LifeDrainPerSecond { get; }

    public Color AuraColor { get; }
}

namespace KiAscension.Common;

public readonly struct KiResourceSnapshot
{
    public KiResourceSnapshot(
        int maxKi,
        int regenPerSecond,
        int passiveDrainPerSecond,
        float techniqueCostMultiplier)
    {
        MaxKi = maxKi;
        RegenPerSecond = regenPerSecond;
        PassiveDrainPerSecond = passiveDrainPerSecond;
        TechniqueCostMultiplier = techniqueCostMultiplier;
    }

    public int MaxKi { get; }

    public int RegenPerSecond { get; }

    public int PassiveDrainPerSecond { get; }

    public int NetRegenPerSecond => RegenPerSecond - PassiveDrainPerSecond;

    public float TechniqueCostMultiplier { get; }
}

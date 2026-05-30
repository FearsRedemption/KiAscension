namespace KiAscension.Common;

public readonly struct TrainingSourceDefinition
{
    public TrainingSourceDefinition(
        TrainingSource source,
        string displayName,
        int physicalPowerGain,
        int kiPowerGain,
        int physicalPowerCap,
        int kiPowerCap,
        string outgrownMessage)
    {
        Source = source;
        DisplayName = displayName;
        PhysicalPowerGain = physicalPowerGain;
        KiPowerGain = kiPowerGain;
        PhysicalPowerCap = physicalPowerCap;
        KiPowerCap = kiPowerCap;
        OutgrownMessage = outgrownMessage;
    }

    public TrainingSource Source { get; }

    public string DisplayName { get; }

    public int PhysicalPowerGain { get; }

    public int KiPowerGain { get; }

    public int PhysicalPowerCap { get; }

    public int KiPowerCap { get; }

    public string OutgrownMessage { get; }
}

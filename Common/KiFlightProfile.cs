namespace KiAscension.Common;

public readonly struct KiFlightProfile
{
    public KiFlightProfile(
        bool allowsTrueFlight,
        float maxHorizontalSpeed,
        float horizontalAcceleration,
        float maxRiseSpeed,
        float verticalControl,
        float hoverFallSpeed,
        int kiDrainPerSecond,
        string displayNote)
    {
        AllowsTrueFlight = allowsTrueFlight;
        MaxHorizontalSpeed = maxHorizontalSpeed;
        HorizontalAcceleration = horizontalAcceleration;
        MaxRiseSpeed = maxRiseSpeed;
        VerticalControl = verticalControl;
        HoverFallSpeed = hoverFallSpeed;
        KiDrainPerSecond = kiDrainPerSecond;
        DisplayNote = displayNote;
    }

    public bool AllowsTrueFlight { get; }

    public float MaxHorizontalSpeed { get; }

    public float HorizontalAcceleration { get; }

    public float MaxRiseSpeed { get; }

    public float VerticalControl { get; }

    public float HoverFallSpeed { get; }

    public int KiDrainPerSecond { get; }

    public string DisplayNote { get; }
}

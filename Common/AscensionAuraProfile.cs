using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public readonly struct AscensionAuraProfile
{
    public AscensionAuraProfile(
        Color primaryColor,
        Color secondaryColor,
        int dustType,
        float lightStrength,
        float dustScale,
        int idleDustChance,
        bool emitsElectricArcs,
        string visualNote)
    {
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        DustType = dustType;
        LightStrength = lightStrength;
        DustScale = dustScale;
        IdleDustChance = idleDustChance;
        EmitsElectricArcs = emitsElectricArcs;
        VisualNote = visualNote;
    }

    public Color PrimaryColor { get; }

    public Color SecondaryColor { get; }

    public int DustType { get; }

    public float LightStrength { get; }

    public float DustScale { get; }

    public int IdleDustChance { get; }

    public bool EmitsElectricArcs { get; }

    public string VisualNote { get; }
}

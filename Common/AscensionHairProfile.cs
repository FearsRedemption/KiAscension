using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public readonly struct AscensionHairProfile
{
    public AscensionHairProfile(
        AscensionHairStyle hairStyle,
        Color tint,
        bool useNaturalColor,
        string visualNote)
    {
        HairStyle = hairStyle;
        Tint = tint;
        UseNaturalColor = useNaturalColor;
        VisualNote = visualNote;
    }

    public AscensionHairStyle HairStyle { get; }

    public Color Tint { get; }

    public bool UseNaturalColor { get; }

    public string VisualNote { get; }
}

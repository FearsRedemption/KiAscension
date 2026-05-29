namespace KiAscension.Common;

public static class KiFlightProfiles
{
    public static KiFlightProfile Get(AscensionStage stage)
    {
        return stage switch
        {
            AscensionStage.SuperSaiyan => new(true, 6.2f, 0.18f, 4.8f, 0.16f, 1.2f, 8, "Early true flight. Strong, but ki hungry."),
            AscensionStage.SuperSaiyan2 => new(true, 6.8f, 0.22f, 5.3f, 0.19f, 1f, 10, "Sharper air control with higher strain."),
            AscensionStage.SuperSaiyan3 => new(true, 7.3f, 0.24f, 5.7f, 0.2f, 0.9f, 16, "Heavy power flight with brutal sustain cost."),
            AscensionStage.SuperSaiyanGod => new(true, 7.6f, 0.25f, 5.9f, 0.23f, 0.75f, 7, "Calmer divine flight with better efficiency."),
            AscensionStage.SuperSaiyanBlue => new(true, 8.5f, 0.29f, 6.4f, 0.26f, 0.7f, 13, "Fast controlled flight, still expensive."),
            AscensionStage.UltraInstinctSign => new(true, 9.4f, 0.34f, 7f, 0.31f, 0.55f, 9, "Highly responsive evasive flight."),
            AscensionStage.UltraInstinct => new(true, 10.2f, 0.38f, 7.5f, 0.36f, 0.45f, 5, "Mastered efficient endgame flight."),
            _ => new(false, 0f, 0f, 0f, 0f, 0f, 0, stage == AscensionStage.Awakened ? "Awakened movement only. True flight starts at Super Saiyan." : "True flight locked.")
        };
    }
}

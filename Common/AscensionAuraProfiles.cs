using Microsoft.Xna.Framework;
using Terraria.ID;

namespace KiAscension.Common;

public static class AscensionAuraProfiles
{
    public static AscensionAuraProfile Get(AscensionStage stage)
    {
        return stage switch
        {
            AscensionStage.Awakened => new(new Color(255, 244, 120), new Color(255, 255, 210), DustID.GemTopaz, 0.26f, 0.9f, 5, false, "Faint pale-gold shimmer."),
            AscensionStage.SuperSaiyan => new(new Color(255, 224, 80), new Color(255, 255, 180), DustID.GemTopaz, 0.45f, 1.05f, 4, false, "Sharp rising gold aura."),
            AscensionStage.SuperSaiyan2 => new(new Color(255, 240, 110), Color.White, DustID.GemTopaz, 0.55f, 1.15f, 3, true, "Brighter gold aura with electric arcs."),
            AscensionStage.SuperSaiyan3 => new(new Color(255, 200, 60), Color.White, DustID.GemTopaz, 0.65f, 1.3f, 2, true, "Large heavy gold aura."),
            AscensionStage.SuperSaiyanGod => new(new Color(245, 55, 80), new Color(255, 150, 170), DustID.Torch, 0.48f, 1.05f, 4, false, "Cleaner red divine aura."),
            AscensionStage.SuperSaiyanBlue => new(new Color(70, 210, 255), Color.White, DustID.GemSapphire, 0.58f, 1.2f, 3, false, "Controlled blue aura."),
            AscensionStage.UltraInstinctSign => new(new Color(200, 220, 255), new Color(90, 120, 180), DustID.GemDiamond, 0.62f, 1.2f, 3, true, "Unstable silver-blue shimmer."),
            AscensionStage.UltraInstinct => new(new Color(235, 245, 255), Color.White, DustID.GemDiamond, 0.72f, 1.35f, 2, true, "Clean mastered silver-white aura."),
            _ => new(new Color(255, 244, 175), new Color(255, 255, 220), DustID.GemTopaz, 0.14f, 0.75f, 7, false, "Minimal base ki presence.")
        };
    }
}

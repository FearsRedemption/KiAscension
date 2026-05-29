using System;

namespace KiAscension.Common;

public static class KiResourceMath
{
    private const float MinimumTechniqueCostMultiplier = 0.68f;
    private const float GodKiEfficiencyBonus = 0.08f;
    private const float UltraInstinctEfficiencyBonus = 0.12f;

    public static float GetTechniqueCostMultiplier(int kiPowerLevel, AscensionStage stage)
    {
        float multiplier = 1f - Math.Max(0, kiPowerLevel - 1) * 0.012f;

        if (stage is AscensionStage.SuperSaiyanGod or AscensionStage.SuperSaiyanBlue)
        {
            multiplier -= GodKiEfficiencyBonus;
        }
        else if (stage is AscensionStage.UltraInstinctSign or AscensionStage.UltraInstinct)
        {
            multiplier -= UltraInstinctEfficiencyBonus;
        }

        return Math.Clamp(multiplier, MinimumTechniqueCostMultiplier, 1f);
    }

    public static int ScaleTechniqueCost(int baseCost, int kiPowerLevel, AscensionStage stage)
    {
        if (baseCost <= 0)
        {
            return 0;
        }

        return Math.Max(1, (int)Math.Ceiling(baseCost * GetTechniqueCostMultiplier(kiPowerLevel, stage)));
    }

    public static int ScaleChannelCostForTicks(int baseCostPerSecond, int ticks, int kiPowerLevel, AscensionStage stage)
    {
        if (baseCostPerSecond <= 0 || ticks <= 0)
        {
            return 0;
        }

        float scaledPerSecond = baseCostPerSecond * GetTechniqueCostMultiplier(kiPowerLevel, stage);
        return Math.Max(1, (int)Math.Ceiling(scaledPerSecond * ticks / 60f));
    }
}

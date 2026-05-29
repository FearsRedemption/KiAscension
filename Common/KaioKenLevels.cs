using System;
using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public static class KaioKenLevels
{
    public static readonly KaioKenLevelDefinition[] Definitions =
    {
        new("Off", 0, AscensionProgressionGate.None, 1f, 1f, 0, 0, Color.Transparent),
        new("Kaio-Ken", 700, AscensionProgressionGate.None, 1.1f, 1.06f, 1, 1, new Color(255, 70, 55)),
        new("Kaio-Ken x2", 1100, AscensionProgressionGate.None, 1.16f, 1.08f, 2, 1, new Color(255, 65, 55)),
        new("Kaio-Ken x3", 1800, AscensionProgressionGate.EyeOfCthulhu, 1.24f, 1.1f, 3, 2, new Color(255, 55, 50)),
        new("Kaio-Ken x5", 3400, AscensionProgressionGate.WorldEvilBoss, 1.36f, 1.14f, 4, 3, new Color(255, 45, 45)),
        new("Kaio-Ken x10", 6500, AscensionProgressionGate.Skeletron, 1.55f, 1.2f, 6, 4, new Color(255, 40, 40)),
        new("Kaio-Ken x20", 12000, AscensionProgressionGate.WallOfFlesh, 1.8f, 1.26f, 8, 6, new Color(255, 35, 35)),
        new("Kaio-Ken x50", 24000, AscensionProgressionGate.MechanicalBoss, 2.1f, 1.34f, 11, 8, new Color(255, 30, 35)),
        new("Kaio-Ken x100", 42000, AscensionProgressionGate.Plantera, 2.45f, 1.42f, 14, 11, new Color(255, 25, 35)),
        new("Kaio-Ken x200", 72000, AscensionProgressionGate.MoonLord, 2.9f, 1.52f, 18, 15, new Color(255, 20, 35))
    };

    public static int MaxLevelIndex => Definitions.Length - 1;

    public static KaioKenLevelDefinition Get(int levelIndex)
    {
        return Definitions[Math.Clamp(levelIndex, 0, MaxLevelIndex)];
    }

    public static int GetHighestUnlockedIndex(int totalPowerExperience)
    {
        int highest = 0;

        for (int i = 1; i < Definitions.Length; i++)
        {
            KaioKenLevelDefinition level = Definitions[i];

            if (totalPowerExperience < level.RequiredPower || !AscensionStages.IsGateSatisfied(level.RequiredGate))
            {
                continue;
            }

            highest = i;
        }

        return highest;
    }
}

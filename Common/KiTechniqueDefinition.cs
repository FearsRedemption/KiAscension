using Microsoft.Xna.Framework;

namespace KiAscension.Common;

public readonly struct KiTechniqueDefinition
{
    public KiTechniqueDefinition(
        KiTechnique technique,
        KiTechniqueBehavior behavior,
        KiTechniqueCategory category,
        KiTechniqueSourceType sourceType,
        KiTechniqueCollisionStyle collisionStyle,
        string displayName,
        AscensionStage requiredStage,
        int requiredKiPower,
        int baseDamage,
        int initialKiCost,
        int channelKiCostPerSecond,
        int useTime,
        float shootSpeed,
        float knockback,
        int penetration,
        int timeLeft,
        float projectileScale,
        Color color,
        string progressionNote)
    {
        Technique = technique;
        Behavior = behavior;
        Category = category;
        SourceType = sourceType;
        CollisionStyle = collisionStyle;
        DisplayName = displayName;
        RequiredStage = requiredStage;
        RequiredKiPower = requiredKiPower;
        BaseDamage = baseDamage;
        InitialKiCost = initialKiCost;
        ChannelKiCostPerSecond = channelKiCostPerSecond;
        UseTime = useTime;
        ShootSpeed = shootSpeed;
        Knockback = knockback;
        Penetration = penetration;
        TimeLeft = timeLeft;
        ProjectileScale = projectileScale;
        Color = color;
        ProgressionNote = progressionNote;
    }

    public KiTechnique Technique { get; }

    public KiTechniqueBehavior Behavior { get; }

    public KiTechniqueCategory Category { get; }

    public KiTechniqueSourceType SourceType { get; }

    public KiTechniqueCollisionStyle CollisionStyle { get; }

    public string DisplayName { get; }

    public AscensionStage RequiredStage { get; }

    public int RequiredKiPower { get; }

    public int BaseDamage { get; }

    public int InitialKiCost { get; }

    public int ChannelKiCostPerSecond { get; }

    public int UseTime { get; }

    public float ShootSpeed { get; }

    public float Knockback { get; }

    public int Penetration { get; }

    public int TimeLeft { get; }

    public float ProjectileScale { get; }

    public Color Color { get; }

    public string ProgressionNote { get; }

    public bool CanBeHeld => Behavior == KiTechniqueBehavior.Beam;

    public bool PiercesEnemies => Penetration < 0 || Penetration > 1;

    public bool TracksCursor => Behavior == KiTechniqueBehavior.SteeringDisk;

    public bool IgnoresTerrain => CollisionStyle is KiTechniqueCollisionStyle.GuidedPiercing
        or KiTechniqueCollisionStyle.TerrainPassingUltimate;

    public string CategoryLabel => Category switch
    {
        KiTechniqueCategory.BasicBlast => "Basic ki blast",
        KiTechniqueCategory.Barrage => "Barrage",
        KiTechniqueCategory.ContinuousBeam => "Continuous beam",
        KiTechniqueCategory.CuttingDisk => "Cutting disk",
        KiTechniqueCategory.HeavyBlast => "Heavy blast",
        KiTechniqueCategory.Ultimate => "Ultimate",
        _ => Category.ToString()
    };

    public string SourceLabel => SourceType switch
    {
        KiTechniqueSourceType.GeneralKi => "General ki",
        KiTechniqueSourceType.TurtleSchool => "Turtle School",
        KiTechniqueSourceType.GohanLine => "Gohan line",
        KiTechniqueSourceType.Namekian => "Namekian",
        KiTechniqueSourceType.VegetaLine => "Vegeta line",
        KiTechniqueSourceType.EarthlingTactical => "Earthling tactical",
        KiTechniqueSourceType.FriezaLine => "Frieza line",
        KiTechniqueSourceType.GodKi => "God ki",
        KiTechniqueSourceType.UltraInstinct => "Ultra Instinct",
        _ => SourceType.ToString()
    };

    public string CollisionLabel => CollisionStyle switch
    {
        KiTechniqueCollisionStyle.TerrainBlocked => "Terrain blocked",
        KiTechniqueCollisionStyle.SustainedLine => "Sustained line",
        KiTechniqueCollisionStyle.GuidedPiercing => "Guided piercing",
        KiTechniqueCollisionStyle.HeavyImpact => "Heavy impact",
        KiTechniqueCollisionStyle.TerrainPassingUltimate => "Terrain-passing ultimate",
        _ => CollisionStyle.ToString()
    };
}

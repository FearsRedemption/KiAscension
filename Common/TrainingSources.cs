namespace KiAscension.Common;

public static class TrainingSources
{
    public static TrainingSourceDefinition Get(TrainingSource source)
    {
        return source switch
        {
            TrainingSource.MeditationFocus => new(
                source,
                "Ki Training Focus",
                4,
                3,
                900,
                1200,
                "The focus can no longer push your limits. Seek harder training."),
            TrainingSource.WeightedGear => new(
                source,
                "Weighted Training",
                3,
                0,
                2200,
                0,
                "You have outgrown this level of weight training."),
            TrainingSource.WoodenWeightBench => new(
                source,
                "Wooden Weight Bench",
                2,
                0,
                1200,
                0,
                "This equipment can no longer push your limits."),
            TrainingSource.CopperWeightBench => new(
                source,
                "Copper Weight Bench",
                3,
                0,
                2600,
                0,
                "This equipment can no longer push your limits."),
            TrainingSource.WoodenTrainingBag => new(
                source,
                "Wooden Training Bag",
                2,
                1,
                1600,
                600,
                "This equipment can no longer push your limits."),
            TrainingSource.MeditationMat => new(
                source,
                "Meditation Mat",
                0,
                2,
                0,
                1600,
                "This equipment can no longer push your limits."),
            TrainingSource.GravityRoom => new(
                source,
                "Gravity Room",
                4,
                3,
                9000,
                9000,
                "This gravity room can no longer push your limits."),
            _ => new(source, "Training", 0, 0, 0, 0, "This training no longer helps.")
        };
    }
}

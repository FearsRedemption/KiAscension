using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Hairs;

public class SuperSaiyanHair : ModHair
{
    public override bool AvailableDuringCharacterCreation => false;

    public override void SetStaticDefaults()
    {
        HairID.Sets.DrawBackHair[Type] = true;
    }
}

public class SuperSaiyan2Hair : ModHair
{
    public override bool AvailableDuringCharacterCreation => false;

    public override void SetStaticDefaults()
    {
        HairID.Sets.DrawBackHair[Type] = true;
    }
}

public class SuperSaiyan3Hair : ModHair
{
    public override bool AvailableDuringCharacterCreation => false;

    public override void SetStaticDefaults()
    {
        HairID.Sets.DrawBackHair[Type] = true;
    }
}

public class GodHair : ModHair
{
    public override bool AvailableDuringCharacterCreation => false;
}

public class SuperSaiyanBlueHair : ModHair
{
    public override string Texture => "KiAscension/Hairs/SuperSaiyan2Hair";

    public override bool AvailableDuringCharacterCreation => false;

    public override void SetStaticDefaults()
    {
        HairID.Sets.DrawBackHair[Type] = true;
    }
}

public class UltraInstinctSignHair : ModHair
{
    public override string Texture => "KiAscension/Hairs/UltraInstinctHair";

    public override bool AvailableDuringCharacterCreation => false;

    public override void SetStaticDefaults()
    {
        HairID.Sets.DrawBackHair[Type] = true;
    }
}

public class UltraInstinctHair : ModHair
{
    public override bool AvailableDuringCharacterCreation => false;

    public override void SetStaticDefaults()
    {
        HairID.Sets.DrawBackHair[Type] = true;
    }
}

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Hairs;

public class SuperSaiyanHair : ModHair
{
    public override string AltTexture => "KiAscension/Hairs/SuperSaiyanHair_Alt";

    public override bool AvailableDuringCharacterCreation => false;
}

public class SuperSaiyan2Hair : ModHair
{
    public override string AltTexture => "KiAscension/Hairs/SuperSaiyan2Hair_Alt";

    public override bool AvailableDuringCharacterCreation => false;
}

public class SuperSaiyan3Hair : ModHair
{
    public override string AltTexture => "KiAscension/Hairs/SuperSaiyan3Hair_Alt";

    public override bool AvailableDuringCharacterCreation => false;

    public override void SetStaticDefaults()
    {
        HairID.Sets.DrawBackHair[Type] = true;
    }
}

public class GodHair : ModHair
{
    public override string AltTexture => "KiAscension/Hairs/GodHair_Alt";

    public override bool AvailableDuringCharacterCreation => false;
}

public class SuperSaiyanBlueHair : ModHair
{
    public override string AltTexture => "KiAscension/Hairs/SuperSaiyanBlueHair_Alt";

    public override bool AvailableDuringCharacterCreation => false;
}

public class UltraInstinctSignHair : ModHair
{
    public override string AltTexture => "KiAscension/Hairs/UltraInstinctSignHair_Alt";

    public override bool AvailableDuringCharacterCreation => false;
}

public class UltraInstinctHair : ModHair
{
    public override string AltTexture => "KiAscension/Hairs/UltraInstinctHair_Alt";

    public override bool AvailableDuringCharacterCreation => false;
}

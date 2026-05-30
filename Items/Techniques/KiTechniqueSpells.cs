using KiAscension.Common;
using Terraria;
using Terraria.ID;

namespace KiAscension.Items.Techniques;

public class BasicKiBlastSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.BasicKiBlast;
}

public class KiBarrageSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.KiBarrage;
}

public class MasenkoSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.Masenko;
}

public class KamehamehaSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.Kamehameha;
}

public class DeathBeamSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.DeathBeam;
}

public class DestructoDiskSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.DestructoDisk;
}

public class GalickGunSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.GalickGun;
}

public class SpecialBeamCannonSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.SpecialBeamCannon;

    protected override void AddTechniqueRecipe(Recipe recipe)
    {
        recipe.AddIngredient(ItemID.Stinger, 4);
    }
}

public class BigBangAttackSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.BigBangAttack;

    protected override void AddTechniqueRecipe(Recipe recipe)
    {
        recipe.AddIngredient(ItemID.HellstoneBar, 6);
    }
}

public class FinalFlashSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.FinalFlash;

    protected override void AddTechniqueRecipe(Recipe recipe)
    {
        recipe.AddIngredient(ItemID.SoulofLight, 4);
    }
}

public class SpiritBombSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.SpiritBomb;

    protected override void AddTechniqueRecipe(Recipe recipe)
    {
        recipe.AddIngredient(ItemID.SoulofLight, 8);
        recipe.AddIngredient(ItemID.SoulofNight, 8);
    }
}

public class GodKamehamehaSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.GodKamehameha;

    protected override void AddTechniqueRecipe(Recipe recipe)
    {
        recipe.AddIngredient(ItemID.HallowedBar, 8);
    }
}

public class UltraInstinctBarrageSpell : KiTechniqueItem
{
    protected override KiTechnique Technique => KiTechnique.UltraInstinctBarrage;

    protected override void AddTechniqueRecipe(Recipe recipe)
    {
        recipe.AddIngredient(ItemID.FragmentVortex, 6);
        recipe.AddIngredient(ItemID.FragmentNebula, 6);
    }
}

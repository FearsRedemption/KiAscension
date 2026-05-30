using System.Collections.Generic;
using KiAscension.Items;
using KiAscension.Items.Combat;
using KiAscension.Items.Techniques;
using Terraria;
using Terraria.ModLoader;

namespace KiAscension.Systems;

public class AscensionWeaponRebalance : GlobalItem
{
    private const float NonKiWeaponDamageMultiplier = 1f;

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (NonKiWeaponDamageMultiplier >= 1f || !IsReplacedProgressionWeapon(item))
        {
            return;
        }

        damage *= NonKiWeaponDamageMultiplier;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (NonKiWeaponDamageMultiplier >= 1f || !IsReplacedProgressionWeapon(item))
        {
            return;
        }

        tooltips.Add(new TooltipLine(Mod, "KiAscensionReplacedProgression", "Ki Ascension progression eventually outscales normal weapons, but they still work while training."));
    }

    private static bool IsReplacedProgressionWeapon(Item item)
    {
        return item.damage > 0
            && item.type != ModContent.ItemType<KiTrainingFocus>()
            && item.ModItem is not SaiyanStrike
            && item.ModItem is not KiTechniqueItem
            && !item.accessory
            && !item.vanity;
    }
}

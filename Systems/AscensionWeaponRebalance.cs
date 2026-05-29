using System.Collections.Generic;
using KiAscension.Items;
using KiAscension.Items.Techniques;
using Terraria;
using Terraria.ModLoader;

namespace KiAscension.Systems;

public class AscensionWeaponRebalance : GlobalItem
{
    private const float NonKiWeaponDamageMultiplier = 0.18f;

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (!IsReplacedProgressionWeapon(item))
        {
            return;
        }

        damage *= NonKiWeaponDamageMultiplier;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!IsReplacedProgressionWeapon(item))
        {
            return;
        }

        tooltips.Add(new TooltipLine(Mod, "KiAscensionReplacedProgression", "Ki Ascension progression heavily outscales normal weapon damage."));
    }

    private static bool IsReplacedProgressionWeapon(Item item)
    {
        return item.damage > 0
            && item.type != ModContent.ItemType<KiTrainingFocus>()
            && item.ModItem is not KiTechniqueItem
            && !item.accessory
            && !item.vanity;
    }
}

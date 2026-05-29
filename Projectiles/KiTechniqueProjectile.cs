using KiAscension.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Projectiles;

public class KiTechniqueProjectile : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DiamondBolt}";

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 90;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.light = 0.55f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 12;
    }

    public override void AI()
    {
        KiTechniqueDefinition technique = KiTechniques.Get((int)Projectile.ai[0]);
        Projectile.rotation = technique.Technique == KiTechnique.DestructoDisk
            ? Main.GameUpdateCount * 0.35f
            : Projectile.velocity.ToRotation();
        Lighting.AddLight(Projectile.Center, technique.Color.ToVector3() * 0.55f);

        if (Main.rand.NextBool(GetDustFrequency(technique)))
        {
            int dust = Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                GetDustType(technique),
                0f,
                0f,
                145,
                technique.Color,
                technique.ProjectileScale);

            Main.dust[dust].velocity = Projectile.velocity * -0.15f;
            Main.dust[dust].noGravity = true;
        }
    }

    public override void OnKill(int timeLeft)
    {
        KiTechniqueDefinition technique = KiTechniques.Get((int)Projectile.ai[0]);
        int burstCount = technique.Technique is KiTechnique.BigBangAttack or KiTechnique.SpiritBomb ? 24 : 10;

        for (int i = 0; i < burstCount; i++)
        {
            int dust = Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                GetDustType(technique),
                Main.rand.NextFloat(-3f, 3f),
                Main.rand.NextFloat(-3f, 3f),
                130,
                technique.Color,
                technique.ProjectileScale);

            Main.dust[dust].noGravity = true;
        }
    }

    private static int GetDustFrequency(KiTechniqueDefinition technique)
    {
        return technique.Technique is KiTechnique.KiBarrage or KiTechnique.UltraInstinctBarrage ? 2 : 3;
    }

    private static int GetDustType(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.Kamehameha or KiTechnique.GodKamehameha => DustID.GemSapphire,
            KiTechnique.GalickGun => DustID.GemAmethyst,
            KiTechnique.DestructoDisk or KiTechnique.FinalFlash => DustID.GemTopaz,
            KiTechnique.SpiritBomb or KiTechnique.UltraInstinctBarrage => DustID.GemDiamond,
            KiTechnique.BigBangAttack => DustID.Electric,
            _ => DustID.GemTopaz
        };
    }
}

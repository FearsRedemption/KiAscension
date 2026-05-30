using System;
using KiAscension.Common;
using KiAscension.Players;
using KiAscension.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Projectiles;

public class KiTechniqueProjectile : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DiamondBolt}";

    private const float BaseBeamLength = 640f;
    private const int BeamDrainIntervalTicks = 15;

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

    public override bool ShouldUpdatePosition()
    {
        return CurrentTechnique.Behavior != KiTechniqueBehavior.Beam;
    }

    public override void AI()
    {
        KiTechniqueDefinition technique = CurrentTechnique;

        ApplyBehaviorDefaults(technique);

        switch (technique.Behavior)
        {
            case KiTechniqueBehavior.Beam:
                BeamAI(technique);
                break;
            case KiTechniqueBehavior.SteeringDisk:
                SteeringDiskAI(technique);
                break;
            case KiTechniqueBehavior.HeavyBlast:
                HeavyBlastAI(technique);
                break;
            default:
                Projectile.rotation = Projectile.velocity.ToRotation();
                break;
        }

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

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        KiTechniqueDefinition technique = CurrentTechnique;

        if (technique.Behavior != KiTechniqueBehavior.Beam)
        {
            return null;
        }

        float collisionPoint = 0f;
        return Collision.CheckAABBvLineCollision(
            new Vector2(targetHitbox.Left, targetHitbox.Top),
            new Vector2(targetHitbox.Width, targetHitbox.Height),
            GetBeamStart(),
            GetBeamEnd(technique),
            GetBeamWidth(technique),
            ref collisionPoint);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        KiTechniqueDefinition technique = CurrentTechnique;

        if (technique.Behavior != KiTechniqueBehavior.Beam)
        {
            return true;
        }

        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Vector2 start = GetBeamStart() - Main.screenPosition;
        Vector2 end = GetBeamEnd(technique) - Main.screenPosition;
        Vector2 direction = end - start;
        float length = direction.Length();
        float rotation = direction.ToRotation();
        float width = GetBeamWidth(technique);

        Main.EntitySpriteDraw(
            pixel,
            start,
            new Rectangle(0, 0, 1, 1),
            technique.Color * 0.55f,
            rotation,
            new Vector2(0f, 0.5f),
            new Vector2(length, width),
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            pixel,
            start,
            new Rectangle(0, 0, 1, 1),
            Color.White * 0.82f,
            rotation,
            new Vector2(0f, 0.5f),
            new Vector2(length, Math.Max(3f, width * 0.35f)),
            SpriteEffects.None);

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return CurrentTechnique.Color;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
        {
            return;
        }

        Player owner = Main.player[Projectile.owner];

        if (owner is null || !owner.active)
        {
            return;
        }

        owner.GetModPlayer<KiPlayer>().AddKiExperience(Math.Max(1, damageDone / 9), false);
    }

    public override void OnKill(int timeLeft)
    {
        KiTechniqueDefinition technique = CurrentTechnique;
        int burstCount = technique.Category switch
        {
            KiTechniqueCategory.Ultimate => 38,
            KiTechniqueCategory.HeavyBlast => 24,
            KiTechniqueCategory.CuttingDisk => 14,
            _ => 10
        };

        if (technique.Behavior != KiTechniqueBehavior.Beam)
        {
            KiSoundSystem.PlayTechniqueImpact(Projectile.Center, technique);
        }

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

    private KiTechniqueDefinition CurrentTechnique => KiTechniques.Get((int)Projectile.ai[0]);

    private void ApplyBehaviorDefaults(KiTechniqueDefinition technique)
    {
        if (Projectile.localAI[0] == 1f)
        {
            return;
        }

        Projectile.localAI[0] = 1f;
        Projectile.penetrate = technique.Penetration;
        Projectile.timeLeft = Math.Max(2, technique.TimeLeft);
        Projectile.scale = technique.ProjectileScale;
        Projectile.tileCollide = !technique.IgnoresTerrain;
        Projectile.localNPCHitCooldown = technique.Behavior == KiTechniqueBehavior.Beam ? 8 : 12;

        if (technique.Behavior == KiTechniqueBehavior.SteeringDisk)
        {
            Projectile.localNPCHitCooldown = 10;
        }

        if (technique.Behavior == KiTechniqueBehavior.Beam)
        {
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 6;
        }
    }

    private void BeamAI(KiTechniqueDefinition technique)
    {
        Player player = Main.player[Projectile.owner];

        if (player is null || !player.active || player.dead)
        {
            Projectile.Kill();
            return;
        }

        Vector2 origin = player.MountedCenter;

        if (Projectile.owner == Main.myPlayer)
        {
            Vector2 aim = Main.MouseWorld - origin;

            if (aim.LengthSquared() > 16f)
            {
                Projectile.velocity = Vector2.Normalize(aim);
                Projectile.netUpdate = Main.GameUpdateCount % 8UL == 0UL;
            }
        }

        Projectile.velocity = NormalizeOrDefault(Projectile.velocity, new Vector2(player.direction, 0f));
        Projectile.Center = origin + Projectile.velocity * 24f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.timeLeft = 2;

        player.ChangeDir(Projectile.velocity.X >= 0f ? 1 : -1);
        player.itemRotation = Projectile.rotation;
        player.itemTime = 2;
        player.itemAnimation = 2;

        if (!player.channel)
        {
            Projectile.Kill();
            return;
        }

        if (Main.GameUpdateCount % BeamDrainIntervalTicks == 0UL
            && Main.netMode != NetmodeID.MultiplayerClient
            && !player.GetModPlayer<KiPlayer>().TryConsumeTechniqueChannelKi(technique, BeamDrainIntervalTicks))
        {
            FizzleBeam(technique);
        }
    }

    private void SteeringDiskAI(KiTechniqueDefinition technique)
    {
        Player player = Main.player[Projectile.owner];

        if (Projectile.owner == Main.myPlayer && player is not null && player.active)
        {
            Vector2 aim = Main.MouseWorld - Projectile.Center;

            if (aim.LengthSquared() > 64f)
            {
                Vector2 desiredVelocity = Vector2.Normalize(aim) * technique.ShootSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.085f);
                Projectile.netUpdate = Main.GameUpdateCount % 10UL == 0UL;
            }
        }

        Projectile.rotation += 0.45f * Math.Sign(Projectile.velocity.X == 0f ? 1f : Projectile.velocity.X);
    }

    private void HeavyBlastAI(KiTechniqueDefinition technique)
    {
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (technique.Technique == KiTechnique.SpiritBomb)
        {
            Projectile.velocity *= 0.988f;
        }
    }

    private Vector2 GetBeamStart()
    {
        Player player = Main.player[Projectile.owner];
        return player is null || !player.active ? Projectile.Center : player.MountedCenter;
    }

    private Vector2 GetBeamEnd(KiTechniqueDefinition technique)
    {
        Vector2 start = GetBeamStart();
        Vector2 direction = NormalizeOrDefault(Projectile.velocity, Vector2.UnitX);
        float beamLength = BaseBeamLength + technique.ProjectileScale * 80f;

        if (!technique.IgnoresTerrain)
        {
            beamLength = GetTerrainBlockedDistance(start, direction, beamLength);
        }

        return start + direction * beamLength;
    }

    private static float GetBeamWidth(KiTechniqueDefinition technique)
    {
        return 12f + technique.ProjectileScale * 8f;
    }

    private static Vector2 NormalizeOrDefault(Vector2 value, Vector2 fallback)
    {
        return value.LengthSquared() <= 0.001f ? fallback : Vector2.Normalize(value);
    }

    private void FizzleBeam(KiTechniqueDefinition technique)
    {
        KiSoundSystem.PlayLowKiFizzle(Projectile.Center);

        if (!Main.dedServ)
        {
            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    GetDustType(technique),
                    Main.rand.NextFloat(-1.2f, 1.2f),
                    Main.rand.NextFloat(-1.2f, 1.2f),
                    160,
                    technique.Color,
                    0.8f);

                Main.dust[dust].noGravity = true;
            }
        }

        Projectile.Kill();
    }

    private static float GetTerrainBlockedDistance(Vector2 start, Vector2 direction, float maxDistance)
    {
        for (float distance = 16f; distance <= maxDistance; distance += 8f)
        {
            Point tilePoint = (start + direction * distance).ToTileCoordinates();

            if (!WorldGen.InWorld(tilePoint.X, tilePoint.Y, 10))
            {
                return distance;
            }

            Tile tile = Framing.GetTileSafely(tilePoint.X, tilePoint.Y);

            if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
            {
                return Math.Max(16f, distance - 8f);
            }
        }

        return maxDistance;
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
            KiTechnique.DeathBeam => DustID.PinkTorch,
            KiTechnique.SpecialBeamCannon => DustID.GreenTorch,
            KiTechnique.Masenko => DustID.YellowTorch,
            KiTechnique.BigBangAttack => DustID.Electric,
            _ => DustID.GemTopaz
        };
    }
}

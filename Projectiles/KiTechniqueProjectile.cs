using System;
using KiAscension.Common;
using KiAscension.Players;
using KiAscension.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.Projectiles;

public class KiTechniqueProjectile : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DiamondBolt}";

    private const int BeamDrainIntervalTicks = 15;
    private const int SpiritBombMaxChargeTicks = 180;

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
        KiTechniqueDefinition technique = CurrentTechnique;
        return technique.Behavior != KiTechniqueBehavior.Beam && !IsSpiritBombCharging(technique);
    }

    public override bool? CanDamage()
    {
        KiTechniqueDefinition technique = CurrentTechnique;

        if (technique.Behavior == KiTechniqueBehavior.Beam && !IsBeamReleased(technique))
        {
            return false;
        }

        if (IsSpiritBombCharging(technique))
        {
            return false;
        }

        return null;
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
                GetDustScale(technique));

            Main.dust[dust].velocity = Projectile.velocity * -0.15f;
            Main.dust[dust].noGravity = true;
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        KiTechniqueDefinition technique = CurrentTechnique;

        if (technique.Behavior != KiTechniqueBehavior.Beam || !IsBeamReleased(technique))
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

        if (technique.Behavior == KiTechniqueBehavior.Beam)
        {
            DrawBeam(technique);
            return false;
        }

        DrawNonBeamProjectile(technique);
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
            KiTechniqueCategory.Ultimate => 48,
            KiTechniqueCategory.HeavyBlast => 28,
            KiTechniqueCategory.CuttingDisk => 16,
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
                GetDustScale(technique));

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
            Projectile.width = 28;
            Projectile.height = 18;
        }

        if (technique.Technique == KiTechnique.DeathBeam)
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 8;
        }

        if (technique.Technique == KiTechnique.BigBangAttack)
        {
            Projectile.width = 30;
            Projectile.height = 30;
        }

        if (technique.Technique == KiTechnique.SpiritBomb)
        {
            Projectile.timeLeft = 720;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
        }

        if (technique.Behavior == KiTechniqueBehavior.Beam)
        {
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = GetBeamHitCooldown(technique);
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

        Projectile.localAI[1]++;

        if (!IsBeamReleased(technique))
        {
            if (Main.GameUpdateCount % 24UL == 0UL)
            {
                KiSoundSystem.PlayTechniqueSustain(Projectile.Center, technique);
            }

            return;
        }

        if (Projectile.ai[1] == 0f)
        {
            Projectile.ai[1] = 1f;
            KiSoundSystem.PlayTechniqueRelease(Projectile.Center, technique);
            Projectile.netUpdate = true;
        }

        if (Main.GameUpdateCount % 42UL == 0UL)
        {
            KiSoundSystem.PlayTechniqueSustain(Projectile.Center, technique);
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
        if (technique.Technique == KiTechnique.SpiritBomb)
        {
            SpiritBombAI(technique);
            return;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        if (technique.Technique == KiTechnique.BigBangAttack)
        {
            Projectile.rotation += 0.04f;
        }
    }

    private void SpiritBombAI(KiTechniqueDefinition technique)
    {
        Player player = Main.player[Projectile.owner];

        if (player is null || !player.active || player.dead)
        {
            Projectile.Kill();
            return;
        }

        if (Projectile.localAI[1] >= 0f && player.channel && Projectile.localAI[1] < SpiritBombMaxChargeTicks)
        {
            Projectile.localAI[1]++;
            Projectile.Center = player.MountedCenter + new Vector2(0f, -54f - Projectile.localAI[1] * 0.08f);
            Projectile.velocity = Vector2.Zero;
            Projectile.scale = MathHelper.Lerp(0.8f, 3.2f, Projectile.localAI[1] / SpiritBombMaxChargeTicks);
            Projectile.width = Projectile.height = Math.Max(28, (int)(26 * Projectile.scale));
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Main.GameUpdateCount % BeamDrainIntervalTicks == 0UL
                && Main.netMode != NetmodeID.MultiplayerClient
                && !player.GetModPlayer<KiPlayer>().TryConsumeTechniqueChannelKi(technique, BeamDrainIntervalTicks))
            {
                LaunchSpiritBomb(technique);
            }

            if (Main.GameUpdateCount % 34UL == 0UL)
            {
                KiSoundSystem.PlayTechniqueSustain(Projectile.Center, technique);
            }

            return;
        }

        if (Projectile.localAI[1] >= 0f)
        {
            LaunchSpiritBomb(technique);
        }

        Projectile.rotation += 0.02f;
        Projectile.velocity *= 0.994f;
    }

    private void LaunchSpiritBomb(KiTechniqueDefinition technique)
    {
        float chargeTicks = Math.Max(1f, Projectile.localAI[1]);
        float chargeProgress = MathHelper.Clamp(chargeTicks / SpiritBombMaxChargeTicks, 0.2f, 1f);
        Vector2 direction = new((float)Math.Cos(Projectile.ai[1]), (float)Math.Sin(Projectile.ai[1]));
        Projectile.localAI[1] = -chargeTicks;
        Projectile.velocity = NormalizeOrDefault(direction, Vector2.UnitX) * MathHelper.Lerp(3.2f, technique.ShootSpeed, chargeProgress);
        Projectile.damage = Math.Max(1, (int)(Projectile.damage * MathHelper.Lerp(0.7f, 1.75f, chargeProgress)));
        Projectile.scale = MathHelper.Lerp(1.25f, 3.4f, chargeProgress);
        Projectile.width = Projectile.height = Math.Max(30, (int)(28 * Projectile.scale));
        Projectile.timeLeft = Math.Max(120, technique.TimeLeft);
        KiSoundSystem.PlayTechniqueRelease(Projectile.Center, technique);
        Projectile.netUpdate = true;
    }

    private Vector2 GetBeamStart()
    {
        Player player = Main.player[Projectile.owner];
        return player is null || !player.active ? Projectile.Center : player.MountedCenter;
    }

    private Vector2 GetBeamEnd(KiTechniqueDefinition technique)
    {
        return GetBeamEnd(technique, out _);
    }

    private Vector2 GetBeamEnd(KiTechniqueDefinition technique, out bool blockedByTerrain)
    {
        Vector2 start = GetBeamStart();
        Vector2 direction = NormalizeOrDefault(Projectile.velocity, Vector2.UnitX);
        float beamLength = GetBeamMaxLength(technique);
        blockedByTerrain = false;

        if (!technique.IgnoresTerrain)
        {
            beamLength = GetTerrainBlockedDistance(start, direction, beamLength, out blockedByTerrain);
        }

        return start + direction * beamLength;
    }

    private static float GetBeamWidth(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.SpecialBeamCannon => 8f,
            KiTechnique.FinalFlash => 30f,
            KiTechnique.GodKamehameha => 24f,
            KiTechnique.GalickGun => 18f,
            _ => 16f + technique.ProjectileScale * 4f
        };
    }

    private static float GetBeamMaxLength(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.FinalFlash => 2800f,
            KiTechnique.GodKamehameha => 3000f,
            KiTechnique.SpecialBeamCannon => 2500f,
            KiTechnique.GalickGun => 2200f,
            _ => 1900f
        };
    }

    private static int GetBeamChargeTicks(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.Kamehameha => 14,
            KiTechnique.GalickGun => 18,
            KiTechnique.SpecialBeamCannon => 32,
            KiTechnique.FinalFlash => 54,
            KiTechnique.GodKamehameha => 26,
            _ => 12
        };
    }

    private static int GetBeamHitCooldown(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.FinalFlash => 4,
            KiTechnique.SpecialBeamCannon => 5,
            _ => 6
        };
    }

    private static float GetBeamSegmentLength(KiTechniqueDefinition technique)
    {
        return technique.Technique == KiTechnique.SpecialBeamCannon ? 34f : 46f;
    }

    private static float GetBeamStreamSpeed(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.FinalFlash => 5.5f,
            KiTechnique.GalickGun => 6.2f,
            KiTechnique.SpecialBeamCannon => 7.4f,
            _ => 4.8f
        };
    }

    private bool IsBeamReleased(KiTechniqueDefinition technique)
    {
        return technique.Behavior == KiTechniqueBehavior.Beam && Projectile.localAI[1] >= GetBeamChargeTicks(technique);
    }

    private bool IsSpiritBombCharging(KiTechniqueDefinition technique)
    {
        return technique.Technique == KiTechnique.SpiritBomb && Projectile.localAI[1] >= 0f;
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

    private static float GetTerrainBlockedDistance(Vector2 start, Vector2 direction, float maxDistance, out bool blockedByTerrain)
    {
        blockedByTerrain = false;

        for (float distance = 16f; distance <= maxDistance; distance += 8f)
        {
            Point tilePoint = (start + direction * distance).ToTileCoordinates();

            if (!WorldGen.InWorld(tilePoint.X, tilePoint.Y, 10))
            {
                blockedByTerrain = true;
                return distance;
            }

            Tile tile = Framing.GetTileSafely(tilePoint.X, tilePoint.Y);

            if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
            {
                blockedByTerrain = true;
                return Math.Max(16f, distance - 8f);
            }
        }

        return maxDistance;
    }

    private static int GetDustFrequency(KiTechniqueDefinition technique)
    {
        return technique.Technique is KiTechnique.KiBarrage or KiTechnique.UltraInstinctBarrage ? 2 : 3;
    }

    private static float GetDustScale(KiTechniqueDefinition technique)
    {
        return technique.Technique == KiTechnique.SpiritBomb ? Math.Max(1.2f, ProjectileScaleFallback(technique)) : technique.ProjectileScale;
    }

    private static float ProjectileScaleFallback(KiTechniqueDefinition technique)
    {
        return Math.Max(0.8f, technique.ProjectileScale);
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

    private static Texture2D GetEffectTexture(string assetName)
    {
        try
        {
            return ModContent.Request<Texture2D>($"KiAscension/Assets/Effects/{assetName}", AssetRequestMode.ImmediateLoad).Value;
        }
        catch
        {
            return TextureAssets.MagicPixel.Value;
        }
    }

    private void DrawBeam(KiTechniqueDefinition technique)
    {
        Texture2D segmentTexture = GetEffectTexture("KiBeamSegment");
        Texture2D headTexture = GetEffectTexture("KiBeamHead");
        Texture2D impactTexture = GetEffectTexture("KiBeamImpact");
        Vector2 start = GetBeamStart() - Main.screenPosition;
        Vector2 worldEnd = GetBeamEnd(technique, out bool blockedByTerrain);
        Vector2 end = worldEnd - Main.screenPosition;
        Vector2 direction = end - start;
        float length = direction.Length();
        float rotation = direction.ToRotation();
        float width = GetBeamWidth(technique);
        Vector2 normalizedDirection = length <= 0.001f ? Vector2.UnitX : Vector2.Normalize(direction);

        DrawChargeOrb(GetBeamStart(), technique, IsBeamReleased(technique) ? 0.55f : 1f);

        if (!IsBeamReleased(technique))
        {
            return;
        }

        float segmentLength = GetBeamSegmentLength(technique);
        float streamOffset = (Main.GameUpdateCount * GetBeamStreamSpeed(technique)) % segmentLength;
        Color outerColor = technique.Color * 0.38f;
        Color coreColor = Color.White * 0.88f;

        for (float offset = -streamOffset; offset < length; offset += segmentLength)
        {
            float drawLength = Math.Min(segmentLength + 6f, length - Math.Max(0f, offset));

            if (drawLength <= 0f)
            {
                continue;
            }

            Vector2 drawPosition = start + normalizedDirection * Math.Max(0f, offset);
            Rectangle source = new(0, 0, segmentTexture.Width, segmentTexture.Height);
            Vector2 origin = new(0f, segmentTexture.Height / 2f);
            Vector2 outerScale = new(drawLength / segmentTexture.Width, width * 1.85f / segmentTexture.Height);
            Vector2 innerScale = new(drawLength / segmentTexture.Width, Math.Max(3f, width * 0.62f) / segmentTexture.Height);

            Main.EntitySpriteDraw(segmentTexture, drawPosition, source, outerColor, rotation, origin, outerScale, SpriteEffects.None);
            Main.EntitySpriteDraw(segmentTexture, drawPosition, source, coreColor, rotation, origin, innerScale, SpriteEffects.None);

            if (technique.Technique == KiTechnique.SpecialBeamCannon)
            {
                float spiral = (float)Math.Sin((offset + Main.GameUpdateCount * 8f) * 0.045f) * width * 0.42f;
                Vector2 perpendicular = normalizedDirection.RotatedBy(MathHelper.PiOver2) * spiral;
                Main.EntitySpriteDraw(segmentTexture, drawPosition + perpendicular, source, technique.Color * 0.5f, rotation, origin, new Vector2(drawLength / segmentTexture.Width, 2.4f / segmentTexture.Height), SpriteEffects.None);
            }
        }

        Main.EntitySpriteDraw(
            headTexture,
            end,
            null,
            technique.Color * 0.85f,
            rotation,
            new Vector2(headTexture.Width / 2f, headTexture.Height / 2f),
            Math.Max(0.8f, width / 18f),
            SpriteEffects.None);

        if (blockedByTerrain)
        {
            Main.EntitySpriteDraw(
                impactTexture,
                end,
                null,
                technique.Color * 0.8f,
                Main.GameUpdateCount * 0.09f,
                new Vector2(impactTexture.Width / 2f, impactTexture.Height / 2f),
                Math.Max(0.9f, width / 16f),
                SpriteEffects.None);
        }
    }

    private void DrawNonBeamProjectile(KiTechniqueDefinition technique)
    {
        if (technique.Technique == KiTechnique.DeathBeam)
        {
            DrawDeathBeamShot(technique);
            return;
        }

        Texture2D texture = technique.Behavior == KiTechniqueBehavior.SteeringDisk
            ? GetEffectTexture("KiDisk")
            : GetEffectTexture("KiOrbProjectile");

        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.2f + Projectile.whoAmI) * 0.05f;
        float scale = Projectile.scale * pulse;

        if (technique.Technique == KiTechnique.SpiritBomb)
        {
            scale = Projectile.scale * (Projectile.localAI[1] >= 0f ? 1f : 1.08f);
        }

        Main.EntitySpriteDraw(
            texture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color,
            Projectile.rotation,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None);

        if (technique.Behavior == KiTechniqueBehavior.SteeringDisk)
        {
            return;
        }

        Main.EntitySpriteDraw(
            texture,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White * 0.55f,
            -Projectile.rotation * 0.5f,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale * 0.54f,
            SpriteEffects.None);
    }

    private void DrawDeathBeamShot(KiTechniqueDefinition technique)
    {
        Texture2D segmentTexture = GetEffectTexture("KiBeamSegment");
        Vector2 direction = NormalizeOrDefault(Projectile.velocity, Vector2.UnitX);
        Vector2 start = Projectile.Center - direction * 42f - Main.screenPosition;
        Rectangle source = new(0, 0, segmentTexture.Width, segmentTexture.Height);
        Vector2 origin = new(0f, segmentTexture.Height / 2f);

        Main.EntitySpriteDraw(
            segmentTexture,
            start,
            source,
            technique.Color * 0.72f,
            direction.ToRotation(),
            origin,
            new Vector2(54f / segmentTexture.Width, 4f / segmentTexture.Height),
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            GetEffectTexture("KiBeamHead"),
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color,
            Projectile.rotation,
            new Vector2(16f, 16f),
            0.42f,
            SpriteEffects.None);
    }

    private void DrawChargeOrb(Vector2 worldPosition, KiTechniqueDefinition technique, float intensity)
    {
        Texture2D texture = GetEffectTexture("KiChargeOrb");
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.18f) * 0.08f;
        float scale = MathHelper.Clamp(intensity, 0.4f, 1.35f) * pulse * (technique.Technique == KiTechnique.FinalFlash ? 1.25f : 1f);
        Main.EntitySpriteDraw(
            texture,
            worldPosition + NormalizeOrDefault(Projectile.velocity, Vector2.UnitX) * 18f - Main.screenPosition,
            null,
            technique.Color * 0.85f,
            Main.GameUpdateCount * 0.045f,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None);
    }
}

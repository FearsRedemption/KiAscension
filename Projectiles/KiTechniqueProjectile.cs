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

    public static string GetOwnedTechniqueDebugText(Player player)
    {
        if (player is null || !player.active)
        {
            return "none";
        }

        int projectileType = ModContent.ProjectileType<KiTechniqueProjectile>();

        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile projectile = Main.projectile[i];

            if (!projectile.active || projectile.owner != player.whoAmI || projectile.type != projectileType)
            {
                continue;
            }

            KiTechniqueDefinition technique = KiTechniques.Get((int)projectile.ai[0]);

            if (technique.Technique == KiTechnique.SpiritBomb)
            {
                float chargeTicks = projectile.localAI[1] >= 0f ? projectile.localAI[1] : -projectile.localAI[1];
                int percent = (int)(MathHelper.Clamp(chargeTicks / SpiritBombMaxChargeTicks, 0f, 1f) * 100f);
                return projectile.localAI[1] >= 0f
                    ? $"{technique.DisplayName}: charging {percent}%"
                    : $"{technique.DisplayName}: launched {percent}%";
            }

            if (technique.Behavior == KiTechniqueBehavior.Beam)
            {
                int chargeTicks = GetBeamChargeTicks(technique);
                int percent = (int)(MathHelper.Clamp(projectile.localAI[1] / chargeTicks, 0f, 1f) * 100f);
                string state = IsBeamReleased(technique, projectile) ? "released" : "charging";
                return $"{technique.DisplayName}: {state} {percent}%, {GetBeamMaxLength(technique):0}px";
            }

            return $"{technique.DisplayName}: active {projectile.timeLeft}t";
        }

        return "none";
    }

    public static bool IsChannelDrainActive(Projectile projectile)
    {
        if (projectile is null || !projectile.active || projectile.type != ModContent.ProjectileType<KiTechniqueProjectile>())
        {
            return false;
        }

        KiTechniqueDefinition technique = KiTechniques.Get((int)projectile.ai[0]);

        if (technique.Technique == KiTechnique.SpiritBomb)
        {
            return projectile.localAI[1] >= 0f;
        }

        return IsBeamReleased(technique, projectile);
    }

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
                BoltAI(technique);
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
        KiTechniqueDefinition technique = CurrentTechnique;

        if (damageDone > 0)
        {
            SpawnImpactFeedback(technique, target, damageDone);

            if (Projectile.owner == Main.myPlayer
                && (technique.Behavior == KiTechniqueBehavior.Beam || technique.PiercesEnemies)
                && Main.GameUpdateCount % 18UL == 0UL)
            {
                KiSoundSystem.PlayTechniqueImpact(target.Center, technique);

                if (technique.Technique == KiTechnique.FinalFlash)
                {
                    KiFeedbackSystem.RequestScreenShake(target.Center, 2.8f, 8);
                }
            }
        }

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
        int burstCount = technique.Technique switch
        {
            KiTechnique.BasicKiBlast => 6,
            KiTechnique.KiBarrage => 4,
            KiTechnique.UltraInstinctBarrage => 5,
            KiTechnique.DeathBeam => 3,
            KiTechnique.Masenko => 14,
            KiTechnique.BigBangAttack => 38,
            KiTechnique.SpiritBomb => 72,
            _ => technique.Category switch
            {
                KiTechniqueCategory.Ultimate => 48,
                KiTechniqueCategory.HeavyBlast => 28,
                KiTechniqueCategory.CuttingDisk => 16,
                _ => 10
            }
        };

        if (technique.Behavior != KiTechniqueBehavior.Beam)
        {
            KiSoundSystem.PlayTechniqueImpact(Projectile.Center, technique);
        }

        if (technique.Technique == KiTechnique.BigBangAttack)
        {
            KiFeedbackSystem.RequestScreenShake(Projectile.Center, 4.5f, 14);
        }
        else if (technique.Technique == KiTechnique.SpiritBomb)
        {
            KiFeedbackSystem.RequestScreenShake(Projectile.Center, 9f, 26);
        }

        if (!Main.dedServ && technique.Category is KiTechniqueCategory.Ultimate or KiTechniqueCategory.HeavyBlast)
        {
            Lighting.AddLight(Projectile.Center, technique.Color.ToVector3() * (technique.Technique == KiTechnique.SpiritBomb ? 1.15f : 0.75f));
        }

        float burstSpeed = technique.Technique switch
        {
            KiTechnique.SpiritBomb => 6.5f,
            KiTechnique.BigBangAttack => 4.8f,
            KiTechnique.FinalFlash => 4.2f,
            _ => technique.Category is KiTechniqueCategory.Ultimate or KiTechniqueCategory.HeavyBlast ? 3.8f : 2.6f
        };

        float burstScale = technique.Technique switch
        {
            KiTechnique.SpiritBomb => 1.65f,
            KiTechnique.BigBangAttack => 1.35f,
            _ => technique.Category is KiTechniqueCategory.Ultimate or KiTechniqueCategory.HeavyBlast ? 1.2f : 1f
        };

        for (int i = 0; i < burstCount; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(burstSpeed, burstSpeed);
            int dust = Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                GetDustType(technique),
                velocity.X,
                velocity.Y,
                130,
                technique.Color,
                GetDustScale(technique) * burstScale);

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

        if (technique.Technique == KiTechnique.BasicKiBlast)
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.localNPCHitCooldown = 10;
        }

        if (technique.Technique == KiTechnique.KiBarrage)
        {
            Projectile.width = 9;
            Projectile.height = 9;
            Projectile.localNPCHitCooldown = 9;
            Projectile.timeLeft -= (int)Projectile.ai[1] * 2;
        }

        if (technique.Technique == KiTechnique.UltraInstinctBarrage)
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.localNPCHitCooldown = 7;
            Projectile.timeLeft -= (int)Projectile.ai[1];
        }

        if (technique.Technique == KiTechnique.Masenko)
        {
            Projectile.width = 28;
            Projectile.height = 14;
            Projectile.localNPCHitCooldown = 10;
        }

        if (technique.Behavior == KiTechniqueBehavior.SteeringDisk)
        {
            Projectile.localNPCHitCooldown = 10;
            Projectile.width = 34;
            Projectile.height = 16;
        }

        if (technique.Technique == KiTechnique.DeathBeam)
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.localNPCHitCooldown = 5;
        }

        if (technique.Technique == KiTechnique.BigBangAttack)
        {
            Projectile.width = 40;
            Projectile.height = 40;
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
            if (technique.Technique == KiTechnique.FinalFlash)
            {
                KiFeedbackSystem.RequestScreenShake(Projectile.Center, 7.5f, 22);
            }
            else if (technique.Technique == KiTechnique.GodKamehameha)
            {
                KiFeedbackSystem.RequestScreenShake(Projectile.Center, 3f, 10);
            }

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

    private void BoltAI(KiTechniqueDefinition technique)
    {
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (technique.Technique == KiTechnique.DeathBeam)
        {
            Projectile.extraUpdates = 1;
            Lighting.AddLight(Projectile.Center, technique.Color.ToVector3() * 0.7f);
            return;
        }

        if (technique.Technique == KiTechnique.UltraInstinctBarrage)
        {
            Projectile.extraUpdates = 1;
            Projectile.velocity *= 1.002f;

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, -Projectile.velocity.X * 0.15f, -Projectile.velocity.Y * 0.15f, 120, Color.White, 0.7f);
                Main.dust[dust].noGravity = true;
            }
        }
    }

    private void HeavyBlastAI(KiTechniqueDefinition technique)
    {
        if (technique.Technique == KiTechnique.SpiritBomb)
        {
            SpiritBombAI(technique);
            return;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        if (technique.Technique == KiTechnique.Masenko)
        {
            Projectile.velocity *= 1.002f;
        }

        if (technique.Technique == KiTechnique.BigBangAttack)
        {
            Projectile.localAI[1]++;
            Projectile.rotation += 0.045f;

            if (Projectile.localAI[1] <= 12f)
            {
                Projectile.velocity *= 0.93f;
                Projectile.scale = MathHelper.Lerp(0.9f, technique.ProjectileScale, Projectile.localAI[1] / 12f);
            }
            else
            {
                Projectile.velocity *= 1.003f;
            }

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 115, technique.Color, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(1.5f, 1.5f);
            }
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
            Lighting.AddLight(Projectile.Center, technique.Color.ToVector3() * (0.45f + Projectile.localAI[1] / SpiritBombMaxChargeTicks * 0.45f));

            if (Main.rand.NextBool(2))
            {
                Vector2 dustVelocity = Main.rand.NextVector2CircularEdge(2.5f, 2.5f);
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, dustVelocity.X, dustVelocity.Y, 90, technique.Color, 0.8f + Projectile.localAI[1] / SpiritBombMaxChargeTicks);
                Main.dust[dust].noGravity = true;
            }

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
        KiFeedbackSystem.RequestScreenShake(Projectile.Center, 6f, 18);
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
            KiTechnique.FinalFlash => 42f,
            KiTechnique.GodKamehameha => 26f,
            KiTechnique.GalickGun => 24f,
            KiTechnique.Kamehameha => 20f,
            _ => 16f + technique.ProjectileScale * 4f
        };
    }

    private static float GetBeamMaxLength(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.FinalFlash => 3600f,
            KiTechnique.GodKamehameha => 3400f,
            KiTechnique.SpecialBeamCannon => 2700f,
            KiTechnique.GalickGun => 2600f,
            KiTechnique.Kamehameha => 2400f,
            _ => 2000f
        };
    }

    private static int GetBeamChargeTicks(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.Kamehameha => 14,
            KiTechnique.GalickGun => 18,
            KiTechnique.SpecialBeamCannon => 36,
            KiTechnique.FinalFlash => 72,
            KiTechnique.GodKamehameha => 20,
            _ => 12
        };
    }

    private static int GetBeamHitCooldown(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.FinalFlash => 3,
            KiTechnique.SpecialBeamCannon => 5,
            _ => 6
        };
    }

    private static float GetBeamSegmentLength(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.SpecialBeamCannon => 30f,
            KiTechnique.FinalFlash => 64f,
            KiTechnique.GodKamehameha => 42f,
            _ => 46f
        };
    }

    private static float GetBeamStreamSpeed(KiTechniqueDefinition technique)
    {
        return technique.Technique switch
        {
            KiTechnique.FinalFlash => 5.5f,
            KiTechnique.GalickGun => 7f,
            KiTechnique.SpecialBeamCannon => 8.2f,
            KiTechnique.GodKamehameha => 7.6f,
            _ => 4.8f
        };
    }

    private bool IsBeamReleased(KiTechniqueDefinition technique)
    {
        return IsBeamReleased(technique, Projectile);
    }

    private static bool IsBeamReleased(KiTechniqueDefinition technique, Projectile projectile)
    {
        return technique.Behavior == KiTechniqueBehavior.Beam && projectile.localAI[1] >= GetBeamChargeTicks(technique);
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
        KiFeedbackSystem.RequestScreenShake(Projectile.Center, 2.2f, 8);

        if (Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers)
        {
            Main.player[Projectile.owner]?.GetModPlayer<KiPlayer>().ReportTechniqueFeedback($"{technique.DisplayName} fizzled: out of ki.", new Color(255, 190, 120));
        }

        if (!Main.dedServ)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.9f, 0.35f, 0.12f));

            for (int i = 0; i < 18; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2.4f, 2.4f);
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    GetDustType(technique),
                    velocity.X,
                    velocity.Y,
                    160,
                    technique.Color,
                    0.95f);

                Main.dust[dust].noGravity = true;
            }
        }

        Projectile.Kill();
    }

    private void SpawnImpactFeedback(KiTechniqueDefinition technique, NPC target, int damageDone)
    {
        if (Main.dedServ)
        {
            return;
        }

        int dustCount = technique.Technique switch
        {
            KiTechnique.BasicKiBlast => 5,
            KiTechnique.KiBarrage => 3,
            KiTechnique.UltraInstinctBarrage => 4,
            KiTechnique.DeathBeam => 3,
            KiTechnique.Masenko => 10,
            KiTechnique.BigBangAttack => 18,
            KiTechnique.SpiritBomb => 28,
            _ => technique.Category switch
            {
                KiTechniqueCategory.Ultimate => 18,
                KiTechniqueCategory.HeavyBlast => 12,
                KiTechniqueCategory.CuttingDisk => 8,
                KiTechniqueCategory.ContinuousBeam => 4,
                _ => 5
            }
        };

        float scale = technique.Technique switch
        {
            KiTechnique.DeathBeam => 0.55f,
            KiTechnique.BasicKiBlast or KiTechnique.KiBarrage => 0.7f,
            KiTechnique.SpiritBomb => 1.8f,
            KiTechnique.BigBangAttack => 1.35f,
            _ => technique.Category is KiTechniqueCategory.Ultimate or KiTechniqueCategory.HeavyBlast ? 1.25f : 0.85f
        };

        for (int i = 0; i < dustCount; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(2.2f, 2.2f);
            int dust = Dust.NewDust(target.position, target.width, target.height, GetDustType(technique), velocity.X, velocity.Y, 110, technique.Color, scale);
            Main.dust[dust].noGravity = true;
        }

        if (technique.Category is KiTechniqueCategory.Ultimate or KiTechniqueCategory.HeavyBlast && damageDone > 0)
        {
            Lighting.AddLight(target.Center, technique.Color.ToVector3() * 0.5f);
        }
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
        Color outerColor = technique.Color * (technique.Technique == KiTechnique.FinalFlash ? 0.52f : 0.38f);
        Color coreColor = technique.Technique == KiTechnique.GodKamehameha ? new Color(255, 235, 245) * 0.94f : Color.White * 0.88f;

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

            if (technique.Technique == KiTechnique.GalickGun)
            {
                float flicker = (float)Math.Sin((offset + Main.GameUpdateCount * 11f) * 0.055f) * width * 0.18f;
                Vector2 perpendicular = normalizedDirection.RotatedBy(MathHelper.PiOver2) * flicker;
                Main.EntitySpriteDraw(segmentTexture, drawPosition + perpendicular, source, new Color(255, 120, 255) * 0.42f, rotation, origin, new Vector2(drawLength / segmentTexture.Width, width * 0.34f / segmentTexture.Height), SpriteEffects.None);
            }

            if (technique.Technique == KiTechnique.SpecialBeamCannon)
            {
                float spiral = (float)Math.Sin((offset + Main.GameUpdateCount * 8f) * 0.045f) * width * 0.42f;
                Vector2 perpendicular = normalizedDirection.RotatedBy(MathHelper.PiOver2) * spiral;
                Main.EntitySpriteDraw(segmentTexture, drawPosition + perpendicular, source, technique.Color * 0.5f, rotation, origin, new Vector2(drawLength / segmentTexture.Width, 2.4f / segmentTexture.Height), SpriteEffects.None);
                Main.EntitySpriteDraw(segmentTexture, drawPosition - perpendicular, source, Color.White * 0.32f, rotation, origin, new Vector2(drawLength / segmentTexture.Width, 1.7f / segmentTexture.Height), SpriteEffects.None);
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
                Math.Max(0.9f, width / 14f),
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

        if (technique.Technique == KiTechnique.Masenko)
        {
            DrawMasenkoShot(technique);
            return;
        }

        if (technique.Technique == KiTechnique.BigBangAttack)
        {
            DrawBigBangShot(technique);
            return;
        }

        if (technique.Technique == KiTechnique.SpiritBomb)
        {
            DrawSpiritBombShot(technique);
            return;
        }

        if (technique.Behavior == KiTechniqueBehavior.SteeringDisk)
        {
            DrawDiskShot(technique);
            return;
        }

        if (technique.Behavior == KiTechniqueBehavior.Barrage)
        {
            DrawBarrageShot(technique);
            return;
        }

        DrawOrbShot(technique);
    }

    private void DrawOrbShot(KiTechniqueDefinition technique)
    {
        Texture2D texture = GetEffectTexture("KiOrbProjectile");
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.2f + Projectile.whoAmI) * 0.04f;
        float scale = Projectile.scale * pulse;

        Main.EntitySpriteDraw(
            texture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color,
            Projectile.rotation,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None);

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

    private void DrawBarrageShot(KiTechniqueDefinition technique)
    {
        Texture2D segmentTexture = GetEffectTexture("KiBeamSegment");
        Texture2D orbTexture = GetEffectTexture("KiOrbProjectile");
        Vector2 direction = NormalizeOrDefault(Projectile.velocity, Vector2.UnitX);
        float length = technique.Technique == KiTechnique.UltraInstinctBarrage ? 72f : 38f;
        float width = technique.Technique == KiTechnique.UltraInstinctBarrage ? 4.5f : 7f;
        Color trailColor = technique.Technique == KiTechnique.UltraInstinctBarrage ? Color.White * 0.55f : technique.Color * 0.5f;
        Rectangle source = new(0, 0, segmentTexture.Width, segmentTexture.Height);
        Vector2 origin = new(0f, segmentTexture.Height / 2f);

        Main.EntitySpriteDraw(
            segmentTexture,
            Projectile.Center - direction * length - Main.screenPosition,
            source,
            trailColor,
            direction.ToRotation(),
            origin,
            new Vector2(length / segmentTexture.Width, width / segmentTexture.Height),
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            orbTexture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color,
            Projectile.rotation,
            new Vector2(orbTexture.Width / 2f, orbTexture.Height / 2f),
            Projectile.scale,
            SpriteEffects.None);
    }

    private void DrawMasenkoShot(KiTechniqueDefinition technique)
    {
        Texture2D segmentTexture = GetEffectTexture("KiBeamSegment");
        Texture2D headTexture = GetEffectTexture("KiBeamHead");
        Vector2 direction = NormalizeOrDefault(Projectile.velocity, Vector2.UnitX);
        float length = 76f;
        Rectangle source = new(0, 0, segmentTexture.Width, segmentTexture.Height);
        Vector2 origin = new(0f, segmentTexture.Height / 2f);

        Main.EntitySpriteDraw(
            segmentTexture,
            Projectile.Center - direction * length * 0.75f - Main.screenPosition,
            source,
            technique.Color * 0.72f,
            direction.ToRotation(),
            origin,
            new Vector2(length / segmentTexture.Width, 16f / segmentTexture.Height),
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            segmentTexture,
            Projectile.Center - direction * length * 0.55f - Main.screenPosition,
            source,
            Color.White * 0.62f,
            direction.ToRotation(),
            origin,
            new Vector2(length * 0.66f / segmentTexture.Width, 6f / segmentTexture.Height),
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            headTexture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color,
            Projectile.rotation,
            new Vector2(headTexture.Width / 2f, headTexture.Height / 2f),
            0.72f,
            SpriteEffects.None);
    }

    private void DrawDiskShot(KiTechniqueDefinition technique)
    {
        Texture2D texture = GetEffectTexture("KiDisk");
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.3f) * 0.04f;
        Vector2 scale = new(Projectile.scale * 1.28f * pulse, Projectile.scale * 0.62f);

        Main.EntitySpriteDraw(
            texture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color,
            Projectile.rotation,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale,
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            texture,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White * 0.42f,
            -Projectile.rotation * 1.4f,
            new Vector2(texture.Width / 2f, texture.Height / 2f),
            scale * 0.72f,
            SpriteEffects.None);
    }

    private void DrawBigBangShot(KiTechniqueDefinition technique)
    {
        Texture2D orbTexture = GetEffectTexture("KiOrbProjectile");
        Texture2D chargeTexture = GetEffectTexture("KiChargeOrb");
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.16f) * 0.06f;
        float scale = Projectile.scale * pulse;

        Main.EntitySpriteDraw(
            chargeTexture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color * 0.58f,
            -Projectile.rotation * 0.55f,
            new Vector2(chargeTexture.Width / 2f, chargeTexture.Height / 2f),
            scale * 1.32f,
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            orbTexture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color,
            Projectile.rotation,
            new Vector2(orbTexture.Width / 2f, orbTexture.Height / 2f),
            scale,
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            orbTexture,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White * 0.62f,
            -Projectile.rotation,
            new Vector2(orbTexture.Width / 2f, orbTexture.Height / 2f),
            scale * 0.45f,
            SpriteEffects.None);
    }

    private void DrawSpiritBombShot(KiTechniqueDefinition technique)
    {
        Texture2D orbTexture = GetEffectTexture("KiOrbProjectile");
        Texture2D chargeTexture = GetEffectTexture("KiChargeOrb");
        float chargingPulse = Projectile.localAI[1] >= 0f ? (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.08f : 0.08f;
        float scale = Projectile.scale * (1f + chargingPulse);

        Main.EntitySpriteDraw(
            chargeTexture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color * 0.42f,
            Main.GameUpdateCount * 0.018f,
            new Vector2(chargeTexture.Width / 2f, chargeTexture.Height / 2f),
            scale * 1.42f,
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            orbTexture,
            Projectile.Center - Main.screenPosition,
            null,
            technique.Color,
            Projectile.rotation,
            new Vector2(orbTexture.Width / 2f, orbTexture.Height / 2f),
            scale,
            SpriteEffects.None);

        Main.EntitySpriteDraw(
            orbTexture,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White * 0.68f,
            -Projectile.rotation * 0.35f,
            new Vector2(orbTexture.Width / 2f, orbTexture.Height / 2f),
            scale * 0.48f,
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
        float chargeScale = technique.Technique switch
        {
            KiTechnique.FinalFlash => 1.8f,
            KiTechnique.GodKamehameha => 1.28f,
            KiTechnique.GalickGun => 1.14f,
            KiTechnique.SpecialBeamCannon => 0.78f,
            _ => 1f
        };
        float scale = MathHelper.Clamp(intensity, 0.4f, 1.35f) * pulse * chargeScale;
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

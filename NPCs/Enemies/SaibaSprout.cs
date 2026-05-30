using KiAscension.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension.NPCs.Enemies;

public class SaibaSprout : ModNPC
{
    private const float MaxRunSpeed = 1.65f;
    private const int ShotCooldownTicks = 110;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
    }

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 42;
        NPC.damage = 14;
        NPC.defense = 2;
        NPC.lifeMax = 52;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 120f;
        NPC.knockBackResist = 0.72f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        Player player = spawnInfo.Player;

        if (player is null || !player.ZoneOverworldHeight)
        {
            return 0f;
        }

        return Main.dayTime ? 0.018f : 0.035f;
    }

    public override void AI()
    {
        NPC.TargetClosest();
        Player target = Main.player[NPC.target];

        if (target is null || !target.active || target.dead)
        {
            NPC.velocity.X *= 0.94f;
            return;
        }

        MoveToward(target);
        TryFireKiShot(target);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ)
        {
            return;
        }

        int dustCount = NPC.life <= 0 ? 18 : 5;

        for (int i = 0; i < dustCount; i++)
        {
            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Grass);
            dust.velocity *= NPC.life <= 0 ? 1.8f : 0.85f;
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(Terraria.GameContent.ItemDropRules.ItemDropRule.Common(ModContent.ItemType<KiFragment>(), 4));
    }

    private void MoveToward(Player target)
    {
        float direction = target.Center.X >= NPC.Center.X ? 1f : -1f;
        NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X + direction * 0.08f, -MaxRunSpeed, MaxRunSpeed);

        if (NPC.collideX && NPC.velocity.Y == 0f)
        {
            NPC.velocity.Y = -5.2f;
        }

        NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0f ? 1 : -1;
    }

    private void TryFireKiShot(Player target)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            return;
        }

        NPC.ai[0]++;

        if (NPC.ai[0] < ShotCooldownTicks || Vector2.DistanceSquared(NPC.Center, target.Center) > 520f * 520f)
        {
            return;
        }

        if (!Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height))
        {
            return;
        }

        NPC.ai[0] = 0f;
        Vector2 velocity = Vector2.Normalize(target.Center - NPC.Center) * 7.5f;
        int projectileIndex = Projectile.NewProjectile(
            NPC.GetSource_FromAI(),
            NPC.Center,
            velocity,
            ProjectileID.DiamondBolt,
            Math.Max(8, NPC.damage / 3),
            1.2f,
            Main.myPlayer);

        if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles)
        {
            Projectile projectile = Main.projectile[projectileIndex];
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 120;
            projectile.netUpdate = true;
        }

        NPC.netUpdate = true;
    }
}

using System;
using KiAscension.Common;
using KiAscension.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace KiAscension.Systems;

public class KiAuraDrawLayer : PlayerDrawLayer
{
    private const int FrameWidth = 96;
    private const int FrameHeight = 128;
    private const int FrameCount = 8;

    public override Position GetDefaultPosition()
    {
        return PlayerDrawLayers.BeforeFirstVanillaLayer;
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        if (drawInfo.hideEntirePlayer || drawInfo.drawPlayer is null || !drawInfo.drawPlayer.active)
        {
            return false;
        }

        KiPlayer kiPlayer = drawInfo.drawPlayer.GetModPlayer<KiPlayer>();
        return kiPlayer.HasVisibleSaiyanAura || kiPlayer.HasVisibleKaioKenAura || kiPlayer.IsKiFlying;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.shadow != 0f)
        {
            return;
        }

        Player player = drawInfo.drawPlayer;
        KiPlayer kiPlayer = player.GetModPlayer<KiPlayer>();
        Texture2D texture = ModContent.Request<Texture2D>("KiAscension/Assets/Effects/KiAura").Value;
        Texture2D electricTexture = ModContent.Request<Texture2D>("KiAscension/Assets/Effects/KiAuraElectric").Value;
        int frame = (int)((Main.GameUpdateCount / 4UL + (ulong)player.whoAmI) % FrameCount);
        Rectangle source = GetFrame(frame);
        Vector2 position = drawInfo.Center - Main.screenPosition + new Vector2(0f, player.gfxOffY + 8f);
        Vector2 origin = new(FrameWidth * 0.5f, 94f);

        if (kiPlayer.HasVisibleSaiyanAura)
        {
            DrawSaiyanAura(ref drawInfo, texture, electricTexture, source, frame, position, origin, kiPlayer);
        }

        if (kiPlayer.HasVisibleKaioKenAura)
        {
            DrawKaioKenAura(ref drawInfo, texture, electricTexture, source, frame, position, origin, kiPlayer);
        }
    }

    private static void DrawSaiyanAura(
        ref PlayerDrawSet drawInfo,
        Texture2D texture,
        Texture2D electricTexture,
        Rectangle source,
        int frame,
        Vector2 position,
        Vector2 origin,
        KiPlayer kiPlayer)
    {
        AscensionAuraProfile profile = kiPlayer.SaiyanChargeIntensity > 0f && kiPlayer.UnlockedStageIndex > kiPlayer.CurrentStageIndex
            ? AscensionAuraProfiles.Get(AscensionStages.Get(kiPlayer.UnlockedStageIndex).Stage)
            : AscensionAuraProfiles.Get(kiPlayer.CurrentStage.Stage);

        float chargeIntensity = Math.Max(kiPlayer.SaiyanChargeIntensity, kiPlayer.BreakthroughIntensity);
        float activeIntensity = kiPlayer.CurrentStageIndex > 0 ? 0.5f + kiPlayer.CurrentStageIndex * 0.035f : 0.12f;
        float intensity = activeIntensity + chargeIntensity * 0.82f;
        float collapse = MathHelper.Clamp(1f - kiPlayer.SaiyanPowerDownIntensity * 0.9f, 0.08f, 1f);
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount / 5.2f) * (0.035f + chargeIntensity * 0.045f);
        float baseScale = (0.7f + profile.DustScale * 0.1f + intensity * 0.16f) * pulse;
        float opacity = MathHelper.Clamp((0.2f + intensity * 0.38f) * collapse, 0.03f, 0.82f);
        Vector2 collapseScale = new(0.9f + chargeIntensity * 0.16f, collapse);
        Vector2 wideScale = new(baseScale * collapseScale.X * (1.08f + chargeIntensity * 0.12f), baseScale * collapseScale.Y * (1.04f + chargeIntensity * 0.18f));
        Vector2 coreScale = new(baseScale * collapseScale.X * 0.84f, baseScale * collapseScale.Y * 0.95f);
        Vector2 collapsedOffset = new(0f, kiPlayer.SaiyanPowerDownIntensity * 16f - chargeIntensity * 5f);

        DrawAura(ref drawInfo, texture, source, position + collapsedOffset, origin, profile.PrimaryColor * (opacity * 0.62f), wideScale);
        DrawAura(ref drawInfo, texture, GetFrame(frame + 2), position + collapsedOffset + new Vector2(0f, -3f - chargeIntensity * 4f), origin, profile.PrimaryColor * opacity, coreScale);
        DrawAura(ref drawInfo, texture, GetFrame(frame + 5), position + collapsedOffset + new Vector2(0f, -8f), origin, Color.White * (opacity * 0.18f + chargeIntensity * 0.08f), coreScale * 0.72f);

        if (profile.EmitsElectricArcs || kiPlayer.BreakthroughIntensity > 0.25f || chargeIntensity > 0.78f)
        {
            float electricOpacity = MathHelper.Clamp(opacity * 0.85f + kiPlayer.BreakthroughIntensity * 0.25f + chargeIntensity * 0.18f, 0.08f, 0.82f);
            DrawAura(ref drawInfo, electricTexture, GetFrame(frame + 1), position + collapsedOffset + new Vector2(0f, -3f), origin, profile.SecondaryColor * electricOpacity, wideScale * 0.98f);
        }
    }

    private static void DrawKaioKenAura(
        ref PlayerDrawSet drawInfo,
        Texture2D texture,
        Texture2D electricTexture,
        Rectangle source,
        int frame,
        Vector2 position,
        Vector2 origin,
        KiPlayer kiPlayer)
    {
        Color kaioKenColor = kiPlayer.CurrentKaioKenLevel.AuraColor == Color.Transparent
            ? new Color(255, 70, 55)
            : kiPlayer.CurrentKaioKenLevel.AuraColor;
        float activeIntensity = kiPlayer.IsKaioKenActive ? 0.44f + kiPlayer.CurrentKaioKenLevelIndex * 0.034f : 0.1f;
        float intensity = activeIntensity + kiPlayer.KaioKenChargeIntensity * 0.78f;
        float collapse = MathHelper.Clamp(1f - kiPlayer.KaioKenPowerDownIntensity * 0.86f, 0.08f, 1f);
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount / 3.2f) * (0.07f + kiPlayer.KaioKenChargeIntensity * 0.04f);
        float scale = (0.66f + intensity * 0.2f) * pulse;
        float opacity = MathHelper.Clamp((0.2f + intensity * 0.42f) * collapse, 0.04f, 0.78f);
        Vector2 kaioScale = new(scale * (1.02f + kiPlayer.KaioKenChargeIntensity * 0.18f), scale * collapse * (1.05f + kiPlayer.KaioKenChargeIntensity * 0.22f));
        Vector2 jitter = new((float)Math.Sin(Main.GameUpdateCount * 0.47f + kiPlayer.CurrentKaioKenLevelIndex) * 2.2f * intensity, kiPlayer.KaioKenPowerDownIntensity * 12f);

        DrawAura(ref drawInfo, texture, source, position + new Vector2(0f, 1f) + jitter, origin, kaioKenColor * opacity, kaioScale);
        DrawAura(ref drawInfo, texture, GetFrame(frame + 3), position + new Vector2(0f, -5f) - jitter * 0.35f, origin, Color.White * (opacity * 0.18f), kaioScale * 0.76f);
        DrawAura(ref drawInfo, electricTexture, GetFrame(frame + 4), position + jitter * 0.4f, origin, kaioKenColor * (opacity * 0.36f), kaioScale * 0.96f);
    }

    private static Rectangle GetFrame(int frame)
    {
        int wrappedFrame = frame % FrameCount;
        return new Rectangle(wrappedFrame * FrameWidth, 0, FrameWidth, FrameHeight);
    }

    private static void DrawAura(
        ref PlayerDrawSet drawInfo,
        Texture2D texture,
        Rectangle source,
        Vector2 position,
        Vector2 origin,
        Color color,
        Vector2 scale)
    {
        DrawData data = new(texture, position, source, color, 0f, origin, scale, SpriteEffects.None, 0f)
        {
            ignorePlayerRotation = true
        };

        drawInfo.DrawDataCache.Add(data);
    }
}

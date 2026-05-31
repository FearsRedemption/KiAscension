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
        StageDefinition visualStage = kiPlayer.SaiyanChargeIntensity > 0f && kiPlayer.UnlockedStageIndex > kiPlayer.CurrentStageIndex
            ? AscensionStages.Get(kiPlayer.UnlockedStageIndex)
            : kiPlayer.CurrentStage;
        AscensionAuraProfile profile = AscensionAuraProfiles.Get(visualStage.Stage);

        float chargeIntensity = Math.Max(kiPlayer.SaiyanChargeIntensity, kiPlayer.BreakthroughIntensity);
        float activeIntensity = kiPlayer.CurrentStageIndex > 0 ? 0.5f + kiPlayer.CurrentStageIndex * 0.035f : 0.12f;
        float intensity = activeIntensity + chargeIntensity * 0.82f;
        float collapse = MathHelper.Clamp(1f - kiPlayer.SaiyanPowerDownIntensity * 0.9f, 0.08f, 1f);
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount / 5.2f) * (0.035f + chargeIntensity * 0.045f);
        float stageMass = GetSaiyanAuraMass(visualStage.Stage);
        float baseScale = (0.7f + profile.DustScale * 0.1f + intensity * 0.16f + stageMass * 0.05f) * pulse;
        float opacity = MathHelper.Clamp((0.2f + intensity * 0.38f) * collapse, 0.03f, 0.82f);
        Vector2 collapseScale = new(0.9f + chargeIntensity * 0.16f, collapse);
        Vector2 wideScale = new(
            baseScale * collapseScale.X * (1.08f + chargeIntensity * 0.12f + stageMass * 0.05f),
            baseScale * collapseScale.Y * (1.04f + chargeIntensity * 0.18f + stageMass * 0.08f));
        Vector2 coreScale = new(baseScale * collapseScale.X * 0.84f, baseScale * collapseScale.Y * 0.95f);
        Vector2 collapsedOffset = new(0f, kiPlayer.SaiyanPowerDownIntensity * 16f - chargeIntensity * 5f);

        DrawAura(ref drawInfo, texture, source, position + collapsedOffset + new Vector2(0f, 2f), origin, profile.PrimaryColor * (opacity * 0.36f), new Vector2(wideScale.X * 1.18f, wideScale.Y * 1.08f));
        DrawAura(ref drawInfo, texture, source, position + collapsedOffset, origin, profile.PrimaryColor * (opacity * 0.62f), wideScale);
        DrawFlameFlares(ref drawInfo, texture, frame, position + collapsedOffset, origin, profile, visualStage.Stage, chargeIntensity, opacity, collapse);
        DrawAura(ref drawInfo, texture, GetFrame(frame + 2), position + collapsedOffset + new Vector2(0f, -3f - chargeIntensity * 4f), origin, profile.PrimaryColor * opacity, coreScale);
        DrawAura(ref drawInfo, texture, GetFrame(frame + 5), position + collapsedOffset + new Vector2(0f, -8f), origin, Color.White * (opacity * 0.18f + chargeIntensity * 0.08f), coreScale * 0.72f);

        if (profile.EmitsElectricArcs || kiPlayer.BreakthroughIntensity > 0.25f || chargeIntensity > 0.78f)
        {
            float electricOpacity = MathHelper.Clamp(opacity * 0.85f + kiPlayer.BreakthroughIntensity * 0.25f + chargeIntensity * 0.18f, 0.08f, 0.82f);
            DrawAura(ref drawInfo, electricTexture, GetFrame(frame + 1), position + collapsedOffset + new Vector2(0f, -3f), origin, profile.SecondaryColor * electricOpacity, wideScale * 0.98f);
            DrawAura(ref drawInfo, electricTexture, GetFrame(frame + 4), position + collapsedOffset + new Vector2(0f, -8f), origin, Color.White * (electricOpacity * 0.52f), new Vector2(wideScale.X * 0.72f, wideScale.Y * 0.92f));
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
        float scale = (0.68f + intensity * 0.23f) * pulse;
        float opacity = MathHelper.Clamp((0.2f + intensity * 0.42f) * collapse, 0.04f, 0.78f);
        Vector2 kaioScale = new(scale * (1.02f + kiPlayer.KaioKenChargeIntensity * 0.18f), scale * collapse * (1.05f + kiPlayer.KaioKenChargeIntensity * 0.22f));
        Vector2 jitter = new((float)Math.Sin(Main.GameUpdateCount * 0.47f + kiPlayer.CurrentKaioKenLevelIndex) * 2.2f * intensity, kiPlayer.KaioKenPowerDownIntensity * 12f);

        DrawAura(ref drawInfo, texture, GetFrame(frame + 6), position + new Vector2(-3f, 1f) - jitter * 0.25f, origin, kaioKenColor * (opacity * 0.42f), new Vector2(kaioScale.X * 1.22f, kaioScale.Y * 1.08f));
        DrawAura(ref drawInfo, texture, source, position + new Vector2(0f, 1f) + jitter, origin, kaioKenColor * opacity, kaioScale);
        DrawAura(ref drawInfo, texture, GetFrame(frame + 2), position + new Vector2(3f, -3f) + jitter * 0.45f, origin, new Color(255, 30, 28) * (opacity * 0.62f), new Vector2(kaioScale.X * 0.82f, kaioScale.Y * 1.16f));
        DrawAura(ref drawInfo, texture, GetFrame(frame + 3), position + new Vector2(0f, -5f) - jitter * 0.35f, origin, Color.White * (opacity * 0.18f), kaioScale * 0.76f);
        DrawAura(ref drawInfo, electricTexture, GetFrame(frame + 4), position + jitter * 0.4f, origin, kaioKenColor * (opacity * 0.52f), kaioScale * 0.96f);
    }

    private static void DrawFlameFlares(
        ref PlayerDrawSet drawInfo,
        Texture2D texture,
        int frame,
        Vector2 position,
        Vector2 origin,
        AscensionAuraProfile profile,
        AscensionStage stage,
        float chargeIntensity,
        float opacity,
        float collapse)
    {
        if (stage == AscensionStage.Base)
        {
            return;
        }

        float sidePulse = (float)Math.Sin(Main.GameUpdateCount * 0.19f + frame) * 2.5f;
        float heightBoost = stage is AscensionStage.SuperSaiyan3 or AscensionStage.UltraInstinct ? 1.16f : 1f;
        float sideScale = 0.46f + chargeIntensity * 0.2f + GetSaiyanAuraMass(stage) * 0.04f;
        Color sideColor = profile.PrimaryColor * MathHelper.Clamp(opacity * 0.42f + chargeIntensity * 0.12f, 0.04f, 0.55f);

        DrawAura(ref drawInfo, texture, GetFrame(frame + 3), position + new Vector2(-14f - sidePulse, -4f), origin, sideColor, new Vector2(sideScale * 0.72f, sideScale * heightBoost * collapse));
        DrawAura(ref drawInfo, texture, GetFrame(frame + 6), position + new Vector2(14f + sidePulse, -6f), origin, sideColor, new Vector2(sideScale * 0.72f, sideScale * heightBoost * collapse));

        if (stage is AscensionStage.SuperSaiyanGod or AscensionStage.SuperSaiyanBlue or AscensionStage.UltraInstinctSign or AscensionStage.UltraInstinct)
        {
            DrawAura(ref drawInfo, texture, GetFrame(frame + 1), position + new Vector2(0f, -10f - chargeIntensity * 5f), origin, profile.SecondaryColor * (opacity * 0.2f), new Vector2(sideScale * 0.85f, sideScale * 0.78f * collapse));
        }
    }

    private static float GetSaiyanAuraMass(AscensionStage stage)
    {
        return stage switch
        {
            AscensionStage.SuperSaiyan2 => 1f,
            AscensionStage.SuperSaiyan3 => 2f,
            AscensionStage.SuperSaiyanGod => 0.7f,
            AscensionStage.SuperSaiyanBlue => 1.15f,
            AscensionStage.UltraInstinctSign => 1.35f,
            AscensionStage.UltraInstinct => 1.6f,
            _ => 0f
        };
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

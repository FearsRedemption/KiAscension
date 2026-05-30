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
    private const int FrameCount = 6;

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
        int frame = (int)((Main.GameUpdateCount / 5UL + (ulong)player.whoAmI) % FrameCount);
        Rectangle source = new(frame * FrameWidth, 0, FrameWidth, FrameHeight);
        Vector2 position = drawInfo.Center - Main.screenPosition + new Vector2(0f, player.gfxOffY + 8f);
        Vector2 origin = new(FrameWidth * 0.5f, 94f);

        if (kiPlayer.HasVisibleSaiyanAura)
        {
            DrawSaiyanAura(ref drawInfo, texture, source, position, origin, kiPlayer);
        }

        if (kiPlayer.HasVisibleKaioKenAura)
        {
            DrawKaioKenAura(ref drawInfo, texture, source, position, origin, kiPlayer);
        }
    }

    private static void DrawSaiyanAura(
        ref PlayerDrawSet drawInfo,
        Texture2D texture,
        Rectangle source,
        Vector2 position,
        Vector2 origin,
        KiPlayer kiPlayer)
    {
        AscensionAuraProfile profile = kiPlayer.SaiyanChargeIntensity > 0f && kiPlayer.UnlockedStageIndex > kiPlayer.CurrentStageIndex
            ? AscensionAuraProfiles.Get(AscensionStages.Get(kiPlayer.UnlockedStageIndex).Stage)
            : AscensionAuraProfiles.Get(kiPlayer.CurrentStage.Stage);

        float activeIntensity = kiPlayer.CurrentStageIndex > 0 ? 0.58f : 0.12f;
        float intensity = activeIntensity + kiPlayer.SaiyanChargeIntensity * 0.62f + kiPlayer.BreakthroughIntensity * 0.75f;
        float collapse = 1f - kiPlayer.SaiyanPowerDownIntensity * 0.72f;
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount / 7f) * 0.045f;
        float scale = (0.72f + profile.DustScale * 0.12f + intensity * 0.16f) * collapse * pulse;
        float opacity = MathHelper.Clamp((0.26f + intensity * 0.35f) * collapse, 0.04f, 0.78f);

        DrawAura(ref drawInfo, texture, source, position, origin, profile.PrimaryColor * opacity, scale);

        if (profile.EmitsElectricArcs || kiPlayer.BreakthroughIntensity > 0.35f)
        {
            float electricOpacity = MathHelper.Clamp(opacity * 0.55f + kiPlayer.BreakthroughIntensity * 0.2f, 0.08f, 0.62f);
            DrawAura(ref drawInfo, texture, source, position + new Vector2(0f, -2f), origin, profile.SecondaryColor * electricOpacity, scale * 0.92f);
        }
    }

    private static void DrawKaioKenAura(
        ref PlayerDrawSet drawInfo,
        Texture2D texture,
        Rectangle source,
        Vector2 position,
        Vector2 origin,
        KiPlayer kiPlayer)
    {
        Color kaioKenColor = kiPlayer.CurrentKaioKenLevel.AuraColor == Color.Transparent
            ? new Color(255, 70, 55)
            : kiPlayer.CurrentKaioKenLevel.AuraColor;
        float activeIntensity = kiPlayer.IsKaioKenActive ? 0.48f + kiPlayer.CurrentKaioKenLevelIndex * 0.035f : 0.1f;
        float intensity = activeIntensity + kiPlayer.KaioKenChargeIntensity * 0.65f;
        float collapse = 1f - kiPlayer.KaioKenPowerDownIntensity * 0.78f;
        float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount / 4f) * 0.08f;
        float scale = (0.66f + intensity * 0.18f) * collapse * pulse;
        float opacity = MathHelper.Clamp((0.22f + intensity * 0.38f) * collapse, 0.04f, 0.74f);

        DrawAura(ref drawInfo, texture, source, position + new Vector2(0f, 1f), origin, kaioKenColor * opacity, scale);
        DrawAura(ref drawInfo, texture, source, position + new Vector2(0f, -3f), origin, Color.White * (opacity * 0.18f), scale * 0.82f);
    }

    private static void DrawAura(
        ref PlayerDrawSet drawInfo,
        Texture2D texture,
        Rectangle source,
        Vector2 position,
        Vector2 origin,
        Color color,
        float scale)
    {
        DrawData data = new(texture, position, source, color, 0f, origin, scale, SpriteEffects.None, 0f)
        {
            ignorePlayerRotation = true
        };

        drawInfo.DrawDataCache.Add(data);
    }
}

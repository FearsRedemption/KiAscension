using System;
using System.Collections.Generic;
using KiAscension.Items.Techniques;
using KiAscension.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace KiAscension.Systems;

public class KiHudSystem : ModSystem
{
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars", StringComparison.Ordinal));

        if (resourceBarIndex == -1)
        {
            return;
        }

        layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
            "KiAscension: Ki Status",
            DrawKiStatus,
            InterfaceScaleType.UI));
    }

    private static bool DrawKiStatus()
    {
        if (Main.gameMenu || Main.LocalPlayer is null || !Main.LocalPlayer.active)
        {
            return true;
        }

        KiPlayer kiPlayer = Main.LocalPlayer.GetModPlayer<KiPlayer>();
        Vector2 position = new(18f, 84f);

        DrawText($"Kai Lv {kiPlayer.KaiLevel}  Power {kiPlayer.PowerExperience}  Ki Power {kiPlayer.KiPowerExperience}", position, new Color(255, 235, 135));
        DrawBar(new Rectangle((int)position.X, (int)position.Y + 24, 220, 8), kiPlayer.GetKaiLevelProgress(), new Color(255, 215, 90));

        DrawText($"Ki {kiPlayer.Ki}/{kiPlayer.MaxKi}  Regen {kiPlayer.KiRegenPerSecond}/s", position + new Vector2(0f, 38f), new Color(125, 225, 255));
        DrawBar(new Rectangle((int)position.X, (int)position.Y + 62, 220, 8), kiPlayer.GetKiProgress(), new Color(80, 205, 255));

        DrawText($"Form: {kiPlayer.CurrentStage.DisplayName}", position + new Vector2(0f, 76f), kiPlayer.CurrentStage.AuraColor);
        DrawText($"Kaio-Ken: {kiPlayer.CurrentKaioKenLevel.DisplayName}", position + new Vector2(0f, 96f), kiPlayer.CurrentKaioKenLevel.AuraColor == Color.Transparent ? new Color(190, 190, 190) : kiPlayer.CurrentKaioKenLevel.AuraColor);
        DrawText(GetHeldTechniqueText(), position + new Vector2(0f, 116f), new Color(180, 235, 255));
        DrawText(kiPlayer.GetNextCeilingText(), position + new Vector2(0f, 136f), new Color(235, 235, 235));

        return true;
    }

    private static string GetHeldTechniqueText()
    {
        return Main.LocalPlayer.HeldItem?.ModItem is KiTechniqueItem techniqueItem
            ? $"Spell: {techniqueItem.DisplayName.Value}"
            : "Spell: equip a ki technique";
    }

    private static void DrawText(string text, Vector2 position, Color color)
    {
        Utils.DrawBorderString(Main.spriteBatch, text, position, color, 0.88f);
    }

    private static void DrawBar(Rectangle area, float progress, Color fillColor)
    {
        progress = MathHelper.Clamp(progress, 0f, 1f);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, area, new Color(20, 24, 32, 190));
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(area.X + 1, area.Y + 1, area.Width - 2, area.Height - 2), new Color(5, 8, 12, 210));

        int fillWidth = Math.Max(0, (int)((area.Width - 2) * progress));

        if (fillWidth > 0)
        {
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(area.X + 1, area.Y + 1, fillWidth, area.Height - 2), fillColor);
        }
    }
}

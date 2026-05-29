using System;
using System.Collections.Generic;
using KiAscension.Common;
using KiAscension.Items.Techniques;
using KiAscension.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace KiAscension.Systems;

public class KiHudSystem : ModSystem
{
    private static bool showStatsPanel;
    private static bool showDevPanel;

    public static void ToggleStatsPanel()
    {
        showStatsPanel = !showStatsPanel;
    }

    public static void ToggleDevPanel()
    {
        showDevPanel = !showDevPanel;
    }

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
        KiResourceSnapshot resources = kiPlayer.KiResources;
        Vector2 position = new(18f, 84f);

        DrawText($"Kai Lv {kiPlayer.KaiLevel}  Power {kiPlayer.PowerExperience}  Ki Power {kiPlayer.KiPowerExperience}", position, new Color(255, 235, 135));
        DrawBar(new Rectangle((int)position.X, (int)position.Y + 24, 220, 8), kiPlayer.GetKaiLevelProgress(), new Color(255, 215, 90));

        DrawText($"Ki {kiPlayer.Ki}/{resources.MaxKi}  Net {FormatSigned(resources.NetRegenPerSecond)}/s", position + new Vector2(0f, 38f), new Color(125, 225, 255));
        DrawBar(new Rectangle((int)position.X, (int)position.Y + 62, 220, 8), kiPlayer.GetKiProgress(), new Color(80, 205, 255));

        DrawText($"Form: {kiPlayer.CurrentStage.DisplayName}", position + new Vector2(0f, 76f), kiPlayer.CurrentStage.AuraColor);
        DrawText($"Kaio-Ken: {kiPlayer.CurrentKaioKenLevel.DisplayName}", position + new Vector2(0f, 96f), kiPlayer.CurrentKaioKenLevel.AuraColor == Color.Transparent ? new Color(190, 190, 190) : kiPlayer.CurrentKaioKenLevel.AuraColor);
        DrawText(GetHeldTechniqueText(), position + new Vector2(0f, 116f), new Color(180, 235, 255));
        DrawText(kiPlayer.GetNextCeilingText(), position + new Vector2(0f, 136f), new Color(235, 235, 235));

        if (showStatsPanel)
        {
            DrawStatsPanel(kiPlayer, resources);
        }

        if (showDevPanel)
        {
            DrawDevPanel(kiPlayer, resources);
        }

        return true;
    }

    private static void DrawStatsPanel(KiPlayer kiPlayer, KiResourceSnapshot resources)
    {
        Vector2 position = new(Math.Max(280f, Main.screenWidth - 420f), 84f);
        List<(string Text, Color Color)> lines = new()
        {
            ($"Kai Level {kiPlayer.KaiLevel}", new Color(255, 235, 135)),
            ($"Physical Power: {kiPlayer.PowerExperience}  Ki Power: {kiPlayer.KiPowerExperience}", Color.White),
            ($"Saiyan Form: {kiPlayer.CurrentStage.DisplayName}", kiPlayer.CurrentStage.AuraColor),
            ($"Unlocked Form: {kiPlayer.UnlockedStage.DisplayName}", kiPlayer.UnlockedStage.AuraColor),
            ($"Kaio-Ken: {kiPlayer.CurrentKaioKenLevel.DisplayName}", GetKaioKenTextColor(kiPlayer)),
            ($"Unlocked Kaio-Ken: {kiPlayer.UnlockedKaioKenLevel.DisplayName}", GetKaioKenTextColor(kiPlayer)),
            ($"Ki: {kiPlayer.Ki}/{resources.MaxKi}", new Color(125, 225, 255)),
            ($"Ki Regen: {resources.RegenPerSecond}/s  Drain: {resources.PassiveDrainPerSecond}/s  Net: {FormatSigned(resources.NetRegenPerSecond)}/s", new Color(125, 225, 255)),
            ($"Technique Cost: {(resources.TechniqueCostMultiplier * 100f):0}% of base", new Color(180, 235, 255)),
            ($"Damage: x{kiPlayer.CombinedDamageMultiplier:0.00}  Speed: x{kiPlayer.CombinedSpeedMultiplier:0.00}", new Color(255, 220, 130)),
            ($"Defense: +{kiPlayer.CurrentStage.DefenseBonus}  HP Regen: +{kiPlayer.CurrentStage.LifeRegenBonus}/s", new Color(170, 255, 175)),
            ($"Flight Control Hook: x{kiPlayer.FlightControlMultiplier:0.00}", new Color(190, 210, 255)),
            (kiPlayer.GetNextCeilingText(), new Color(235, 235, 235)),
            (kiPlayer.NextKaioKenGateText, new Color(255, 150, 130))
        };

        AddHeldSpellLines(lines, kiPlayer);
        DrawPanel(position, 392, "Ki Stats", lines);
    }

    private static void DrawDevPanel(KiPlayer kiPlayer, KiResourceSnapshot resources)
    {
        Vector2 position = new(Math.Max(280f, Main.screenWidth - 420f), showStatsPanel ? 438f : 84f);
        List<(string Text, Color Color)> lines = new()
        {
            ("Read-only dev inspector", new Color(255, 200, 100)),
            ($"NetMode: {GetNetModeName()}  Player: {Main.LocalPlayer.whoAmI}", Color.White),
            ($"Form Index: {kiPlayer.CurrentStageIndex}/{kiPlayer.UnlockedStageIndex}", kiPlayer.CurrentStage.AuraColor),
            ($"Kaio-Ken Index: {kiPlayer.CurrentKaioKenLevelIndex}/{kiPlayer.UnlockedKaioKenLevelIndex}", GetKaioKenTextColor(kiPlayer)),
            ($"Technique Index: {kiPlayer.SelectedTechniqueIndex}/{kiPlayer.HighestUnlockedTechniqueIndex}", new Color(180, 235, 255)),
            ($"Power Total: {kiPlayer.TotalPowerExperience}", new Color(255, 235, 135)),
            ($"Ki Snapshot: max {resources.MaxKi}, regen {resources.RegenPerSecond}, drain {resources.PassiveDrainPerSecond}", new Color(125, 225, 255)),
            ($"Pending Witness Gate: {(kiPlayer.HasPendingWitnessBreakthrough ? "yes" : "no")}", new Color(255, 150, 130)),
            ("Future: grant/reset buttons stay out until config-gated.", new Color(180, 180, 180))
        };

        DrawPanel(position, 392, "Ki Dev", lines);
    }

    private static void AddHeldSpellLines(List<(string Text, Color Color)> lines, KiPlayer kiPlayer)
    {
        if (Main.LocalPlayer.HeldItem?.ModItem is not KiTechniqueItem techniqueItem)
        {
            lines.Add(("Held Spell: none", new Color(180, 180, 180)));
            return;
        }

        KiTechniqueDefinition technique = techniqueItem.TechniqueDefinition;
        lines.Add(($"Held Spell: {technique.DisplayName}", technique.Color));
        lines.Add(technique.Behavior == KiTechniqueBehavior.Beam
            ? ($"Held Cost: {kiPlayer.GetTechniqueInitialKiCost(technique)} start, {kiPlayer.GetTechniqueChannelKiCostPerSecond(technique)}/s", technique.Color)
            : ($"Held Cost: {kiPlayer.GetTechniqueInitialKiCost(technique)}", technique.Color));
    }

    private static string GetHeldTechniqueText()
    {
        return Main.LocalPlayer.HeldItem?.ModItem is KiTechniqueItem techniqueItem
            ? $"Spell: {techniqueItem.DisplayName.Value}"
            : "Spell: equip a ki technique";
    }

    private static string FormatSigned(int value)
    {
        return value > 0 ? $"+{value}" : value.ToString();
    }

    private static string GetNetModeName()
    {
        return Main.netMode switch
        {
            NetmodeID.SinglePlayer => "Single",
            NetmodeID.MultiplayerClient => "Client",
            NetmodeID.Server => "Server",
            _ => Main.netMode.ToString()
        };
    }

    private static Color GetKaioKenTextColor(KiPlayer kiPlayer)
    {
        return kiPlayer.CurrentKaioKenLevel.AuraColor == Color.Transparent
            ? new Color(190, 190, 190)
            : kiPlayer.CurrentKaioKenLevel.AuraColor;
    }

    private static void DrawPanel(Vector2 position, int width, string title, List<(string Text, Color Color)> lines)
    {
        int lineHeight = 20;
        int height = 42 + lines.Count * lineHeight;
        Rectangle outer = new((int)position.X, (int)position.Y, width, height);
        Rectangle inner = new(outer.X + 2, outer.Y + 2, outer.Width - 4, outer.Height - 4);

        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, outer, new Color(10, 12, 18, 220));
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, inner, new Color(24, 28, 38, 218));
        DrawText(title, position + new Vector2(12f, 10f), new Color(255, 235, 135), 1f);

        for (int i = 0; i < lines.Count; i++)
        {
            DrawText(lines[i].Text, position + new Vector2(12f, 36f + i * lineHeight), lines[i].Color, 0.8f);
        }
    }

    private static void DrawText(string text, Vector2 position, Color color)
    {
        DrawText(text, position, color, 0.88f);
    }

    private static void DrawText(string text, Vector2 position, Color color, float scale)
    {
        Utils.DrawBorderString(Main.spriteBatch, text, position, color, scale);
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

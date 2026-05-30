using System;
using System.Collections.Generic;
using KiAscension.Common;
using KiAscension.Items.Techniques;
using KiAscension.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace KiAscension.Systems;

public class KiHudSystem : ModSystem
{
    private const int HudWidth = 314;
    private const int PanelWidth = 432;
    private const int PanelPadding = 12;
    private const int RowHeight = 19;

    private static readonly Color PanelOuter = new(255, 198, 76, 220);
    private static readonly Color PanelInner = new(13, 16, 26, 232);
    private static readonly Color PanelBand = new(31, 34, 49, 226);
    private static readonly Color MutedText = new(175, 182, 202);
    private static readonly Color HeaderText = new(255, 228, 128);
    private static readonly Color LockedText = new(255, 128, 112);
    private static readonly Color KiBlue = new(82, 210, 255);

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

        DrawMainHud(kiPlayer, resources);

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

    private static void DrawMainHud(KiPlayer kiPlayer, KiResourceSnapshot resources)
    {
        Rectangle panel = new(18, 84, HudWidth, 214);
        DrawPanelShell(panel, "KI ASCENSION", kiPlayer.CurrentStage.AuraColor);

        int x = panel.X + PanelPadding;
        int y = panel.Y + 34;
        int width = panel.Width - PanelPadding * 2;

        DrawLabelValue("Kai Level", kiPlayer.KaiLevel.ToString(), new Vector2(x, y), width, HeaderText);
        DrawBar(new Rectangle(x, y + 18, width, 8), kiPlayer.GetKaiLevelProgress(), new Color(255, 216, 76), new Color(94, 68, 22));
        y += 32;

        DrawLabelValue("Ki", $"{kiPlayer.Ki}/{resources.MaxKi}", new Vector2(x, y), width, KiBlue);
        DrawBar(new Rectangle(x, y + 18, width, 10), kiPlayer.GetKiProgress(), KiBlue, new Color(19, 55, 72));
        y += 34;

        DrawLabelValue("Net", $"{FormatSigned(resources.NetRegenPerSecond)}/s", new Vector2(x, y), width / 2 - 4, resources.NetRegenPerSecond >= 0 ? new Color(132, 255, 170) : LockedText);
        DrawLabelValue("Drain", $"{resources.PassiveDrainPerSecond}/s", new Vector2(x + width / 2 + 4, y), width / 2 - 4, resources.PassiveDrainPerSecond > 0 ? LockedText : MutedText);
        y += 22;

        DrawDivider(new Rectangle(x, y, width, 1), kiPlayer.CurrentStage.AuraColor);
        y += 8;

        DrawCompactRow("Form", kiPlayer.CurrentStage.DisplayName, new Vector2(x, y), width, kiPlayer.CurrentStage.AuraColor);
        y += RowHeight;
        DrawCompactRow("Kaio-Ken", kiPlayer.CurrentKaioKenLevel.DisplayName, new Vector2(x, y), width, GetKaioKenTextColor(kiPlayer));
        y += RowHeight;
        DrawCompactRow("Held", GetHeldTechniqueName(kiPlayer), new Vector2(x, y), width, GetHeldTechniqueColor(kiPlayer));
        y += RowHeight;
        DrawCompactRow("Training", kiPlayer.ActiveTrainingDisplayName, new Vector2(x, y), width, kiPlayer.IsUsingTrainingStation ? new Color(150, 230, 255) : MutedText);
        y += RowHeight;

        string gate = kiPlayer.GetNextCeilingText();
        DrawWrappedText($"Next: {gate}", new Vector2(x, y), width, new Color(232, 234, 242), 0.72f, panel.Bottom - y - 6);
    }

    private static void DrawStatsPanel(KiPlayer kiPlayer, KiResourceSnapshot resources)
    {
        int height = showDevPanel ? Math.Min(388, Main.screenHeight - 420) : Math.Min(520, Main.screenHeight - 108);
        height = Math.Max(300, height);
        Rectangle panel = new(Math.Max(350, Main.screenWidth - PanelWidth - 18), 84, PanelWidth, height);
        DrawPanelShell(panel, "KI STATS", kiPlayer.CurrentStage.AuraColor);

        Rectangle content = GetContentArea(panel);
        int columnGap = 16;
        int columnWidth = (content.Width - columnGap) / 2;
        int leftY = content.Y;
        int rightY = content.Y;

        leftY = DrawSection(content.X, leftY, columnWidth, "Progression", kiPlayer.CurrentStage.AuraColor);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Kai Level", kiPlayer.KaiLevel.ToString(), HeaderText);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Physical", kiPlayer.PowerExperience.ToString(), new Color(255, 205, 150));
        leftY = DrawMetric(content.X, leftY, columnWidth, "Ki Power", kiPlayer.KiPowerExperience.ToString(), KiBlue);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Form", kiPlayer.CurrentStage.DisplayName, kiPlayer.CurrentStage.AuraColor);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Unlocked", kiPlayer.UnlockedStage.DisplayName, kiPlayer.UnlockedStage.AuraColor);
        leftY = DrawWrappedBlock(content.X, leftY + 4, columnWidth, kiPlayer.GetNextCeilingText(), new Color(232, 234, 242), panel.Bottom - PanelPadding);
        leftY = DrawWrappedBlock(content.X, leftY + 4, columnWidth, kiPlayer.NextKaioKenGateText, LockedText, panel.Bottom - PanelPadding);

        int rightX = content.X + columnWidth + columnGap;
        rightY = DrawSection(rightX, rightY, columnWidth, "Combat", KiBlue);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Ki", $"{kiPlayer.Ki}/{resources.MaxKi}", KiBlue);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Regen", $"{resources.RegenPerSecond}/s", new Color(132, 255, 170));
        rightY = DrawMetric(rightX, rightY, columnWidth, "Drain", $"{resources.PassiveDrainPerSecond}/s", resources.PassiveDrainPerSecond > 0 ? LockedText : MutedText);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Cost", $"{resources.TechniqueCostMultiplier * 100f:0}% base", new Color(180, 235, 255));
        rightY = DrawMetric(rightX, rightY, columnWidth, "Damage", $"x{kiPlayer.CombinedDamageMultiplier:0.00}", new Color(255, 220, 130));
        rightY = DrawMetric(rightX, rightY, columnWidth, "Speed", $"x{kiPlayer.CombinedSpeedMultiplier:0.00}", new Color(255, 220, 130));
        rightY = DrawMetric(rightX, rightY, columnWidth, "Strike", $"x{kiPlayer.PhysicalDamageMultiplier:0.00}  C{kiPlayer.MeleeComboStep}", new Color(255, 205, 150));
        rightY = DrawMetric(rightX, rightY, columnWidth, "Training", kiPlayer.ActiveTrainingDisplayName, kiPlayer.IsUsingTrainingStation ? new Color(150, 230, 255) : MutedText);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Defense", $"+{kiPlayer.CurrentStage.DefenseBonus}", new Color(170, 255, 175));
        rightY = DrawMetric(rightX, rightY, columnWidth, "HP Regen", $"+{kiPlayer.CurrentStage.LifeRegenBonus}/s", new Color(170, 255, 175));
        rightY = DrawWrappedBlock(rightX, rightY + 4, columnWidth, kiPlayer.GetFlightStatusText(), new Color(190, 210, 255), panel.Bottom - PanelPadding);

        int lowerY = Math.Max(leftY, rightY) + 8;
        if (lowerY < panel.Bottom - 70)
        {
            DrawHeldSpellPanel(new Rectangle(content.X, lowerY, content.Width, panel.Bottom - lowerY - PanelPadding), kiPlayer);
        }
        else
        {
            DrawOverflowHint(panel);
        }
    }

    private static void DrawDevPanel(KiPlayer kiPlayer, KiResourceSnapshot resources)
    {
        int width = PanelWidth;
        int x = Math.Max(350, Main.screenWidth - width - 18);
        int y = showStatsPanel ? Math.Min(Main.screenHeight - 334, 486) : 84;

        if (showStatsPanel && y < 486)
        {
            x = Math.Max(18, Main.screenWidth - width * 2 - 34);
            y = 84;
        }

        Rectangle panel = new(x, Math.Max(84, y), width, 318);
        DrawPanelShell(panel, "DEV INSPECTOR", new Color(255, 168, 76));

        Rectangle content = GetContentArea(panel);
        int columnGap = 16;
        int columnWidth = (content.Width - columnGap) / 2;
        int rightX = content.X + columnWidth + columnGap;
        int leftY = content.Y;
        int rightY = content.Y;

        leftY = DrawSection(content.X, leftY, columnWidth, "Network", new Color(255, 168, 76));
        leftY = DrawMetric(content.X, leftY, columnWidth, "Mode", GetNetModeName(), Color.White);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Player", Main.LocalPlayer.whoAmI.ToString(), Color.White);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Power", kiPlayer.TotalPowerExperience.ToString(), HeaderText);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Witness", kiPlayer.HasPendingWitnessBreakthrough ? "pending" : "clear", kiPlayer.HasPendingWitnessBreakthrough ? LockedText : new Color(132, 255, 170));

        rightY = DrawSection(rightX, rightY, columnWidth, "State", KiBlue);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Form", $"{kiPlayer.CurrentStageIndex}/{kiPlayer.UnlockedStageIndex}", kiPlayer.CurrentStage.AuraColor);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Kaio-Ken", $"{kiPlayer.CurrentKaioKenLevelIndex}/{kiPlayer.UnlockedKaioKenLevelIndex}", GetKaioKenTextColor(kiPlayer));
        rightY = DrawMetric(rightX, rightY, columnWidth, "Technique", $"{kiPlayer.SelectedTechniqueIndex}/{kiPlayer.HighestUnlockedTechniqueIndex}", KiBlue);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Ki", $"{resources.MaxKi} max, {resources.RegenPerSecond}/s", KiBlue);

        leftY = DrawSection(content.X, leftY + 8, columnWidth, "World Gates", new Color(255, 168, 76));
        leftY = DrawMetric(content.X, leftY, columnWidth, "Eye defeated", FormatBool(NPC.downedBoss1), NPC.downedBoss1 ? new Color(132, 255, 170) : LockedText);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Evil boss", FormatBool(NPC.downedBoss2), NPC.downedBoss2 ? new Color(132, 255, 170) : LockedText);
        leftY = DrawMetric(content.X, leftY, columnWidth, "Skeletron", FormatBool(NPC.downedBoss3), NPC.downedBoss3 ? new Color(132, 255, 170) : LockedText);

        rightY = DrawSection(rightX, rightY + 8, columnWidth, "World Gates", new Color(255, 168, 76));
        rightY = DrawMetric(rightX, rightY, columnWidth, "Hardmode", FormatBool(Main.hardMode), Main.hardMode ? new Color(132, 255, 170) : LockedText);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Mech boss", FormatBool(NPC.downedMechBossAny), NPC.downedMechBossAny ? new Color(132, 255, 170) : LockedText);
        rightY = DrawMetric(rightX, rightY, columnWidth, "Plantera", FormatBool(NPC.downedPlantBoss), NPC.downedPlantBoss ? new Color(132, 255, 170) : LockedText);

        int noteY = Math.Max(leftY, rightY) + 8;
        DrawWrappedText("Read-only testing view. Grant/reset buttons stay out until config-gated.", new Vector2(content.X, noteY), content.Width, MutedText, 0.74f, panel.Bottom - noteY - PanelPadding);
    }

    private static void DrawHeldSpellPanel(Rectangle area, KiPlayer kiPlayer)
    {
        if (area.Height < 54)
        {
            return;
        }

        DrawSection(area.X, area.Y, area.Width, "Held Spell", GetHeldTechniqueColor(kiPlayer));
        int y = area.Y + 22;

        if (Main.LocalPlayer.HeldItem?.ModItem is not KiTechniqueItem techniqueItem)
        {
            DrawWrappedText("Held Spell: none", new Vector2(area.X, y), area.Width, MutedText, 0.76f, area.Bottom - y);
            return;
        }

        KiTechniqueDefinition technique = techniqueItem.TechniqueDefinition;
        bool unlocked = kiPlayer.IsTechniqueUnlocked(technique);
        string name = $"Held Spell: {technique.DisplayName}{(unlocked ? string.Empty : " (Locked)")}";
        y = DrawWrappedBlock(area.X, y, area.Width, name, unlocked ? technique.Color : LockedText, area.Bottom);

        if (!unlocked)
        {
            DrawWrappedText($"Reason: {kiPlayer.GetTechniqueLockReason(technique)}", new Vector2(area.X, y + 2), area.Width, LockedText, 0.76f, area.Bottom - y - 2);
            return;
        }

        y = DrawWrappedBlock(area.X, y + 2, area.Width, $"{technique.CategoryLabel} | {technique.SourceLabel}", MutedText, area.Bottom);
        string cost = technique.Behavior == KiTechniqueBehavior.Beam
            ? $"Cost: {kiPlayer.GetTechniqueInitialKiCost(technique)} start, {kiPlayer.GetTechniqueChannelKiCostPerSecond(technique)}/s"
            : $"Cost: {kiPlayer.GetTechniqueInitialKiCost(technique)}";
        DrawWrappedText(cost, new Vector2(area.X, y + 2), area.Width, KiBlue, 0.76f, area.Bottom - y - 2);
    }

    private static Rectangle GetContentArea(Rectangle panel)
    {
        return new Rectangle(panel.X + PanelPadding, panel.Y + 38, panel.Width - PanelPadding * 2, panel.Height - 50);
    }

    private static void DrawPanelShell(Rectangle panel, string title, Color accent)
    {
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, panel, new Color(5, 6, 12, 214));
        DrawBorder(panel, PanelOuter);

        Rectangle inner = new(panel.X + 2, panel.Y + 2, panel.Width - 4, panel.Height - 4);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, inner, PanelInner);

        Rectangle header = new(panel.X + 3, panel.Y + 3, panel.Width - 6, 27);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, header, PanelBand);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(header.X, header.Bottom - 2, header.Width, 2), accent * 0.9f);

        DrawText(title, new Vector2(panel.X + PanelPadding, panel.Y + 9), HeaderText, 0.82f);
    }

    private static void DrawBorder(Rectangle area, Color color)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Main.spriteBatch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 1), color);
        Main.spriteBatch.Draw(pixel, new Rectangle(area.X, area.Bottom - 1, area.Width, 1), color);
        Main.spriteBatch.Draw(pixel, new Rectangle(area.X, area.Y, 1, area.Height), color);
        Main.spriteBatch.Draw(pixel, new Rectangle(area.Right - 1, area.Y, 1, area.Height), color);
    }

    private static int DrawSection(int x, int y, int width, string title, Color accent)
    {
        DrawText(title.ToUpperInvariant(), new Vector2(x, y), accent, 0.74f);
        DrawDivider(new Rectangle(x, y + 16, width, 1), accent);
        return y + 22;
    }

    private static int DrawMetric(int x, int y, int width, string label, string value, Color valueColor)
    {
        if (y > Main.screenHeight - 28)
        {
            return y;
        }

        int labelWidth = Math.Min(82, width / 2);
        DrawText(label, new Vector2(x, y), MutedText, 0.72f);
        DrawClippedText(value, new Vector2(x + labelWidth, y), width - labelWidth, valueColor, 0.72f);
        return y + RowHeight;
    }

    private static void DrawLabelValue(string label, string value, Vector2 position, int width, Color valueColor)
    {
        DrawText(label.ToUpperInvariant(), position, MutedText, 0.7f);
        Vector2 valueSize = FontAssets.MouseText.Value.MeasureString(value) * 0.76f;
        DrawText(value, new Vector2(position.X + width - valueSize.X, position.Y), valueColor, 0.76f);
    }

    private static void DrawCompactRow(string label, string value, Vector2 position, int width, Color valueColor)
    {
        DrawText(label, position, MutedText, 0.72f);
        DrawClippedText(value, new Vector2(position.X + 72f, position.Y), width - 72, valueColor, 0.72f);
    }

    private static int DrawWrappedBlock(int x, int y, int width, string text, Color color, int bottom)
    {
        int maxHeight = Math.Max(0, bottom - y);
        return y + (int)DrawWrappedText(text, new Vector2(x, y), width, color, 0.72f, maxHeight) + 2;
    }

    private static float DrawWrappedText(string text, Vector2 position, int maxWidth, Color color, float scale, int maxHeight)
    {
        if (string.IsNullOrWhiteSpace(text) || maxHeight <= 0)
        {
            return 0f;
        }

        List<string> lines = WrapText(text, maxWidth, scale);
        float lineHeight = FontAssets.MouseText.Value.LineSpacing * scale * 0.86f;
        float y = position.Y;
        float drawn = 0f;

        for (int i = 0; i < lines.Count; i++)
        {
            if (drawn + lineHeight > maxHeight)
            {
                DrawText("...", new Vector2(position.X, y), MutedText, scale);
                drawn += lineHeight;
                break;
            }

            DrawText(lines[i], new Vector2(position.X, y), color, scale);
            y += lineHeight;
            drawn += lineHeight;
        }

        return drawn;
    }

    private static List<string> WrapText(string text, int maxWidth, float scale)
    {
        List<string> lines = new();
        string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string current = string.Empty;

        foreach (string word in words)
        {
            string candidate = string.IsNullOrEmpty(current) ? word : $"{current} {word}";

            if (MeasureTextWidth(candidate, scale) <= maxWidth)
            {
                current = candidate;
                continue;
            }

            if (!string.IsNullOrEmpty(current))
            {
                lines.Add(current);
            }

            current = word;

            while (MeasureTextWidth(current, scale) > maxWidth && current.Length > 4)
            {
                int splitIndex = Math.Max(3, current.Length - 3);

                while (splitIndex > 3 && MeasureTextWidth(current[..splitIndex] + "-", scale) > maxWidth)
                {
                    splitIndex--;
                }

                lines.Add(current[..splitIndex] + "-");
                current = current[splitIndex..];
            }
        }

        if (!string.IsNullOrEmpty(current))
        {
            lines.Add(current);
        }

        return lines;
    }

    private static void DrawClippedText(string text, Vector2 position, int maxWidth, Color color, float scale)
    {
        string clipped = ClipText(text, maxWidth, scale);
        DrawText(clipped, position, color, scale);
    }

    private static string ClipText(string text, int maxWidth, float scale)
    {
        if (MeasureTextWidth(text, scale) <= maxWidth)
        {
            return text;
        }

        const string suffix = "...";
        int length = text.Length;

        while (length > 0 && MeasureTextWidth(text[..length] + suffix, scale) > maxWidth)
        {
            length--;
        }

        return length <= 0 ? suffix : text[..length] + suffix;
    }

    private static float MeasureTextWidth(string text, float scale)
    {
        return FontAssets.MouseText.Value.MeasureString(text).X * scale;
    }

    private static void DrawDivider(Rectangle area, Color color)
    {
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, area, color * 0.55f);
    }

    private static void DrawOverflowHint(Rectangle panel)
    {
        DrawText("More stats hidden by screen size", new Vector2(panel.X + PanelPadding, panel.Bottom - 23), MutedText, 0.68f);
    }

    private static string GetHeldTechniqueName(KiPlayer kiPlayer)
    {
        if (Main.LocalPlayer.HeldItem?.ModItem is not KiTechniqueItem techniqueItem)
        {
            return "none";
        }

        KiTechniqueDefinition technique = techniqueItem.TechniqueDefinition;
        return kiPlayer.IsTechniqueUnlocked(technique)
            ? technique.DisplayName
            : $"{technique.DisplayName} (Locked)";
    }

    private static Color GetHeldTechniqueColor(KiPlayer kiPlayer)
    {
        if (Main.LocalPlayer.HeldItem?.ModItem is not KiTechniqueItem techniqueItem)
        {
            return MutedText;
        }

        KiTechniqueDefinition technique = techniqueItem.TechniqueDefinition;
        return kiPlayer.IsTechniqueUnlocked(technique) ? technique.Color : LockedText;
    }

    private static string FormatSigned(int value)
    {
        return value > 0 ? $"+{value}" : value.ToString();
    }

    private static string FormatBool(bool value)
    {
        return value ? "true" : "false";
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
            ? MutedText
            : kiPlayer.CurrentKaioKenLevel.AuraColor;
    }

    private static void DrawText(string text, Vector2 position, Color color, float scale)
    {
        Utils.DrawBorderString(Main.spriteBatch, text, position, color, scale);
    }

    private static void DrawBar(Rectangle area, float progress, Color fillColor, Color backgroundColor)
    {
        progress = MathHelper.Clamp(progress, 0f, 1f);
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Main.spriteBatch.Draw(pixel, area, new Color(4, 6, 12, 220));
        Main.spriteBatch.Draw(pixel, new Rectangle(area.X + 1, area.Y + 1, area.Width - 2, area.Height - 2), backgroundColor * 0.85f);

        int fillWidth = Math.Max(0, (int)((area.Width - 2) * progress));

        if (fillWidth > 0)
        {
            Main.spriteBatch.Draw(pixel, new Rectangle(area.X + 1, area.Y + 1, fillWidth, area.Height - 2), fillColor);
        }

        DrawBorder(area, fillColor * 0.65f);
    }
}

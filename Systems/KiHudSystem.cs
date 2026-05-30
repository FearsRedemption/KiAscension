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
    private const int HudWidth = 258;
    private const int HudHeight = 168;
    private const int PanelWidth = 408;
    private const int PanelPadding = 10;
    private const int RowHeight = 17;
    private const float TinyTextScale = 0.58f;
    private const float SmallTextScale = 0.64f;
    private const float PanelTextScale = 0.68f;

    private static readonly Color PanelOuter = new(255, 198, 76, 185);
    private static readonly Color PanelInner = new(7, 10, 18, 226);
    private static readonly Color PanelBand = new(18, 22, 34, 218);
    private static readonly Color ModuleFill = new(12, 18, 31, 218);
    private static readonly Color ModuleEdge = new(70, 82, 112, 145);
    private static readonly Color MutedText = new(152, 164, 188);
    private static readonly Color HeaderText = new(255, 228, 128);
    private static readonly Color LockedText = new(255, 112, 92);
    private static readonly Color KiBlue = new(82, 210, 255);
    private static readonly Color GoodText = new(132, 255, 170);

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
        Rectangle panel = new(16, 78, HudWidth, HudHeight);
        DrawPanelShell(panel, "KI // SCOUTER", kiPlayer.CurrentStage.AuraColor, compact: true);

        Rectangle content = GetContentArea(panel, compact: true);
        int x = content.X;
        int y = content.Y;
        int width = content.Width;
        int gap = 6;
        int half = (width - gap) / 2;

        DrawMicroModule(new Rectangle(x, y, half, 24), "KAI", $"LV {kiPlayer.KaiLevel}", HeaderText);
        DrawMicroModule(
            new Rectangle(x + half + gap, y, half, 24),
            "FLOW",
            $"{FormatSigned(resources.NetRegenPerSecond)}/s",
            resources.NetRegenPerSecond >= 0 ? GoodText : LockedText);
        y += 30;

        DrawKiBar(
            new Rectangle(x, y, width, 23),
            kiPlayer.GetKiProgress(),
            $"KI {kiPlayer.Ki}/{resources.MaxKi}",
            resources.PassiveDrainPerSecond > 0 ? $"DRN {resources.PassiveDrainPerSecond}/s" : "STABLE");
        y += 30;

        DrawBadge(
            new Rectangle(x, y, kiPlayer.IsKaioKenActive ? half : width, 20),
            "FORM",
            ShortFormName(kiPlayer.CurrentStage.DisplayName),
            kiPlayer.CurrentStage.AuraColor);

        if (kiPlayer.IsKaioKenActive)
        {
            DrawBadge(
                new Rectangle(x + half + gap, y, half, 20),
                "KK",
                ShortKaioKenName(kiPlayer.CurrentKaioKenLevel.DisplayName),
                GetKaioKenTextColor(kiPlayer));
        }
        y += 26;

        y = DrawHeldSpellHud(new Rectangle(x, y, width, 36), kiPlayer);
        y += 4;

        DrawDivider(new Rectangle(x, y, width, 1), kiPlayer.CurrentStage.AuraColor);
        y += 5;

        string gate = kiPlayer.GetNextCeilingText();
        DrawText("GATE", new Vector2(x, y), MutedText, TinyTextScale);
        DrawWrappedText(gate, new Vector2(x + 36, y), width - 36, new Color(226, 232, 242), TinyTextScale, panel.Bottom - y - 7);
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
        DrawWrappedText("Read-only testing view. Grant/reset buttons stay out until config-gated.", new Vector2(content.X, noteY), content.Width, MutedText, PanelTextScale, panel.Bottom - noteY - PanelPadding);
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
            DrawWrappedText("Spell: none", new Vector2(area.X, y), area.Width, MutedText, PanelTextScale, area.Bottom - y);
            return;
        }

        KiTechniqueDefinition technique = techniqueItem.TechniqueDefinition;
        bool unlocked = kiPlayer.IsTechniqueUnlocked(technique);
        string name = $"Spell: {technique.DisplayName}{(unlocked ? string.Empty : " [LOCKED]")}";
        y = DrawWrappedBlock(area.X, y, area.Width, name, unlocked ? technique.Color : LockedText, area.Bottom);

        if (!unlocked)
        {
            DrawWrappedText($"Requires: {kiPlayer.GetTechniqueLockReason(technique)}", new Vector2(area.X, y + 2), area.Width, LockedText, PanelTextScale, area.Bottom - y - 2);
            return;
        }

        y = DrawWrappedBlock(area.X, y + 2, area.Width, $"{technique.CategoryLabel} | {technique.SourceLabel}", MutedText, area.Bottom);
        string cost = technique.Behavior == KiTechniqueBehavior.Beam
            ? $"Cost: {kiPlayer.GetTechniqueInitialKiCost(technique)} start, {kiPlayer.GetTechniqueChannelKiCostPerSecond(technique)}/s"
            : $"Cost: {kiPlayer.GetTechniqueInitialKiCost(technique)}";
        DrawWrappedText(cost, new Vector2(area.X, y + 2), area.Width, KiBlue, PanelTextScale, area.Bottom - y - 2);
    }

    private static int DrawHeldSpellHud(Rectangle area, KiPlayer kiPlayer)
    {
        if (Main.LocalPlayer.HeldItem?.ModItem is not KiTechniqueItem techniqueItem)
        {
            DrawBadge(area, "SPELL", "none", MutedText);
            return area.Bottom;
        }

        KiTechniqueDefinition technique = techniqueItem.TechniqueDefinition;
        bool unlocked = kiPlayer.IsTechniqueUnlocked(technique);
        Color color = unlocked ? technique.Color : LockedText;
        string status = unlocked ? "READY" : "LOCKED";

        DrawModuleBack(area, color);
        DrawText("SPELL", new Vector2(area.X + 7, area.Y + 5), MutedText, TinyTextScale);
        DrawClippedText(technique.DisplayName, new Vector2(area.X + 48, area.Y + 5), area.Width - 104, color, SmallTextScale);
        DrawText(status, new Vector2(area.Right - 51, area.Y + 5), unlocked ? GoodText : LockedText, TinyTextScale);

        string detail = unlocked
            ? $"{technique.CategoryLabel} | {kiPlayer.GetTechniqueInitialKiCost(technique)} ki"
            : $"Req: {kiPlayer.GetTechniqueLockReason(technique)}";
        DrawClippedText(detail, new Vector2(area.X + 7, area.Y + 20), area.Width - 14, unlocked ? MutedText : new Color(255, 170, 124), TinyTextScale);
        return area.Bottom;
    }

    private static void DrawMicroModule(Rectangle area, string label, string value, Color valueColor)
    {
        DrawModuleBack(area, valueColor);
        DrawText(label, new Vector2(area.X + 6, area.Y + 4), MutedText, TinyTextScale);
        Vector2 valueSize = FontAssets.MouseText.Value.MeasureString(value) * SmallTextScale;
        DrawText(value, new Vector2(area.Right - 7 - valueSize.X, area.Y + 4), valueColor, SmallTextScale);
    }

    private static void DrawBadge(Rectangle area, string label, string value, Color accent)
    {
        DrawModuleBack(area, accent);
        DrawText(label, new Vector2(area.X + 7, area.Y + 4), MutedText, TinyTextScale);
        DrawClippedText(value, new Vector2(area.X + 44, area.Y + 4), area.Width - 51, accent, SmallTextScale);
    }

    private static void DrawKiBar(Rectangle area, float progress, string label, string status)
    {
        DrawModuleBack(area, KiBlue);
        Rectangle bar = new(area.X + 7, area.Y + 14, area.Width - 14, 5);
        DrawBar(bar, progress, KiBlue, new Color(13, 46, 64));
        DrawText(label, new Vector2(area.X + 7, area.Y + 3), KiBlue, SmallTextScale);
        Vector2 statusSize = FontAssets.MouseText.Value.MeasureString(status) * TinyTextScale;
        DrawText(status, new Vector2(area.Right - 7 - statusSize.X, area.Y + 4), status.Contains("DRN", StringComparison.Ordinal) ? LockedText : GoodText, TinyTextScale);
    }

    private static void DrawModuleBack(Rectangle area, Color accent)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Main.spriteBatch.Draw(pixel, area, ModuleFill);
        Main.spriteBatch.Draw(pixel, new Rectangle(area.X, area.Y, area.Width, 1), ModuleEdge);
        Main.spriteBatch.Draw(pixel, new Rectangle(area.X, area.Bottom - 1, area.Width, 1), ModuleEdge * 0.7f);
        Main.spriteBatch.Draw(pixel, new Rectangle(area.X, area.Y, 2, area.Height), accent * 0.72f);
        Main.spriteBatch.Draw(pixel, new Rectangle(area.Right - 1, area.Y + 2, 1, area.Height - 4), accent * 0.2f);
    }

    private static Rectangle GetContentArea(Rectangle panel, bool compact = false)
    {
        int headerHeight = compact ? 26 : 34;
        int bottomPadding = compact ? PanelPadding : PanelPadding + 4;
        return new Rectangle(panel.X + PanelPadding, panel.Y + headerHeight, panel.Width - PanelPadding * 2, panel.Height - headerHeight - bottomPadding);
    }

    private static void DrawPanelShell(Rectangle panel, string title, Color accent, bool compact = false)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Main.spriteBatch.Draw(pixel, new Rectangle(panel.X - 1, panel.Y - 1, panel.Width + 2, panel.Height + 2), accent * 0.18f);
        Main.spriteBatch.Draw(pixel, panel, new Color(3, 5, 10, 220));
        DrawBorder(panel, PanelOuter * 0.76f);

        Rectangle inner = new(panel.X + 2, panel.Y + 2, panel.Width - 4, panel.Height - 4);
        Main.spriteBatch.Draw(pixel, inner, PanelInner);

        int headerHeight = compact ? 20 : 25;
        Rectangle header = new(panel.X + 3, panel.Y + 3, panel.Width - 6, headerHeight);
        Main.spriteBatch.Draw(pixel, header, PanelBand);
        Main.spriteBatch.Draw(pixel, new Rectangle(header.X, header.Bottom - 1, header.Width, 1), accent * 0.82f);
        Main.spriteBatch.Draw(pixel, new Rectangle(panel.X + 4, panel.Y + 4, 2, panel.Height - 8), accent * 0.42f);
        Main.spriteBatch.Draw(pixel, new Rectangle(header.Right - 42, header.Y + 5, 28, 1), accent * 0.5f);
        Main.spriteBatch.Draw(pixel, new Rectangle(header.Right - 10, header.Y + 5, 4, 1), accent * 0.85f);

        DrawText(title, new Vector2(panel.X + PanelPadding, panel.Y + (compact ? 7 : 8)), HeaderText, compact ? 0.64f : 0.74f);
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
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Rectangle band = new(x, y, width, 17);
        Main.spriteBatch.Draw(pixel, band, ModuleFill * 0.72f);
        Main.spriteBatch.Draw(pixel, new Rectangle(x, y, 2, band.Height), accent * 0.75f);
        Main.spriteBatch.Draw(pixel, new Rectangle(x, band.Bottom - 1, width, 1), accent * 0.36f);
        DrawText(title.ToUpperInvariant(), new Vector2(x + 6, y + 2), accent, SmallTextScale);
        return y + 22;
    }

    private static int DrawMetric(int x, int y, int width, string label, string value, Color valueColor)
    {
        if (y > Main.screenHeight - 28)
        {
            return y;
        }

        int labelWidth = Math.Min(82, width / 2);
        DrawText(label, new Vector2(x, y), MutedText, PanelTextScale);
        DrawClippedText(value, new Vector2(x + labelWidth, y), width - labelWidth, valueColor, PanelTextScale);
        return y + RowHeight;
    }

    private static void DrawLabelValue(string label, string value, Vector2 position, int width, Color valueColor)
    {
        DrawText(label.ToUpperInvariant(), position, MutedText, TinyTextScale);
        Vector2 valueSize = FontAssets.MouseText.Value.MeasureString(value) * SmallTextScale;
        DrawText(value, new Vector2(position.X + width - valueSize.X, position.Y), valueColor, SmallTextScale);
    }

    private static void DrawCompactRow(string label, string value, Vector2 position, int width, Color valueColor)
    {
        DrawText(label, position, MutedText, PanelTextScale);
        DrawClippedText(value, new Vector2(position.X + 72f, position.Y), width - 72, valueColor, PanelTextScale);
    }

    private static int DrawWrappedBlock(int x, int y, int width, string text, Color color, int bottom)
    {
        int maxHeight = Math.Max(0, bottom - y);
        return y + (int)DrawWrappedText(text, new Vector2(x, y), width, color, PanelTextScale, maxHeight) + 2;
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

    private static string ShortFormName(string displayName)
    {
        return displayName switch
        {
            "Base Saiyan" => "Base",
            "Awakened State" => "Awakened",
            "Super Saiyan" => "SSJ",
            "Super Saiyan 2" => "SSJ2",
            "Super Saiyan 3" => "SSJ3",
            "Super Saiyan God" => "God",
            "Super Saiyan Blue" => "Blue",
            "Ultra Instinct Sign" => "UI Sign",
            "Ultra Instinct" => "UI",
            _ => displayName
        };
    }

    private static string ShortKaioKenName(string displayName)
    {
        return displayName switch
        {
            "Off" => "Off",
            "Kaio-Ken" => "x1",
            _ when displayName.StartsWith("Kaio-Ken ", StringComparison.Ordinal) => displayName["Kaio-Ken ".Length..],
            _ => displayName
        };
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

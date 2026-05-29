using System;
using System.Collections.Generic;
using System.IO;
using KiAscension.Common;
using KiAscension.Items;
using KiAscension.Items.Techniques;
using KiAscension.Systems;
using KiAscension.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KiAscension.Players;

public class KiPlayer : ModPlayer
{
    private const int BaseMaxKi = 70;
    private const int KaiLevelExperienceFactor = 220;
    private const int WitnessRange = 1200;
    private const int TapInputThreshold = 12;
    private const int BaseSaiyanChargeTicks = 45;
    private const int SaiyanChargeTicksPerStage = 18;
    private const int SaiyanPowerDownChargeTicks = 90;
    private const int BaseKaioKenChargeTicks = 30;
    private const int KaioKenChargeTicksPerLevel = 8;
    private const int KaioKenPowerDownChargeTicks = 60;
    private const int GravityRoomRadiusTiles = 18;
    private const int TrainingIntervalTicks = 120;

    private int pendingAnnouncementStage = -1;
    private int highestAnnouncedTechniqueIndex;
    private int naturalHairStyle;
    private Color naturalHairColor;
    private bool naturalHairCaptured;
    private bool shownProgressionNotice;
    private int saiyanPowerUpTicks;
    private int saiyanPowerDownTicks;
    private int kaioKenPowerUpTicks;
    private int kaioKenPowerDownTicks;
    private int trainingTicks;

    public int PowerExperience { get; private set; }

    public int KiPowerExperience { get; private set; }

    public int KaiLevel { get; private set; }

    public int CurrentStageIndex { get; private set; }

    public int UnlockedStageIndex { get; private set; }

    public int CurrentKaioKenLevelIndex { get; private set; }

    public int UnlockedKaioKenLevelIndex { get; private set; }

    public int Ki { get; private set; }

    public int SelectedTechniqueIndex { get; private set; }

    public bool IsWeightTraining { get; set; }

    public int TotalPowerExperience => PowerExperience + KiPowerExperience;

    public int PhysicalPowerLevel => CalculateKaiLevel(PowerExperience);

    public int KiPowerLevel => CalculateKaiLevel(KiPowerExperience);

    public int MaxKi => BaseMaxKi + CurrentStage.MaxKiBonus + Math.Max(0, KaiLevel - 1) * 8 + Math.Max(0, KiPowerLevel - 1) * 6;

    public int KiRegenPerSecond => 1 + CurrentStage.KiRegenBonus + Math.Max(0, KaiLevel - 1) / 4 + Math.Max(0, KiPowerLevel - 1) / 5;

    public KiResourceSnapshot KiResources => new(MaxKi, KiRegenPerSecond, ActiveKiDrainPerSecond, KiResourceMath.GetTechniqueCostMultiplier(KiPowerLevel, CurrentStage.Stage));

    public StageDefinition CurrentStage => AscensionStages.Get(CurrentStageIndex);

    public StageDefinition NextStage => AscensionStages.Get(UnlockedStageIndex + 1);

    public KaioKenLevelDefinition CurrentKaioKenLevel => KaioKenLevels.Get(CurrentKaioKenLevelIndex);

    public bool IsKaioKenActive => CurrentKaioKenLevelIndex > 0;

    public StageDefinition UnlockedStage => AscensionStages.Get(UnlockedStageIndex);

    public KaioKenLevelDefinition UnlockedKaioKenLevel => KaioKenLevels.Get(UnlockedKaioKenLevelIndex);

    public float CombinedDamageMultiplier => CurrentStage.DamageMultiplier * CurrentKaioKenLevel.DamageMultiplier;

    public float CombinedSpeedMultiplier => CurrentStage.SpeedMultiplier * CurrentKaioKenLevel.SpeedMultiplier;

    public int ActiveKiDrainPerSecond => CurrentStage.KiDrainPerSecond + CurrentKaioKenLevel.KiDrainPerSecond;

    public int ActiveLifeDrainPerSecond => CurrentKaioKenLevel.LifeDrainPerSecond;

    public float FlightControlMultiplier => CurrentStage.FlightControlMultiplier;

    public string NextKaioKenGateText
    {
        get
        {
            if (UnlockedKaioKenLevelIndex >= KaioKenLevels.MaxLevelIndex)
            {
                return "Kaio-Ken: mastered";
            }

            KaioKenLevelDefinition next = KaioKenLevels.Get(UnlockedKaioKenLevelIndex + 1);

            if (TotalPowerExperience < next.RequiredPower)
            {
                return $"Next Kaio-Ken: {next.DisplayName} {TotalPowerExperience}/{next.RequiredPower} total power";
            }

            return AscensionStages.IsGateSatisfied(next.RequiredGate)
                ? $"Next Kaio-Ken ready: {next.DisplayName}"
                : $"Next Kaio-Ken: {next.DisplayName}; {AscensionStages.GetGateText(next.RequiredGate)}";
        }
    }

    public int HighestUnlockedTechniqueIndex => KiTechniques.GetHighestUnlockedIndex(KiPowerExperience, UnlockedStageIndex);

    public KiTechniqueDefinition CurrentTechnique => KiTechniques.Get(Math.Clamp(SelectedTechniqueIndex, 0, HighestUnlockedTechniqueIndex));

    public int ExperienceForNextKaiLevel => KaiLevelExperienceFactor * KaiLevel * KaiLevel;

    public bool HasPendingWitnessBreakthrough =>
        UnlockedStageIndex < AscensionStages.MaxStageIndex
        && TotalPowerExperience >= NextStage.RequiredExperience
        && AscensionStages.IsGateSatisfied(NextStage)
        && NextStage.RequiresWitnessLoss;

    public override void Initialize()
    {
        PowerExperience = 0;
        KiPowerExperience = 0;
        KaiLevel = 1;
        CurrentStageIndex = 0;
        UnlockedStageIndex = 0;
        CurrentKaioKenLevelIndex = 0;
        UnlockedKaioKenLevelIndex = 0;
        Ki = BaseMaxKi;
        SelectedTechniqueIndex = 0;
        highestAnnouncedTechniqueIndex = 0;
        naturalHairStyle = 0;
        naturalHairColor = Color.White;
        naturalHairCaptured = false;
        shownProgressionNotice = false;
        saiyanPowerUpTicks = 0;
        saiyanPowerDownTicks = 0;
        kaioKenPowerUpTicks = 0;
        kaioKenPowerDownTicks = 0;
        trainingTicks = 0;
    }

    public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
    {
        Item focus = new();
        focus.SetDefaults(ModContent.ItemType<KiTrainingFocus>());

        Item basicBlast = new();
        basicBlast.SetDefaults(ModContent.ItemType<BasicKiBlastSpell>());

        return new[] { focus, basicBlast };
    }

    public override void SaveData(TagCompound tag)
    {
        tag["PowerExperience"] = PowerExperience;
        tag["KiPowerExperience"] = KiPowerExperience;
        tag["KaiLevel"] = KaiLevel;
        tag["CurrentStageIndex"] = CurrentStageIndex;
        tag["UnlockedStageIndex"] = UnlockedStageIndex;
        tag["CurrentKaioKenLevelIndex"] = CurrentKaioKenLevelIndex;
        tag["UnlockedKaioKenLevelIndex"] = UnlockedKaioKenLevelIndex;
        tag["Ki"] = Ki;
        tag["SelectedTechniqueIndex"] = SelectedTechniqueIndex;
        tag["HighestAnnouncedTechniqueIndex"] = highestAnnouncedTechniqueIndex;
    }

    public override void LoadData(TagCompound tag)
    {
        PowerExperience = tag.ContainsKey("PowerExperience") ? tag.GetInt("PowerExperience") : 0;
        KiPowerExperience = tag.ContainsKey("KiPowerExperience") ? tag.GetInt("KiPowerExperience") : PowerExperience;
        KaiLevel = tag.ContainsKey("KaiLevel") ? tag.GetInt("KaiLevel") : CalculateKaiLevel(TotalPowerExperience);
        CurrentStageIndex = tag.ContainsKey("CurrentStageIndex") ? tag.GetInt("CurrentStageIndex") : 0;
        UnlockedStageIndex = tag.ContainsKey("UnlockedStageIndex") ? tag.GetInt("UnlockedStageIndex") : 0;
        CurrentKaioKenLevelIndex = tag.ContainsKey("CurrentKaioKenLevelIndex") ? tag.GetInt("CurrentKaioKenLevelIndex") : 0;
        UnlockedKaioKenLevelIndex = tag.ContainsKey("UnlockedKaioKenLevelIndex") ? tag.GetInt("UnlockedKaioKenLevelIndex") : 0;
        Ki = tag.ContainsKey("Ki") ? tag.GetInt("Ki") : BaseMaxKi;
        SelectedTechniqueIndex = tag.ContainsKey("SelectedTechniqueIndex") ? tag.GetInt("SelectedTechniqueIndex") : 0;

        UnlockedStageIndex = Math.Clamp(UnlockedStageIndex, 0, AscensionStages.MaxStageIndex);
        CurrentStageIndex = Math.Clamp(CurrentStageIndex, 0, UnlockedStageIndex);
        UnlockedKaioKenLevelIndex = Math.Clamp(Math.Max(UnlockedKaioKenLevelIndex, KaioKenLevels.GetHighestUnlockedIndex(TotalPowerExperience)), 0, KaioKenLevels.MaxLevelIndex);
        CurrentKaioKenLevelIndex = Math.Clamp(CurrentKaioKenLevelIndex, 0, UnlockedKaioKenLevelIndex);
        KaiLevel = Math.Max(1, CalculateKaiLevel(TotalPowerExperience));
        Ki = Math.Clamp(Ki, 0, MaxKi);
        SelectedTechniqueIndex = Math.Clamp(SelectedTechniqueIndex, 0, HighestUnlockedTechniqueIndex);
        highestAnnouncedTechniqueIndex = tag.ContainsKey("HighestAnnouncedTechniqueIndex")
            ? Math.Clamp(tag.GetInt("HighestAnnouncedTechniqueIndex"), 0, HighestUnlockedTechniqueIndex)
            : HighestUnlockedTechniqueIndex;
        pendingAnnouncementStage = -1;
    }

    public override void ResetEffects()
    {
        IsWeightTraining = false;
    }

    public override void OnEnterWorld()
    {
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        EnsureInventoryItem(ModContent.ItemType<KiTrainingFocus>());
        EnsureInventoryItem(ModContent.ItemType<BasicKiBlastSpell>());
    }

    public void NetSend(BinaryWriter writer)
    {
        WriteState(writer);
    }

    public void NetReceive(BinaryReader reader)
    {
        ReadState(reader);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)KiAscensionMessageType.SyncPlayerState);
        packet.Write((byte)Player.whoAmI);
        WriteState(packet);
        packet.Send(toWho, fromWho);
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        KiPlayer clone = (KiPlayer)targetCopy;
        clone.PowerExperience = PowerExperience;
        clone.KiPowerExperience = KiPowerExperience;
        clone.KaiLevel = KaiLevel;
        clone.CurrentStageIndex = CurrentStageIndex;
        clone.UnlockedStageIndex = UnlockedStageIndex;
        clone.CurrentKaioKenLevelIndex = CurrentKaioKenLevelIndex;
        clone.UnlockedKaioKenLevelIndex = UnlockedKaioKenLevelIndex;
        clone.Ki = Ki;
        clone.SelectedTechniqueIndex = SelectedTechniqueIndex;
        clone.highestAnnouncedTechniqueIndex = highestAnnouncedTechniqueIndex;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        KiPlayer clone = (KiPlayer)clientPlayer;

        if (clone.CurrentStageIndex == CurrentStageIndex
            && clone.CurrentKaioKenLevelIndex == CurrentKaioKenLevelIndex
            && clone.SelectedTechniqueIndex == SelectedTechniqueIndex)
        {
            return;
        }

        SendClientSelection();
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        HandleSaiyanPowerUpInput();
        HandleSaiyanPowerDownInput();
        HandleKaioKenPowerUpInput();
        HandleKaioKenPowerDownInput();
        HandleInspectionInput();
    }

    public override void PostUpdateEquips()
    {
        StageDefinition stage = CurrentStage;
        Player.statDefense += stage.DefenseBonus;
        Player.endurance = MathHelper.Clamp(Player.endurance + stage.DefenseBonus * 0.003f, 0f, 0.9f);
        Player.lifeRegen += stage.LifeRegenBonus * 2;
    }

    public override void PostUpdateRunSpeeds()
    {
        StageDefinition stage = CurrentStage;
        float speedMultiplier = CombinedSpeedMultiplier;
        Player.maxRunSpeed *= speedMultiplier;
        Player.accRunSpeed *= speedMultiplier;
        Player.runAcceleration *= speedMultiplier;

        if (IsWeightTraining)
        {
            Player.maxRunSpeed *= 0.84f;
            Player.accRunSpeed *= 0.78f;
            Player.runAcceleration *= 0.78f;
        }

        if (IsNearGravityRoomCore())
        {
            Player.maxRunSpeed *= 0.82f;
            Player.accRunSpeed *= 0.75f;
            Player.runAcceleration *= 0.75f;
        }
    }

    public override void PostUpdate()
    {
        RechargeKi();
        DrainActiveTransformations();
        HandleTraining();
        RefreshTechniqueUnlocks(false);
        RefreshKaioKenUnlocks(false);
        ShowProgressionNoticeOnce();

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            AnnouncePendingBreakthrough();
        }
        else
        {
            TryUnlockAvailableStages(false);
        }

        if (!Main.dedServ)
        {
            ApplyAscensionVisuals();
        }

        DrawAura();
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= CombinedDamageMultiplier;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || target.friendly || target.townNPC || damageDone <= 0)
        {
            return;
        }

        if (Player.HeldItem?.ModItem is KiTechniqueItem)
        {
            return;
        }

        AddPowerExperience(Math.Max(1, damageDone / 14), false);
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            return;
        }

        for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
        {
            Player otherPlayer = Main.player[playerIndex];

            if (otherPlayer is null || !otherPlayer.active || otherPlayer.whoAmI == Player.whoAmI)
            {
                continue;
            }

            if (Vector2.DistanceSquared(otherPlayer.Center, Player.Center) > WitnessRange * WitnessRange)
            {
                continue;
            }

            otherPlayer.GetModPlayer<KiPlayer>().WitnessLoss(Player.Center, "ally");
        }
    }

    public bool TryConsumeKi(int amount)
    {
        if (!HasKi(amount))
        {
            return false;
        }

        Ki -= amount;
        SyncStateIfServer();
        return true;
    }

    public bool HasKi(int amount)
    {
        return Ki >= amount;
    }

    public bool HasKiForTechnique(KiTechniqueDefinition technique)
    {
        return HasKi(GetTechniqueInitialKiCost(technique));
    }

    public bool TryConsumeTechniqueInitialKi(KiTechniqueDefinition technique)
    {
        return TryConsumeKi(GetTechniqueInitialKiCost(technique));
    }

    public bool TryConsumeTechniqueChannelKi(KiTechniqueDefinition technique, int ticks)
    {
        return TryConsumeKi(GetTechniqueChannelKiCost(technique, ticks));
    }

    public void AddExperience(int amount, bool announce)
    {
        AddPowerExperience(amount, announce);
    }

    public void AddPowerExperience(int amount, bool announce)
    {
        AddProgress(amount, 0, announce);
    }

    public void AddKiExperience(int amount, bool announce)
    {
        AddProgress(0, amount, announce);
    }

    public void AddTrainingExperience(int powerAmount, int kiPowerAmount, bool announce)
    {
        AddProgress(powerAmount, kiPowerAmount, announce);
    }

    public int GetKiTechniqueDamage(KiTechniqueDefinition technique)
    {
        float kiPowerMultiplier = 1f + Math.Max(0, KiPowerLevel - 1) * 0.055f;
        return Math.Max(1, (int)(technique.BaseDamage * kiPowerMultiplier));
    }

    public int GetTechniqueInitialKiCost(KiTechniqueDefinition technique)
    {
        return KiResourceMath.ScaleTechniqueCost(technique.InitialKiCost, KiPowerLevel, CurrentStage.Stage);
    }

    public int GetTechniqueChannelKiCostPerSecond(KiTechniqueDefinition technique)
    {
        return KiResourceMath.ScaleTechniqueCost(technique.ChannelKiCostPerSecond, KiPowerLevel, CurrentStage.Stage);
    }

    public int GetTechniqueChannelKiCost(KiTechniqueDefinition technique, int ticks)
    {
        return KiResourceMath.ScaleChannelCostForTicks(technique.ChannelKiCostPerSecond, ticks, KiPowerLevel, CurrentStage.Stage);
    }

    private void AddProgress(int powerAmount, int kiPowerAmount, bool announce)
    {
        if ((powerAmount <= 0 && kiPowerAmount <= 0) || Main.netMode == NetmodeID.MultiplayerClient)
        {
            return;
        }

        int oldPowerExperience = PowerExperience;
        int oldKiPowerExperience = KiPowerExperience;
        int oldKaiLevel = KaiLevel;
        int oldUnlockedStageIndex = UnlockedStageIndex;
        int oldUnlockedKaioKenLevelIndex = UnlockedKaioKenLevelIndex;
        int oldHighestTechniqueIndex = HighestUnlockedTechniqueIndex;
        PowerExperience += Math.Max(0, powerAmount);
        KiPowerExperience += Math.Max(0, kiPowerAmount);
        KaiLevel = CalculateKaiLevel(TotalPowerExperience);

        if (announce && Player.whoAmI == Main.myPlayer)
        {
            if (powerAmount > 0)
            {
                Main.NewText($"+{powerAmount} physical power", new Color(255, 220, 130));
            }

            if (kiPowerAmount > 0)
            {
                Main.NewText($"+{kiPowerAmount} ki power", new Color(120, 220, 255));
            }
        }

        TryUnlockAvailableStages(false);
        RefreshKaioKenUnlocks(true);
        RefreshTechniqueUnlocks(true);
        AnnounceStateDelta(oldPowerExperience, oldKiPowerExperience, oldKaiLevel, oldUnlockedStageIndex, oldUnlockedKaioKenLevelIndex, oldHighestTechniqueIndex);
        SyncStateIfServer();
    }

    public void WitnessLoss(Vector2 lossPosition, string source)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            return;
        }

        if (!HasPendingWitnessBreakthrough)
        {
            return;
        }

        int oldPowerExperience = PowerExperience;
        int oldKiPowerExperience = KiPowerExperience;
        int oldKaiLevel = KaiLevel;
        int oldUnlockedStageIndex = UnlockedStageIndex;
        int oldUnlockedKaioKenLevelIndex = UnlockedKaioKenLevelIndex;
        int oldHighestTechniqueIndex = HighestUnlockedTechniqueIndex;
        TryUnlockAvailableStages(true);
        RefreshKaioKenUnlocks(true);
        AnnounceStateDelta(oldPowerExperience, oldKiPowerExperience, oldKaiLevel, oldUnlockedStageIndex, oldUnlockedKaioKenLevelIndex, oldHighestTechniqueIndex);
        SyncStateIfServer();

        if (Player.whoAmI == Main.myPlayer)
        {
            StageDefinition unlockedStage = AscensionStages.Get(UnlockedStageIndex);
            Main.NewText($"A nearby {source} loss pushes your ceiling beyond its limit: {unlockedStage.DisplayName}.", unlockedStage.AuraColor);
        }
    }

    private void TryUnlockAvailableStages(bool witnessedLoss)
    {
        while (UnlockedStageIndex < AscensionStages.MaxStageIndex)
        {
            StageDefinition next = AscensionStages.Get(UnlockedStageIndex + 1);

            if (TotalPowerExperience < next.RequiredExperience)
            {
                return;
            }

            if (!AscensionStages.IsGateSatisfied(next))
            {
                AnnounceProgressionGate(next);
                return;
            }

            if (next.RequiresWitnessLoss && !witnessedLoss)
            {
                AnnouncePendingBreakthrough();
                return;
            }

            UnlockedStageIndex++;
            CurrentStageIndex = Math.Clamp(CurrentStageIndex, 0, UnlockedStageIndex);
            Ki = MaxKi;
            pendingAnnouncementStage = -1;
            witnessedLoss = false;
            RefreshTechniqueUnlocks(true);

        }
    }

    private void AnnounceProgressionGate(StageDefinition stage)
    {
        int nextStageIndex = UnlockedStageIndex + 1;
        int gateAnnouncementMarker = -nextStageIndex;

        if (Player.whoAmI == Main.myPlayer && pendingAnnouncementStage != gateAnnouncementMarker)
        {
            Main.NewText($"{stage.DisplayName} needs one more trial: {AscensionStages.GetGateText(stage.RequiredGate)}.", new Color(255, 180, 120));
        }

        pendingAnnouncementStage = gateAnnouncementMarker;
    }

    private void AnnouncePendingBreakthrough()
    {
        if (!HasPendingWitnessBreakthrough)
        {
            return;
        }

        int nextStageIndex = UnlockedStageIndex + 1;

        if (Player.whoAmI == Main.myPlayer && pendingAnnouncementStage != nextStageIndex)
        {
            Main.NewText($"{NextStage.DisplayName} is within reach, but you need a breaking point.", new Color(255, 150, 110));
        }

        pendingAnnouncementStage = nextStageIndex;
    }

    public float GetKiProgress()
    {
        return MaxKi <= 0 ? 0f : Ki / (float)MaxKi;
    }

    public float GetKaiLevelProgress()
    {
        int previousLevelExperience = KaiLevelExperienceFactor * Math.Max(0, KaiLevel - 1) * Math.Max(0, KaiLevel - 1);
        int nextLevelExperience = ExperienceForNextKaiLevel;

        if (nextLevelExperience <= previousLevelExperience)
        {
            return 1f;
        }

        return MathHelper.Clamp((TotalPowerExperience - previousLevelExperience) / (float)(nextLevelExperience - previousLevelExperience), 0f, 1f);
    }

    public string GetNextCeilingText()
    {
        if (UnlockedStageIndex >= AscensionStages.MaxStageIndex)
        {
            return "Ceiling: mastered";
        }

        StageDefinition next = NextStage;

        if (TotalPowerExperience >= next.RequiredExperience)
        {
            if (!AscensionStages.IsGateSatisfied(next))
            {
                return $"Ceiling ready: {next.DisplayName}; {AscensionStages.GetGateText(next.RequiredGate)}";
            }

            return next.RequiresWitnessLoss
                ? $"Ceiling ready: {next.DisplayName} needs a breaking point"
                : $"Ceiling ready: {next.DisplayName}";
        }

        return $"Next ceiling: {next.DisplayName} {TotalPowerExperience}/{next.RequiredExperience} total power";
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        int oldPowerExperience = PowerExperience;
        int oldKiPowerExperience = KiPowerExperience;
        int oldKaiLevel = KaiLevel;
        int oldUnlockedStageIndex = UnlockedStageIndex;
        int oldUnlockedKaioKenLevelIndex = UnlockedKaioKenLevelIndex;
        int oldHighestTechniqueIndex = HighestUnlockedTechniqueIndex;

        ReadState(reader);

        if (Player.whoAmI == Main.myPlayer)
        {
            AnnounceStateDelta(oldPowerExperience, oldKiPowerExperience, oldKaiLevel, oldUnlockedStageIndex, oldUnlockedKaioKenLevelIndex, oldHighestTechniqueIndex);
        }
    }

    public void ReceiveClientSelection(BinaryReader reader)
    {
        int requestedStageIndex = reader.ReadInt32();
        int requestedTechniqueIndex = reader.ReadInt32();
        int requestedKaioKenLevelIndex = reader.ReadInt32();

        CurrentStageIndex = Math.Clamp(requestedStageIndex, 0, UnlockedStageIndex);
        SelectedTechniqueIndex = Math.Clamp(requestedTechniqueIndex, 0, HighestUnlockedTechniqueIndex);
        CurrentKaioKenLevelIndex = Math.Clamp(requestedKaioKenLevelIndex, 0, UnlockedKaioKenLevelIndex);
    }

    private void WriteState(BinaryWriter writer)
    {
        writer.Write(PowerExperience);
        writer.Write(KiPowerExperience);
        writer.Write(KaiLevel);
        writer.Write(CurrentStageIndex);
        writer.Write(UnlockedStageIndex);
        writer.Write(CurrentKaioKenLevelIndex);
        writer.Write(UnlockedKaioKenLevelIndex);
        writer.Write(Ki);
        writer.Write(SelectedTechniqueIndex);
        writer.Write(highestAnnouncedTechniqueIndex);
    }

    private void ReadState(BinaryReader reader)
    {
        PowerExperience = reader.ReadInt32();
        KiPowerExperience = reader.ReadInt32();
        KaiLevel = reader.ReadInt32();
        CurrentStageIndex = reader.ReadInt32();
        UnlockedStageIndex = reader.ReadInt32();
        CurrentKaioKenLevelIndex = reader.ReadInt32();
        UnlockedKaioKenLevelIndex = reader.ReadInt32();
        Ki = reader.ReadInt32();
        SelectedTechniqueIndex = reader.ReadInt32();
        highestAnnouncedTechniqueIndex = reader.ReadInt32();

        UnlockedStageIndex = Math.Clamp(UnlockedStageIndex, 0, AscensionStages.MaxStageIndex);
        CurrentStageIndex = Math.Clamp(CurrentStageIndex, 0, UnlockedStageIndex);
        UnlockedKaioKenLevelIndex = Math.Clamp(Math.Max(UnlockedKaioKenLevelIndex, KaioKenLevels.GetHighestUnlockedIndex(TotalPowerExperience)), 0, KaioKenLevels.MaxLevelIndex);
        CurrentKaioKenLevelIndex = Math.Clamp(CurrentKaioKenLevelIndex, 0, UnlockedKaioKenLevelIndex);
        KaiLevel = Math.Max(1, CalculateKaiLevel(TotalPowerExperience));
        Ki = Math.Clamp(Ki, 0, MaxKi);
        SelectedTechniqueIndex = Math.Clamp(SelectedTechniqueIndex, 0, HighestUnlockedTechniqueIndex);
        highestAnnouncedTechniqueIndex = Math.Clamp(highestAnnouncedTechniqueIndex, 0, HighestUnlockedTechniqueIndex);
    }

    private void AnnounceStateDelta(
        int oldPowerExperience,
        int oldKiPowerExperience,
        int oldKaiLevel,
        int oldUnlockedStageIndex,
        int oldUnlockedKaioKenLevelIndex,
        int oldHighestTechniqueIndex)
    {
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        if ((PowerExperience > oldPowerExperience || KiPowerExperience > oldKiPowerExperience) && KaiLevel > oldKaiLevel)
        {
            Main.NewText($"Kai Level increased: {KaiLevel}", new Color(255, 230, 130));
        }

        if (UnlockedStageIndex > oldUnlockedStageIndex)
        {
            for (int stageIndex = oldUnlockedStageIndex + 1; stageIndex <= UnlockedStageIndex; stageIndex++)
            {
                StageDefinition stage = AscensionStages.Get(stageIndex);
                Main.NewText($"Ceiling surpassed: {stage.DisplayName}", stage.AuraColor);
            }
        }

        if (UnlockedKaioKenLevelIndex > oldUnlockedKaioKenLevelIndex)
        {
            for (int levelIndex = oldUnlockedKaioKenLevelIndex + 1; levelIndex <= UnlockedKaioKenLevelIndex; levelIndex++)
            {
                KaioKenLevelDefinition level = KaioKenLevels.Get(levelIndex);
                Main.NewText($"Kaio-Ken unlocked: {level.DisplayName}", level.AuraColor);
            }
        }

        int highestTechniqueIndex = HighestUnlockedTechniqueIndex;

        if (highestTechniqueIndex > oldHighestTechniqueIndex)
        {
            for (int techniqueIndex = oldHighestTechniqueIndex + 1; techniqueIndex <= highestTechniqueIndex; techniqueIndex++)
            {
                KiTechniqueDefinition technique = KiTechniques.Get(techniqueIndex);
                Main.NewText($"Technique unlocked: {technique.DisplayName}", technique.Color);
                GrantTechniqueItem(technique);
            }
        }
    }

    private void GrantTechniqueItem(KiTechniqueDefinition technique)
    {
        int itemType = KiTechniques.GetItemType(technique.Technique);

        EnsureInventoryItem(itemType);
    }

    private void EnsureInventoryItem(int itemType)
    {
        foreach (Item item in Player.inventory)
        {
            if (!item.IsAir && item.type == itemType)
            {
                return;
            }
        }

        Player.QuickSpawnItem(new EntitySource_Misc("KiAscensionItemGrant"), itemType);
    }

    private void SendClientSelection()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            return;
        }

        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)KiAscensionMessageType.ClientSelection);
        packet.Write((byte)Player.whoAmI);
        packet.Write(CurrentStageIndex);
        packet.Write(SelectedTechniqueIndex);
        packet.Write(CurrentKaioKenLevelIndex);
        packet.Send();
    }

    private void SyncStateIfServer()
    {
        if (Main.netMode == NetmodeID.Server)
        {
            SyncPlayer(-1, -1, false);
        }
    }

    private static int CalculateKaiLevel(int powerExperience)
    {
        return Math.Max(1, 1 + (int)Math.Sqrt(Math.Max(0, powerExperience) / (float)KaiLevelExperienceFactor));
    }

    private void RefreshTechniqueUnlocks(bool announce)
    {
        int highestUnlocked = HighestUnlockedTechniqueIndex;
        SelectedTechniqueIndex = Math.Clamp(SelectedTechniqueIndex, 0, highestUnlocked);

        if (highestUnlocked <= highestAnnouncedTechniqueIndex)
        {
            return;
        }

        highestAnnouncedTechniqueIndex = highestUnlocked;
    }

    private void RefreshKaioKenUnlocks(bool announce)
    {
        int highestUnlocked = KaioKenLevels.GetHighestUnlockedIndex(TotalPowerExperience);
        UnlockedKaioKenLevelIndex = Math.Max(UnlockedKaioKenLevelIndex, highestUnlocked);
        CurrentKaioKenLevelIndex = Math.Clamp(CurrentKaioKenLevelIndex, 0, UnlockedKaioKenLevelIndex);
    }

    private void HandleSaiyanPowerUpInput()
    {
        if (AscensionKeybindSystem.PowerUpKey.Current)
        {
            saiyanPowerUpTicks++;
            saiyanPowerDownTicks = 0;
            int targetStageIndex = UnlockedStageIndex;

            if (targetStageIndex > CurrentStageIndex && saiyanPowerUpTicks >= GetSaiyanPowerUpDuration(targetStageIndex))
            {
                SetForm(targetStageIndex);
                saiyanPowerUpTicks = 0;
            }

            return;
        }

        if (saiyanPowerUpTicks > 0 && saiyanPowerUpTicks <= TapInputThreshold)
        {
            ShiftForm(1);
        }

        saiyanPowerUpTicks = 0;
    }

    private void HandleSaiyanPowerDownInput()
    {
        if (AscensionKeybindSystem.PowerDownKey.Current)
        {
            saiyanPowerDownTicks++;
            saiyanPowerUpTicks = 0;

            if (CurrentStageIndex > 0 && saiyanPowerDownTicks >= SaiyanPowerDownChargeTicks)
            {
                SetForm(0);
                saiyanPowerDownTicks = 0;
            }

            return;
        }

        if (saiyanPowerDownTicks > 0 && saiyanPowerDownTicks <= TapInputThreshold)
        {
            ShiftForm(-1);
        }

        saiyanPowerDownTicks = 0;
    }

    private void HandleKaioKenPowerUpInput()
    {
        if (AscensionKeybindSystem.KaioKenPowerUpKey.Current)
        {
            kaioKenPowerUpTicks++;
            kaioKenPowerDownTicks = 0;
            int targetLevelIndex = UnlockedKaioKenLevelIndex;

            if (targetLevelIndex > CurrentKaioKenLevelIndex && kaioKenPowerUpTicks >= GetKaioKenPowerUpDuration(targetLevelIndex))
            {
                SetKaioKenLevel(targetLevelIndex);
                kaioKenPowerUpTicks = 0;
            }

            return;
        }

        if (kaioKenPowerUpTicks > 0 && kaioKenPowerUpTicks <= TapInputThreshold)
        {
            ShiftKaioKen(1);
        }

        kaioKenPowerUpTicks = 0;
    }

    private void HandleKaioKenPowerDownInput()
    {
        if (AscensionKeybindSystem.KaioKenPowerDownKey.Current)
        {
            kaioKenPowerDownTicks++;
            kaioKenPowerUpTicks = 0;

            if (CurrentKaioKenLevelIndex > 0 && kaioKenPowerDownTicks >= KaioKenPowerDownChargeTicks)
            {
                SetKaioKenLevel(0);
                kaioKenPowerDownTicks = 0;
            }

            return;
        }

        if (kaioKenPowerDownTicks > 0 && kaioKenPowerDownTicks <= TapInputThreshold)
        {
            ShiftKaioKen(-1);
        }

        kaioKenPowerDownTicks = 0;
    }

    private void HandleInspectionInput()
    {
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        if (AscensionKeybindSystem.ToggleStatsKey.JustPressed)
        {
            KiHudSystem.ToggleStatsPanel();
        }

        if (AscensionKeybindSystem.ToggleDevPanelKey.JustPressed)
        {
            KiHudSystem.ToggleDevPanel();
        }
    }

    private static int GetSaiyanPowerUpDuration(int targetStageIndex)
    {
        return BaseSaiyanChargeTicks + targetStageIndex * SaiyanChargeTicksPerStage;
    }

    private static int GetKaioKenPowerUpDuration(int targetLevelIndex)
    {
        return BaseKaioKenChargeTicks + targetLevelIndex * KaioKenChargeTicksPerLevel;
    }

    private void ShiftForm(int direction)
    {
        SetForm(Math.Clamp(CurrentStageIndex + direction, 0, UnlockedStageIndex));
    }

    private void SetForm(int stageIndex)
    {
        int nextStageIndex = Math.Clamp(stageIndex, 0, UnlockedStageIndex);

        if (nextStageIndex == CurrentStageIndex)
        {
            return;
        }

        CurrentStageIndex = nextStageIndex;
        StageDefinition stage = CurrentStage;

        if (Player.whoAmI == Main.myPlayer)
        {
            Main.NewText($"Form: {stage.DisplayName}", stage.AuraColor);
        }

        SendClientSelection();
    }

    private void ShiftKaioKen(int direction)
    {
        SetKaioKenLevel(Math.Clamp(CurrentKaioKenLevelIndex + direction, 0, UnlockedKaioKenLevelIndex));
    }

    private void SetKaioKenLevel(int levelIndex)
    {
        int nextLevelIndex = Math.Clamp(levelIndex, 0, UnlockedKaioKenLevelIndex);

        if (nextLevelIndex == CurrentKaioKenLevelIndex)
        {
            return;
        }

        CurrentKaioKenLevelIndex = nextLevelIndex;
        KaioKenLevelDefinition level = CurrentKaioKenLevel;

        if (Player.whoAmI == Main.myPlayer)
        {
            Main.NewText(CurrentKaioKenLevelIndex == 0 ? "Kaio-Ken released." : $"Kaio-Ken: {level.DisplayName}", level.AuraColor);
        }

        SendClientSelection();
    }

    private void RechargeKi()
    {
        if (Main.GameUpdateCount % 60UL != 0UL)
        {
            return;
        }

        Ki = Math.Min(MaxKi, Ki + KiRegenPerSecond);
        SyncStateIfServer();
    }

    private void DrainActiveTransformations()
    {
        if (Main.GameUpdateCount % 60UL != 0UL)
        {
            return;
        }

        int kiDrain = ActiveKiDrainPerSecond;

        if (kiDrain > 0)
        {
            if (Ki >= kiDrain)
            {
                Ki -= kiDrain;
            }
            else
            {
                Ki = 0;
                StepDownFromStrain();
            }

            SyncStateIfServer();
        }

        int lifeDrain = ActiveLifeDrainPerSecond;

        if (lifeDrain > 0)
        {
            Player.statLife = Math.Max(1, Player.statLife - lifeDrain);
        }
    }

    private void StepDownFromStrain()
    {
        if (CurrentKaioKenLevelIndex > 0)
        {
            CurrentKaioKenLevelIndex = Math.Max(0, CurrentKaioKenLevelIndex - 1);

            if (Player.whoAmI == Main.myPlayer)
            {
                Main.NewText("Your ki falters and Kaio-Ken strains out.", new Color(255, 100, 90));
            }

            SendClientSelection();
            return;
        }

        if (CurrentStageIndex <= 0)
        {
            return;
        }

        CurrentStageIndex = Math.Max(0, CurrentStageIndex - 1);

        if (Player.whoAmI == Main.myPlayer)
        {
            Main.NewText("Your ki drops and the form slips.", new Color(190, 210, 255));
        }

        SendClientSelection();
    }

    private void HandleTraining()
    {
        bool inGravityRoom = IsNearGravityRoomCore();

        if (!IsWeightTraining && !inGravityRoom)
        {
            trainingTicks = 0;
            return;
        }

        bool movingUnderLoad = Math.Abs(Player.velocity.X) > 0.45f || Math.Abs(Player.velocity.Y) > 0.45f || Player.controlJump;

        if (!movingUnderLoad)
        {
            return;
        }

        trainingTicks++;

        if (trainingTicks < TrainingIntervalTicks)
        {
            return;
        }

        trainingTicks = 0;
        int physicalPower = IsWeightTraining ? 3 : 0;
        int kiPower = 0;

        if (inGravityRoom)
        {
            physicalPower += 4;
            kiPower += 3;
        }

        AddTrainingExperience(physicalPower, kiPower, false);
    }

    private bool IsNearGravityRoomCore()
    {
        int centerX = (int)(Player.Center.X / 16f);
        int centerY = (int)(Player.Center.Y / 16f);
        int tileType = ModContent.TileType<GravityRoomCoreTile>();

        for (int x = centerX - GravityRoomRadiusTiles; x <= centerX + GravityRoomRadiusTiles; x++)
        {
            for (int y = centerY - GravityRoomRadiusTiles; y <= centerY + GravityRoomRadiusTiles; y++)
            {
                Tile tile = Framing.GetTileSafely(x, y);

                if (tile.HasTile && tile.TileType == tileType)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void ApplyAscensionVisuals()
    {
        if (!naturalHairCaptured || CurrentStageIndex == 0)
        {
            naturalHairStyle = Player.hair;
            naturalHairColor = Player.hairColor;
            naturalHairCaptured = true;
        }

        AscensionVisuals.Apply(Player, CurrentStage, naturalHairStyle, naturalHairColor);
    }

    private void ShowProgressionNoticeOnce()
    {
        if (shownProgressionNotice || Player.whoAmI != Main.myPlayer || Main.GameUpdateCount < 90UL)
        {
            return;
        }

        shownProgressionNotice = true;
        Main.NewText("Ki Ascension active: normal weapons are heavily weakened. Use ki spells and train with weights or gravity rooms.", new Color(255, 230, 130));
    }

    private void DrawAura()
    {
        if (Main.dedServ)
        {
            return;
        }

        StageDefinition stage = CurrentStage;
        float saiyanChargeIntensity = GetChargeIntensity(saiyanPowerUpTicks, GetSaiyanPowerUpDuration(Math.Max(CurrentStageIndex + 1, UnlockedStageIndex)));
        float powerDownIntensity = GetChargeIntensity(saiyanPowerDownTicks, SaiyanPowerDownChargeTicks);

        if (CurrentStageIndex > 0 || saiyanChargeIntensity > 0f || powerDownIntensity > 0f)
        {
            Color auraColor = saiyanChargeIntensity > 0f && UnlockedStageIndex > CurrentStageIndex
                ? AscensionStages.Get(UnlockedStageIndex).AuraColor
                : stage.AuraColor;
            float lightStrength = CurrentStageIndex > 0 ? 0.45f : 0.2f;
            lightStrength += saiyanChargeIntensity * 0.35f;
            lightStrength = Math.Max(0.08f, lightStrength * (1f - powerDownIntensity * 0.65f));
            Lighting.AddLight(Player.Center, auraColor.ToVector3() * lightStrength);

            if (Main.rand.NextBool(CurrentStageIndex >= 3 ? 2 : 4))
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemTopaz, 0f, -1.2f, 140, auraColor, 1.05f + saiyanChargeIntensity * 0.8f);
                Main.dust[dust].noGravity = true;
            }
        }

        if (IsKaioKenActive || kaioKenPowerUpTicks > 0 || kaioKenPowerDownTicks > 0)
        {
            KaioKenLevelDefinition kaioKen = CurrentKaioKenLevel;
            float chargeIntensity = GetChargeIntensity(kaioKenPowerUpTicks, GetKaioKenPowerUpDuration(Math.Max(CurrentKaioKenLevelIndex + 1, UnlockedKaioKenLevelIndex)));
            float scale = 1.1f + CurrentKaioKenLevelIndex * 0.05f + chargeIntensity * 0.8f;
            Lighting.AddLight(Player.Center, new Vector3(0.65f, 0.08f, 0.05f) * (0.25f + chargeIntensity * 0.4f));

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, Main.rand.NextFloat(-1.3f, 1.3f), Main.rand.NextFloat(-1.7f, 0.4f), 120, kaioKen.AuraColor, scale);
                Main.dust[dust].noGravity = true;
            }
        }
    }

    private static float GetChargeIntensity(int ticks, int requiredTicks)
    {
        return requiredTicks <= 0 ? 0f : MathHelper.Clamp(ticks / (float)requiredTicks, 0f, 1f);
    }
}

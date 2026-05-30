using System;
using System.Collections.Generic;
using System.IO;
using KiAscension.Common;
using KiAscension.Items;
using KiAscension.Items.Combat;
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
    private const int BaseMaxKi = 140;
    private const int KaiLevelExperienceFactor = 220;
    private const int WitnessRange = 1200;
    private const int TapInputThreshold = 12;
    private const int BaseSaiyanChargeTicks = 45;
    private const int SaiyanChargeTicksPerStage = 18;
    private const int SaiyanPowerDownChargeTicks = 90;
    private const int BaseKaioKenChargeTicks = 30;
    private const int KaioKenChargeTicksPerLevel = 8;
    private const int KaioKenPowerDownChargeTicks = 60;
    private const int BreakthroughBurstTicks = 120;
    private const int GravityRoomRadiusTiles = 18;
    private const int TrainingIntervalTicks = 120;
    private const int StationTrainingDurationTicks = 360;
    private const int StationTrainingIntervalTicks = 90;
    private const float StationTrainingRangePixels = 108f;

    private int pendingAnnouncementStage = -1;
    private int highestAnnouncedTechniqueIndex;
    private int naturalHairStyle;
    private Color naturalHairColor;
    private bool naturalHairCaptured;
    private bool shownProgressionNotice;
    private int highestWitnessedStageIndex;
    private int saiyanPowerUpTicks;
    private int saiyanPowerDownTicks;
    private int kaioKenPowerUpTicks;
    private int kaioKenPowerDownTicks;
    private int breakthroughBurstTicks;
    private float kiFlightDrainAccumulator;
    private int trainingTicks;
    private int trainingOutgrownNoticeCooldown;
    private int meleeComboStep;
    private int meleeComboTicks;
    private bool isUsingTrainingStation;
    private TrainingSource activeTrainingStation;
    private Point activeTrainingStationTile;
    private int activeTrainingStationTicks;
    private int activeTrainingStationIntervalTicks;

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

    public bool IsKiFlying { get; private set; }

    public bool IsUsingTrainingStation => isUsingTrainingStation;

    public string ActiveTrainingDisplayName => isUsingTrainingStation
        ? TrainingSources.Get(activeTrainingStation).DisplayName
        : "none";

    public float ActiveTrainingProgress => isUsingTrainingStation
        ? MathHelper.Clamp(activeTrainingStationTicks / (float)StationTrainingDurationTicks, 0f, 1f)
        : 0f;

    public int TotalPowerExperience => PowerExperience + KiPowerExperience;

    public int PhysicalPowerLevel => CalculateKaiLevel(PowerExperience);

    public int KiPowerLevel => CalculateKaiLevel(KiPowerExperience);

    public int MaxKi => BaseMaxKi + CurrentStage.MaxKiBonus + Math.Max(0, KaiLevel - 1) * 10 + Math.Max(0, KiPowerLevel - 1) * 12;

    public int KiRegenPerSecond => 4 + CurrentStage.KiRegenBonus + Math.Max(0, KaiLevel - 1) / 3 + Math.Max(0, KiPowerLevel - 1) / 2;

    public KiResourceSnapshot KiResources => new(MaxKi, KiRegenPerSecond, ActiveKiDrainPerSecond, KiResourceMath.GetTechniqueCostMultiplier(KiPowerLevel, CurrentStage.Stage));

    public float SaiyanChargeIntensity => GetChargeIntensity(saiyanPowerUpTicks, GetSaiyanPowerUpDuration(Math.Max(CurrentStageIndex + 1, UnlockedStageIndex)));

    public float SaiyanPowerDownIntensity => GetChargeIntensity(saiyanPowerDownTicks, SaiyanPowerDownChargeTicks);

    public float BreakthroughIntensity => GetChargeIntensity(breakthroughBurstTicks, BreakthroughBurstTicks);

    public float KaioKenChargeIntensity => GetChargeIntensity(kaioKenPowerUpTicks, GetKaioKenPowerUpDuration(Math.Max(CurrentKaioKenLevelIndex + 1, UnlockedKaioKenLevelIndex)));

    public float KaioKenPowerDownIntensity => GetChargeIntensity(kaioKenPowerDownTicks, KaioKenPowerDownChargeTicks);

    public bool HasVisibleSaiyanAura => CurrentStageIndex > 0 || SaiyanChargeIntensity > 0f || SaiyanPowerDownIntensity > 0f || BreakthroughIntensity > 0f;

    public bool HasVisibleKaioKenAura => IsKaioKenActive || KaioKenChargeIntensity > 0f || KaioKenPowerDownIntensity > 0f;

    public StageDefinition CurrentStage => AscensionStages.Get(CurrentStageIndex);

    public StageDefinition NextStage => AscensionStages.Get(UnlockedStageIndex + 1);

    public KaioKenLevelDefinition CurrentKaioKenLevel => KaioKenLevels.Get(CurrentKaioKenLevelIndex);

    public bool IsKaioKenActive => CurrentKaioKenLevelIndex > 0;

    public StageDefinition UnlockedStage => AscensionStages.Get(UnlockedStageIndex);

    public KaioKenLevelDefinition UnlockedKaioKenLevel => KaioKenLevels.Get(UnlockedKaioKenLevelIndex);

    public float CombinedDamageMultiplier => CurrentStage.DamageMultiplier * CurrentKaioKenLevel.DamageMultiplier;

    public float CombinedSpeedMultiplier => CurrentStage.SpeedMultiplier * CurrentKaioKenLevel.SpeedMultiplier;

    public float PhysicalDamageMultiplier => 1f + Math.Max(0, PhysicalPowerLevel - 1) * 0.045f;

    public int ActiveKiDrainPerSecond => CurrentStage.KiDrainPerSecond + CurrentKaioKenLevel.KiDrainPerSecond;

    public int ActiveLifeDrainPerSecond => CurrentKaioKenLevel.LifeDrainPerSecond;

    public float FlightControlMultiplier => CurrentStage.FlightControlMultiplier;

    public KiFlightProfile CurrentFlightProfile => KiFlightProfiles.Get(CurrentStage.Stage);

    public int KiFlightDrainPerSecond => GetKiFlightDrainPerSecond(CurrentFlightProfile);

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
                : $"{next.DisplayName} locked: {AscensionStages.GetGateText(next.RequiredGate)}";
        }
    }

    public int HighestUnlockedTechniqueIndex => KiTechniques.GetHighestUnlockedIndex(KiPowerExperience, UnlockedStageIndex);

    public KiTechniqueDefinition CurrentTechnique => KiTechniques.Get(Math.Clamp(SelectedTechniqueIndex, 0, HighestUnlockedTechniqueIndex));

    public int MeleeComboStep => meleeComboStep;

    public int NextMeleeComboStep => meleeComboTicks > 0 ? meleeComboStep % 3 + 1 : 1;

    public string NextMeleeComboName => NextMeleeComboStep switch
    {
        1 => "Quick Punch",
        2 => "Heavy Punch",
        3 => "Rising Kick",
        _ => "Strike"
    };

    public int ExperienceForNextKaiLevel => KaiLevelExperienceFactor * KaiLevel * KaiLevel;

    public bool HasPendingWitnessBreakthrough =>
        UnlockedStageIndex < AscensionStages.MaxStageIndex
        && TotalPowerExperience >= NextStage.RequiredExperience
        && AscensionStages.IsGateSatisfied(NextStage)
        && NextStage.RequiresWitnessLoss
        && highestWitnessedStageIndex < UnlockedStageIndex + 1;

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
        highestWitnessedStageIndex = 0;
        naturalHairStyle = 0;
        naturalHairColor = Color.White;
        naturalHairCaptured = false;
        shownProgressionNotice = false;
        saiyanPowerUpTicks = 0;
        saiyanPowerDownTicks = 0;
        kaioKenPowerUpTicks = 0;
        kaioKenPowerDownTicks = 0;
        breakthroughBurstTicks = 0;
        kiFlightDrainAccumulator = 0f;
        trainingTicks = 0;
        trainingOutgrownNoticeCooldown = 0;
        meleeComboStep = 0;
        meleeComboTicks = 0;
        isUsingTrainingStation = false;
        activeTrainingStation = TrainingSource.MeditationFocus;
        activeTrainingStationTile = Point.Zero;
        activeTrainingStationTicks = 0;
        activeTrainingStationIntervalTicks = 0;
        IsKiFlying = false;
    }

    public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
    {
        Item focus = new();
        focus.SetDefaults(ModContent.ItemType<KiTrainingFocus>());

        Item strike = new();
        strike.SetDefaults(ModContent.ItemType<SaiyanStrike>());

        Item basicBlast = new();
        basicBlast.SetDefaults(ModContent.ItemType<BasicKiBlastSpell>());

        return new[] { focus, strike, basicBlast };
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
        tag["HighestWitnessedStageIndex"] = highestWitnessedStageIndex;
        tag["NaturalHairCaptured"] = naturalHairCaptured ? 1 : 0;

        if (naturalHairCaptured)
        {
            tag["NaturalHairStyle"] = naturalHairStyle;
            tag["NaturalHairColorR"] = (int)naturalHairColor.R;
            tag["NaturalHairColorG"] = (int)naturalHairColor.G;
            tag["NaturalHairColorB"] = (int)naturalHairColor.B;
        }
    }

    public override void LoadData(TagCompound tag)
    {
        PowerExperience = tag.ContainsKey("PowerExperience") ? tag.GetInt("PowerExperience") : 0;
        KiPowerExperience = tag.ContainsKey("KiPowerExperience") ? tag.GetInt("KiPowerExperience") : PowerExperience;
        KaiLevel = tag.ContainsKey("KaiLevel") ? tag.GetInt("KaiLevel") : CalculateKaiLevel(TotalPowerExperience);
        CurrentStageIndex = tag.ContainsKey("CurrentStageIndex") ? tag.GetInt("CurrentStageIndex") : 0;
        UnlockedStageIndex = tag.ContainsKey("UnlockedStageIndex") ? tag.GetInt("UnlockedStageIndex") : 0;
        highestWitnessedStageIndex = tag.ContainsKey("HighestWitnessedStageIndex")
            ? tag.GetInt("HighestWitnessedStageIndex")
            : InferWitnessedStageIndex(UnlockedStageIndex);
        CurrentKaioKenLevelIndex = tag.ContainsKey("CurrentKaioKenLevelIndex") ? tag.GetInt("CurrentKaioKenLevelIndex") : 0;
        UnlockedKaioKenLevelIndex = tag.ContainsKey("UnlockedKaioKenLevelIndex") ? tag.GetInt("UnlockedKaioKenLevelIndex") : 0;
        Ki = tag.ContainsKey("Ki") ? tag.GetInt("Ki") : BaseMaxKi;
        SelectedTechniqueIndex = tag.ContainsKey("SelectedTechniqueIndex") ? tag.GetInt("SelectedTechniqueIndex") : 0;
        naturalHairCaptured = tag.ContainsKey("NaturalHairCaptured") && tag.GetInt("NaturalHairCaptured") == 1;
        naturalHairStyle = tag.ContainsKey("NaturalHairStyle") ? tag.GetInt("NaturalHairStyle") : 0;
        naturalHairColor = tag.ContainsKey("NaturalHairColorR")
            ? new Color(tag.GetInt("NaturalHairColorR"), tag.GetInt("NaturalHairColorG"), tag.GetInt("NaturalHairColorB"))
            : Color.White;

        highestWitnessedStageIndex = Math.Clamp(Math.Max(highestWitnessedStageIndex, InferWitnessedStageIndex(UnlockedStageIndex)), 0, AscensionStages.MaxStageIndex);
        KaiLevel = Math.Max(1, CalculateKaiLevel(TotalPowerExperience));
        RevalidateProgressionAgainstCurrentWorld();
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

        RevalidateProgressionAgainstCurrentWorld();
        CaptureNaturalHairIfNeeded();
        EnsureInventoryItem(ModContent.ItemType<KiTrainingFocus>());
        EnsureInventoryItem(ModContent.ItemType<SaiyanStrike>());
        EnsureInventoryItem(ModContent.ItemType<BasicKiBlastSpell>());
        EnsureUnlockedTechniqueItems();
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
        clone.highestWitnessedStageIndex = highestWitnessedStageIndex;
        clone.breakthroughBurstTicks = breakthroughBurstTicks;
        clone.IsKiFlying = IsKiFlying;
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
        HandleKiFlight();
        HandleTraining();
        HandleMeleeComboDecay();
        RefreshTechniqueUnlocks(false);
        RefreshKaioKenUnlocks(false);
        ShowProgressionNoticeOnce();

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            AnnouncePendingBreakthrough();
        }
        else
        {
            if (TryUnlockAvailableStages(false))
            {
                SyncStateIfServer();
            }
        }

        breakthroughBurstTicks = Math.Max(0, breakthroughBurstTicks - 1);
        trainingOutgrownNoticeCooldown = Math.Max(0, trainingOutgrownNoticeCooldown - 1);

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

        if (Player.HeldItem?.ModItem is KiTechniqueItem or SaiyanStrike)
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

    public bool IsTechniqueUnlocked(KiTechniqueDefinition technique)
    {
        return KiTechniques.IsUnlocked(technique, KiPowerExperience, UnlockedStageIndex);
    }

    public string GetTechniqueLockReason(KiTechniqueDefinition technique)
    {
        if (IsTechniqueUnlocked(technique))
        {
            return string.Empty;
        }

        List<string> requirements = new();

        if (UnlockedStageIndex < (int)technique.RequiredStage)
        {
            requirements.Add(AscensionStages.Get((int)technique.RequiredStage).DisplayName);
        }

        if (KiPowerExperience < technique.RequiredKiPower)
        {
            requirements.Add($"{technique.RequiredKiPower} Ki Power");
        }

        return requirements.Count == 0 ? "Locked" : $"Requires {string.Join(" + ", requirements)}";
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

    public void ApplyTrainingSource(TrainingSource source, bool announce)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            return;
        }

        TrainingSourceDefinition definition = TrainingSources.Get(source);
        int physicalGain = GetCappedTrainingGain(PowerExperience, definition.PhysicalPowerCap, definition.PhysicalPowerGain);
        int kiGain = GetCappedTrainingGain(KiPowerExperience, definition.KiPowerCap, definition.KiPowerGain);

        if (physicalGain <= 0 && kiGain <= 0)
        {
            AnnounceTrainingOutgrown(definition, announce);
            return;
        }

        AddTrainingExperience(physicalGain, kiGain, announce);
    }

    public void RequestTrainingStationUse(TrainingSource source, int tileX, int tileY)
    {
        if (!IsStationTrainingSource(source))
        {
            return;
        }

        if (IsTrainingSourceOutgrown(source))
        {
            AnnounceTrainingOutgrown(TrainingSources.Get(source), true);
            return;
        }

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SetTrainingStationDisplay(source, tileX, tileY);
            SendTrainingStationUse(source, tileX, tileY);
            ShowTrainingStationStarted(source);
            return;
        }

        BeginTrainingStation(source, tileX, tileY, true);
    }

    public float GetSaiyanStrikeComboDamageMultiplier()
    {
        return NextMeleeComboStep switch
        {
            2 => 1.12f,
            3 => 1.32f,
            _ => 1f
        };
    }

    public float GetSaiyanStrikeComboKnockbackMultiplier()
    {
        return NextMeleeComboStep == 3 ? 1.35f : 1f;
    }

    public int RegisterSaiyanStrikeHit(int damageDone)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || damageDone <= 0)
        {
            return NextMeleeComboStep;
        }

        int landedComboStep = NextMeleeComboStep;
        meleeComboStep = landedComboStep;
        meleeComboTicks = 75;
        int comboBonus = landedComboStep == 3 ? 3 : 0;
        AddPowerExperience(Math.Max(1, damageDone / 12) + comboBonus, false);
        return landedComboStep;
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

    private bool TryUnlockAvailableStages(bool witnessedLoss)
    {
        bool unlockedAnyStage = false;

        while (UnlockedStageIndex < AscensionStages.MaxStageIndex)
        {
            int previousUnlockedStageIndex = UnlockedStageIndex;
            int previousCurrentStageIndex = CurrentStageIndex;
            StageDefinition next = AscensionStages.Get(UnlockedStageIndex + 1);

            if (TotalPowerExperience < next.RequiredExperience)
            {
                return unlockedAnyStage;
            }

            if (!AscensionStages.IsGateSatisfied(next))
            {
                AnnounceProgressionGate(next);
                return unlockedAnyStage;
            }

            bool witnessSatisfied = !next.RequiresWitnessLoss
                || witnessedLoss
                || highestWitnessedStageIndex >= UnlockedStageIndex + 1;

            if (!witnessSatisfied)
            {
                AnnouncePendingBreakthrough();
                return unlockedAnyStage;
            }

            if (next.RequiresWitnessLoss && witnessedLoss)
            {
                highestWitnessedStageIndex = Math.Max(highestWitnessedStageIndex, UnlockedStageIndex + 1);
            }

            bool shouldBreakthrough = previousCurrentStageIndex == previousUnlockedStageIndex;
            UnlockNextSaiyanStage(shouldBreakthrough);
            unlockedAnyStage = true;
            pendingAnnouncementStage = -1;
            witnessedLoss = false;
            RefreshTechniqueUnlocks(true);

        }

        return unlockedAnyStage;
    }

    private void UnlockNextSaiyanStage(bool shouldBreakthrough)
    {
        int newStageIndex = Math.Clamp(UnlockedStageIndex + 1, 0, AscensionStages.MaxStageIndex);
        UnlockedStageIndex = newStageIndex;

        if (shouldBreakthrough)
        {
            BeginSaiyanBreakthrough(newStageIndex);
            return;
        }

        CurrentStageIndex = Math.Clamp(CurrentStageIndex, 0, UnlockedStageIndex);
        Ki = Math.Clamp(Ki, 0, MaxKi);
    }

    private void BeginSaiyanBreakthrough(int stageIndex)
    {
        CurrentStageIndex = Math.Clamp(stageIndex, 0, UnlockedStageIndex);
        Ki = MaxKi;
        breakthroughBurstTicks = BreakthroughBurstTicks;
        saiyanPowerUpTicks = 0;
        saiyanPowerDownTicks = 0;

        StageDefinition stage = CurrentStage;

        if (Player.whoAmI == Main.myPlayer)
        {
            Main.NewText($"Breakthrough: {stage.DisplayName}!", stage.AuraColor);
            KiSoundSystem.PlayTransformationComplete(Player.Center);
        }

        if (Main.dedServ)
        {
            return;
        }

        AscensionAuraProfile aura = AscensionAuraProfiles.Get(stage.Stage);

        for (int i = 0; i < 28; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(3.6f, 3.6f);
            int dust = Dust.NewDust(Player.position, Player.width, Player.height, aura.DustType, velocity.X, velocity.Y - 1.8f, 90, aura.PrimaryColor, 1.25f);
            Main.dust[dust].noGravity = true;
        }
    }

    private void AnnounceProgressionGate(StageDefinition stage)
    {
        int nextStageIndex = UnlockedStageIndex + 1;
        int gateAnnouncementMarker = -nextStageIndex;

        if (Player.whoAmI == Main.myPlayer && pendingAnnouncementStage != gateAnnouncementMarker)
        {
            Main.NewText($"{stage.DisplayName} locked: {AscensionStages.GetGateText(stage.RequiredGate)}.", new Color(255, 180, 120));
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
            Main.NewText($"{NextStage.DisplayName} ready, but requires witness-loss breakthrough.", new Color(255, 150, 110));
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
                return $"{next.DisplayName} locked: {AscensionStages.GetGateText(next.RequiredGate)}";
            }

            return next.RequiresWitnessLoss
                ? highestWitnessedStageIndex >= UnlockedStageIndex + 1
                    ? $"Ceiling ready: {next.DisplayName}"
                    : $"{next.DisplayName} ready, but requires witness-loss breakthrough"
                : $"Ceiling ready: {next.DisplayName}";
        }

        return $"Next ceiling: {next.DisplayName} {TotalPowerExperience}/{next.RequiredExperience} total power";
    }

    public string GetFlightStatusText()
    {
        KiFlightProfile profile = CurrentFlightProfile;

        if (!profile.AllowsTrueFlight)
        {
            return $"Flight: locked ({profile.DisplayNote})";
        }

        string state = IsKiFlying ? "active" : "ready";
        return $"Flight: {state}, {KiFlightDrainPerSecond}/s ki drain, x{FlightControlMultiplier:0.00} control";
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

    public void ReceiveTrainingStationUse(BinaryReader reader)
    {
        TrainingSource source = (TrainingSource)reader.ReadInt32();
        int tileX = reader.ReadInt32();
        int tileY = reader.ReadInt32();

        BeginTrainingStation(source, tileX, tileY, false);
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
        writer.Write(highestWitnessedStageIndex);
        writer.Write(breakthroughBurstTicks);
        writer.Write(IsKiFlying);
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
        highestWitnessedStageIndex = reader.ReadInt32();
        breakthroughBurstTicks = reader.ReadInt32();
        IsKiFlying = reader.ReadBoolean();

        highestWitnessedStageIndex = Math.Clamp(Math.Max(highestWitnessedStageIndex, InferWitnessedStageIndex(UnlockedStageIndex)), 0, AscensionStages.MaxStageIndex);
        KaiLevel = Math.Max(1, CalculateKaiLevel(TotalPowerExperience));
        RevalidateProgressionAgainstCurrentWorld();
        highestAnnouncedTechniqueIndex = Math.Clamp(highestAnnouncedTechniqueIndex, 0, HighestUnlockedTechniqueIndex);
        breakthroughBurstTicks = Math.Clamp(breakthroughBurstTicks, 0, BreakthroughBurstTicks);
        IsKiFlying = IsKiFlying && CurrentFlightProfile.AllowsTrueFlight;
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

    private void EnsureUnlockedTechniqueItems()
    {
        for (int techniqueIndex = 0; techniqueIndex <= HighestUnlockedTechniqueIndex; techniqueIndex++)
        {
            GrantTechniqueItem(KiTechniques.Get(techniqueIndex));
        }
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

    private void SendTrainingStationUse(TrainingSource source, int tileX, int tileY)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)KiAscensionMessageType.TrainingStationUse);
        packet.Write((byte)Player.whoAmI);
        packet.Write((int)source);
        packet.Write(tileX);
        packet.Write(tileY);
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

    private void RevalidateProgressionAgainstCurrentWorld()
    {
        highestWitnessedStageIndex = Math.Clamp(highestWitnessedStageIndex, 0, AscensionStages.MaxStageIndex);
        UnlockedStageIndex = GetHighestStageAllowedInCurrentWorld();
        CurrentStageIndex = Math.Clamp(CurrentStageIndex, 0, UnlockedStageIndex);
        UnlockedKaioKenLevelIndex = Math.Clamp(KaioKenLevels.GetHighestUnlockedIndex(TotalPowerExperience), 0, KaioKenLevels.MaxLevelIndex);
        CurrentKaioKenLevelIndex = Math.Clamp(CurrentKaioKenLevelIndex, 0, UnlockedKaioKenLevelIndex);
        Ki = Math.Clamp(Ki, 0, MaxKi);
        SelectedTechniqueIndex = Math.Clamp(SelectedTechniqueIndex, 0, HighestUnlockedTechniqueIndex);
    }

    private int GetHighestStageAllowedInCurrentWorld()
    {
        int highest = 0;

        for (int stageIndex = 1; stageIndex <= AscensionStages.MaxStageIndex; stageIndex++)
        {
            StageDefinition stage = AscensionStages.Get(stageIndex);

            if (TotalPowerExperience < stage.RequiredExperience || !AscensionStages.IsGateSatisfied(stage))
            {
                break;
            }

            if (stage.RequiresWitnessLoss && highestWitnessedStageIndex < stageIndex)
            {
                break;
            }

            highest = stageIndex;
        }

        return highest;
    }

    private static int InferWitnessedStageIndex(int unlockedStageIndex)
    {
        int witnessedStageIndex = 0;

        for (int stageIndex = 1; stageIndex <= Math.Clamp(unlockedStageIndex, 0, AscensionStages.MaxStageIndex); stageIndex++)
        {
            if (AscensionStages.Get(stageIndex).RequiresWitnessLoss)
            {
                witnessedStageIndex = stageIndex;
            }
        }

        return witnessedStageIndex;
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
        UnlockedKaioKenLevelIndex = highestUnlocked;
        CurrentKaioKenLevelIndex = Math.Clamp(CurrentKaioKenLevelIndex, 0, UnlockedKaioKenLevelIndex);
    }

    private void HandleKiFlight()
    {
        IsKiFlying = false;
        KiFlightProfile profile = CurrentFlightProfile;

        if (!profile.AllowsTrueFlight || Player.dead || Player.mount.Active || Player.frozen || Player.webbed)
        {
            kiFlightDrainAccumulator = 0f;
            return;
        }

        bool wantsFlight = Player.controlJump || Player.controlUp || Player.controlDown;
        bool airborneOrTakingOff = Math.Abs(Player.velocity.Y) > 0.05f || Player.controlJump || Player.controlUp;

        if (!wantsFlight || !airborneOrTakingOff)
        {
            kiFlightDrainAccumulator = 0f;
            return;
        }

        int drainPerSecond = KiFlightDrainPerSecond;

        if (drainPerSecond > 0 && Ki <= 0)
        {
            kiFlightDrainAccumulator = 0f;
            return;
        }

        if (!TryDrainKiFlight(drainPerSecond))
        {
            return;
        }

        IsKiFlying = true;
        Player.noFallDmg = true;
        Player.fallStart = (int)(Player.position.Y / 16f);
        ApplyKiFlightMovement(profile);
    }

    private void ApplyKiFlightMovement(KiFlightProfile profile)
    {
        float control = Math.Max(1f, FlightControlMultiplier);
        float maxHorizontalSpeed = profile.MaxHorizontalSpeed * control;
        float horizontalAcceleration = profile.HorizontalAcceleration * control;

        if (Player.controlLeft)
        {
            Player.velocity.X = Math.Max(Player.velocity.X - horizontalAcceleration, -maxHorizontalSpeed);
        }
        else if (Player.controlRight)
        {
            Player.velocity.X = Math.Min(Player.velocity.X + horizontalAcceleration, maxHorizontalSpeed);
        }
        else
        {
            Player.velocity.X = MathHelper.Lerp(Player.velocity.X, 0f, 0.025f * control);
        }

        float targetVerticalVelocity = Math.Min(Player.velocity.Y, profile.HoverFallSpeed);

        if (Player.controlJump || Player.controlUp)
        {
            targetVerticalVelocity = -profile.MaxRiseSpeed * control;
        }
        else if (Player.controlDown)
        {
            targetVerticalVelocity = profile.HoverFallSpeed * 2.6f;
        }

        Player.velocity.Y = MathHelper.Lerp(Player.velocity.Y, targetVerticalVelocity, profile.VerticalControl);
    }

    private bool TryDrainKiFlight(int drainPerSecond)
    {
        if (drainPerSecond <= 0)
        {
            return true;
        }

        kiFlightDrainAccumulator += drainPerSecond / 60f;
        int drainAmount = (int)kiFlightDrainAccumulator;

        if (drainAmount <= 0)
        {
            return true;
        }

        if (!TryConsumeKi(drainAmount))
        {
            kiFlightDrainAccumulator = 0f;
            return false;
        }

        kiFlightDrainAccumulator -= drainAmount;
        return true;
    }

    private int GetKiFlightDrainPerSecond(KiFlightProfile profile)
    {
        if (!profile.AllowsTrueFlight)
        {
            return 0;
        }

        int trainingDiscount = Math.Max(0, KiPowerLevel - 1) / 8;
        return Math.Max(1, profile.KiDrainPerSecond - trainingDiscount);
    }

    private void HandleSaiyanPowerUpInput()
    {
        if (AscensionKeybindSystem.PowerUpKey.Current)
        {
            if (saiyanPowerUpTicks == 0 && UnlockedStageIndex > CurrentStageIndex && Player.whoAmI == Main.myPlayer)
            {
                KiSoundSystem.PlayPowerUpStart(Player.Center);
            }

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
            if (saiyanPowerDownTicks == 0 && CurrentStageIndex > 0 && Player.whoAmI == Main.myPlayer)
            {
                KiSoundSystem.PlayPowerDown(Player.Center);
            }

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
            if (kaioKenPowerUpTicks == 0 && UnlockedKaioKenLevelIndex > CurrentKaioKenLevelIndex && Player.whoAmI == Main.myPlayer)
            {
                KiSoundSystem.PlayPowerUpStart(Player.Center);
            }

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
            if (kaioKenPowerDownTicks == 0 && CurrentKaioKenLevelIndex > 0 && Player.whoAmI == Main.myPlayer)
            {
                KiSoundSystem.PlayPowerDown(Player.Center);
            }

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
        int oldStageIndex = CurrentStageIndex;

        if (nextStageIndex == CurrentStageIndex)
        {
            return;
        }

        CurrentStageIndex = nextStageIndex;
        StageDefinition stage = CurrentStage;

        if (Player.whoAmI == Main.myPlayer)
        {
            Main.NewText($"Form: {stage.DisplayName}", stage.AuraColor);
            if (nextStageIndex > oldStageIndex)
            {
                KiSoundSystem.PlayTransformationComplete(Player.Center);
            }
            else
            {
                KiSoundSystem.PlayPowerDown(Player.Center);
            }
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
            if (CurrentKaioKenLevelIndex == 0)
            {
                KiSoundSystem.PlayPowerDown(Player.Center);
            }
            else
            {
                KiSoundSystem.PlayKaioKenActivation(Player.Center);
            }
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
                KiSoundSystem.PlayLowKiFizzle(Player.Center);
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
            KiSoundSystem.PlayLowKiFizzle(Player.Center);
        }

        SendClientSelection();
    }

    private void HandleTraining()
    {
        HandleActiveTrainingStation();

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

        if (IsWeightTraining)
        {
            ApplyTrainingSource(TrainingSource.WeightedGear, false);
        }

        if (inGravityRoom)
        {
            ApplyTrainingSource(TrainingSource.GravityRoom, false);
        }
    }

    private void BeginTrainingStation(TrainingSource source, int tileX, int tileY, bool announce)
    {
        if (!IsStationTrainingSource(source) || !IsTrainingStationTile(source, tileX, tileY))
        {
            return;
        }

        Vector2 stationCenter = new((tileX + 1f) * 16f, (tileY + 1f) * 16f);

        if (Vector2.Distance(Player.Center, stationCenter) > StationTrainingRangePixels)
        {
            return;
        }

        if (IsTrainingSourceOutgrown(source))
        {
            AnnounceTrainingOutgrown(TrainingSources.Get(source), true);
            return;
        }

        isUsingTrainingStation = true;
        activeTrainingStation = source;
        activeTrainingStationTile = new Point(tileX, tileY);
        activeTrainingStationTicks = StationTrainingDurationTicks;
        activeTrainingStationIntervalTicks = 0;

        if (announce)
        {
            ShowTrainingStationStarted(source);
        }
    }

    private void HandleActiveTrainingStation()
    {
        if (!isUsingTrainingStation)
        {
            return;
        }

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            activeTrainingStationTicks--;

            if (activeTrainingStationTicks <= 0)
            {
                StopTrainingStation();
            }

            return;
        }

        if (!IsTrainingStationTile(activeTrainingStation, activeTrainingStationTile.X, activeTrainingStationTile.Y))
        {
            StopTrainingStation();
            return;
        }

        Vector2 stationCenter = new((activeTrainingStationTile.X + 1f) * 16f, (activeTrainingStationTile.Y + 1f) * 16f);

        if (Vector2.Distance(Player.Center, stationCenter) > StationTrainingRangePixels)
        {
            StopTrainingStation();
            return;
        }

        activeTrainingStationTicks--;
        activeTrainingStationIntervalTicks++;

        if (activeTrainingStationIntervalTicks >= StationTrainingIntervalTicks)
        {
            activeTrainingStationIntervalTicks = 0;
            ApplyTrainingSource(activeTrainingStation, true);

            if (IsTrainingSourceOutgrown(activeTrainingStation))
            {
                StopTrainingStation();
            }
        }

        if (activeTrainingStationTicks <= 0)
        {
            StopTrainingStation();
        }
    }

    private void StopTrainingStation()
    {
        isUsingTrainingStation = false;
        activeTrainingStationTicks = 0;
        activeTrainingStationIntervalTicks = 0;
    }

    private void SetTrainingStationDisplay(TrainingSource source, int tileX, int tileY)
    {
        isUsingTrainingStation = true;
        activeTrainingStation = source;
        activeTrainingStationTile = new Point(tileX, tileY);
        activeTrainingStationTicks = StationTrainingDurationTicks;
        activeTrainingStationIntervalTicks = 0;
    }

    private void ShowTrainingStationStarted(TrainingSource source)
    {
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        TrainingSourceDefinition definition = TrainingSources.Get(source);
        Main.NewText($"Training: {definition.DisplayName}", new Color(210, 230, 255));
    }

    private static bool IsStationTrainingSource(TrainingSource source)
    {
        return source is TrainingSource.WoodenWeightBench
            or TrainingSource.CopperWeightBench
            or TrainingSource.WoodenTrainingBag
            or TrainingSource.MeditationMat;
    }

    private bool IsTrainingSourceOutgrown(TrainingSource source)
    {
        TrainingSourceDefinition definition = TrainingSources.Get(source);
        return GetCappedTrainingGain(PowerExperience, definition.PhysicalPowerCap, definition.PhysicalPowerGain) <= 0
            && GetCappedTrainingGain(KiPowerExperience, definition.KiPowerCap, definition.KiPowerGain) <= 0;
    }

    private static bool IsTrainingStationTile(TrainingSource source, int tileX, int tileY)
    {
        int expectedTileType = source switch
        {
            TrainingSource.WoodenWeightBench => ModContent.TileType<WoodenWeightBenchTile>(),
            TrainingSource.CopperWeightBench => ModContent.TileType<CopperWeightBenchTile>(),
            TrainingSource.WoodenTrainingBag => ModContent.TileType<WoodenTrainingBagTile>(),
            TrainingSource.MeditationMat => ModContent.TileType<MeditationMatTile>(),
            _ => -1
        };

        if (expectedTileType < 0)
        {
            return false;
        }

        Tile tile = Framing.GetTileSafely(tileX, tileY);
        return tile.HasTile && tile.TileType == expectedTileType;
    }

    private void HandleMeleeComboDecay()
    {
        if (meleeComboTicks <= 0)
        {
            meleeComboStep = 0;
            return;
        }

        meleeComboTicks--;
    }

    private int GetCappedTrainingGain(int currentValue, int cap, int gain)
    {
        if (gain <= 0 || cap <= 0 || currentValue >= cap)
        {
            return 0;
        }

        return Math.Min(gain, cap - currentValue);
    }

    private void AnnounceTrainingOutgrown(TrainingSourceDefinition definition, bool announce)
    {
        if (!announce || trainingOutgrownNoticeCooldown > 0 || Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        trainingOutgrownNoticeCooldown = 240;
        Main.NewText(definition.OutgrownMessage, new Color(210, 210, 210));
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
        CaptureNaturalHairIfNeeded();

        if (CurrentStageIndex <= 0)
        {
            RestoreNaturalHair();
            return;
        }

        AscensionVisuals.Apply(Player, CurrentStage, naturalHairStyle, naturalHairColor);
    }

    private void CaptureNaturalHairIfNeeded()
    {
        if (naturalHairCaptured)
        {
            return;
        }

        bool currentHairIsAscensionHair = AscensionVisuals.IsAscensionHairStyle(Player.hair);
        naturalHairStyle = currentHairIsAscensionHair ? 0 : Player.hair;
        naturalHairColor = currentHairIsAscensionHair ? Color.White : Player.hairColor;
        naturalHairCaptured = true;
    }

    private void RestoreNaturalHair()
    {
        if (!naturalHairCaptured)
        {
            CaptureNaturalHairIfNeeded();
        }

        Player.hair = naturalHairStyle;
        Player.hairColor = naturalHairColor;
    }

    private void ShowProgressionNoticeOnce()
    {
        if (shownProgressionNotice || Player.whoAmI != Main.myPlayer || Main.GameUpdateCount < 90UL)
        {
            return;
        }

        shownProgressionNotice = true;
        Main.NewText("Ki Ascension active: ki spells and Saiyan Strike are your main growth path, but normal weapons still work while the melee system grows.", new Color(255, 230, 130));
    }

    private void DrawAura()
    {
        if (Main.dedServ)
        {
            return;
        }

        StageDefinition stage = CurrentStage;
        AscensionAuraProfile auraProfile = AscensionAuraProfiles.Get(stage.Stage);
        float saiyanChargeIntensity = SaiyanChargeIntensity;
        float powerDownIntensity = SaiyanPowerDownIntensity;
        float breakthroughIntensity = BreakthroughIntensity;

        if (CurrentStageIndex > 0 || saiyanChargeIntensity > 0f || powerDownIntensity > 0f || breakthroughIntensity > 0f)
        {
            AscensionAuraProfile visibleProfile = saiyanChargeIntensity > 0f && UnlockedStageIndex > CurrentStageIndex
                ? AscensionAuraProfiles.Get(AscensionStages.Get(UnlockedStageIndex).Stage)
                : auraProfile;
            Color auraColor = visibleProfile.PrimaryColor;
            float lightStrength = CurrentStageIndex > 0 ? visibleProfile.LightStrength : 0.2f;
            lightStrength += saiyanChargeIntensity * 0.35f;
            lightStrength += breakthroughIntensity * 0.55f;
            lightStrength = Math.Max(0.08f, lightStrength * (1f - powerDownIntensity * 0.65f));
            Lighting.AddLight(Player.Center, auraColor.ToVector3() * lightStrength);

            int dustChance = breakthroughIntensity > 0.3f ? 1 : Math.Max(1, visibleProfile.IdleDustChance);

            if (Main.rand.NextBool(dustChance))
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height, visibleProfile.DustType, 0f, -1.2f - breakthroughIntensity, 140, auraColor, visibleProfile.DustScale + saiyanChargeIntensity * 0.8f + breakthroughIntensity * 0.9f);
                Main.dust[dust].noGravity = true;
            }

            bool emitElectricArc = visibleProfile.EmitsElectricArcs
                ? Main.rand.NextBool(breakthroughIntensity > 0.15f ? 4 : 10)
                : breakthroughIntensity > 0.45f && Main.rand.NextBool(4);

            if (emitElectricArc)
            {
                int spark = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Electric, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2.4f, 0.4f), 80, visibleProfile.SecondaryColor, 0.9f + breakthroughIntensity);
                Main.dust[spark].noGravity = true;
            }
        }

        if (IsKaioKenActive || kaioKenPowerUpTicks > 0 || kaioKenPowerDownTicks > 0)
        {
            KaioKenLevelDefinition kaioKen = CurrentKaioKenLevel;
            float chargeIntensity = KaioKenChargeIntensity;
            float scale = 1.1f + CurrentKaioKenLevelIndex * 0.05f + chargeIntensity * 0.8f;
            Lighting.AddLight(Player.Center, new Vector3(0.65f, 0.08f, 0.05f) * (0.25f + chargeIntensity * 0.4f));

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, Main.rand.NextFloat(-1.3f, 1.3f), Main.rand.NextFloat(-1.7f, 0.4f), 120, kaioKen.AuraColor, scale);
                Main.dust[dust].noGravity = true;
            }
        }

        if (IsKiFlying)
        {
            Color flightColor = CurrentStage.AuraColor;
            Lighting.AddLight(Player.Center, flightColor.ToVector3() * 0.25f);

            if (Main.rand.NextBool(2))
            {
                Vector2 trailVelocity = new(-Player.direction * Main.rand.NextFloat(0.6f, 1.8f), Main.rand.NextFloat(0.8f, 2.2f));
                int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemDiamond, trailVelocity.X, trailVelocity.Y, 130, flightColor, 0.9f);
                Main.dust[dust].noGravity = true;
            }
        }
    }

    private static float GetChargeIntensity(int ticks, int requiredTicks)
    {
        return requiredTicks <= 0 ? 0f : MathHelper.Clamp(ticks / (float)requiredTicks, 0f, 1f);
    }
}

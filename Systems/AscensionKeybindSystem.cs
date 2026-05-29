using Terraria.ModLoader;

namespace KiAscension.Systems;

public class AscensionKeybindSystem : ModSystem
{
    public static ModKeybind PowerUpKey { get; private set; }

    public static ModKeybind PowerDownKey { get; private set; }

    public static ModKeybind KaioKenPowerUpKey { get; private set; }

    public static ModKeybind KaioKenPowerDownKey { get; private set; }

    public static ModKeybind ToggleStatsKey { get; private set; }

    public static ModKeybind ToggleDevPanelKey { get; private set; }

    public override void Load()
    {
        PowerUpKey = KeybindLoader.RegisterKeybind(Mod, "Ascend Form", "Z");
        PowerDownKey = KeybindLoader.RegisterKeybind(Mod, "Descend Form", "X");
        KaioKenPowerUpKey = KeybindLoader.RegisterKeybind(Mod, "Kaio-Ken Power Up", "C");
        KaioKenPowerDownKey = KeybindLoader.RegisterKeybind(Mod, "Kaio-Ken Power Down", "V");
        ToggleStatsKey = KeybindLoader.RegisterKeybind(Mod, "Toggle Ki Stats", "B");
        ToggleDevPanelKey = KeybindLoader.RegisterKeybind(Mod, "Toggle Ki Dev Panel", "N");
    }

    public override void Unload()
    {
        PowerUpKey = null;
        PowerDownKey = null;
        KaioKenPowerUpKey = null;
        KaioKenPowerDownKey = null;
        ToggleStatsKey = null;
        ToggleDevPanelKey = null;
    }
}

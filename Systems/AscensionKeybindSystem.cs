using Terraria.ModLoader;

namespace KiAscension.Systems;

public class AscensionKeybindSystem : ModSystem
{
    public static ModKeybind PowerUpKey { get; private set; }

    public static ModKeybind PowerDownKey { get; private set; }

    public static ModKeybind NextTechniqueKey { get; private set; }

    public static ModKeybind PreviousTechniqueKey { get; private set; }

    public override void Load()
    {
        PowerUpKey = KeybindLoader.RegisterKeybind(Mod, "Ascend Form", "Z");
        PowerDownKey = KeybindLoader.RegisterKeybind(Mod, "Descend Form", "X");
        NextTechniqueKey = KeybindLoader.RegisterKeybind(Mod, "Next Ki Technique", "C");
        PreviousTechniqueKey = KeybindLoader.RegisterKeybind(Mod, "Previous Ki Technique", "V");
    }

    public override void Unload()
    {
        PowerUpKey = null;
        PowerDownKey = null;
        NextTechniqueKey = null;
        PreviousTechniqueKey = null;
    }
}

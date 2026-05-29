using Terraria.ModLoader;

namespace KiAscension.Systems;

public class AscensionKeybindSystem : ModSystem
{
    public static ModKeybind PowerUpKey { get; private set; }

    public static ModKeybind PowerDownKey { get; private set; }

    public override void Load()
    {
        PowerUpKey = KeybindLoader.RegisterKeybind(Mod, "Ascend Form", "Z");
        PowerDownKey = KeybindLoader.RegisterKeybind(Mod, "Descend Form", "X");
    }

    public override void Unload()
    {
        PowerUpKey = null;
        PowerDownKey = null;
    }
}

using System.IO;
using KiAscension.Common;
using KiAscension.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KiAscension;

public class KiAscension : Mod
{
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        KiAscensionMessageType messageType = (KiAscensionMessageType)reader.ReadByte();
        byte playerIndex = reader.ReadByte();

        if (playerIndex >= Main.maxPlayers)
        {
            return;
        }

        KiPlayer kiPlayer = Main.player[playerIndex].GetModPlayer<KiPlayer>();

        switch (messageType)
        {
            case KiAscensionMessageType.SyncPlayerState:
                if (Main.netMode == NetmodeID.Server)
                {
                    return;
                }

                kiPlayer.ReceivePlayerSync(reader);
                break;

            case KiAscensionMessageType.ClientSelection:
                if (Main.netMode == NetmodeID.Server && playerIndex == whoAmI)
                {
                    kiPlayer.ReceiveClientSelection(reader);
                    kiPlayer.SyncPlayer(-1, whoAmI, false);
                }

                break;
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace KiAscension.Systems;

public class KiFeedbackSystem : ModSystem
{
    private static int screenShakeTicks;
    private static float screenShakeStrength;

    public static void RequestScreenShake(Vector2 worldPosition, float strength, int ticks)
    {
        if (Main.dedServ || Main.LocalPlayer is null || !Main.LocalPlayer.active)
        {
            return;
        }

        float distance = Vector2.Distance(Main.LocalPlayer.Center, worldPosition);
        float falloff = MathHelper.Clamp(1f - distance / 1800f, 0f, 1f);

        if (falloff <= 0f)
        {
            return;
        }

        screenShakeStrength = Math.Max(screenShakeStrength, strength * falloff);
        screenShakeTicks = Math.Max(screenShakeTicks, ticks);
    }

    public override void ModifyScreenPosition()
    {
        if (screenShakeTicks <= 0 || screenShakeStrength <= 0f)
        {
            return;
        }

        Main.screenPosition += Main.rand.NextVector2Circular(screenShakeStrength, screenShakeStrength);
        screenShakeTicks--;
        screenShakeStrength *= 0.9f;
    }
}

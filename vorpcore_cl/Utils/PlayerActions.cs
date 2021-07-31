using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpcore_cl.Utils
{
    public class PlayerActions : BaseScript
    {
        public static void TeleportToCoords(float x, float y, float z, float heading = 0.0f)
        {
            var playerPedId = API.PlayerPedId();
            API.SetEntityCoords(playerPedId, x, y, z, true, true, true, false);
            API.SetEntityHeading(playerPedId, heading);
        }
    }
}

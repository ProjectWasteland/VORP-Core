using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpcore_cl.Utils
{
    internal class AdminActions : BaseScript
    {
        public static async Task TeleportAndFoundGroundAsync(Vector3 tpCoords)
        {
            var groundZ = 0.0F;
            var normal = new Vector3(1.0f, 1.0f, 1.0f);
            var foundGround = false;

            for (var i = 1; i < 1000.0; i++)
            {
                API.SetEntityCoords(API.PlayerPedId(), tpCoords.X, tpCoords.Y, i, true, true, true, false);
                foundGround = API.GetGroundZAndNormalFor_3dCoord(tpCoords.X, tpCoords.Y, i, ref groundZ, ref normal);
                await Delay(1);
                if (foundGround)
                {
                    API.SetEntityCoords(API.PlayerPedId(), tpCoords.X, tpCoords.Y, i, true, true, true, false);
                    break;
                }
            }
        }
    }
}

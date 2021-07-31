using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using vorpcore_cl.Utils;
using static CitizenFX.Core.Native.API;

namespace vorpcore_cl.Scripts
{
    internal class VoiceChat : BaseScript
    {
        public static bool activeVoiceChat = false;
        public static List<float> voiceRange = new List<float>();
        public static int voiceRangeSelected;

        public static uint keyRange;

        //Tecla L
        public VoiceChat()
        {
            Tick += SetVoiceChat;
            Tick += StartVoiceChat;
        }

        private async Task StartVoiceChat()
        {
            if (GetConfig.isLoading && activeVoiceChat)
            {
                Function.Call((Hash)0x08797A8C03868CB8, voiceRange[voiceRangeSelected]);
                Function.Call((Hash)0xEC8703E4536A9952);
                Function.Call((Hash)0x58125B691F6827D5, voiceRange[voiceRangeSelected]);
            }

            await Delay(10000);
        }

        private async Task SetVoiceChat()
        {
            if (GetConfig.isLoading && activeVoiceChat)
            {
                if (IsControlJustPressed(0, keyRange))
                {
                    Debug.WriteLine(keyRange.ToString());
                    voiceRangeSelected = (voiceRangeSelected + 1) % voiceRange.Count;
                    TriggerEvent("vorp:TipRight",
                                 string.Format(GetConfig.Langs["VoiceRangeChanged"],
                                               voiceRange[voiceRangeSelected].ToString()), 4000);
                    Function.Call((Hash)0x08797A8C03868CB8, voiceRange[voiceRangeSelected]);
                    Function.Call((Hash)0xEC8703E4536A9952);
                    Function.Call((Hash)0x58125B691F6827D5, voiceRange[voiceRangeSelected]);
                }

                if (IsControlPressed(0, keyRange))
                {
                    var playerCoords = GetEntityCoords(PlayerPedId(), true, true);
                    Function.Call((Hash)0x2A32FAA57B937173, 0x94FDAE17, playerCoords.X, playerCoords.Y,
                                  playerCoords.Z - 0.5f, 0.0f, 0.0f, 0.0f, 0, 0.0f, 0.0f,
                                  voiceRange[voiceRangeSelected], voiceRange[voiceRangeSelected], 1.0, 255, 179, 38,
                                  200, false, true, 2, false, false, false, false);
                }
            }
        }
    }
}

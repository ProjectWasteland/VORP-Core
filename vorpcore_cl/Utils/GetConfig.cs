using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using vorpcore_cl.Scripts;

namespace vorpcore_cl.Utils
{
    public class GetConfig : BaseScript
    {
        public static JObject Config = new JObject();
        public static Dictionary<string, string> Langs = new Dictionary<string, string>();

        public static bool isLoading;

        public GetConfig()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:SendConfig"] +=
                    new Action<string, ExpandoObject>(LoadDefaultConfig);
            TriggerServerEvent($"{API.GetCurrentResourceName()}:getConfig");
        }

        private void LoadDefaultConfig(string dc, ExpandoObject dl)
        {
            Config = JObject.Parse(dc);

            foreach (var l in dl)
            {
                Langs[l.Key] = l.Value.ToString();
            }

            InitScripts();
        }

        public void InitScripts()
        {
            DiscRichPresence.drp_active = Config["ActiveDRP"].ToObject<bool>();
            IDHeads.UseIDHeads = Config["HeadId"].ToObject<bool>();
            IDHeads.UseKeyMode = Config["ModeKey"].ToObject<bool>();
            VoiceChat.activeVoiceChat = Config["ActiveVoiceChat"].ToObject<bool>();
            IDHeads.keyShow = FromHex(Config["KeyShowIds"].ToString());

            VoiceChat.keyRange = FromHex(Config["KeySwapVoiceRange"].ToString());

            var voiceRangeDefault = Config["DefaultVoiceRange"].ToObject<float>();
            foreach (var r in Config["VoiceRanges"])
            {
                VoiceChat.voiceRange.Add(r.ToObject<float>());
            }

            if (VoiceChat.voiceRange.IndexOf(voiceRangeDefault) != -1)
            {
                VoiceChat.voiceRangeSelected = VoiceChat.voiceRange.IndexOf(voiceRangeDefault);
            }

            isLoading = true;
        }

        public static uint FromHex(string value)
        {
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
            }

            return (uint)int.Parse(value, NumberStyles.HexNumber);
        }
    }
}

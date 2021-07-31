using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace vorpcore_cl.Ui
{
    public class Uicore : BaseScript
    {
        public Uicore()
        {
            // crear trigger events y asignar a los eventos de añadir y remover dinero, tambien recordar cambiar en la db a decimal el dinero y el oro
            EventHandlers["vorp:updateUi"] += new Action<string>(updateUI);
            EventHandlers["vorp:showUi"] += new Action<bool>(showUI);
        }

        public void updateUI(string stringJson)
        {
            API.SendNuiMessage(stringJson);
        }

        public void showUI(bool active)
        {
            var jsonpost = "{\"type\": \"ui\",\"action\":\"hide\"}";
            if (active)
            {
                jsonpost = "{\"type\": \"ui\",\"action\":\"show\"}";
            }

            API.SendNuiMessage(jsonpost);
        }
    }
}

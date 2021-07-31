using System;
using System.Collections.Generic;
using CitizenFX.Core;
using Newtonsoft.Json.Linq;
using vorpcore_sv.Scripts;

namespace vorpcore_sv.Utils
{
    internal class ApiController : BaseScript
    {
        public delegate Dictionary<string, dynamic> auxDelegate(int source);

        public delegate Dictionary<string, Dictionary<string, dynamic>> getUsersSource();

        public ApiController()
        {
            EventHandlers["vorp:getCharacter"] += new Action<int, dynamic>(getCharacter);
            EventHandlers["vorp:addMoney"] += new Action<int, int, double>(addMoney);
            EventHandlers["vorp:removeMoney"] += new Action<int, int, double>(removeMoney);

            EventHandlers["vorp:addXp"] += new Action<int, int>(addXp);
            EventHandlers["vorp:removeXp"] += new Action<int, int>(removeXp);

            EventHandlers["vorp:setJob"] += new Action<int, string>(setJob);
            EventHandlers["vorp:setGroup"] += new Action<int, string>(setGroup);
            EventHandlers["getCore"] += new Action<CallbackDelegate>(cb =>
            {
                var corefunctions = new Dictionary<string, dynamic>
                {
                        ["getUser"] = new auxDelegate(getUser),
                        ["maxCharacters"] = LoadConfig.Config["MaxCharacters"].ToObject<int>(),
                        ["addRpcCallback"] = new Action<string, CallbackDelegate>((name, callback) =>
                        {
                            try
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Vorp Core: {name} function callback registered!");
                                Console.ForegroundColor = ConsoleColor.White;

                                Callbacks.ServerCallBacks[name] = callback;
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                            }
                        }),
                        ["getUsers"] = new getUsersSource(getConnectedUsers),
                        ["sendLog"] = new Action<string, string>((msg, type) => { LogManager.WriteLog(msg, type); })
                };
                cb.Invoke(corefunctions);
            });
        }

        private ApiController(bool asHelper)
        {
            if (!asHelper)
            {
                throw new InvalidOperationException($"{nameof(asHelper)} must be true if initialization was intended");
            }
        }

        public static Dictionary<string, dynamic> getUser(int source)
        {
            var steam = "steam:" + new ApiController(true).Players[source].Identifiers["steam"];
            if (LoadUsers._users.ContainsKey(steam))
            {
                return LoadUsers._users[steam].GetUser();
            }

            return null;
        }

        public static Dictionary<string, Dictionary<string, dynamic>> getConnectedUsers()
        {
            var UsersDictionary = new Dictionary<string, Dictionary<string, dynamic>>();
            foreach (var player in new ApiController(true).Players)
            {
                var steam = "steam:" + player.Identifiers["steam"];
                if (LoadUsers._users.ContainsKey(steam) && !UsersDictionary.ContainsKey(steam))
                {
                    UsersDictionary.Add(steam, LoadUsers._users[steam].GetUser());
                }
            }

            return UsersDictionary;
        }

        public static Player getSource(int handle)
        {
            Player p = null;

            try
            {
                p = new ApiController(true).Players[handle];
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Server Handle Not Found");
                p = null;
            }

            return p;
        }

        private void removeMoney(int handle, int typeCash, double quantity)
        {
            var player = getSource(handle);
            var sid = "steam:" + player.Identifiers["steam"];

            if (LoadUsers._users.ContainsKey(sid))
            {
                LoadUsers._users[sid].GetUsedCharacter().removeCurrency(typeCash, quantity);

                var nuipost = new JObject();
                nuipost.Add("type", "ui");
                nuipost.Add("action", "update");
                nuipost.Add("moneyquanty", LoadUsers._users[sid].GetUsedCharacter().Money);
                nuipost.Add("goldquanty", LoadUsers._users[sid].GetUsedCharacter().Gold);
                nuipost.Add("rolquanty", LoadUsers._users[sid].GetUsedCharacter().Rol);
                nuipost.Add("xp", LoadUsers._users[sid].GetUsedCharacter().Xp);
                nuipost.Add("serverId", handle);

                player.TriggerEvent("vorp:updateUi", nuipost.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning removeMoney: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void addMoney(int handle, int typeCash, double quantity)
        {
            var player = getSource(handle);

            var sid = "steam:" + player.Identifiers["steam"];

            if (LoadUsers._users.ContainsKey(sid))
            {
                LoadUsers._users[sid].GetUsedCharacter().addCurrency(typeCash, quantity);

                var nuipost = new JObject();
                nuipost.Add("type", "ui");
                nuipost.Add("action", "update");
                nuipost.Add("moneyquanty", LoadUsers._users[sid].GetUsedCharacter().Money);
                nuipost.Add("goldquanty", LoadUsers._users[sid].GetUsedCharacter().Gold);
                nuipost.Add("rolquanty", LoadUsers._users[sid].GetUsedCharacter().Rol);
                nuipost.Add("xp", LoadUsers._users[sid].GetUsedCharacter().Xp);
                nuipost.Add("serverId", handle);

                player.TriggerEvent("vorp:updateUi", nuipost.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning addMoney: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void addXp(int handle, int quantity)
        {
            var player = getSource(handle);

            var sid = "steam:" + player.Identifiers["steam"];

            if (LoadUsers._users.ContainsKey(sid))
            {
                LoadUsers._users[sid].GetUsedCharacter().addXp(quantity);

                var nuipost = new JObject();
                nuipost.Add("type", "ui");
                nuipost.Add("action", "setxp");
                nuipost.Add("xp", LoadUsers._users[sid].GetUsedCharacter().Xp);

                player.TriggerEvent("vorp:updateUi", nuipost.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning addXp: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void removeXp(int handle, int quantity)
        {
            var player = getSource(handle);

            var sid = "steam:" + player.Identifiers["steam"];

            if (LoadUsers._users.ContainsKey(sid))
            {
                LoadUsers._users[sid].GetUsedCharacter().removeXp(quantity);

                var nuipost = new JObject();
                nuipost.Add("type", "ui");
                nuipost.Add("action", "setxp");
                nuipost.Add("xp", LoadUsers._users[sid].GetUsedCharacter().Xp);

                player.TriggerEvent("vorp:updateUi", nuipost.ToString());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning removeXp: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void setJob(int handle, string job)
        {
            var player = getSource(handle);

            var sid = "steam:" + player.Identifiers["steam"];

            if (LoadUsers._users.ContainsKey(sid))
            {
                LoadUsers._users[sid].GetUsedCharacter().setJob(job);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning setJob: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void setGroup(int handle, string group)
        {
            var player = getSource(handle);

            var sid = "steam:" + player.Identifiers["steam"];

            if (LoadUsers._users.ContainsKey(sid))
            {
                LoadUsers._users[sid].GetUsedCharacter().setGroup(group);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning setGroup: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void getCharacter(int handle, dynamic cb)
        {
            var player = getSource(handle);

            var sid = "steam:" + player.Identifiers["steam"];

            if (!LoadUsers._users.ContainsKey(sid))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: User not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                cb(LoadUsers._users[sid].GetUsedCharacter().getCharacter());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using CitizenFX.Core;
using Newtonsoft.Json.Linq;
using vorpcore_sv.Utils;

namespace vorpcore_sv.Class
{
    //class for users that contains their characters
    public class User : BaseScript
    {
        private string _group; //User admin group
        private int _playerwarnings; //Used for admins to know how many warnings a user has
        private readonly Dictionary<int, Character> _usercharacters;
        private int usedCharacterId = -1;

        public int UsedCharacterId
        {
            get => usedCharacterId;
            set
            {
                usedCharacterId = value;
                foreach (var player in Players)
                {
                    var steamid = "steam:" + player.Identifiers["steam"];
                    if (steamid == Identifier)
                    {
                        Source = int.Parse(player.Handle);
                        _usercharacters[value].Source = Source;
                        player.TriggerEvent("vorp:SelectedCharacter", usedCharacterId);
                        var postUi = new JObject();
                        postUi.Add("type", "ui");
                        postUi.Add("action", "update");
                        postUi.Add("moneyquanty", _usercharacters[usedCharacterId].Money);
                        postUi.Add("goldquanty", _usercharacters[usedCharacterId].Gold);
                        postUi.Add("rolquanty", _usercharacters[usedCharacterId].Rol);
                        postUi.Add("serverId", player.Handle);
                        postUi.Add("xp", _usercharacters[usedCharacterId].Xp);

                        player.TriggerEvent("vorp:updateUi", postUi.ToString());
                        break;
                    }
                }

                TriggerEvent("vorp:SelectedCharacter", Source, _usercharacters[usedCharacterId].getCharacter());
            }
        }

        public int Source { get; set; } = -1;

        public int Numofcharacters { get; set; }

        public string Identifier { get; }

        public string License { get; set; }

        public string Group
        {
            get => _group;
            set
            {
                _group = value;
                Exports["ghmattimysql"].execute("UPDATE users SET `group` = ? WHERE `identifier` = ?",
                                                new object[] { _group, Identifier });
            }
        }

        public int Playerwarnings
        {
            get => _playerwarnings;
            set
            {
                _playerwarnings = value;
                Exports["ghmattimysql"].execute("UPDATE users SET `warnings` = ? WHERE `identifier` = ?",
                                                new object[] { _playerwarnings, Identifier });
            }
        }

        public User(string identifier, string group, int playerwarnings, string license)
        {
            Identifier = identifier;
            _group = group;
            _playerwarnings = playerwarnings;
            _usercharacters = new Dictionary<int, Character>();
            License = license;
            LoadCharacters(identifier);
        }

        public Dictionary<string, dynamic> GetUser()
        {
            var character = new Dictionary<string, dynamic>();
            if (_usercharacters.ContainsKey(usedCharacterId))
            {
                character = _usercharacters[usedCharacterId].getCharacter();
            }

            var userCharacters = new List<Dictionary<string, dynamic>>();
            foreach (var chara in _usercharacters)
            {
                userCharacters.Add(chara.Value.getCharacter());
            }

            var auxdic = new Dictionary<string, dynamic>
            {
                    ["getIdentifier"] = Identifier,
                    ["getGroup"] = Group,
                    ["getPlayerwarnings"] = Playerwarnings,
                    ["source"] = Source,
                    ["setGroup"] = new Action<string>(group =>
                    {
                        try
                        {
                            Group = group;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }),
                    ["setPlayerWarnings"] = new Action<int>(warnings =>
                    {
                        try
                        {
                            Playerwarnings = warnings;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }),
                    ["getUsedCharacter"] = character,
                    ["getUserCharacters"] = userCharacters,
                    ["getNumOfCharacters"] = Numofcharacters,
                    ["addCharacter"] = new Action<string, string, string, string>((firstname, lastname, skin, comps) =>
                    {
                        Numofcharacters++;
                        try
                        {
                            addCharacter(firstname, lastname, skin, comps);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }),
                    ["removeCharacter"] = new Action<int>(charid =>
                    {
                        try
                        {
                            if (_usercharacters.ContainsKey(charid))
                            {
                                delCharacter(charid);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }),
                    ["setUsedCharacter"] = new Action<int>(charid =>
                    {
                        try
                        {
                            SetUsedCharacter(charid);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    })
            };
            return auxdic;
        }

        private async void LoadCharacters(string identifier)
        {
            Exports["ghmattimysql"].execute("SELECT * FROM characters WHERE identifier =?", new[] { identifier },
                                            new Action<dynamic>(usercharacters =>
                                            {
                                                Numofcharacters = usercharacters.Count;
                                                if (usercharacters.Count > 0)
                                                {
                                                    foreach (object icharacter in usercharacters)
                                                    {
                                                        IDictionary<string, object> character = (dynamic)icharacter;
                                                        if (character.ContainsKey("identifier"))
                                                        {
                                                            var newCharacter =
                                                                    new Character(identifier,
                                                                        Convert.ToInt32(character
                                                                                ["charidentifier"]),
                                                                        (string)character["group"],
                                                                        (string)character["job"],
                                                                        int.Parse(character["jobgrade"]
                                                                                .ToString()),
                                                                        (string)character["firstname"],
                                                                        (string)character["lastname"]
                                                                        , (string)character["inventory"],
                                                                        (string)character["status"],
                                                                        (string)character["coords"],
                                                                        double.Parse(character["money"].ToString())
                                                                        , double.Parse(character["gold"].ToString()),
                                                                        double.Parse(character["rol"].ToString()),
                                                                        int.Parse(character["xp"].ToString()),
                                                                        Convert.ToBoolean(character["isdead"]),
                                                                        (string)character["skinPlayer"],
                                                                        (string)character["compPlayer"]);
                                                            if (_usercharacters
                                                                    .ContainsKey(newCharacter.CharIdentifier))
                                                            {
                                                                _usercharacters[newCharacter.CharIdentifier] =
                                                                        newCharacter;
                                                            }
                                                            else
                                                            {
                                                                _usercharacters.Add(newCharacter.CharIdentifier,
                                                                    newCharacter);
                                                            }
                                                        }
                                                    }

                                                    Debug.WriteLine("User characters" + usercharacters.Count);
                                                }
                                            }));
        }

        public async void addCharacter(string firstname, string lastname, string skin, string comps)
        {
            var newChar = new Character(Identifier, LoadConfig.Config["initGroup"].ToString(),
                                        LoadConfig.Config["initJob"].ToString(),
                                        LoadConfig.Config["initJobGrade"].ToObject<int>(), firstname, lastname, "{}",
                                        "{}", "{}", LoadConfig.Config["initMoney"].ToObject<double>(),
                                        LoadConfig.Config["initGold"].ToObject<double>(),
                                        LoadConfig.Config["initRol"].ToObject<double>(),
                                        LoadConfig.Config["initXp"].ToObject<int>(), false, skin, comps);
            var charidentifier = await newChar.SaveNewCharacterInDb();
            _usercharacters.Add(charidentifier, newChar);
            Debug.WriteLine("Added new character with identifier " +
                            _usercharacters[charidentifier].PlayerVar.Identifiers["steam"]);
            UsedCharacterId = charidentifier;
        }

        public void delCharacter(int charIdentifier)
        {
            if (_usercharacters.ContainsKey(charIdentifier))
            {
                _usercharacters[charIdentifier].DeleteCharacter();
                _usercharacters.Remove(charIdentifier);
                Debug.WriteLine($"Character with charid {charIdentifier} deleted from user {Identifier} successfully");
            }
        }

        public Character GetUsedCharacter()
        {
            if (_usercharacters.ContainsKey(UsedCharacterId))
            {
                return _usercharacters[UsedCharacterId];
            }

            return null;
        }

        public void SetUsedCharacter(int charid)
        {
            if (_usercharacters.ContainsKey(charid))
            {
                UsedCharacterId = charid;
            }
        }

        public void SaveUser()
        {
            foreach (var character in _usercharacters)
            {
                character.Value.SaveCharacterInDb();
            }
        }
    }
}

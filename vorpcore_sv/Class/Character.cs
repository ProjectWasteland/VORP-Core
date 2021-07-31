using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json.Linq;

namespace vorpcore_sv.Class
{
    //Class for user characters
    public class Character : BaseScript
    {
        private string comps;

        private string skin;
        private int source;

        private Player userPlayer;

        public int Source
        {
            set => source = value;
        }

        public Player PlayerVar => Players[source];

        public string Identifier { get; }

        public int CharIdentifier { get; set; }

        public string Group { get; private set; }

        public string Job { get; private set; }

        public int Jobgrade { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Inventory { get; set; }

        public string Status { get; set; }

        public string Coords { get; set; }

        public double Money { get; private set; }

        public double Gold { get; private set; }

        public double Rol { get; private set; }

        public int Xp { get; private set; }

        public bool IsDead { get; private set; }

        public string Skin
        {
            get => skin;
            set
            {
                skin = value;
                Exports["ghmattimysql"]
                        .execute("UPDATE characters SET `skinPlayer` = ? WHERE `identifier` = ? AND `charidentifier` = ?",
                                 new object[] { value, Identifier, CharIdentifier });
            }
        }

        public string Comps
        {
            get => comps;
            set
            {
                comps = value;
                Exports["ghmattimysql"]
                        .execute("UPDATE characters SET `compPlayer` = ? WHERE `identifier` = ? AND `charidentifier` = ?",
                                 new object[] { value, Identifier, CharIdentifier });
            }
        }

        public Character(string identifier)
        {
            Identifier = identifier;
            Job = "unemployed";
            Group = "user";
            Inventory = "{}";
        }

        public Character(string identifier, int charIdentifier, string group, string job, int jobgrade,
                         string firstname, string lastname, string inventory, string status, string coords,
                         double money, double gold, double rol, int xp, bool isdead, string skin, string comps)
        {
            Identifier = identifier;
            CharIdentifier = charIdentifier;
            Group = group;
            Job = job;
            Jobgrade = jobgrade;
            Firstname = firstname;
            Lastname = lastname;
            Inventory = inventory;
            Status = status;
            Coords = coords;
            Money = money;
            Gold = gold;
            Rol = rol;
            Xp = xp;
            IsDead = isdead;
            this.skin = skin;
            this.comps = comps;
        }

        public Character(string identifier, string group, string job, int jobgrade, string firstname, string lastname,
                         string inventory, string status, string coords, double money, double gold, double rol, int xp,
                         bool isdead, string skin, string comps)
        {
            Identifier = identifier;
            Group = group;
            Job = job;
            Jobgrade = jobgrade;
            Firstname = firstname;
            Lastname = lastname;
            Inventory = inventory;
            Status = status;
            Coords = coords;
            Money = money;
            Gold = gold;
            Rol = rol;
            Xp = xp;
            IsDead = isdead;
            this.skin = skin;
            this.comps = comps;
        }

        public Dictionary<string, dynamic> getCharacter()
        {
            var userData = new Dictionary<string, dynamic>();
            userData.Add("identifier", Identifier);
            userData.Add("charIdentifier", CharIdentifier);
            userData.Add("group", Group);
            userData.Add("job", Job);
            userData.Add("jobGrade", Jobgrade);
            userData.Add("money", Money);
            userData.Add("gold", Gold);
            userData.Add("rol", Rol);
            userData.Add("xp", Xp);
            userData.Add("firstname", Firstname);
            userData.Add("lastname", Lastname);
            userData.Add("inventory", Inventory);
            userData.Add("status", Status);
            userData.Add("coords", Coords);
            userData.Add("isdead", IsDead);
            userData.Add("skin", skin);
            userData.Add("comps", comps);
            userData.Add("setStatus", new Action<string>(status =>
            {
                try
                {
                    Status = status;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setJobGrade", new Action<int>(jobgrade =>
            {
                try
                {
                    Jobgrade = jobgrade;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setGroup", new Action<string>(g =>
            {
                try
                {
                    Group = g;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setJob", new Action<string>(j =>
            {
                try
                {
                    Job = j;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setMoney", new Action<double>(m =>
            {
                try
                {
                    Money = m;
                    updateCharUi();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setGold", new Action<double>(g =>
            {
                try
                {
                    Gold = g;
                    updateCharUi();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setRol", new Action<double>(r =>
            {
                try
                {
                    Rol = r;
                    updateCharUi();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setXp", new Action<int>(x =>
            {
                try
                {
                    Xp = x;
                    updateCharUi();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setFirstname", new Action<string>(f =>
            {
                try
                {
                    Firstname = f;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setLastname", new Action<string>(l =>
            {
                try
                {
                    Lastname = l;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("updateSkin", new Action<string>(nskin =>
            {
                try
                {
                    Skin = nskin;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("updateComps", new Action<string>(ncomps =>
            {
                try
                {
                    Comps = ncomps;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("addCurrency", new Action<int, double>((currency, quantity) =>
            {
                try
                {
                    addCurrency(currency, quantity);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("removeCurrency", new Action<int, double>((currency, quantity) =>
            {
                try
                {
                    removeCurrency(currency, quantity);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("addXp", new Action<int>(xp =>
            {
                try
                {
                    addXp(xp);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("removeXp", new Action<int>(xp =>
            {
                try
                {
                    removeXp(xp);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            return userData;
        }

        public void updateCharUi()
        {
            var nuipost = new JObject();
            nuipost.Add("type", "ui");
            nuipost.Add("action", "update");
            nuipost.Add("moneyquanty", Money);
            nuipost.Add("goldquanty", Gold);
            nuipost.Add("rolquanty", Rol);
            nuipost.Add("xp", Xp);
            nuipost.Add("serverId", source.ToString());

            PlayerVar.TriggerEvent("vorp:updateUi", nuipost.ToString());
        }

        public void addCurrency(int currency, double quantity)
        {
            switch (currency)
            {
                case 0:
                    Money += quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET money=money + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 1:
                    Gold += quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET gold=gold + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 2:
                    Rol += quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET rol=rol + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
            }

            updateCharUi();
        }

        public void removeCurrency(int currency, double quantity)
        {
            switch (currency)
            {
                case 0:
                    Money -= quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET money=money - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 1:
                    Gold -= quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET gold=gold - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 2:
                    Rol -= quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET rol=rol - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
            }

            updateCharUi();
        }

        public void addXp(int quantity)
        {
            Xp += quantity;
            //Exports["ghmattimysql"].execute($"UPDATE characters SET xp=xp + ? WHERE identifier=?", new object[] { quantity, identifier });
            updateCharUi();
        }

        public void removeXp(int quantity)
        {
            Xp -= quantity;
            //Exports["ghmattimysql"].execute($"UPDATE characters SET xp=xp - ? WHERE identifier=?", new object[] { quantity, identifier });
            updateCharUi();
        }

        public void setJob(string newjob)
        {
            Job = newjob;
            //Exports["ghmattimysql"].execute($"UPDATE characters SET job=? WHERE identifier=?", new string[] { newjob, identifier });
        }

        public void setGroup(string newgroup)
        {
            Group = newgroup;
            //Exports["ghmattimysql"].execute($"UPDATE characters SET `group`=? WHERE identifier=?", new string[] { newgroup.ToString(), identifier });
        }

        public void setDead(bool dead)
        {
            IsDead = dead;
            //int intdead = dead ? 1 : 0;
            //Exports["ghmattimysql"].execute("UPDATE characters SET `isdead` = ? WHERE `identifier` = ?", new object[] { intdead, identifier });
        }

        public async Task<int> SaveNewCharacterInDb()
        {
            var character = await Exports["ghmattimysql"]
                    .executeSync("INSERT INTO characters(`identifier`,`group`,`money`,`gold`,`rol`,`xp`,`inventory`,`job`,`status`,`firstname`,`lastname`,`skinPlayer`,`compPlayer`,`jobgrade`,`coords`,`isdead`) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)",
                                 new object[]
                                 {
                                         Identifier, Group, Money, Gold, Rol, Xp, Inventory, Job, Status, Firstname,
                                         Lastname, skin, comps, Jobgrade, Coords, IsDead ? 1 : 0
                                 });
            CharIdentifier = (int)character.insertId;
            return (int)character.insertId;
        }

        public void DeleteCharacter()
        {
            Exports["ghmattimysql"].execute("DELETE FROM characters WHERE `identifier` = ? AND `charidentifier` = ? ",
                                            new object[] { Identifier, CharIdentifier });
        }

        public void SaveCharacterCoords(string coords)
        {
            Coords = coords;
            Exports["ghmattimysql"]
                    .execute("UPDATE characters SET `coords` = ? WHERE `identifier` = ? AND `charidentifier` = ?",
                             new object[] { coords, Identifier, CharIdentifier });
        }

        public void SaveCharacterInDb()
        {
            Exports["ghmattimysql"]
                    .execute("UPDATE characters SET `group` = ?,`money` = ?,`gold` = ?,`rol` = ?,`xp` = ?,`job` = ?, `status` = ?,`firstname` = ?, `lastname` = ?, `jobgrade` = ?,`coords` = ?,`isdead` = ? WHERE `identifier` = ? AND `charidentifier` = ?",
                             new object[]
                             {
                                     Group, Money, Gold, Rol, Xp, Job, Status, Firstname, Lastname, Jobgrade, Coords,
                                     IsDead ? 1 : 0, Identifier, CharIdentifier
                             });
        }
    }
}

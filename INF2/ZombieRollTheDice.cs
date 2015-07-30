using System;
using System.Collections.Generic;
using InfinityScript;

namespace INF2
{
    public class ZombieRollTheDice : BaseScript
    {
        private List<string> PlayerStop = new List<string>();
        public const int NumOfRolls = 60;
        public ZombieRollTheDice()
        {
            PlayerConnected += RollTheDice_PlayerConnected;
            PlayerDisconnected += player => PlayerStop.Add(player.GetField<string>("name"));
        }


        void RollTheDice_PlayerConnected(Entity obj)
        {
            obj.Call("visionsetnakedforplayer", "", 1);
            OnPlayerSpawned(obj);
            obj.SpawnedPlayer += () => OnPlayerSpawned(obj);
        }


        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            PlayerStop.Add(player.GetField<string>("name"));
            if (player == null || !player.IsPlayer)
            {
                return;
            }
            if (attacker == null || !attacker.IsPlayer)
            {
                return;
            }
            if (Utility.GetPlayerTeam(attacker) == Utility.GetPlayerTeam(player))
            {
                return;
            }
            if (Utility.GetPlayerTeam(player) == "axis")
            {
                player.SetField("rtd_canroll", 1);
            }
        }

        public void OnPlayerSpawned(Entity player)
        {
            if (Utility.GetPlayerTeam(player) == "axis")
            {
                if (PlayerStop.Contains(player.GetField<string>("name")))
                    PlayerStop.Remove(player.GetField<string>("name"));
                if (!player.HasField("rtd_canroll") || player.GetField<int>("rtd_canroll") == 1)
                {
                    ResetPlayer(player);
                    AfterDelay(50, () => DoRandom(player));
                }
            }
        }

        public void DoRandom(Entity player)
        {
            int? roll = new Random().Next(NumOfRolls);
            var rollname = "";
            switch (roll)
            {
                case 0:
                    rollname = "None";
                    break;
                case 1:
                    rollname = "^2XM25";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("xm25_mp");
                    player.Call("setweaponammostock", "xm25_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("xm25_mp"));
                    break;
                case 2:
                    rollname = "^2Extra Speed";
                    OnInterval(100, () => Speed(player, 1.5));
                    break;
                case 3:
                    rollname = "^1You are a one hit kill";
                    player.SetField("maxhealth", 1);
                    player.Health = 1;
                    break;
                case 4:
                    rollname = "^2Triple HP";
                    player.SetField("maxhealth", player.Health * 3);
                    player.Health = player.Health * 3;
                    break;
                case 5:
                    rollname = "^2Triple HP";
                    player.SetField("maxhealth", player.Health * 3);
                    player.Health = player.Health * 3;
                    break;
                case 6:
                    rollname = "^1You are a one hit kill";
                    player.SetField("maxhealth", 1);
                    player.Health = 1;
                    break;
                case 7:
                    rollname = "^2SMAW";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_smaw_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("iw5_smaw_mp"));
                    break;
                case 8:
                    rollname = "^1Stinger";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("stinger_mp");
                    player.Call("setweaponammostock", "stinger_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("stinger_mp"));
                    break;
                case 9:
                    rollname = "^2Extra Speed";
                    OnInterval(100, () => Speed(player, 1.5));
                    break;
                case 10:
                    rollname = "^2Extra Speed";
                    OnInterval(100, () => Speed(player, 1.5));
                    break;
                case 11:
                    rollname = "^2Triple HP";
                    player.SetField("maxhealth", player.Health * 3);
                    player.Health = player.Health * 3;
                    break;
                case 12:
                    rollname = "^2AA12";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_aa12_mp_xmags_camo11");
                    player.Call("setweaponammostock", "iw5_aa12_mp_xmags_camo11", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("iw5_aa12_mp_xmags_camo11"));
                    break;
                case 13:
                    rollname = "^2Triple HP";
                    player.SetField("maxhealth", player.Health * 3);
                    player.Health = player.Health * 3;
                    break;
                case 14:
                    rollname = "^1You are a one hit kill";
                    player.SetField("maxhealth", 1);
                    player.Health = 1;
                    break;
                case 15:
                    rollname = "^1Turtle";
                    OnInterval(100, () => Speed(player, 0.4f));
                    break;
                case 16:
                    rollname = "^1Turtle";
                    OnInterval(100, () => Speed(player, 0.4f));
                    break;
                case 17:
                    rollname = "^2Javelin";
                    player.GiveWeapon("javelin_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("javelin_mp"));
                    break;
                case 18:
                    rollname = "^2Flash Bang";
                    player.GiveWeapon("flash_grenade_mp");
                    player.Call("setweaponammoclip", "flash_grenade_mp", 1);
                    player.Call("setweaponammostock", "flash_grenade_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("flash_grenade_mp"));
                    break;
                case 19:
                    rollname = "^1You are a one hit kill";
                    player.SetField("maxhealth", 1);
                    player.Health = 1;
                    break;
                case 20:
                    rollname = "^2One Ammo MK14";
                    player.GiveWeapon("iw5_mk14_mp_acog");
                    player.Call("setweaponammoclip", "iw5_mk14_mp_acog", 1);
                    player.Call("setweaponammostock", "iw5_mk14_mp_acog", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("iw5_mk14_mp_acog"));
                    break;
                case 21:
                    rollname = "^2Triple HP";
                    player.SetField("maxhealth", player.Health * 3);
                    player.Health = player.Health * 3;
                    break;
                case 22:
                    rollname = "^2SMAW";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_smaw_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("iw5_smaw_mp"));
                    break;
                case 23:
                    rollname = "^1Can't Jump";
                    player.Call("allowjump", false);
                    break;
                case 24:
                    rollname = "^1Can't Jump";
                    player.Call("allowjump", false);
                    break;
                case 25:
                    rollname = "^2Riotshield";
                    player.SetPerk("specialty_fastermelee", true, true);
                    player.SetPerk("specialty_lightweight", true, true);
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("riotshield_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("riotshield_mp"));
                    player.AfterDelay(150,
                                      entity =>
                                      player.Call("attachshieldmodel", "weapon_riot_shield_mp", "tag_shield_back"));
                    break;
                case 26:
                    rollname = "^2C4";
                    player.GiveWeapon("c4_mp");
                    player.Call("setweaponammoclip", "c4_mp", 1);
                    player.Call("setweaponammostock", "c4_mp", 0);
                    break;
                case 27:
                    rollname = "^2Grenade Lanucher";
                    player.GiveWeapon("gl_mp");
                    player.Call("setweaponammostock", "gl_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("gl_mp"));
                    break;
                case 28:
                    rollname = "^2Riotshield";
                    player.SetPerk("specialty_fastermelee", true, true);
                    player.SetPerk("specialty_lightweight", true, true);
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("riotshield_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("riotshield_mp"));
                    player.AfterDelay(150,
                                      entity =>
                                      player.Call("attachshieldmodel", "weapon_riot_shield_mp", "tag_shield_back"));
                    break;
                case 29:
                    rollname = "None";
                    break;
                case 30:
                    rollname = "^1USP45 AKIMBO";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_usp45_mp_akimbo");
                    player.Call("setweaponammostock", "iw5_usp45_mp_akimbo", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("iw5_usp45_mp_akimbo"));
                    break;
                case 31:
                    rollname = "None";
                    break;
                case 32:
                    rollname = "^1Stinger";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("stinger_mp");
                    player.Call("setweaponammostock", "stinger_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("stinger_mp"));
                    break;
                case 33:
                    rollname = "^2Smoke";
                    player.GiveWeapon("smoke_grenade_mp");
                    player.Call("setweaponammoclip", "smoke_grenade_mp", 1);
                    player.Call("setweaponammostock", "smoke_grenade_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("smoke_grenade_mp"));
                    break;
                case 34:
                    rollname = "None";
                    break;
                case 35:
                    rollname = "None";
                    break;
                case 36:
                    rollname = "^2Riotshield";
                    player.SetPerk("specialty_fastermelee", true, true);
                    player.SetPerk("specialty_lightweight", true, true);
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("riotshield_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("riotshield_mp"));
                    player.AfterDelay(150,
                                      entity =>
                                      player.Call("attachshieldmodel", "weapon_riot_shield_mp", "tag_shield_back"));
                    break;
                case 37:
                    rollname = "None";
                    break;
                case 38:
                    rollname = "^2Desert Eagle";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_deserteagle_mp");
                    player.Call("setweaponammostock", "iw5_deserteagle_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("iw5_deserteagle_mp"));
                    break;
                case 39:
                    rollname = "^2Riotshield";
                    player.SetPerk("specialty_fastermelee", true, true);
                    player.SetPerk("specialty_lightweight", true, true);
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("riotshield_mp");
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("riotshield_mp"));
                    player.AfterDelay(150,
                                      entity =>
                                      player.Call("attachshieldmodel", "weapon_riot_shield_mp", "tag_shield_back"));
                    break;
                case 40:
                    rollname = "^2Concussion Grenade";
                    player.GiveWeapon("concussion_grenade_mp");
                    player.Call("setweaponammoclip", "concussion_grenade_mp", 1);
                    player.Call("setweaponammostock", "concussion_grenade_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("concussion_grenade_mp"));
                    break;
                case 41:
                    rollname = "^2Throwing Knife";
                    player.GiveWeapon("throwingknife_mp");
                    player.Call("setweaponammoclip", "throwingknife_mp", 1);
                    player.Call("setweaponammostock", "throwingknife_mp", 0);
                    break;
                case 42:
                    rollname = "^2EMP Grenade";
                    player.GiveWeapon("emp_grenade_mp");
                    player.Call("setweaponammoclip", "emp_grenade_mp", 1);
                    player.Call("setweaponammostock", "emp_grenade_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("emp_grenade_mp"));
                    break;
                case 43:
                    rollname = "None";
                    break;
                case 44:
                    rollname = "None";
                    break;
                case 45:
                    rollname = "^1You Die After 3 Second";
                    player.SetField("rtd_canroll", 1);
                    AfterDelay(3000, () => player.Call("suicide"));
                    break;
                case 46:
                    rollname = "None";
                    break;
                case 47:
                    rollname = "None";
                    break;
                case 48:
                    rollname = "^2M320";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("m320_mp");
                    player.Call("setweaponammoclip", "m320_mp", 1);
                    player.Call("setweaponammostock", "m320_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("m320_mp"));
                    break;
                case 49:
                    rollname = "None";
                    break;
                case 50:
                    rollname = "^2One Ammo Barrett";
                    player.GiveWeapon("iw5_barrett_mp");
                    player.Call("setweaponammoclip", "iw5_barrett_mp", 1);
                    player.Call("setweaponammostock", "iw5_barrett_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("iw5_barrett_mp"));
                    break;
                case 51:
                    rollname = "^2Unlimited Grenades";
                    OnInterval(100, () => Nades(player, 99));
                    break;
                case 52:
                    rollname = "^2RPG";
                    player.TakeWeapon(player.CurrentWeapon);
                    player.GiveWeapon("iw5_barrett_mp");
                    player.Call("setweaponammoclip", "iw5_barrett_mp", 1);
                    player.Call("setweaponammostock", "iw5_barrett_mp", 0);
                    AfterDelay(300, () => player.SwitchToWeaponImmediate("iw5_barrett_mp"));
                    break;
                case 53:
                    rollname = "^2Unlimited Grenades";
                    OnInterval(100, () => Nades(player, 99));
                    break;
                case 54:
                    rollname = "None";
                    break;
                case 55:
                    rollname = "^2Unlimited Grenades";
                    OnInterval(100, () => Nades(player, 99));
                    break;
                case 56:
                    rollname = "^1Zombie Incantation";
                    player.SetField("zombie_incantation", 1);
                    break;
                case 57:
                    rollname = "None";
                    break;
                case 58:
                    rollname = "^1Zombie Incantation";
                    player.SetField("zombie_incantation", 1);
                    break;
                case 59:
                    rollname = "^1Zombie Incantation";
                    player.SetField("zombie_incantation", 1);
                    break;
            }
            PrintRollNames(player, rollname, 0, roll);
        }

        public void PrintRollNames(Entity player, string name, int index, int? roll)
        {
            player.Call("iPrintLnBold", string.Format("You rolled {0} - {1}", roll + 1, name));
            Call(334, string.Format("{0} rolled [{1}] - {2}", player.GetField<string>("name"), roll + 1, name));
            HudElem elem = player.HasField("rtd_rolls") ? player.GetField<HudElem>("rtd_rolls") : HudElem.CreateFontString(player, "bigfixed", 0.6f);
            elem.SetPoint("RIGHT", "RIGHT", -90, 165 - ((index - 1) * 13));
            elem.SetText(string.Format("[{0}] {1}", roll + 1, name));
            player.SetField("rtd_rolls", new Parameter(elem));
        }

        public void ResetPlayer(Entity player)
        {
            player.SetField("rtd_canroll", 0);
            player.SetField("zombie_incantation", 0);
            player.Call("setmovespeedscale", 1f);
        }

        public bool Speed(Entity player, double scale)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            player.Call("setmovespeedscale", new Parameter((float)scale));
            return true;
        }

        public bool Nades(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            var offhand = player.Call<string>("getcurrentoffhand");
            player.Call("setweaponammoclip", offhand, amount);
            player.Call("givemaxammo", offhand);
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF2
{
    public class INF2 : BaseScript
    {
        public INF2()
        {
            InitWeapons();

            Call("setdvarifuninitialized", "scr_inf2_initweapon", "iw5_mp9_mp");

            PlayerConnected += delegate (Entity player)
            {
                player.SetField("inf2_money", 500);
                player.SetField("inf2_point", 0);
                player.SetField("inf2_3rd", "0");
                player.SetField("oldweapon", "");
                player.SetField("gamblerstate", "idle");
                player.SetField("incantation", 0);
                player.SetField("zombie_incantation", 0);
                player.SetField("rtd_canroll", 1);

                player.SetClientDvar("phys_gravity_ragdoll", "400");

                CreatePlayerMoneyHud(player);

                Credits(player);

                player.Call("notifyonplayercommand", "tab", "+scores");
                player.Call("notifyonplayercommand", "-tab", "-scores");

                player.Call("notifyonplayercommand", "use3rd", "+actionslot 1");
                player.OnNotify("use3rd", ent =>
                {
                    string value = ent.GetField<string>("inf2_3rd") == "0" ? "1" : "0";
                    ent.SetField("inf2_3rd", value);
                    ent.SetClientDvar("camera_thirdPerson", value);
                    ent.SetClientDvar("camera_thirdPersonCrosshairOffset", "0");
                });

                OnPlayerSpawned(player);

                player.SpawnedPlayer += () => OnPlayerSpawned(player);

            };

            //Fogs
            //AfterDelay(100, () =>
            //{
            //    AfterDelay(100, () =>
            //    {
            //        Entity afermathEnt = Call<Entity>("getent", "mp_global_intermission", "classname");
            //        if (afermathEnt != null)
            //        {
            //            Vector3 up = Call<Vector3>("anglestoup", afermathEnt.GetField<Vector3>("angles"));
            //            Vector3 right = Call<Vector3>("anglestoright", afermathEnt.GetField<Vector3>("angles"));

            //            AfterDelay(100, () =>
            //            {
            //                Entity[] FXs = new Entity[]
            //                {
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(0, 0, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(0, 3000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(0, -3000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(3000, 0, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(3000, 3000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(3000, -3000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(-3000, 0, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(-3000, 3000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(-3000, -3000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(0, 6000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(0, -6000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(6000, 0, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(6000, 6000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(6000, -6000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(-6000, 0, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(-6000, 6000, 700), up, right),
            //                    Call<Entity>("spawnfx", Call<int>("loadfx", "dust/nuke_aftermath_mp"), afermathEnt.Origin + new Vector3(-6000, -6000, 700), up, right)
            //                };

            //                foreach (Entity fx in FXs)
            //                {
            //                    Call("triggerfx", fx);
            //                }
            //            });
            //        }
            //    });
            //});

            Log.Write(LogLevel.Info, "China Magic Infect Mod by A2ON");
        }

        public void OnPlayerSpawned(Entity player)
        {
            if (Utility.GetPlayerTeam(player) == "allies")
            {
                player.SetField("firstweapon", Call<string>("getdvar", "scr_inf2_initweapon"));
                player.SetField("secondweapon", "none");
                player.TakeAllWeapons();
                player.GiveWeapon(Call<string>("getdvar", "scr_inf2_initweapon"));
                player.Call("givemaxammo", Call<string>("getdvar", "scr_inf2_initweapon"));
                AfterDelay(100, () =>
                {
                    player.GiveWeapon("trophy_mp");
                    player.GiveWeapon("claymore_mp");
                    player.Call("givemaxammo", "trophy_mp");
                    player.Call("givemaxammo", "claymore_mp");
                });
                AfterDelay(300, () => player.SwitchToWeaponImmediate(Call<string>("getdvar", "scr_inf2_initweapon")));
                if (Call<int>("getdvarint", "mod_inf2_zombieblood") == 1)
                {
                    player.Call("hide");
                }
                player.SetPerk("specialty_holdbreathwhileads", true, false);
                player.SetPerk("specialty_fastermelee", true, false);
                player.SetPerk("specialty_bulletaccuracy", true, false);
                player.SetPerk("specialty_fastoffhand", true, false);
                player.SetPerk("specialty_quickdraw", true, false);
                player.SetPerk("specialty_longerrange", true, false);

                player.Call("setviewmodel", new Parameter[] { "viewmodel_base_viewhands" });

                player.SetClientDvar("g_compassshowenemies", "0");
            }
            else
            {
                SetModel(player);
                player.SetPerk("specialty_falldamage", true, false);
                player.SetPerk("specialty_lightweight", true, false);
                player.SetPerk("specialty_longersprint", true, false);
                //player.SetPerk("specialty_grenadepulldeath", true, false);
                player.SetPerk("specialty_fastoffhand", true, false);
                player.SetPerk("specialty_fastreload", true, false);
                player.SetPerk("specialty_paint", true, false);
                player.SetPerk("specialty_autospot", true, false);
                player.SetPerk("specialty_stalker", true, false);
                player.SetPerk("specialty_marksman", true, false);
                player.SetPerk("specialty_quickswap", true, false);
                player.SetPerk("specialty_quickdraw", true, false);
                player.SetPerk("specialty_fastermelee", true, false);
                player.SetPerk("specialty_selectivehearing", true, false);
                player.SetPerk("specialty_steadyaimpro", true, false);
                player.SetPerk("specialty_sitrep", true, false);
                player.SetPerk("specialty_detectexplosive", true, false);
                player.SetPerk("specialty_fastsprintrecovery", true, false);
                player.SetPerk("specialty_fastmeleerecovery", true, false);
                player.SetPerk("specialty_bulletpenetration", true, false);
                player.SetPerk("specialty_bulletaccuracy", true, false);

                player.SetClientDvar("g_compassshowenemies", "1");

                if (Call<int>("getteamscore", "axis") <= 1)
                {
                    player.Call("givemaxammo", player.CurrentWeapon);
                    player.SetField("maxhealth", 1000);
                    player.Health = 1000;
                }
                else
                {
                    player.SetField("maxhealth", 100);
                    player.Health = 100;
                }
            }
        }

        public string getSniperEnv(string mapname)
        {
            switch (mapname)
            {
                case "mp_alpha":
                case "mp_bootleg":
                case "mp_exchange":
                case "mp_hardhat":
                case "mp_interchange":
                case "mp_mogadishu":
                case "mp_paris":
                case "mp_plaza2":
                case "mp_underground":
                case "mp_cement":
                case "mp_hillside_ss":
                case "mp_overwatch":
                case "mp_terminal_cls":
                case "mp_aground_ss":
                case "mp_courtyard_ss":
                case "mp_meteora":
                case "mp_morningwood":
                case "mp_qadeem":
                case "mp_crosswalk_ss":
                case "mp_italy":
                case "mp_boardwalk":
                case "mp_roughneck":
                case "mp_nola":
                    return "urban";
                case "mp_dome":
                case "mp_restrepo_ss":
                case "mp_burn_ss":
                case "mp_seatown":
                case "mp_shipbreaker":
                case "mp_moab":
                    return "desert";
                case "mp_bravo":
                case "mp_carbon":
                case "mp_park":
                case "mp_six_ss":
                case "mp_village":
                case "mp_lambeth":
                    return "woodland";
                case "mp_radar":
                    return "arctic";
            }
            return "";
        }
        private string getModelEnv(string mapname)
        {
            switch (mapname)
            {
                case "mp_alpha":
                case "mp_dome":
                case "mp_paris":
                case "mp_plaza2":
                case "mp_terminal_cls":
                case "mp_bootleg":
                case "mp_restrepo_ss":
                case "mp_hillside_ss":
                    return "russian_urban";
                case "mp_exchange":
                case "mp_hardhat":
                case "mp_underground":
                case "mp_cement":
                case "mp_overwatch":
                case "mp_nola":
                case "mp_boardwalk":
                case "mp_roughneck":
                case "mp_crosswalk_ss":
                    return "russian_air";
                case "mp_interchange":
                case "mp_lambeth":
                case "mp_six_ss":
                case "mp_moab":
                case "mp_park":
                    return "russian_woodland";
                case "mp_radar":
                    return "russian_arctic";
                case "mp_seatown":
                case "mp_aground_ss":
                case "mp_burn_ss":
                case "mp_courtyard_ss":
                case "mp_italy":
                case "mp_meteora":
                case "mp_morningwood":
                case "mp_qadeem":
                    return "henchmen";
            }

            return string.Empty;
        }

        private void SetModel(Entity player)
        {
            string[] blockMaps = new string[]
            {
                "mp_seatown",
                "mp_aground_ss",
                "mp_courtyard_ss",
                "mp_italy",
                "mp_meteora",
                "mp_morningwood",
                "mp_hillside_ss",
                "mp_qadeem"
            };
            string[] blockMaps2 = new string[]
            {
                "mp_bravo",
                "mp_carbon",
                "mp_mogadishu",
                "mp_village",
                "mp_shipbreaker",
            };
            string[] blockMaps3 = new string[]
            {
                "mp_seatown",
                "mp_aground_ss",
                "mp_burn_ss",
                "mp_courtyard_ss",
                "mp_italy",
                "mp_meteora",
                "mp_morningwood",
                "mp_hillside_ss",
                "mp_qadeem",
            };

            string str = this.getModelEnv(base.Call<string>("getdvar", new Parameter[] { "mapname" }));
            string str2 = this.getSniperEnv(base.Call<string>("getdvar", new Parameter[] { "mapname" }));

            if (Call<int>("getteamscore", "axis") == 1)
            {
                if (Call<string>("getdvar", new Parameter[] { "mapname" }) == "mp_radar")
                {
                    player.Call("setmodel", new Parameter[] { "mp_body_ally_ghillie_desert_sniper" });
                }
                else
                {
                    if (blockMaps2.Contains(Call<string>("getdvar", new Parameter[] { "mapname" })))
                    {
                        player.Call("setmodel", new Parameter[] { "mp_body_opforce_ghillie_africa_militia_sniper" });
                    }
                    else
                    {
                        player.Call("setmodel", new Parameter[] { "mp_body_ally_ghillie_" + str2 + "_sniper" });
                    }
                }
                player.Call("setviewmodel", new Parameter[] { "viewhands_iw5_ghillie_" + str2 });
            }
            else
            {
                if (blockMaps2.Contains(Call<string>("getdvar", new Parameter[] { "mapname" })))
                {
                    player.Call("setmodel", new Parameter[] { "mp_body_opforce_africa_militia_sniper" });
                }
                else
                {
                    player.Call("setmodel", new Parameter[] { "mp_body_opforce_" + str + "_sniper" });
                }


                if (blockMaps2.Contains(Call<string>("getdvar", new Parameter[] { "mapname" })))
                {
                    player.Call("setviewmodel", new Parameter[] { "viewhands_militia" });
                }
                else if (!blockMaps.Contains(Call<string>("getdvar", new Parameter[] { "mapname" })))
                {
                    player.Call("setviewmodel", new Parameter[] { "viewhands_op_force" });
                }
            }
        }

        private void InitWeapons()
        {
            Weapon._weaponList = new string[] {
                "iw5_44magnum", "iw5_usp45", "iw5_deserteagle", "iw5_mp412", "iw5_p99", "iw5_fnfiveseven", "iw5_fmg9", "iw5_skorpion", "iw5_mp9", "iw5_g18", "iw5_mp5", "iw5_m9", "iw5_p90", "iw5_pp90m1", "iw5_ump45", "iw5_mp7",
                "iw5_ak47", "iw5_m16", "iw5_m4", "iw5_fad", "iw5_acr", "iw5_type95", "iw5_mk14", "iw5_scar", "iw5_g36c", "iw5_cm901", "rpg", "iw5_smaw", "xm25", "riotshield_mp", "m320","stinger_mp","javelin_mp", "iw5_dragunov",
                "iw5_msr", "iw5_barrett", "iw5_rsass", "iw5_as50", "iw5_l96a1", "iw5_ksg", "iw5_1887", "iw5_striker", "iw5_aa12", "iw5_usas12", "iw5_spas12", "iw5_m60", "iw5_mk46", "iw5_pecheneg", "iw5_sa80", "iw5_mg36"
             };
        }

        private void CreatePlayerMoneyHud(Entity player)
        {
            HudElem hud = HudElem.CreateFontString(player, "hudbig", 1f);
            hud.SetPoint("right", "right", -50, 100);
            hud.HideWhenInMenu = true;
            player.OnInterval(100, delegate (Entity ent)
            {
                if (Utility.GetPlayerTeam(player) == "allies")
                {
                    hud.SetText("^3$: ^7" + player.GetField<int>("inf2_money"));
                }
                else
                {
                    hud.SetText("");
                    return false;
                }
                return true;
            });
        }

        private void Credits(Entity ent)
        {
            HudElem credits = HudElem.CreateFontString(ent, "hudbig", 1.0f);
            credits.SetPoint("CENTER", "BOTTOM", 0, -70);
            credits.Call("settext", "China Magic Infect 2015");
            credits.Alpha = 0f;
            credits.SetField("glowcolor", new Vector3(1f, 0.5f, 1f));
            credits.GlowAlpha = 1f;

            HudElem credits2 = HudElem.CreateFontString(ent, "hudbig", 0.6f);
            credits2.SetPoint("CENTER", "BOTTOM", 0, -90);
            credits2.Call("settext", "Created by A2ON. Vesion 1.1.3");
            credits2.Alpha = 0f;
            credits2.SetField("glowcolor", new Vector3(1f, 0.5f, 1f));
            credits2.GlowAlpha = 1f;

            ent.OnNotify("tab", entity =>
            {
                credits.Alpha = 1f;
                credits2.Alpha = 1f;
            });

            ent.OnNotify("-tab", entity =>
            {
                credits.Alpha = 0f;
                credits2.Alpha = 0f;
            });
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
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
            if (Utility.GetPlayerTeam(attacker) == "allies")
            {
                if (Call<int>("getdvarint", "mod_inf2_instakill") == 1)
                {
                    player.Health = 3;
                    return;
                }
                else
                {
                    if (weapon.StartsWith("iw5_msr_mp") || weapon.StartsWith("iw5_l96a1_mp"))
                    {
                        player.Health = 3;
                        return;
                    }
                    if (mod == "MOD_MELEE")
                    {
                        player.Health = 3;
                        return;
                    }
                }
            }
        }

        public string GetTeamVoice()
        {
            if (Call<string>("getdvar", "g_teamalies").ToLower().Contains("delta"))
            {
                return "US";
            }
            if (Call<string>("getdvar", "g_teamalies").ToLower().Contains("pmc"))
            {
                return "PC";
            }
            if (Call<string>("getdvar", "g_teamalies").ToLower().Contains("sas"))
            {
                return "UK";
            }
            if (Call<string>("getdvar", "g_teamalies").ToLower().Contains("gign"))
            {
                return "FR";
            }
            return "";
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
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
            if (Utility.GetPlayerTeam(attacker) == "allies")
            {
                if (Utility.GetPlayerTeam(player) == "axis")
                {
                    if (Call<int>("getdvarint", "mod_inf2_doublepoint") == 1)
                    {
                        attacker.SetField("inf2_money", attacker.GetField<int>("inf2_money") + 200);
                    }
                    else
                    {
                        attacker.SetField("inf2_money", attacker.GetField<int>("inf2_money") + 100);
                    }
                    attacker.Call("playlocalsound", GetTeamVoice()+"_1mc_kill_confirmed");
                    if (player.GetField<int>("zombie_incantation") == 1)
                    {
                        attacker.Health = -1;
                        AfterDelay(10, () =>
                        {
                            Call("RadiusDamage", new Parameter[] { player.Origin, 500, 500, 500, attacker, "MOD_EXPLOSIVE", "nuke_mp" });
                            Call("playfx", new Parameter[] { this.Call<int>("loadfx", new Parameter[] { "explosions/tanker_explosion" }), player.Origin });
                            player.Call("playsound", new Parameter[] { "cobra_helicopter_crash" });
                            player.Call("iprintlnbold", new Parameter[] { "^0Incantation!" });
                        });
                        AfterDelay(100, () => attacker.Health = attacker.GetField<int>("maxhealth"));
                    }
                }
            }
            else
            {
                if (Utility.GetPlayerTeam(attacker) == "axis" && Utility.GetPlayerTeam(player) == "allies")
                {
                    attacker.Call("playlocalsound", GetTeamVoice() + "_1mc_kill_confirmed");
                    if (player.GetField<int>("incantation") == 1)
                    {
                        attacker.Health = -1;
                        AfterDelay(10, () =>
                        {
                            Call("RadiusDamage", new Parameter[] { player.Origin, 500, 500, 500, attacker, "MOD_EXPLOSIVE", "nuke_mp" });
                            Call("playfx", new Parameter[] { this.Call<int>("loadfx", new Parameter[] { "explosions/tanker_explosion" }), player.Origin });
                            player.Call("playsound", new Parameter[] { "cobra_helicopter_crash" });
                            player.Call("iprintlnbold", new Parameter[] { "^0Incantation!" });
                        });
                        AfterDelay(100, () => attacker.Health = attacker.GetField<int>("maxhealth"));
                    }
                    if (Call<int>("getdvarint", "mod_inf2_zombieblood") == 1)
                    {
                        player.Call("show");
                    }
                }
            }
        }
    }
}

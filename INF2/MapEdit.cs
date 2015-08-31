using InfinityScript;
using System;
using System.Collections.Generic;
using System.IO;

namespace INF2
{
    public class MapEdit : BaseScript
    {
        private Entity _airdropCollision;
        private string mapname;
        private string _mapname;
        private Random _rng;
        private int curObjID;
        public static List<Entity> usables = new List<Entity>();

        public MapEdit()
        {
            Call("precacheShader", "hud_icon_cm901");
            _rng = new Random();
            Entity entity = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            _airdropCollision = Call<Entity>("getent", new Parameter[] { entity.GetField<string>("target"), "targetname" });
            //_airdropCollision = Call<Entity>("getentbynum", -1).GetField<Entity>("airDropCrateCollision");
            _mapname = Call<string>("getdvar", new Parameter[] { "mapname" });
            mapname = randomFile(_mapname);
            //mapname = "scripts\\inf2-maps\\" + _mapname + ".txt";
            Call("precachemodel", new Parameter[] { getAlliesFlagModel(_mapname) });
            Call("precachemodel", new Parameter[] { "prop_flag_neutral" });
            Call("precachemodel", new Parameter[] { "com_plasticcase_green_big" });
            Call("precacheshader", new Parameter[] { "waypoint_escort" });
            Call("precacheshader", new Parameter[] { "compass_waypoint_target" });
            Call("precacheshader", new Parameter[] { "compass_waypoint_bomb" });
            Call("precachemodel", new Parameter[] { "weapon_scavenger_grenadebag" });
            Call("precachemodel", new Parameter[] { "weapon_oma_pack" });
            if (File.Exists(mapname))
            {
                loadMapEdit(mapname);
            }
            PlayerConnected += new Action<Entity>(player =>
            {
                player.SetField("attackeddoor", 0);
                player.SetField("repairsleft", 0);
                player.Call("notifyonplayercommand", new Parameter[] { "triggeruse", "+activate" });
                player.OnNotify("triggeruse", (Action<Entity>)(ent => HandleUseables(player)));
                UsablesHud(player);
            });
        }

        public void HandleUseables(Entity player)
        {
            foreach (Entity entity in usables)
            {
                if (player.Origin.DistanceTo(entity.Origin) < ((float)entity.GetField<int>("range")))
                {
                    switch (entity.GetField<string>("usabletype"))
                    {
                        case "door":
                            usedDoor(entity, player);
                            break;
                        case "zipline":
                            usedZipline(entity, player);
                            break;
                        case "mystery":
                            usedMysteryBox(entity, player);
                            break;
                        case "ammo":
                            usedAmmoBox(player);
                            break;
                        case "beacon":
                            usedAirstrikeBeacon(entity, player);
                            break;
                        case "gambler":
                            usedGambler(player);
                            break;
                        case "teleporter":
                            usedTeleporter(entity, player);
                            break;
                    }
                }
            }
        }

        public void MakeUsable(Entity ent, string type, int range)
        {
            ent.SetField("usabletype", type);
            ent.SetField("range", range);
            usables.Add(ent);
        }

        public void UsablesHud(Entity player)
        {
            HudElem message = HudElem.CreateFontString(player, "big", 1.5f);
            message.SetPoint("CENTER", "CENTER", 1, 115);
            message.Alpha = 0.65f;
            OnInterval(100, delegate
            {
                bool flag = false;
                foreach (Entity entity in usables)
                {
                    if (player.Origin.DistanceTo(entity.Origin) >= ((float)entity.GetField<int>("range")))
                    {
                        continue;
                    }
                    switch (entity.GetField<string>("usabletype"))
                    {
                        case "door":
                            message.SetText(getDoorText(entity, player));
                            break;

                        case "zipline":
                            message.SetText(getZiplineText(entity, player));
                            break;

                        case "ammo":
                            message.SetText(getAmmoBoxText(entity, player));
                            break;

                        case "gambler":
                            message.SetText(getGamblerText(entity, player));
                            break;

                        case "beacon":
                            message.SetText(getBeaconText(entity, player));
                            break;

                        case "mystery":
                            message.SetText(getMysteryText(entity, player));
                            break;

                        case "teleporter":
                            message.SetText(getTeleporterText(entity, player));
                            break;

                        default:
                            message.SetText("");
                            break;
                    }
                    flag = true;
                }
                if (!flag)
                {
                    message.SetText("");
                }
                return true;
            });
        }

        public string getDoorText(Entity door, Entity player)
        {
            int field = door.GetField<int>("hp");
            int num2 = door.GetField<int>("maxhp");
            if (!(Utility.GetPlayerTeam(player) == "allies"))
            {
                string str2;
                if ((Utility.GetPlayerTeam(player) == "axis") && ((str2 = door.GetField<string>("state")) != null))
                {
                    if (str2 == "open")
                    {
                        return "Door is Open.";
                    }
                    if (str2 == "close")
                    {
                        return "Press ^3[{+activate}] ^7to attack the door.";
                    }
                    if (str2 == "broken")
                    {
                        return "^1Door is Broken.";
                    }
                }
            }
            else
            {
                switch (door.GetField<string>("state"))
                {
                    case "open":
                        return string.Concat(new object[] { "Door is Open. Press ^3[{+activate}] ^7to close it. (", field, "/", num2, ")" });

                    case "close":
                        return string.Concat(new object[] { "Door is Closed. Press ^3[{+activate}] ^7to open it. (", field, "/", num2, ")" });

                    case "broken":
                        return "^1Door is Broken.";
                }
            }
            return "";
        }

        public string getGamblerText(Entity box, Entity player)
        {
            if (!(Utility.GetPlayerTeam(player) == "allies"))
            {
                return "";
            }
            if (player.GetField<string>("gamblerstate") == "idle")
            {
                return "Press ^3[{+activate}] ^7to use Gambler [Cost: ^2$^3500^7].";
            }
            return "";
        }

        public string getAmmoBoxText(Entity box, Entity player)
        {
            if (!(Utility.GetPlayerTeam(player) == "allies"))
            {
                return "";
            }
            return "Press ^3[{+activate}] ^7to by Ammo. [Cost: ^2$^3300^7]";
        }

        public string getMysteryText(Entity box, Entity player)
        {
            if (!(Utility.GetPlayerTeam(player) == "allies"))
            {
                return "";
            }
            if (!box.GetField<string>("state").Equals("waiting"))
            {
                return "Press ^3[{+activate}] ^7to use Mystery Box. [Cost: ^2$^3500^7]";
            }
            if (box.GetField<string>("player").Equals(player.Name) && box.GetField<string>("state").Equals("waiting"))
            {
                return "Press ^3[{+activate}] ^7to trade you weapon.";
            }
            if (box.GetField<string>("state") == "using")
            {
                return "";
            }
            return "";
        }

        public string getZiplineText(Entity box, Entity player)
        {
            if (box.GetField<string>("state") == "using")
            {
                return "";
            }
            return "Press ^3[{+activate}] ^7to to use Zipline.";
        }

        public string getBeaconText(Entity box, Entity player)
        {
            if (box.GetField<string>("state") == "waiting")
            {
                if (Utility.GetPlayerTeam(player) == "allies")
                {
                    return "Press ^3[{+activate}] ^7to call Designated Airstrike. [Cost: ^2$^3100^7]";
                }
                else
                {
                    return "Press ^3[{+activate}] ^7to call Designated Airstrike.";
                }
            }
            if (box.GetField<string>("state") == "idle")
            {
                return "^1Designated airstrike is offline.";
            }
            return "";
        }
        public string getTeleporterText(Entity box, Entity player)
        {
            if (Utility.GetPlayerTeam(player) == "allies")
            {
                return "Press ^3[{+activate}] ^7to use Teleporter. [Cost: ^2$^3500^7]";
            }
            else
            {
                return "Press ^3[{+activate}] ^7to use Teleporter.";
            }
        }

        public void CreateAirstrikeBeacon(Vector3 origin, Vector3 angle)
        {
            Call("precacheshader", new Parameter[] { "dpad_killstreak_precision_airstrike" });
            Entity ent = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            ent.Call("setmodel", new Parameter[] { "com_plasticcase_enemy" });
            ent.SetField("angles", new Parameter(angle));
            ent.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
            ent.SetField("state", "waiting");
            int num = 0x1f - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_icon", new Parameter[] { num, "dpad_killstreak_precision_airstrike" });

            MakeUsable(ent, "beacon", 50);
        }

        public void CreateAmmoBox(Vector3 origin, Vector3 angle)
        {
            Entity box = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            box.Call("setmodel", new Parameter[] { "com_plasticcase_friendly" });
            box.SetField("angles", new Parameter(angle));
            box.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
            CreateBoxShader(box, "waypoint_ammo_friendly");
            Vector3 v = origin;
            v.Z += 17f;
            Entity laptop = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(v) });
            laptop.Call("setmodel", new Parameter[] { "com_laptop_2_open" });
            LaptopRotate(laptop);
            int num = 0x1f - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_team", new Parameter[] { num, "allies" });
            Call("objective_icon", new Parameter[] { num, "waypoint_ammo_friendly" });

            MakeUsable(box, "ammo", 50);
        }

        public void CreateBoxShader(Entity box, string shader)
        {
            HudElem elem = HudElem.NewTeamHudElem("allies");
            elem.SetShader(shader, 20, 20);
            elem.X = box.Origin.X;
            elem.Y = box.Origin.Y;
            elem.Z = box.Origin.Z + 40f;
            elem.Call("SetWayPoint", new Parameter[] { 1, 1 });
        }

        public void CreateDoor(Vector3 open, Vector3 close, Vector3 angle, int size, int height, int hp, int range)
        {
            double num = ((size / 2) - 0.5) * -1.0;
            Entity ent = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(open) });
            for (int i = 0; i < size; i++)
            {
                Entity entity2 = spawnCrate(open + ((Vector3)(new Vector3(0f, 30f, 0f) * ((float)num))), new Vector3(0f, 0f, 0f));
                entity2.Call("setModel", new Parameter[] { "com_plasticcase_enemy" });
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { ent });
                for (int j = 1; j < height; j++)
                {
                    Entity entity3 = spawnCrate((open + ((Vector3)(new Vector3(0f, 30f, 0f) * ((float)num)))) - (new Vector3(70f, 0f, 0f) * j), new Vector3(0f, 0f, 0f));
                    entity3.Call("setModel", new Parameter[] { "com_plasticcase_enemy" });
                    entity3.Call("enablelinkto", new Parameter[0]);
                    entity3.Call("linkto", new Parameter[] { ent });
                }
                num++;
            }
            ent.SetField("angles", new Parameter(angle));
            ent.SetField("state", "open");
            ent.SetField("hp", hp);
            ent.SetField("maxhp", hp);
            ent.SetField("open", new Parameter(open));
            ent.SetField("close", new Parameter(close));

            MakeUsable(ent, "door", range);
        }

        public void CreateElevator(Vector3 enter, Vector3 exit)
        {
            Entity flag = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(enter) });
            flag.Call("setModel", new Parameter[] { getAlliesFlagModel(_mapname) });
            Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(exit) }).Call("setModel", new Parameter[] { "prop_flag_neutral" });
            CreateFlagShader(flag);
            int num = 0x1f - curObjID++;
            Call(0x1af, new Parameter[] { num, "active" });
            Call(0x1b3, new Parameter[] { num, new Parameter(flag.Origin) });
            Call(0x1b2, new Parameter[] { num, "waypoint_escort" });
            OnInterval(100, delegate
            {
                foreach (Entity entity in Utility.getPlayerList())
                {
                    if (entity.Origin.DistanceTo(enter) <= 50f)
                    {
                        entity.Call("setorigin", exit);
                    }
                }
                return flag != null;
            });
        }

        public void CreateFlagShader(Entity flag)
        {
            HudElem elem = HudElem.NewHudElem();
            elem.SetShader("waypoint_escort", 20, 20);
            elem.Alpha = 0.6f;
            elem.X = flag.Origin.X;
            elem.Y = flag.Origin.Y;
            elem.Z = flag.Origin.Z + 100f;
            elem.Call("SetWayPoint", new Parameter[] { 1, 1 });
        }

        public Entity CreateFloor(Vector3 corner1, Vector3 corner2)
        {
            float num = corner1.X - corner2.X;
            if (num < 0f)
            {
                num *= -1f;
            }
            float num2 = corner1.Y - corner2.Y;
            if (num2 < 0f)
            {
                num2 *= -1f;
            }
            int num3 = (int)Math.Round((double)(num / 50f), 0);
            int num4 = (int)Math.Round((double)(num2 / 30f), 0);
            Vector3 vector = corner2 - corner1;
            Vector3 vector2 = new Vector3(vector.X / ((float)num3), vector.Y / ((float)num4), 0f);
            Entity entity = Call<Entity>("spawn", new Parameter[] { "script_origin", new Parameter(new Vector3((corner1.X + corner2.X) / 2f, (corner1.Y + corner2.Y) / 2f, corner1.Z)) });
            for (int i = 0; i < num3; i++)
            {
                for (int j = 0; j < num4; j++)
                {
                    Entity entity2 = spawnCrate((corner1 + (new Vector3(vector2.X, 0f, 0f) * i)) + (new Vector3(0f, vector2.Y, 0f) * j), new Vector3(0f, 0f, 0f));
                    entity2.Call("enablelinkto", new Parameter[0]);
                    entity2.Call("linkto", new Parameter[] { entity });
                }
            }
            return entity;
        }

        public void CreateGambler(Vector3 origin, Vector3 angle)
        {
            Entity box = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            box.Call("setmodel", new Parameter[] { "com_plasticcase_trap_friendly" });
            box.SetField("angles", new Parameter(angle));
            box.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
            CreateBoxShader(box, "cardicon_8ball");
            Vector3 v = origin;
            v.Z += 17f;
            Entity laptop = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(v) });
            laptop.Call("setmodel", new Parameter[] { "com_laptop_2_open" });
            LaptopRotate(laptop);
            int num = 0x1f - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_team", new Parameter[] { num, "allies" });
            Call("objective_icon", new Parameter[] { num, "cardicon_8ball" });

            MakeUsable(box, "gambler", 50);
        }

        //public void CreateJuggernog(Vector3 origin, Vector3 angle)
        //{
        //    Entity box = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
        //    box.Call("setmodel", new Parameter[] { "com_plasticcase_friendly" });
        //    box.SetField("angles", new Parameter(angle));
        //    box.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
        //    CreateBoxShader(box, "cardicon_8ball");
        //    Vector3 v = origin;
        //    v.Z += 17f;
        //    Entity laptop = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(v) });
        //    laptop.Call("setmodel", new Parameter[] { "com_laptop_2_open" });
        //    LaptopRotate(laptop);
        //    int num = 0x1f - curObjID++;
        //    Call("objective_state", new Parameter[] { num, "active" });
        //    Call("objective_position", new Parameter[] { num, new Parameter(origin) });
        //    Call("objective_team", new Parameter[] { num, "allies" });
        //    Call("objective_icon", new Parameter[] { num, "cardicon_8ball" });

        //    MakeUsable(box, "juggernog", 50);
        //}

        public void CreateHiddenTP(Vector3 enter, Vector3 exit)
        {
            Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(enter) }).Call("setModel", new Parameter[] { "weapon_scavenger_grenadebag" });
            Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(exit) }).Call("setModel", new Parameter[] { "com_deploy_ballistic_vest_friend_viewmodel" });
            OnInterval(100, delegate
            {
                foreach (Entity entity in Utility.getPlayerList())
                {
                    if (entity.Origin.DistanceTo(enter) <= 50f)
                    {
                        entity.Call("setorigin", exit);
                    }
                }
                return true;
            });
        }

        public void CreateRamp(Vector3 top, Vector3 bottom)
        {
            int num2 = (int)Math.Ceiling((double)(top.DistanceTo(bottom) / 30f));
            Vector3 vector = new Vector3((top.X - bottom.X) / ((float)num2), (top.Y - bottom.Y) / ((float)num2), (top.Z - bottom.Z) / ((float)num2));
            Vector3 vector2 = base.Call<Vector3>("vectortoangles", new Parameter[] { new Parameter(top - bottom) });
            Vector3 angles = new Vector3(vector2.Z, vector2.Y + 90f, vector2.X);
            for (int i = 0; i <= num2; i++)
            {
                spawnCrateTrap(bottom + (vector * i), angles);
            }
        }


        public void createSpawn(string type, Vector3 origin, Vector3 angle)
        {
            Call<Entity>("spawn", new Parameter[] { type, new Parameter(origin) }).SetField("angles", new Parameter(angle));
        }

        public void CreateTurret(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "sentry_minigun_mp" });
            entity.Call("setmodel", new Parameter[] { "weapon_minigun" });
            entity.SetField("angles", angles);
            AfterDelay(300, () => entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use turret."));
        }
        public void CreateTurret2(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "turret_minigun_mp" });
            entity.Call("setmodel", new Parameter[] { "weapon_minigun" });
            entity.SetField("angles", angles);
            AfterDelay(300, () => entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use turret."));
        }

        public void CreateSentry(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "sentry_minigun_mp" });
            entity.Call("setmodel", new Parameter[] { "sentry_minigun_weak" });
            entity.SetField("angles", angles);
            AfterDelay(300, () => entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use sentry gun."));
        }

        public void CreateSentry2(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "remote_turret_mp" });
            entity.Call("setmodel", new Parameter[] { "mp_remote_turret" });
            entity.SetField("angles", angles);
            AfterDelay(300, () => entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use remote sentry."));
            entity.Call("SetBottomArc", 30);
        }

        public void CreateGL(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "ugv_gl_turret_mp" });
            entity.Call("setmodel", new Parameter[] { "sentry_grenade_launcher" });
            entity.SetField("angles", angles);
            AfterDelay(300, () => entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use grenade launcher."));
            entity.SetField("player", "");
            entity.SetField("state", "idle");
            entity.SetField("statefire", "manual");
        }

        public void CreateSAM(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "sam_mp" });
            entity.Call("setmodel", new Parameter[] { "mp_sam_turret" });
            entity.SetField("angles", angles);
            AfterDelay(300, () => entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use SAM turret."));
            entity.SetField("player", "");
            entity.SetField("state", "idle");
            entity.SetField("statefire", "manual");
        }

        public Entity CreateWall(Vector3 start, Vector3 end)
        {
            float num = new Vector3(start.X, start.Y, 0f).DistanceTo(new Vector3(end.X, end.Y, 0f));
            float num2 = new Vector3(0f, 0f, start.Z).DistanceTo(new Vector3(0f, 0f, end.Z));
            int num3 = (int)Math.Round((double)(num / 55f), 0);
            int num4 = (int)Math.Round((double)(num2 / 30f), 0);
            Vector3 v = end - start;
            Vector3 vector2 = new Vector3(v.X / ((float)num3), v.Y / ((float)num3), v.Z / ((float)num4));
            float x = vector2.X / 4f;
            float y = vector2.Y / 4f;
            Vector3 angles = base.Call<Vector3>("vectortoangles", new Parameter[] { new Parameter(v) });
            angles = new Vector3(0f, angles.Y, 90f);
            Entity entity = Call<Entity>("spawn", new Parameter[] { "script_origin", new Parameter(new Vector3((start.X + end.X) / 2f, (start.Y + end.Y) / 2f, (start.Z + end.Z) / 2f)) });
            for (int i = 0; i < num4; i++)
            {
                Entity entity2 = spawnCrateTrap((start + new Vector3(x, y, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { entity });
                for (int j = 0; j < num3; j++)
                {
                    entity2 = spawnCrateTrap(((start + (new Vector3(vector2.X, vector2.Y, 0f) * j)) + new Vector3(0f, 0f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                    entity2.Call("enablelinkto", new Parameter[0]);
                    entity2.Call("linkto", new Parameter[] { entity });
                }
                entity2 = spawnCrateTrap((new Vector3(end.X, end.Y, start.Z) + new Vector3(x * -1f, y * -1f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { entity });
            }
            return entity;
        }

        public Entity CreateWall2(Vector3 start, Vector3 end)
        {
            float num = new Vector3(start.X, start.Y, 0f).DistanceTo(new Vector3(end.X, end.Y, 0f));
            float num2 = new Vector3(0f, 0f, start.Z).DistanceTo(new Vector3(0f, 0f, end.Z));
            int num3 = (int)Math.Round((double)(num / 55f), 0);
            int num4 = (int)Math.Round((double)(num2 / 30f), 0);
            Vector3 v = end - start;
            Vector3 vector2 = new Vector3(v.X / ((float)num3), v.Y / ((float)num3), v.Z / ((float)num4));
            float x = vector2.X / 4f;
            float y = vector2.Y / 4f;
            Vector3 angles = base.Call<Vector3>("vectortoangles", new Parameter[] { new Parameter(v) });
            angles = new Vector3(0f, angles.Y, 90f);
            Entity entity = Call<Entity>("spawn", new Parameter[] { "script_origin", new Parameter(new Vector3((start.X + end.X) / 2f, (start.Y + end.Y) / 2f, (start.Z + end.Z) / 2f)) });
            for (int i = 0; i < num4; i++)
            {
                Entity entity2 = spawnCrateRed((start + new Vector3(x, y, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { entity });
                for (int j = 0; j < num3; j++)
                {
                    entity2 = spawnCrateRed(((start + (new Vector3(vector2.X, vector2.Y, 0f) * j)) + new Vector3(0f, 0f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                    entity2.Call("enablelinkto", new Parameter[0]);
                    entity2.Call("linkto", new Parameter[] { entity });
                }
                entity2 = spawnCrateRed((new Vector3(end.X, end.Y, start.Z) + new Vector3(x * -1f, y * -1f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { entity });
            }
            return entity;
        }

        public void CreateZipline(Vector3 origin, Vector3 angle, Vector3 endorigin)
        {
            Call("precacheshader", new Parameter[] { "hudicon_neutral" });
            Entity ent = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            ent.Call("setmodel", new Parameter[] { "com_plasticcase_friendly" });
            ent.SetField("angles", new Parameter(angle));
            ent.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
            ent.SetField("state", "idle");
            ent.SetField("endorigin", endorigin);
            int num = 0x1f - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_icon", new Parameter[] { num, "hudicon_neutral" });

            MakeUsable(ent, "zipline", 50);
        }

        public void CreateMysteryBox(Vector3 origin, Vector3 angle)
        {
            Entity box = Call<Entity>("spawn", "script_model", origin);
            box.Call("setmodel", "com_plasticcase_friendly");
            box.SetField("angles", angle);
            box.Call("clonebrushmodeltoscriptmodel", _airdropCollision);
            box.SetField("state", "idle");
            box.SetField("weapon", "");
            box.SetField("player", "");
            box.SetField("destroyed", 0);
            box.SetField("weaponent", -1);
            CreateBoxShader(box, "hud_icon_cm901");
            int num = 31 - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_team", new Parameter[] { num, "allies" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_icon", new Parameter[] { num, "hud_icon_cm901" });

            MakeUsable(box, "mystery", 50);
        }

        public void CreateTeleporter(Vector3 origin, Vector3 angle, Vector3 endorigin)
        {
            Call("precacheshader", new Parameter[] { "hudicon_neutral" });
            Entity box = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            box.Call("setmodel", new Parameter[] { "com_plasticcase_friendly" });
            box.SetField("angles", new Parameter(angle));
            box.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
            box.SetField("endorigin", endorigin);
            Vector3 v = origin;
            v.Z += 17f;
            Entity laptop = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(v) });
            laptop.Call("setmodel", new Parameter[] { "com_laptop_2_open" });
            LaptopRotate(laptop);
            HudElem elem = HudElem.NewHudElem();
            elem.SetShader("hudicon_neutral", 20, 20);
            elem.X = box.Origin.X;
            elem.Y = box.Origin.Y;
            elem.Z = box.Origin.Z + 40f;
            elem.Call("SetWayPoint", new Parameter[] { 1, 1 });
            int num = 0x1f - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_icon", new Parameter[] { num, "hudicon_neutral" });

            MakeUsable(box, "teleporter", 50);
        }

        public void usedGambler(Entity player)
        {
            if (player.IsAlive && Utility.GetPlayerTeam(player) == "allies")
            {
                if (player.GetField<int>("inf2_money") >= 500)
                {
                    if (player.GetField<string>("gamblerstate") == "idle")
                    {
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") - 500);
                        player.SetField("gamblerstate", "using");
                        GamblerThink(player);
                    }
                }
                else
                {
                    player.Call("iprintln", "^1Gambler need $500");
                }
            }
        }

        public void GamblerThink(Entity player)
        {
            player.Call("iprintlnbold", new Parameter[] { "^210" });
            player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" });
            AfterDelay(1000, () => player.Call("iprintlnbold", new Parameter[] { "^29" }));
            AfterDelay(1000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(2000, () => player.Call("iprintlnbold", new Parameter[] { "^28" }));
            AfterDelay(2000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(3000, () => player.Call("iprintlnbold", new Parameter[] { "^27" }));
            AfterDelay(3000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(4000, () => player.Call("iprintlnbold", new Parameter[] { "^26" }));
            AfterDelay(4000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(5000, () => player.Call("iprintlnbold", new Parameter[] { "^25" }));
            AfterDelay(5000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(6000, () => player.Call("iprintlnbold", new Parameter[] { "^24" }));
            AfterDelay(6000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(7000, () => player.Call("iprintlnbold", new Parameter[] { "^23" }));
            AfterDelay(7000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(8000, () => player.Call("iprintlnbold", new Parameter[] { "^22" }));
            AfterDelay(8000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(9000, () => player.Call("iprintlnbold", new Parameter[] { "^21" }));
            AfterDelay(9000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            base.AfterDelay(10000, delegate
            {
                int? rng = new int?(new Random().Next(24));
                switch (rng.Value)
                {
                    case 0:
                        player.Call("iprintlnbold", new Parameter[] { "^1You win nothing." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 1:
                        player.Call("iprintlnbold", new Parameter[] { "^2You win $500." });
                        player.Call("playlocalsound", new Parameter[] { "new_perk_unlocks" });
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") + 500);
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 2:
                        player.Call("iprintlnbold", new Parameter[] { "^2You win $1000." });
                        player.Call("playlocalsound", new Parameter[] { "new_perk_unlocks" });
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") + 0x3e8);
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 3:
                        player.Call("iprintlnbold", new Parameter[] { "^2You win $2000." });
                        player.Call("playlocalsound", new Parameter[] { "new_perk_unlocks" });
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") + 0x7d0);
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 4:
                        player.Call("iprintlnbold", new Parameter[] { "^1You lose $500." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") - 500);
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 5:
                        foreach (var item in Utility.getPlayerList())
                        {
                            if (Utility.GetPlayerTeam(item) == "allies")
                            {
                                item.Call("iprintlnbold", new Parameter[] { "^0Surprise!" });
                                item.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                                item.SetField("inf2_money", 0);
                            }
                        }
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 6:
                        player.Call("iprintlnbold", new Parameter[] { "^1You lose all money." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                        player.SetField("inf2_money", 0);
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 7:
                        player.Call("iprintlnbold", new Parameter[] { "^2You win riotshield in your back." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_start" });
                        player.Call("attachshieldmodel", new Parameter[] { "weapon_riot_shield_mp", "tag_shield_back" });
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 8:
                        player.Call("iprintlnbold", new Parameter[] { "^1Airstrike in your location." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                        player.Call("playsound", new Parameter[] { "US_1mc_use_airstrike" });
                        Vector3 loc = player.Origin;
                        Call("magicbullet", new Parameter[] { "javelin_mp", new Vector3(loc.X, loc.Y, loc.Z + 25000f), loc, player });
                        AfterDelay(0, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                        AfterDelay(400, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                        AfterDelay(800, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                        AfterDelay(1200, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                        AfterDelay(1600, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                        AfterDelay(2000, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                        AfterDelay(2400, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                        AfterDelay(2800, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                        player.SetField("gamblerstate", "idle");
                        return;
                    case 9:
                        player.Call("iprintlnbold", new Parameter[] { "^2You win $10000." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_start" });
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") + 0x2710);
                        player.SetField("gamblerstate", "idle");
                        return;
                    case 10:
                        player.Call("iprintlnbold", new Parameter[] { "^1You live or die in 5 second." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                        AfterDelay(1000, () => player.Call("iprintlnbold", new Parameter[] { "^15" }));
                        AfterDelay(1000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(2000, () => player.Call("iprintlnbold", new Parameter[] { "^14" }));
                        AfterDelay(2000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(3000, () => player.Call("iprintlnbold", new Parameter[] { "^13" }));
                        AfterDelay(3000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(4000, () => player.Call("iprintlnbold", new Parameter[] { "^12" }));
                        AfterDelay(4000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(5000, () => player.Call("iprintlnbold", new Parameter[] { "^11" }));
                        AfterDelay(5000, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(6000, () =>
                        {
                            rng = new int?(new Random().Next(2));
                            switch (rng.Value)
                            {
                                case 0:
                                    player.Call("iprintlnbold", new Parameter[] { "^2You live!" });
                                    return;

                                case 1:
                                    player.Call("iprintlnbold", new Parameter[] { "^1You die!" });
                                    Call("playfx", new Parameter[] { Call<int>("loadfx", new Parameter[] { "props/barrelexp" }), player.Call<Vector3>("gettagorigin", new Parameter[] { "j_head" }) });
                                    player.Call("playsound", new Parameter[] { "nuke_explosion" });
                                    player.Call("suicide", new Parameter[0]);
                                    return;
                            }
                            player.SetField("gamblerstate", "idle");
                        });
                        return;
                    case 11:
                        player.Call("iprintlnbold", new Parameter[] { "^3Grab all players money!" });
                        player.Call("playlocalsound", new Parameter[] { "new_perk_unlocks" });
                        AfterDelay(100, () =>
                        {
                            int money = 0;
                            foreach (var item in Utility.getPlayerList())
                            {
                                if (Utility.GetPlayerTeam(item) == "allies" && item != player)
                                {
                                    money = money + item.GetField<int>("inf2_money");
                                    item.SetField("inf2_money", 0);
                                    item.Call("iprintlnbold", new Parameter[] { "^1Player: " + player.Name + " grabed you all money!" });
                                    item.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                                }
                            }
                            player.SetField("inf2_money", player.GetField<int>("inf2_money") + money);
                        });
                        player.SetField("gamblerstate", "idle");
                        return;
                    case 12:
                        player.Call("iprintlnbold", new Parameter[] { "^1Gambler restart." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                        AfterDelay(1000, () => GamblerThink(player));
                        return;
                    case 13:
                        player.Call("iprintlnbold", new Parameter[] { "^2Wallhack for 1 min." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_start" });
                        player.Call("thermalvisionfofoverlayon", new Parameter[0]);
                        AfterDelay(60000, () =>
                        {
                            player.Call("iprintlnbold", new Parameter[] { "Wallhack off." });
                            player.Call("thermalvisionfofoverlayoff", new Parameter[0]);
                        });
                        player.SetField("gamblerstate", "idle");
                        return;

                    case 14:
                        player.Call("iprintlnbold", new Parameter[] { "^1Incantation." });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                        player.SetField("incantation", 1);
                        player.SetField("gamblerstate", "idle");
                        return;
                    case 15:
                        player.Call("iprintlnbold", new Parameter[] { "^1Airstrike in your location." });
                        player.Call("playlocalsound", new Parameter[] { "US_1mc_use_airstrike" });
                        Call("playsound", new Parameter[] { "nuke_explosion" });
                        Vector3 loc2 = player.Origin;
                        Call("magicbullet", new Parameter[] { "javelin_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 25000f), loc2, player });
                        AfterDelay(0, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 20000f), Utility.rngVec(loc2, 100), player }));
                        AfterDelay(400, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 20000f), Utility.rngVec(loc2, 100), player }));
                        AfterDelay(800, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 20000f), Utility.rngVec(loc2, 100), player }));
                        AfterDelay(1200, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 20000f), Utility.rngVec(loc2, 100), player }));
                        AfterDelay(1600, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 20000f), Utility.rngVec(loc2, 100), player }));
                        AfterDelay(2000, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 20000f), Utility.rngVec(loc2, 100), player }));
                        AfterDelay(2400, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 20000f), Utility.rngVec(loc2, 100), player }));
                        AfterDelay(2800, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc2.X, loc2.Y, loc2.Z + 20000f), Utility.rngVec(loc2, 100), player }));
                        player.SetField("gamblerstate", "idle");
                        return;
                    case 16:
                        player.Call("iprintlnbold", new Parameter[] { "^3You are local tyrant!" });
                        player.Call("playlocalsound", new Parameter[] { "new_perk_unlocks" });
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") + 500);
                        foreach (var item in Utility.getPlayerList())
                        {
                            if (Utility.GetPlayerTeam(item) == "allies" && item != player)
                            {
                                item.Call("iprintlnbold", new Parameter[] { "^2Player: " + player.Name + " gave you $500." });
                                item.Call("playlocalsound", new Parameter[] { "new_perk_unlocks" });
                                item.SetField("inf2_money", item.GetField<int>("inf2_money") + 500);
                            }
                        }
                        player.SetField("gamblerstate", "idle");
                        return;
                    case 17:
                        player.Call("iprintlnbold", new Parameter[] { "^1You infected!" });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_end" });
                        Call("playfx", new Parameter[] { Call<int>("loadfx", new Parameter[] { "props/barrelexp" }), player.Call<Vector3>("gettagorigin", new Parameter[] { "j_head" }) });
                        player.Call("playsound", new Parameter[] { "nuke_explosion" });
                        player.Call("suicide", new Parameter[0]);
                        player.SetField("gamblerstate", "idle");
                        return;
                    case 18:
                        Call("iprintlnbold", new Parameter[] { "^0Team Die or SB Die After 5 second." });
                        player.Call("playsound", new Parameter[] { "mp_bonus_end" });
                        AfterDelay(1000, () => Call("iprintlnbold", new Parameter[] { "^05" }));
                        AfterDelay(1000, () => player.Call("playsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(2000, () => Call("iprintlnbold", new Parameter[] { "^04" }));
                        AfterDelay(2000, () => player.Call("playsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(3000, () => Call("iprintlnbold", new Parameter[] { "^03" }));
                        AfterDelay(3000, () => player.Call("playsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(4000, () => Call("iprintlnbold", new Parameter[] { "^02" }));
                        AfterDelay(4000, () => player.Call("playsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(5000, () => Call("iprintlnbold", new Parameter[] { "^01" }));
                        AfterDelay(5000, () => player.Call("playsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
                        AfterDelay(6000, () =>
                        {
                            rng = new int?(new Random().Next(2));
                            switch (rng.Value)
                            {
                                case 0:
                                    Call("iprintlnbold", new Parameter[] { "^1SB Die!" });
                                    player.Call("playsound", new Parameter[] { "mp_bonus_end" });
                                    return;

                                case 1:
                                    Call("iprintlnbold", new Parameter[] { "^1Booooom! All Human Killed by SB." });
                                    player.Call("playsound", new Parameter[] { "nuke_explosion" });
                                    foreach (var item in Utility.getPlayerList())
                                    {
                                        if (Utility.GetPlayerTeam(item) == "allies")
                                        {
                                            Call("playfx", new Parameter[] { Call<int>("loadfx", new Parameter[] { "props/barrelexp" }), item.Call<Vector3>("gettagorigin", new Parameter[] { "j_head" }) });
                                            AfterDelay(0, () => item.Call("FinishPlayerDamage", null, player, 500, 500, 0, "nuke_mp", player.Origin, new Vector3(), "", 0));
                                        }
                                    }

                                    return;
                            }
                            player.SetField("gamblerstate", "idle");
                        });
                        return;
                    case 19:
                        player.Call("iprintlnbold", new Parameter[] { "^2Air support in comming!" });
                        player.Call("playlocalsound", new Parameter[] { "mp_bonus_start" });
                        player.Health = -1;
                        Call("playsound", new Parameter[] { "US_1mc_use_airstrike" });
                        foreach (var item in Utility.getPlayerList())
                        {
                            if (Utility.GetPlayerTeam(item) == "axis")
                            {
                                Call("magicbullet", new Parameter[] { "stinger_mp", new Vector3(item.Origin.X, item.Origin.Y, item.Origin.Z + 5000f), item.Origin, player });
                            }
                        }
                        AfterDelay(3000, () => player.Health = player.GetField<int>("maxhealth"));
                        return;
                    case 20:
                        Call("iprintlnbold", new Parameter[] { "^2Double Points!" });
                        player.Call("playsound", "mp_level_up");
                        player.SetField("gamblerstate", "idle");
                        if (Call<int>("getdvarint", "mod_inf2_doublepoint") == 1)
                        {
                            return;
                        }
                        Call("setdvar", "mod_inf2_doublepoint", 1);
                        AfterDelay(30000, () =>
                        {
                            Call("setdvar", "mod_inf2_doublepoint", 0);
                            foreach (var item in Utility.getPlayerList())
                            {
                                if (Utility.GetPlayerTeam(item) == "allies")
                                {
                                    item.Call("iprintlnbold", "Double Point off!");
                                }
                            }
                        });
                        return;
                    case 21:
                        Call("iprintlnbold", new Parameter[] { "^2Insta Kill!" });
                        player.Call("playsound", "mp_level_up");
                        player.SetField("gamblerstate", "idle");
                        if (Call<int>("getdvarint", "mod_inf2_instakill") == 1)
                        {
                            return;
                        }
                        Call("setdvar", "mod_inf2_instakill", 1);
                        AfterDelay(30000, () =>
                        {
                            Call("setdvar", "mod_inf2_instakill", 0);
                            foreach (var item in Utility.getPlayerList())
                            {
                                if (Utility.GetPlayerTeam(item) == "allies")
                                {
                                    item.Call("iprintlnbold", "Insta Kill off!");
                                }
                            }
                        });
                        return;
                    case 22:
                        Call("iprintlnbold", new Parameter[] { "^2Max Ammo!" });
                        player.Call("playsound", "mp_level_up");
                        player.SetField("gamblerstate", "idle");
                        foreach (var item in Utility.getPlayerList())
                        {
                            if (!item.IsAlive || Utility.GetPlayerTeam(item) != "allies")
                            {
                                continue;
                            }
                            item.Call("givemaxammo", item.GetField<string>("firstweapon"));
                            item.Call("givemaxammo", item.GetField<string>("secondweapon"));

                            if (!item.HasWeapon("trophy_mp"))
                            {
                                item.GiveWeapon("trophy_mp");
                            }
                            item.Call("setweaponammoclip", "trophy_mp", 99);
                            item.Call("givemaxammo", "trophy_mp");
                        }
                        return;
                    case 23:
                        Call("iprintlnbold", new Parameter[] { "^2Kaboom!" });
                        player.Call("playsound", "nuke_explosion");
                        player.SetField("gamblerstate", "idle");
                        int delay = 1;
                        foreach (var item in Utility.getPlayerList())
                        {
                            if (Utility.GetPlayerTeam(item) == "axis" && item.IsAlive)
                            {
                                AfterDelay(delay * 1000, () =>
                                {
                                    Call("playfx", Call<int>("loadfx", "props/barrelexp"), item.Call<Vector3>("gettagorigin", "j_head"));
                                    item.Call("playsound", new Parameter[] { "explo_mine" });
                                    item.Call("suicide");
                                });
                                delay++;
                            }
                        }
                        return;
                }
            });
        }

        private string getAlliesFlagModel(string mapname)
        {
            switch (mapname)
            {
                case "mp_alpha":
                case "mp_dome":
                case "mp_hardhat":
                case "mp_interchange":
                case "mp_cement":
                case "mp_hillside_ss":
                case "mp_morningwood":
                case "mp_overwatch":
                case "mp_park":
                case "mp_qadeem":
                case "mp_restrepo_ss":
                case "mp_terminal_cls":
                case "mp_roughneck":
                case "mp_boardwalk":
                case "mp_moab":
                case "mp_nola":
                case "mp_radar":
                    return "prop_flag_delta";
                case "mp_exchange":
                    return "prop_flag_american05";
                case "mp_bootleg":
                case "mp_bravo":
                case "mp_mogadishu":
                case "mp_village":
                case "mp_shipbreaker":
                    return "prop_flag_pmc";
                case "mp_paris":
                    return "prop_flag_gign";
                case "mp_plaza2":
                case "mp_aground_ss":
                case "mp_courtyard_ss":
                case "mp_italy":
                case "mp_meteora":
                case "mp_underground":
                    return "prop_flag_sas";
                case "mp_seatown":
                case "mp_carbon":
                case "mp_lambeth":
                    return "prop_flag_seal";
            }
            return "";
        }

        public void GiveAmmo(Entity player)
        {
            if (!player.IsAlive || Utility.GetPlayerTeam(player) != "allies")
            {
                return;
            }
            player.Call("givemaxammo", player.GetField<string>("firstweapon"));
            if (player.GetField<string>("firstweapon") != "none")
                player.Call("givemaxammo", player.GetField<string>("secondweapon"));

            if (!player.HasWeapon("trophy_mp"))
            {
                player.GiveWeapon("trophy_mp");
            }
            player.Call("setweaponammoclip", "trophy_mp", 99);
            player.Call("givemaxammo", "trophy_mp");
        }

        public void LaptopRotate(Entity laptop)
        {
            OnInterval(7000, delegate
            {
                laptop.Call("rotateyaw", new Parameter[] { -360, 7 });
                return laptop != null;
            });
        }

        private void loadMapEdit(string mapname)
        {
            try
            {
                StreamReader reader = new StreamReader(this.mapname);
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    if (!str.StartsWith("//") && !str.Equals(string.Empty))
                    {
                        string[] strArray = str.Split(new char[] { ':' });
                        if (strArray.Length >= 1)
                        {
                            string str2 = strArray[0];
                            switch (str2)
                            {
                                case "crate":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            spawnCrate(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "ramp":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateRamp(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "elevator":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateElevator(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "HiddenTP":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateHiddenTP(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "door":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 7)
                                        {
                                            CreateDoor(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]), Utility.ParseVector3(strArray[2]), int.Parse(strArray[3]), int.Parse(strArray[4]), int.Parse(strArray[5]), int.Parse(strArray[6]));
                                        }
                                        continue;
                                    }
                                case "wall":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateWall(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "wall2":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateWall2(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "floor":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateFloor(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "model":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 3)
                                        {
                                            spawnModel(strArray[0], Utility.ParseVector3(strArray[1]), Utility.ParseVector3(strArray[2]));
                                        }
                                        continue;
                                    }
                                case "turret":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateTurret(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "turret2":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateTurret2(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "sentry":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateSentry(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "sentry2":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateSentry2(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "teleporter":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 3)
                                        {
                                            CreateTeleporter(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]), Utility.ParseVector3(strArray[2]));
                                        }
                                        continue;
                                    }
                                //case "sam":
                                //    {
                                //        strArray = strArray[1].Split(new char[] { ';' });
                                //        if (strArray.Length >= 2)
                                //        {
                                //            CreateSAM(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                //        }
                                //        continue;
                                //    }
                                case "zipline":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 3)
                                        {
                                            CreateZipline(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]), Utility.ParseVector3(strArray[2]));
                                        }
                                        continue;
                                    }
                                case "ammo":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateAmmoBox(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "gambler":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateGambler(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "beacon":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateAirstrikeBeacon(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                                case "mystery":
                                    {
                                        strArray = strArray[1].Split(new char[] { ';' });
                                        if (strArray.Length >= 2)
                                        {
                                            CreateMysteryBox(Utility.ParseVector3(strArray[0]), Utility.ParseVector3(strArray[1]));
                                        }
                                        continue;
                                    }
                            }
                            print("Unknown MapEdit Entry {0}... ignoring", new object[] { str2 });
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                print("error loading mapedit for map {0}: {1}", new object[] { mapname, exception.Message });
            }
        }

        private static void print(string format, params object[] p)
        {
            Log.Write(InfinityScript.LogLevel.All, format, p);
        }

        private string randomFile(string mapname)
        {
            string[] files = Directory.GetFiles("scripts\\inf2-maps\\");
            List<string> list = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Contains(mapname))
                {
                    list.Add(files[i]);
                }
            }
            return list[new Random().Next(list.Count)];
        }

        public void removeSpawn(Entity spawn)
        {
            spawn.Call("delete", new Parameter[0]);
        }

        public static void runOnUsable(Func<Entity, bool> func, string type)
        {
            foreach (Entity entity in usables)
            {
                if (entity.GetField<string>("usabletype") == type)
                {
                    func(entity);
                }
            }
        }

        public Entity spawnCrate(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            entity.Call("setmodel", new Parameter[] { "com_plasticcase_friendly" });
            entity.SetField("angles", new Parameter(angles));
            entity.Call(0x8249, new Parameter[] { _airdropCollision });
            return entity;
        }

        public Entity spawnCrateTrap(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            entity.Call("setmodel", new Parameter[] { "com_plasticcase_trap_friendly" });
            entity.SetField("angles", new Parameter(angles));
            entity.Call(0x8249, new Parameter[] { _airdropCollision });
            return entity;
        }

        public Entity spawnCrateRed(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            entity.Call("setmodel", new Parameter[] { "com_plasticcase_trap_bombsquad" });
            entity.SetField("angles", new Parameter(angles));
            entity.Call(0x8249, new Parameter[] { _airdropCollision });
            return entity;
        }

        public Entity spawnModel(string model, Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            entity.Call("setmodel", new Parameter[] { model });
            entity.SetField("angles", new Parameter(angles));
            return entity;
        }

        public void usedAirstrikeBeacon(Entity box, Entity player)
        {
            if (!player.IsAlive)
            {
                return;
            }
            if (box.GetField<string>("state") == "waiting")
            {
                if (Utility.GetPlayerTeam(player) == "allies")
                {
                    if (player.GetField<int>("inf2_money") < 100)
                    {
                        player.Call("iprintln", new Parameter[] { "^1Designated Airstrike need $100 for human." });
                        return;
                    }
                    player.SetField("inf2_money", player.GetField<int>("inf2_money") - 100);
                }
                box.SetField("state", "idle");
                DoAirstrike(player, box.Origin);
                AfterDelay(0xea60, () => box.SetField("state", "waiting"));
            }
        }

        private void DoAirstrike(Entity player, Vector3 origin)
        {
            player.Call("iprintlnbold", new Parameter[] { "^210" });
            player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" });
            AfterDelay(0x3e8, () => player.Call("iprintlnbold", new Parameter[] { "^29" }));
            AfterDelay(0x3e8, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0x7d0, () => player.Call("iprintlnbold", new Parameter[] { "^28" }));
            AfterDelay(0x7d0, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0xbb8, () => player.Call("iprintlnbold", new Parameter[] { "^27" }));
            AfterDelay(0xbb8, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0xfa0, () => player.Call("iprintlnbold", new Parameter[] { "^26" }));
            AfterDelay(0xfa0, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0x1388, () => player.Call("iprintlnbold", new Parameter[] { "^25" }));
            AfterDelay(0x1388, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0x1770, () => player.Call("iprintlnbold", new Parameter[] { "^24" }));
            AfterDelay(0x1770, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0x1b58, () => player.Call("iprintlnbold", new Parameter[] { "^23" }));
            AfterDelay(0x1b58, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0x1f40, () => player.Call("iprintlnbold", new Parameter[] { "^22" }));
            AfterDelay(0x1f40, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0x2328, () => player.Call("iprintlnbold", new Parameter[] { "^21" }));
            AfterDelay(0x2328, () => player.Call("playlocalsound", new Parameter[] { "ui_mp_nukebomb_timer" }));
            AfterDelay(0x2710, delegate
            {
                player.Call("playsound", new Parameter[] { "US_1mc_use_airstrike" });
                player.Call("iprintlnbold", new Parameter[] { "^1Airstrike Inbound!" });
                player.Call("playsoundasmaster", new Parameter[] { "mp_bonus_end" });
                Vector3 loc = origin;
                Call("magicbullet", new Parameter[] { "javelin_mp", new Vector3(loc.X, loc.Y, loc.Z + 25000f), loc, player });
                AfterDelay(0, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                AfterDelay(400, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                AfterDelay(800, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                AfterDelay(1200, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                AfterDelay(1600, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                AfterDelay(2000, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                AfterDelay(2400, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
                AfterDelay(2800, () => Call("magicbullet", new Parameter[] { "ac130_40mm_mp", new Vector3(loc.X, loc.Y, loc.Z + 20000f), Utility.rngVec(loc, 100), player }));
            });
        }

        public void usedAmmoBox(Entity player)
        {
            if (player.IsAlive && (Utility.GetPlayerTeam(player) != "axis"))
            {
                if (player.GetField<int>("inf2_money") < 300)
                {
                    player.Call("iprintln", new Parameter[] { "^1Ammo need $300." });
                }
                else
                {
                    if (!player.CurrentWeapon.Contains("killstreak") || !player.CurrentWeapon.Contains("airdrop"))
                    {
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") - 300);
                        GiveAmmo(player);
                        player.Call("playlocalsound", new Parameter[] { "new_weapon_unlocks" });
                        player.Call("iprintlnbold", new Parameter[] { "^2Ammo gaved!" });
                    }
                }
            }
        }

        private void usedDoor(Entity door, Entity player)
        {
            Action function = null;
            Action action2 = null;
            Action<Entity> action3 = null;
            if (player.IsAlive)
            {
                if (door.GetField<int>("hp") <= 0)
                {
                    if ((door.GetField<int>("hp") == 0) && (door.GetField<string>("state") != "broken"))
                    {
                        if (door.GetField<string>("state") == "close")
                        {
                            door.Call(0x8277, new Parameter[] { new Parameter(door.GetField<Vector3>("open")), 1f });
                        }
                        door.SetField("state", "broken");
                    }
                }
                else if (Utility.GetPlayerTeam(player) == "allies")
                {
                    if (door.GetField<string>("state") == "open")
                    {
                        door.Call(0x8277, new Parameter[] { new Parameter(door.GetField<Vector3>("close")), 1 });
                        if (function == null)
                        {
                            function = () => door.SetField("state", "close");
                        }
                        AfterDelay(300, function);
                    }
                    else if (door.GetField<string>("state") == "close")
                    {
                        door.Call(0x8277, new Parameter[] { new Parameter(door.GetField<Vector3>("open")), 1 });
                        if (action2 == null)
                        {
                            action2 = () => door.SetField("state", "open");
                        }
                        AfterDelay(300, action2);
                    }
                }
                else if ((door.GetField<string>("state") == "close") && (player.GetField<int>("attackeddoor") == 0))
                {
                    int num = 0;
                    string str = player.Call<string>("getstance", new Parameter[0]);
                    if (str != null)
                    {
                        if (!(str == "prone"))
                        {
                            if (str == "couch")
                            {
                                num = 0x2d;
                            }
                            else if (str == "stand")
                            {
                                num = 90;
                            }
                        }
                        else
                        {
                            num = 20;
                        }
                    }
                    if (_rng.Next(100) < num)
                    {
                        door.SetField("hp", door.GetField<int>("hp") - 1);
                        player.Call("iprintlnbold", new Parameter[] { string.Concat(new object[] { "HIT: ", door.GetField<int>("hp"), "/", door.GetField<int>("maxhp") }) });
                    }
                    else
                    {
                        player.Call("iprintlnbold", new Parameter[] { "^1MISS" });
                    }
                    player.SetField("attackeddoor", 1);
                    if (action3 == null)
                    {
                        action3 = e => player.SetField("attackeddoor", 0);
                    }
                    player.AfterDelay(0x3e8, action3);
                }
            }
        }

        public void usedZipline(Entity box, Entity player)
        {
            Action<Entity> function = null;
            if (player.IsAlive && box.GetField<string>("state") != "using")
            {
                Vector3 startorigin = box.Origin;
                box.SetField("state", "using");
                box.Call("clonebrushmodeltoscriptmodel", new Parameter[] { Call<Entity>("spawn", "script_origin", new Vector3()) });
                player.Call("playerlinkto", new Parameter[] { box });
                box.Call("moveto", new Parameter[] { box.GetField<Vector3>("endorigin"), 5 });
                box.AfterDelay(5000, delegate (Entity ent)
                {
                    if (player.Call<int>("islinked", new Parameter[0]) != 0)
                    {
                        player.Call("unlink", new Parameter[0]);
                        player.Call("setorigin", new Parameter[] { box.GetField<Vector3>("endorigin") });
                    }
                    box.Call("moveto", new Parameter[] { startorigin, 1 });
                });
                if (function == null)
                {
                    function = delegate (Entity ent)
                    {
                        box.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
                        box.SetField("state", "idle");
                    };
                }
                box.AfterDelay(6100, function);
            }
        }

        public void usedMysteryBox(Entity box, Entity player)
        {
            if (player.IsAlive && (Utility.GetPlayerTeam(player) != "axis"))
            {
                if (player.CurrentWeapon.Contains("killstreak") || player.CurrentWeapon.Contains("airdrop"))
                {
                    return;
                }
                if (player.GetField<int>("inf2_money") < 500)
                {
                    player.Call("iprintln", new Parameter[] { "^1Mystery box need $500." });
                    return;
                }
                else
                {
                    player.SetField("inf2_money", player.GetField<int>("inf2_money") - 500);
                    Weapon weapon = Weapon.GetRandomWeapon();
                    if (player.HasWeapon(weapon.Text))
                    {
                        player.Call("givemaxammo", new Parameter[] { weapon.Text });
                        player.SwitchToWeapon(weapon.Text);
                        player.Call("iprintlnbold", weapon.Name);
                    }
                    else
                    {
                        if (player.GetField<string>("secondweapon") != "none")
                        {
                            if (player.GetField<string>("firstweapon") == player.CurrentWeapon)
                            {
                                player.SetField("firstweapon", weapon.Text);
                            }
                            else if (player.GetField<string>("secondweapon") == player.CurrentWeapon)
                            {
                                player.SetField("secondweapon", weapon.Text);
                            }
                            player.TakeWeapon(player.CurrentWeapon);
                        }
                        else
                        {
                            player.SetField("secondweapon", weapon.Text);
                        }
                        player.GiveWeapon(weapon.Text);
                        player.Call("givemaxammo", new Parameter[] { weapon.Text });
                        AfterDelay(300, () => player.SwitchToWeaponImmediate(weapon.Text));
                        player.Call("iprintlnbold", weapon.Name);
                    }
                }
            }
        }

        public void usedTeleporter(Entity box, Entity player)
        {
            if (player.IsAlive)
            {
                if (Utility.GetPlayerTeam(player) == "allies")
                {
                    if (player.GetField<int>("inf2_money") >= 500)
                    {
                        player.SetField("inf2_money", player.GetField<int>("inf2_money") - 500);
                        if (player.Call<int>("islinked", new Parameter[0]) != 0)
                        {
                            player.Call("unlink", new Parameter[0]);
                        }
                        player.Call("shellshock", new Parameter[] { "concussion_grenade_mp", 3 });
                        player.Call("setorigin", new Parameter[] { box.GetField<Vector3>("endorigin") });
                    }
                    else
                    {
                        player.Call("iprintln", new Parameter[] { "^1Teleporter need $500 for human." });
                    }
                }
                else
                {
                    if (player.Call<int>("islinked", new Parameter[0]) != 0)
                    {
                        player.Call("unlink", new Parameter[0]);
                    }
                    player.Call("setorigin", new Parameter[] { box.GetField<Vector3>("endorigin") });
                }
            }
        }
    }
}


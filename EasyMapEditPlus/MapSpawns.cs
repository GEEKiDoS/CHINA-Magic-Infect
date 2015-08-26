using InfinityScript;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasyMapEditPlus
{
    public class MapSpawns : BaseScript
    {
        private bool creating = false;
        private Entity[] undoEntities;
        private Entity currenteditor;

        private Vector3 start;
        private Vector3 end;
        private Vector3 angles;

        private struct Data
        {
            public string name;
            public Vector3 origin;
            public Vector3 endorigin;
            public Vector3 angles;
        }

        private Data lastdata;
        private List<Data> datas = new List<Data>();

        private Entity _airdropCollision;
        private string mapname;
        private Random _rng;
        private int curObjID;

        public MapSpawns()
        {
            Call("precacheShader", "weapon_m16_iw5");
            _rng = new Random();
            mapname = Utility.GetCurrentMapEditFile(Call<string>("getdvar", new Parameter[] { "mapname" }));
            Call("precachemodel", new Parameter[] { getAlliesFlagModel(Call<string>("getdvar", "mapname")) });
            Call("precachemodel", new Parameter[] { "prop_flag_neutral" });
            Call("precachemodel", new Parameter[] { "com_plasticcase_green_big" });
            Call("precacheshader", new Parameter[] { "waypoint_flag_friendly" });
            Call("precacheshader", new Parameter[] { "compass_waypoint_target" });
            Call("precacheshader", new Parameter[] { "compass_waypoint_bomb" });
            Call("precachemodel", new Parameter[] { "weapon_scavenger_grenadebag" });
            Call("precachemodel", new Parameter[] { "weapon_oma_pack" });

            PlayerConnected += new Action<Entity>(player =>
            {
                player.Call("notifyonplayercommand", "fly", "+frag");
                player.OnNotify("fly", (ent) =>
                {
                    if (player.GetField<string>("sessionstate") != "spectator")
                    {
                        player.Call("allowspectateteam", "freelook", true);
                        player.SetField("sessionstate", "spectator");
                        player.Call("setcontents", 0);
                    }
                    else
                    {
                        player.Call("allowspectateteam", "freelook", false);
                        player.SetField("sessionstate", "playing");
                        player.Call("setcontents", 100);
                    }
                });
            });
        }

        public Entity[] CreateAirstrikeBeacon(Vector3 origin, Vector3 angle)
        {
            Call("precacheshader", new Parameter[] { "dpad_killstreak_ac130" });
            Entity ent = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            ent.Call("setmodel", new Parameter[] { "com_plasticcase_enemy" });
            ent.SetField("angles", new Parameter(angle));
            ent.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
            ent.SetField("state", "waiting");
            int num = 0x1f - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_icon", new Parameter[] { num, "dpad_killstreak_ac130" });
            ent.Call("sethintstring", "Press ^3[{+activate}] ^7to call Designated Airstrike [free].");

            return new Entity[] { ent };
        }

        public Entity[] CreateAmmoBox(Vector3 origin, Vector3 angle)
        {
            Entity box = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            box.Call("setmodel", new Parameter[] { "com_plasticcase_green_big_us_dirt" });
            box.SetField("angles", new Parameter(angle));
            box.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
            CreateBoxShader(box, "waypoint_ammo_friendly");
            Vector3 v = origin;
            v.Z += 17f;
            int num = 0x1f - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_team", new Parameter[] { num, "allies" });
            Call("objective_icon", new Parameter[] { num, "waypoint_ammo_friendly" });
            box.Call("sethintstring", "Press ^3[{+activate}] ^7to by ammo [Cost: ^2$^300 ^7].");

            return new Entity[] { box };
        }

        public void CreateBoxShader(Entity box, string shader)
        {
            HudElem elem = HudElem.NewTeamHudElem("allies");
            elem.SetShader(shader, 20, 20);
            elem.Alpha = 0.6f;
            elem.X = box.Origin.X;
            elem.Y = box.Origin.Y;
            elem.Z = box.Origin.Z + 40f;
            elem.Call("SetWayPoint", new Parameter[] { 1, 1 });
        }

        public Entity[] CreateDoor(Vector3 open, Vector3 close, Vector3 angle, int size, int height, int hp, int range)
        {
            List<Entity> _ents = new List<Entity>();
            double num = ((size / 2) - 0.5) * -1.0;
            Entity ent = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(open) });
            _ents.Add(ent);
            for (int i = 0; i < size; i++)
            {
                Entity entity2 = spawnCrate(open + ((Vector3)(new Vector3(0f, 30f, 0f) * ((float)num))), new Vector3(0f, 0f, 0f));
                entity2.Call("setModel", new Parameter[] { "com_plasticcase_enemy" });
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { ent });
                _ents.Add(entity2);
                for (int j = 1; j < height; j++)
                {
                    Entity entity3 = spawnCrate((open + ((Vector3)(new Vector3(0f, 30f, 0f) * ((float)num)))) - (new Vector3(70f, 0f, 0f) * j), new Vector3(0f, 0f, 0f));
                    entity3.Call("setModel", new Parameter[] { "com_plasticcase_enemy" });
                    entity3.Call("enablelinkto", new Parameter[0]);
                    entity3.Call("linkto", new Parameter[] { ent });
                    _ents.Add(entity3);
                }
                num++;
            }
            ent.SetField("angles", new Parameter(angle));
            ent.SetField("state", "open");
            ent.SetField("hp", hp);
            ent.SetField("maxhp", hp);
            ent.SetField("open", new Parameter(open));
            ent.SetField("close", new Parameter(close));

            return _ents.ToArray();
        }

        public Entity[] CreateElevator(Vector3 enter, Vector3 exit)
        {
            Entity flag = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(enter) });
            flag.Call("setModel", new Parameter[] { getAlliesFlagModel(Call<string>("getdvar", "mapname")) });
            Entity flag2 = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(exit) });
            flag2.Call("setModel", new Parameter[] { "prop_flag_neutral" });
            CreateFlagShader(flag);
            int num = 0x1f - curObjID++;
            Call(0x1af, new Parameter[] { num, "active" });
            Call(0x1b3, new Parameter[] { num, new Parameter(flag.Origin) });
            Call(0x1b2, new Parameter[] { num, "waypoint_flag_friendly" });
            OnInterval(100, delegate
            {
                foreach (Entity entity in Utility.getPlayerList())
                {
                    if (entity.Origin.DistanceTo(enter) <= 50f)
                    {
                        entity.Call("setorigin", "exit");
                    }
                }
                return flag != null && flag2 != null;
            });

            return new Entity[] { flag, flag2 };
        }

        public void CreateFlagShader(Entity flag)
        {
            HudElem elem = HudElem.NewHudElem();
            elem.SetShader("waypoint_flag_friendly", 20, 20);
            elem.Alpha = 0.6f;
            elem.X = flag.Origin.X;
            elem.Y = flag.Origin.Y;
            elem.Z = flag.Origin.Z + 100f;
            elem.Call("SetWayPoint", new Parameter[] { 1, 1 });
        }

        public Entity[] CreateFloor(Vector3 corner1, Vector3 corner2)
        {
            List<Entity> _ents = new List<Entity>();

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
            _ents.Add(entity);
            for (int i = 0; i < num3; i++)
            {
                for (int j = 0; j < num4; j++)
                {
                    Entity entity2 = spawnCrate((corner1 + (new Vector3(vector2.X, 0f, 0f) * i)) + (new Vector3(0f, vector2.Y, 0f) * j), new Vector3(0f, 0f, 0f));
                    entity2.Call("enablelinkto", new Parameter[0]);
                    entity2.Call("linkto", new Parameter[] { entity });
                    _ents.Add(entity2);
                }
            }

            return _ents.ToArray();
        }

        public Entity[] CreateGambler(Vector3 origin, Vector3 angle)
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
            box.Call("sethintstring", "Press ^3[{+activate}] ^7to Gamble [Cost: ^2$^3500^7].");

            return new Entity[] { box, laptop };
        }

        public Entity[] CreateHiddenTP(Vector3 enter, Vector3 exit)
        {
            Entity ent1 = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(enter) });
            ent1.Call("setModel", new Parameter[] { "weapon_scavenger_grenadebag" });
            Entity ent2 = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(exit) });
            ent2.Call("setModel", new Parameter[] { "com_deploy_ballistic_vest_friend_viewmodel" });
            OnInterval(100, delegate
            {
                foreach (Entity entity in Utility.getPlayerList())
                {
                    if (entity.Origin.DistanceTo(enter) <= 50f)
                    {
                        entity.Call("setorigin", "exit");
                    }
                }
                return ent1 != null && ent2 != null;
            });

            return new Entity[] { ent1, ent2 };
        }

        public Entity[] CreateRamp(Vector3 top, Vector3 bottom)
        {
            List<Entity> _ents = new List<Entity>();
            int num2 = (int)Math.Ceiling((double)(top.DistanceTo(bottom) / 30f));
            Vector3 vector = new Vector3((top.X - bottom.X) / ((float)num2), (top.Y - bottom.Y) / ((float)num2), (top.Z - bottom.Z) / ((float)num2));
            Vector3 vector2 = base.Call<Vector3>("vectortoangles", new Parameter[] { new Parameter(top - bottom) });
            Vector3 angles = new Vector3(vector2.Z, vector2.Y + 90f, vector2.X);
            for (int i = 0; i <= num2; i++)
            {
                Entity ent = spawnCrateTrap(bottom + (vector * i), angles);
                _ents.Add(ent);
            }

            return _ents.ToArray();
        }

        public Entity[] CreateTurret(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "sentry_minigun_mp" });
            entity.Call("setmodel", new Parameter[] { "weapon_minigun" });
            entity.SetField("angles", angles);
            entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use turret.");

            return new Entity[] { entity };
        }
        public Entity[] CreateTurret2(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "turret_minigun_mp" });
            entity.Call("setmodel", new Parameter[] { "weapon_minigun" });
            entity.SetField("angles", angles);
            entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use turret.");

            return new Entity[] { entity };
        }

        public Entity[] CreateSentry(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "sentry_minigun_mp" });
            entity.Call("setmodel", new Parameter[] { "sentry_minigun_weak" });
            entity.SetField("angles", angles);
            entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use sentry gun.");

            return new Entity[] { entity };
        }
        public Entity[] CreateSentry2(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "remote_turret_mp" });
            entity.Call("setmodel", new Parameter[] { "mp_remote_turret" });
            entity.SetField("angles", angles);
            entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use remote sentry.");
            entity.Call("SetBottomArc", 30);

            return new Entity[] { entity };
        }

        public Entity[] CreateGL(Vector3 origin, Vector3 angles)
        {
            Entity entity = base.Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "ugv_gl_turret_mp" });
            entity.Call("setmodel", new Parameter[] { "sentry_grenade_launcher" });
            entity.SetField("angles", angles);
            entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use grenade launcher.");
            entity.SetField("player", "");
            entity.SetField("state", "idle");
            entity.SetField("statefire", "manual");

            return new Entity[] { entity };
        }

        public Entity[] CreateSAM(Vector3 origin, Vector3 angles)
        {
            Entity entity = Call<Entity>("spawnTurret", new Parameter[] { "misc_turret", new Parameter(origin), "sam_mp" });
            entity.Call("setmodel", new Parameter[] { "mp_sam_turret" });
            entity.SetField("angles", angles);
            entity.Call("sethintstring", "Press ^3[{+activate}] ^7to use remote SAM turret.");
            entity.SetField("player", "");
            entity.SetField("state", "idle");
            entity.SetField("statefire", "manual");

            return new Entity[] { entity };
        }

        public Entity[] CreateWall(Vector3 start, Vector3 end)
        {
            List<Entity> _ents = new List<Entity>();

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
            _ents.Add(entity);
            for (int i = 0; i < num4; i++)
            {
                Entity entity2 = spawnCrateTrap((start + new Vector3(x, y, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { entity });
                _ents.Add(entity2);
                for (int j = 0; j < num3; j++)
                {
                    entity2 = spawnCrateTrap(((start + (new Vector3(vector2.X, vector2.Y, 0f) * j)) + new Vector3(0f, 0f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                    entity2.Call("enablelinkto", new Parameter[0]);
                    entity2.Call("linkto", new Parameter[] { entity });
                    _ents.Add(entity2);
                }
                entity2 = spawnCrateTrap((new Vector3(end.X, end.Y, start.Z) + new Vector3(x * -1f, y * -1f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { entity });
                _ents.Add(entity2);
            }

            return _ents.ToArray();
        }

        public Entity[] CreateWall2(Vector3 start, Vector3 end)
        {
            List<Entity> _ents = new List<Entity>();

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
            _ents.Add(entity);
            for (int i = 0; i < num4; i++)
            {
                Entity entity2 = spawnCrateRed((start + new Vector3(x, y, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { entity });
                _ents.Add(entity2);
                for (int j = 0; j < num3; j++)
                {
                    entity2 = spawnCrateRed(((start + (new Vector3(vector2.X, vector2.Y, 0f) * j)) + new Vector3(0f, 0f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                    entity2.Call("enablelinkto", new Parameter[0]);
                    entity2.Call("linkto", new Parameter[] { entity });
                    _ents.Add(entity2);
                }
                entity2 = spawnCrateRed((new Vector3(end.X, end.Y, start.Z) + new Vector3(x * -1f, y * -1f, 10f)) + (new Vector3(0f, 0f, vector2.Z) * i), angles);
                entity2.Call("enablelinkto", new Parameter[0]);
                entity2.Call("linkto", new Parameter[] { entity });
                _ents.Add(entity2);
            }
            return _ents.ToArray();
        }

        public Entity[] CreateZipline(Vector3 origin, Vector3 angle, Vector3 endorigin)
        {
            Call("precacheshader", new Parameter[] { "hudicon_neutral" });
            Entity ent = Call<Entity>("spawn", new Parameter[] { "script_model", new Parameter(origin) });
            ent.Call("setmodel", new Parameter[] { "com_plasticcase_trap_bombsquad" });
            ent.SetField("angles", new Parameter(angle));
            ent.Call("clonebrushmodeltoscriptmodel", new Parameter[] { _airdropCollision });
            ent.SetField("state", "idle");
            ent.SetField("endorigin", endorigin);
            int num = 0x1f - curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_icon", new Parameter[] { num, "hudicon_neutral" });
            ent.Call("sethintstring", getZiplineText(ent));

            return new Entity[] { ent };
        }

        public Entity[] CreateMysteryBox(Vector3 origin, Vector3 angle)
        {
            Entity box = Call<Entity>("spawn", "script_model", origin);
            box.Call("setmodel", "com_plasticcase_friendly");
            box.SetField("angles", angle);
            box.Call("clonebrushmodeltoscriptmodel", this._airdropCollision);
            box.SetField("state", "idle");
            box.SetField("weapon", "");
            box.SetField("player", "");
            box.SetField("destroyed", 0);
            box.SetField("weaponent", -1);
            CreateBoxShader(box, "weapon_m16_iw5");
            int num = 31 - this.curObjID++;
            Call("objective_state", new Parameter[] { num, "active" });
            Call("objective_position", new Parameter[] { num, new Parameter(origin) });
            Call("objective_icon", new Parameter[] { num, "weapon_m16_iw5" });
            box.Call("sethintstring", "Press ^3[{+activate}]^7 to use Mystery Box. [Cost: ^2$^3500 ^7]");

            Entity trigger = Call<Entity>("spawn", "trigger_radius", origin, 0, 50, 50);
            Log.Debug(trigger.ToString());

            return new Entity[] { box };
        }

        public Entity[] CreateTeleporter(Vector3 origin, Vector3 angle, Vector3 endorigin)
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

            return new Entity[] { box };
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

        public string getZiplineText(Entity box)
        {
            if (box.GetField<string>("state") == "using")
            {
                return "^1Zipline is busy.";
            }
            return "Press ^3[{+activate}] ^7to to use zipline.";
        }

        public void LaptopRotate(Entity laptop)
        {
            OnInterval(0x1b58, delegate
            {
                laptop.Call("rotateyaw", new Parameter[] { -360, 7 });
                return laptop != null;
            });
        }

        //public void MakeUsable(Entity ent, string type, int range)
        //{
        //ent.SetField("usabletype", type);
        //ent.SetField("range", range);
        //usables.Add(ent);
        //}

        //public static void notifyUsables(string notify)
        //{
        //    foreach (Entity entity in usables)
        //    {
        //        entity.Notify(notify, new Parameter[0]);
        //    }
        //}

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

        public void removeSpawn(Entity spawn)
        {
            spawn.Call("delete", new Parameter[0]);
        }

        public override void OnSay(Entity player, string name, string message)
        {
            if (message == "!undo" && !creating)
            {
                foreach (var item in undoEntities)
                {
                    removeSpawn(item);
                }
                datas.Remove(lastdata);
                player.Call("iprintlnbold", "^2Object Undo Complete!");
            }
            if (message == "!saveall" && !creating)
            {
                foreach (var item in datas)
                {
                    if (item.name == "zipline")
                    {
                        File.AppendAllText(mapname, Environment.NewLine + item.name + ": (" + item.origin.X + "," + item.origin.Y + "," + item.origin.Z + ") ; (" + 0 + "," + item.angles.Y + "," + 0 + ") ; (" + item.endorigin.X + "," + item.endorigin.Y + "," + item.endorigin.Z + ")");
                    }
                    else if (item.name == "wall" || item.name == "wall2" || item.name == "ramp" || item.name == "floor" || item.name == "elevator" || item.name == "HiddenTP")
                    {
                        File.AppendAllText(mapname, Environment.NewLine + item.name + ": (" + item.origin.X + "," + item.origin.Y + "," + item.origin.Z + ") ; (" + item.endorigin.X + "," + item.endorigin.Y + "," + item.endorigin.Z + ")");
                    }
                    else
                    {
                        File.AppendAllText(mapname, Environment.NewLine + item.name + ": (" + item.origin.X + "," + item.origin.Y + "," + item.origin.Z + ") ; (" + 0 + "," + item.angles.Y + "," + 0 + ")");
                    }
                }
                datas.Clear();
                player.Call("iprintlnbold", "^2Save Complete!");
            }
            if (message == ("!wall") && !creating)
            {
                start = player.Origin;
                creating = true;
                currenteditor = player;
                player.Call("iprintlnbold", "^2wall start set: " + start);
            }
            else if (message == ("!wall") && creating)
            {
                if (player == currenteditor)
                {
                    end = player.Origin;
                    lastdata = new Data { name = "wall", origin = start, endorigin = end };
                    datas.Add(lastdata);
                    creating = false;

                    undoEntities = CreateWall(lastdata.origin, lastdata.endorigin);

                    player.Call("iprintlnbold", "^2wall end set: " + start);
                }
                else
                {
                    player.Call("iprintlnbold", "^1You are not creater!");
                }
            }
            if (message == ("!wall2") && !creating)
            {
                start = player.Origin;
                creating = true;
                currenteditor = player;
                player.Call("iprintlnbold", "^2wall2 start set: " + start);
            }
            else if (message == ("!wall2") && creating)
            {
                if (player == currenteditor)
                {
                    end = player.Origin;
                    lastdata = new Data { name = "wall2", origin = start, endorigin = end };
                    datas.Add(lastdata);
                    creating = false;

                    undoEntities = CreateWall(lastdata.origin, lastdata.endorigin);

                    player.Call("iprintlnbold", "^2wall2 end set: " + start);
                }
                else
                {
                    player.Call("iprintlnbold", "^1You are not creater!");
                }
            }
            if (message == ("!ramp") && !creating)
            {
                start = player.Origin;
                creating = true;
                currenteditor = player;
                player.Call("iprintlnbold", "^2ramp start set: " + start);
            }
            else if (message == ("!ramp") && creating)
            {
                if (player == currenteditor)
                {
                    end = player.Origin;
                    lastdata = new Data { name = "ramp", origin = start, endorigin = end };
                    datas.Add(lastdata);
                    creating = false;

                    undoEntities = CreateRamp(lastdata.origin, lastdata.endorigin);

                    player.Call("iprintlnbold", "^2ramp end set: " + start);
                }
                else
                {
                    player.Call("iprintlnbold", "^1You are not creater!");
                }
            }
            if (message == ("!floor") && !creating)
            {
                start = player.Origin;
                creating = true;
                currenteditor = player;
                player.Call("iprintlnbold", "^2floor start set: " + start);
            }
            else if (message == ("!floor") && creating)
            {
                if (player == currenteditor)
                {
                    end = player.Origin;
                    lastdata = new Data { name = "floor", origin = start, endorigin = end };
                    datas.Add(lastdata);
                    creating = false;

                    undoEntities = CreateFloor(lastdata.origin, lastdata.endorigin);

                    player.Call("iprintlnbold", "^2floor end set: " + start);
                }
                else
                {
                    player.Call("iprintlnbold", "^1You are not creater!");
                }
            }
            if (message == ("!tp") && !creating)
            {
                start = player.Origin;
                creating = true;
                currenteditor = player;
                player.Call("iprintlnbold", "^2tp start set: " + start);
            }
            else if (message == ("!tp") && creating)
            {
                if (player == currenteditor)
                {
                    end = player.Origin;
                    lastdata = new Data { name = "elevator", origin = start, endorigin = end };
                    datas.Add(lastdata);
                    creating = false;

                    undoEntities = CreateElevator(lastdata.origin, lastdata.endorigin);

                    player.Call("iprintlnbold", "^2tp end set: " + start);
                }
                else
                {
                    player.Call("iprintlnbold", "^1You are not creater!");
                }
            }
            if (message == ("!htp") && !creating)
            {
                start = player.Origin;
                creating = true;
                currenteditor = player;
                player.Call("iprintlnbold", "^2htp start set: " + start);
            }
            else if (message == ("!htp") && creating)
            {
                if (player == currenteditor)
                {
                    end = player.Origin;
                    lastdata = new Data { name = "HiddenTP", origin = start, endorigin = end };
                    datas.Add(lastdata);
                    creating = false;

                    undoEntities = CreateHiddenTP(lastdata.origin, lastdata.endorigin);

                    player.Call("iprintlnbold", "^2htp end set: " + start);
                }
                else
                {
                    player.Call("iprintlnbold", "^1You are not creater!");
                }
            }
            if (message == ("!zipline") && !creating)
            {
                start = player.Origin;
                angles = player.GetField<Vector3>("angles");
                creating = true;
                currenteditor = player;
                player.Call("iprintlnbold", "^2zipline start set: " + start + " angle set: " + angles);
            }
            else if (message == ("!zipline") && creating)
            {
                if (player == currenteditor)
                {
                    end = player.Origin;
                    lastdata = new Data { name = "zipline", origin = start, endorigin = end, angles = angles };
                    datas.Add(lastdata);
                    creating = false;

                    undoEntities = CreateZipline(lastdata.origin, lastdata.angles, lastdata.endorigin);

                    player.Call("iprintlnbold", "^2zipline end set: " + start);
                }
                else
                {
                    player.Call("iprintlnbold", "^1You are not creater!");
                }
            }
            if (message == ("!teleporter") && !creating)
            {
                start = player.Origin;
                angles = player.GetField<Vector3>("angles");
                creating = true;
                currenteditor = player;
                player.Call("iprintlnbold", "^2teleporter start set: " + start + " angle set: " + angles);
            }
            else if (message == ("!teleporter") && creating)
            {
                if (player == currenteditor)
                {
                    end = player.Origin;
                    lastdata = new Data { name = "teleporter", origin = start, endorigin = end, angles = angles };
                    datas.Add(lastdata);
                    creating = false;

                    undoEntities = CreateTeleporter(lastdata.origin, lastdata.angles, lastdata.endorigin);

                    player.Call("iprintlnbold", "^2teleporter end set: " + start);
                }
                else
                {
                    player.Call("iprintlnbold", "^1You are not creater!");
                }
            }
            if (message == ("!mystery") && !creating)
            {
                lastdata = new Data { name = "mystery", origin = player.Origin, angles = player.GetField<Vector3>("angles") };
                datas.Add(lastdata);

                undoEntities = CreateMysteryBox(lastdata.origin, lastdata.angles);

                player.Call("iprintlnbold", "^2mystery set: " + lastdata.origin + " angle set: " + lastdata.angles);
            }
            if (message == ("!ammo") && !creating)
            {
                lastdata = new Data { name = "ammo", origin = player.Origin, angles = player.GetField<Vector3>("angles") };
                datas.Add(lastdata);

                undoEntities = CreateAmmoBox(lastdata.origin, lastdata.angles);

                player.Call("iprintlnbold", "^2ammo set: " + lastdata.origin + " angle set: " + lastdata.angles);
            }
            if (message == ("!gambler") && !creating)
            {
                lastdata = new Data { name = "gambler", origin = player.Origin, angles = player.GetField<Vector3>("angles") };
                datas.Add(lastdata);

                undoEntities = CreateGambler(lastdata.origin, lastdata.angles);

                player.Call("iprintlnbold", "^2gambler set: " + lastdata.origin + " angle set: " + lastdata.angles);
            }
            if (message == ("!beacon") && !creating)
            {
                lastdata = new Data { name = "beacon", origin = player.Origin, angles = player.GetField<Vector3>("angles") };
                datas.Add(lastdata);

                undoEntities = CreateAirstrikeBeacon(lastdata.origin, lastdata.angles);

                player.Call("iprintlnbold", "^2beacon set: " + lastdata.origin + " angle set: " + lastdata.angles);
            }
            if (message == ("!turret") && !creating)
            {
                lastdata = new Data { name = "turret", origin = player.Origin, angles = player.GetField<Vector3>("angles") };
                datas.Add(lastdata);

                undoEntities = CreateTurret(lastdata.origin, lastdata.angles);

                player.Call("iprintlnbold", "^2turret set: " + lastdata.origin + " angle set: " + lastdata.angles);
            }
            if (message == ("!turret2") && !creating)
            {
                lastdata = new Data { name = "turret2", origin = player.Origin, angles = player.GetField<Vector3>("angles") };
                datas.Add(lastdata);

                undoEntities = CreateTurret2(lastdata.origin, lastdata.angles);

                player.Call("iprintlnbold", "^2turret2 set: " + lastdata.origin + " angle set: " + lastdata.angles);
            }
            if (message == ("!sentry") && !creating)
            {
                lastdata = new Data { name = "sentry", origin = player.Origin, angles = player.GetField<Vector3>("angles") };
                datas.Add(lastdata);

                undoEntities = CreateSentry(lastdata.origin, lastdata.angles);

                player.Call("iprintlnbold", "^2sentry set: " + lastdata.origin + " angle set: " + lastdata.angles);
            }
            if (message == ("!sentry2") && !creating)
            {
                lastdata = new Data { name = "sentry2", origin = player.Origin, angles = player.GetField<Vector3>("angles") };
                datas.Add(lastdata);

                undoEntities = CreateSentry2(lastdata.origin, lastdata.angles);

                player.Call("iprintlnbold", "^2sentry2 set: " + lastdata.origin + " angle set: " + lastdata.angles);
            }
        }
    }
}


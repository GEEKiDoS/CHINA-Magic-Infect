using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF2
{
    public class BonusDrops : BaseScript
    {
        //private List<Entity> dropents = new List<Entity>();
        //private Dictionary<int, Delegate> funcs = new Dictionary<int, Delegate>();
        //private Random _rng = new Random();

        public BonusDrops()
        {
            Call("setdvar", "mod_inf2_doublepoint", 0);
            Call("setdvar", "mod_inf2_instakill", 0);
            Call("setdvar", "mod_inf2_zombieblood", 0);

            //PlayerConnected += new Action<Entity>(player => UsableBonusDrops(player));
        }

        //public void UsableBonusDrops(Entity player)
        //{
        //    OnInterval(100, () =>
        //    {
        //        foreach (var entity in dropents)
        //        {
        //            if (player.Origin.DistanceTo(entity.Origin) >= 75)
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                if (Utility.GetPlayerTeam(player) == "allies")
        //                {
        //                    DoFunction(player, entity, funcs[entity.EntRef]);
        //                    Destroy(entity);
        //                    break;
        //                }
        //            }
        //        }

        //        return true;
        //    });
        //}

        //public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        //{
        //    if (Utility.GetPlayerTeam(attacker) == Utility.GetPlayerTeam(player) && player.GetField<int>("inf2_test_bonusdrop") == 0)
        //    {
        //        return;
        //    }
        //    if (Utility.GetPlayerTeam(player) == "axis")
        //    {
        //        if (_rng.Next(8) == 1 || player.GetField<int>("inf2_test_bonusdrop") == 1)
        //        {
        //            switch (_rng.Next(5))
        //            {
        //                case 0:
        //                    BonusDropsReg("Double Points", "com_plasticcase_friendly", player.Origin, new Vector3(), Call<int>("loadfx", "misc/flare_ambient"), new Action(() => DoublePoints()));
        //                    break;
        //                case 1:
        //                    BonusDropsReg("Insta Kill", "com_plasticcase_trap_friendly", player.Origin, new Vector3(), Call<int>("loadfx", "misc/flare_ambient_green"), new Action(() => InstaKill()));
        //                    break;
        //                case 2:
        //                    BonusDropsReg("KaBoom", "projectile_cbu97_clusterbomb", player.Origin + new Vector3(0, 0, 40), player.Call<Vector3>("angles") - new Vector3(90, 0, 0), Call<int>("loadfx", "misc/flare_ambient"), new Action(() => KaBoom()));
        //                    break;
        //                case 3:
        //                    BonusDropsReg("Max Ammo", "com_plasticcase_friendly", player.Origin, new Vector3(), Call<int>("loadfx", "misc/flare_ambient_green"), new Action(() => MaxAmmo()));
        //                    break;
        //                case 4:
        //                    BonusDropsReg("Zombie Blood", "com_plasticcase_enemy", player.Origin, new Vector3(), Call<int>("loadfx", "misc/flare_ambient"), new Action(() => ZombieBlood()));
        //                    break;
        //            }
        //        }
        //    }
        //}

        //public void BonusDropsReg(string text, string model, Vector3 origin, Vector3 angles, int fx, Delegate function)
        //{
        //    Entity block = Call<Entity>("spawn", "script_model", origin + new Vector3(0, 0, 10));
        //    block.Call("setmodel", model);
        //    block.SetField("angles", angles);

        //    Entity spawnfx = Call<Entity>("spawnfx", fx, origin);
        //    block.SetField("fx", spawnfx);
        //    AfterDelay(100, () => { Call("triggerfx", spawnfx); });

        //    block.SetField("text", text);

        //    dropents.Add(block);
        //    funcs.Add(block.EntRef, function);

        //    //timer
        //    AfterDelay(10, () =>
        //    {
        //        AfterDelay(20000, () =>
        //        {
        //            if (block != null)
        //            {
        //                AfterDelay(1000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(1500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(2000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(2500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(3000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(3500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(4000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(4500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(5000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(5500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(6000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(6500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(7000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(7500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(8000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(8500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(9000, () => { if (block != null) block.Call("hide"); });
        //                AfterDelay(9500, () => { if (block != null) block.Call("show"); });
        //                AfterDelay(10000, () =>
        //                {
        //                    if (block != null)
        //                    {
        //                        Destroy(block);
        //                    }
        //                });
        //            }
        //        });
        //    });

        //    //rotate
        //    OnInterval(5000, () =>
        //    {
        //        block.Call("rotateyaw", -360, 5);
        //        return block != null;
        //    });
        //}

        //private void DoFunction(Entity player, Entity block, Delegate func)
        //{
        //    if (player.IsAlive)
        //    {
        //        Entity spawnfx = block.GetField<Entity>("fx");
        //        string text = block.GetField<string>("text");

        //        AfterDelay(10, () => player.Call("playsound", "mp_level_up"));
        //        foreach (var item in Utility.getPlayerList())
        //        {
        //            if (Utility.GetPlayerTeam(item) == "allies")
        //            {
        //                item.Call("iprintlnbold", "^2" + text);
        //            }
        //        }

        //        func.DynamicInvoke();
        //    }
        //}

        //private void Destroy(Entity block)
        //{
        //    dropents.Remove(block);
        //    funcs.Remove(block.EntRef);

        //    Entity spawnfx = block.GetField<Entity>("fx");
        //    spawnfx.Call("delete");
        //    AfterDelay(300, () => block.Call("delete"));
        //}

        //private void DoublePoints()
        //{
        //    if (Call<int>("getdvarint", "mod_inf2_doublepoint") == 1)
        //    {
        //        return;
        //    }
        //    Call("setdvar", "mod_inf2_doublepoint", 1);
        //    AfterDelay(30000, () =>
        //    {
        //        Call("setdvar", "mod_inf2_doublepoint", 0);
        //        foreach (var item in Utility.getPlayerList())
        //        {
        //            if (Utility.GetPlayerTeam(item) == "allies")
        //            {
        //                item.Call("iprintlnbold", "Double Point off!");
        //            }
        //        }
        //    });
        //}

        //private void InstaKill()
        //{
        //    if (Call<int>("getdvarint", "mod_inf2_instakill") == 1)
        //    {
        //        return;
        //    }
        //    Call("setdvar", "mod_inf2_instakill", 1);
        //    AfterDelay(30000, () =>
        //    {
        //        Call("setdvar", "mod_inf2_instakill", 0);
        //        foreach (var item in Utility.getPlayerList())
        //        {
        //            if (Utility.GetPlayerTeam(item) == "allies")
        //            {
        //                item.Call("iprintlnbold", "Insta Kill off!");
        //            }
        //        }
        //    });
        //}

        //private void KaBoom()
        //{
        //    int delay = 1;
        //    foreach (var item in Utility.getPlayerList())
        //    {
        //        if (Utility.GetPlayerTeam(item) == "axis" && item.IsAlive)
        //        {
        //            AfterDelay(delay * 1000, () =>
        //            {
        //                Call("playfx", Call<int>("loadfx", "props/barrelexp"), item.Call<Vector3>("gettagorigin", "j_head"));
        //                Call("physicsexplosionsphere", item.Origin, 230, 0, 3);
        //                item.Call("playsound", new Parameter[] { "explo_mine" });
        //                item.Call("suicide");
        //            });
        //            delay++;
        //        }
        //    }
        //}

        //private void MaxAmmo()
        //{
        //    foreach (var player in Utility.getPlayerList())
        //    {
        //        if (!player.IsAlive || Utility.GetPlayerTeam(player) != "allies")
        //        {
        //            continue;
        //        }
        //        player.Call("givemaxammo", player.GetField<string>("firstweapon"));
        //        if (player.GetField<string>("firstweapon") != "none")
        //            player.Call("givemaxammo", player.GetField<string>("secondweapon"));

        //        if (!player.HasWeapon("trophy_mp"))
        //        {
        //            player.GiveWeapon("trophy_mp");
        //        }
        //        player.Call("setweaponammoclip", "trophy_mp", 99);
        //        player.Call("givemaxammo", "trophy_mp");
        //    }
        //}

        //private void ZombieBlood()
        //{
        //    if (Call<int>("getdvarint", "mod_inf2_zombleblood") == 1)
        //    {
        //        return;
        //    }
        //    Call("setdvar", "mod_inf2_zombleblood", 1);
        //    foreach (var item in Utility.getPlayerList())
        //    {
        //        if (Utility.GetPlayerTeam(item) == "allies")
        //        {
        //            item.Call("hide");
        //        }
        //    }
        //    AfterDelay(30000, () =>
        //    {
        //        Call("setdvar", "mod_inf2_instakill", 0);
        //        foreach (var item in Utility.getPlayerList())
        //        {
        //            if (Utility.GetPlayerTeam(item) == "allies")
        //            {
        //                item.Call("iprintlnbold", "Insta Kill off!");
        //                item.Call("show");
        //            }
        //        }
        //    });
        //}
    }
}

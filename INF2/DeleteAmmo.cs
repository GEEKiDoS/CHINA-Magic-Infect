using InfinityScript;
using System;

namespace INF2
{
    public class DeleteAmmo : BaseScript
    {
        private string[] fixWeapons;

        public DeleteAmmo()
        {
            Action<Entity> action = null;
            this.fixWeapons = new string[] { "iw5_44magnum", "iw5_usp45", "iw5_deserteagle", "iw5_mp412", "iw5_p99", "iw5_fnfiveseven" };
            if (action == null)
            {
                action = delegate (Entity player)
                {
                    this.OnPlayerSpawend(player);
                    player.SpawnedPlayer += () => this.OnPlayerSpawend(player);
                };
            }
            base.PlayerConnected += action;
        }

        public void OnPlayerSpawend(Entity player)
        {
            player.AfterDelay(0, delegate (Entity ent)
            {
                if ((((player != null) && player.IsPlayer) && player.IsAlive) && (player.GetField<string>("sessionteam") != "allies"))
                {
                    foreach (string str in this.fixWeapons)
                    {
                        if (player.CurrentWeapon.Contains(str) && (player.CurrentWeapon.Contains("tactical") || player.CurrentWeapon == "iw5_usp45_mp"))
                        {
                            player.Call("setweaponammoclip", new Parameter[] { player.CurrentWeapon, 0 });
                            player.Call("setweaponammostock", new Parameter[] { player.CurrentWeapon, 0 });
                            break;
                        }
                    }
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace GTA5Wasted
{
    public class GTA5Wasted : BaseScript
    {
        public GTA5Wasted()
        {
            PlayerConnected += delegate (Entity player)
            {
                HudElem hud = HudElem.CreateFontString(player, "hudbig", 2.3f);
                hud.SetPoint("center", "center", 0, -20);
                hud.SetText("Wasted");
                hud.Color = new Vector3(255f, 0f, 0f);
                hud.GlowColor = new Vector3(0.2f, 0.2f, 0.2f);
                hud.GlowAlpha = 0.8f;
                hud.Alpha = 0f;

                HudElem overlay = HudElem.CreateIcon(player, "combathigh_overlay", 640, 480);
                overlay.X = 0f;
                overlay.Y = 0f;
                overlay.AlignX = "left";
                overlay.AlignY = "top";
                overlay.HorzAlign = "fullscreen";
                overlay.VertAlign = "fullscreen";
                overlay.Color = new Vector3(0.2f, 0.2f, 0.2f);
                overlay.Alpha = 0f;

                player.SetField("gta5_hud", new Parameter(hud));
                player.SetField("gta5_overlay", new Parameter(overlay));
                player.SetField("gta5_dead", 0);

                player.SpawnedPlayer += delegate
                {
                    hud.Alpha = 0f;
                    overlay.Alpha = 0f;

                    player.SetField("gta5_dead", 0);
                };
            };
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            HudElem Hud = player.GetField<HudElem>("gta5_hud");
            Hud.Alpha = 0.7f;

            HudElem Overlay = player.GetField<HudElem>("gta5_overlay");
            Overlay.Call("fadeovertime", 1f);
            Overlay.Alpha = 1f;

            player.SetField("gta5_dead", 1);
            OnInterval(10, () =>
            {
                player.Call("setempjammed", true);
                //player.Call("visionsetnakedforplayer", "mpIntro", 1);
                if (player.IsAlive)
                {
                    //player.Call("visionsetnakedforplayer", "", 1);
                    player.Call("setempjammed", false);
                }

                return player.GetField<int>("gta5_dead") != 0;
            });
        }
    }
}

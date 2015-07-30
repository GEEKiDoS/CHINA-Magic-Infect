/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF2
{
    public class INF2Test : BaseScript
    {
        public INF2Test()
        {
            PlayerConnected += player =>
            {
                player.SetField("inf2_test_bonusdrop", 0);
                player.SpawnedPlayer += () => player.SetField("inf2_test_bonusdrop", 0);
            };
        }

        public override void OnSay(Entity player, string name, string message)
        {
            if (message == "!tryroll")
            {
                AfterDelay(100, () =>
                {
                    player.SetField("rtd_canroll", 1);
                    player.Call("suicide");
                });
            }
            if (message == "!trybonusdrop")
            {
                player.SetField("inf2_test_bonusdrop", 1);
                AfterDelay(300, () => player.Call("suicide"));
            }
            if (message == "!incatation")
            {
                player.SetField("incantation", 1);
                player.SetField("zombie_incantation", 1);
                PrintLog("Incatation", "Player: " + player.Name + " now has incatation");
            }
        }

        public static void PrintLog(string model, string text)
        {
            Function.SetEntRef(-1);
            Function.Call("iprintln", "<" + model + "> " + text);
            Log.Debug("<" + model + "> " + text);
        }
    }
}
*/
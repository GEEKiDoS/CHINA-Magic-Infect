using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace MusicPlayer
{
    public class MusicPlayer : BaseScript
    {
        public override void OnSay(Entity player, string name, string message)
        {
            if (message.StartsWith("!play "))
            {
                player.Call("playlocalsound", message.Split(new char[] { ' ' }, 2)[1]);
            }
        }
    }
}

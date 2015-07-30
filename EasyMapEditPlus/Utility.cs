using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InfinityScript;

namespace EasyMapEditPlus
{
    public static class Utility
    {
        public static string GetCurrentMapEditFile(string mapname)
        {
            var dir = Directory.GetFiles("scripts\\inf2-maps");
            foreach (var file in dir)
            {
                if (file.Contains(mapname))
                {
                    return file;
                }
            }
            File.Create("scripts\\inf2-maps\\" + mapname + ".txt");
            return "scripts\\inf2-maps\\" + mapname + ".txt";
        }

        public static Entity[] getPlayerList()
        {
            List<Entity> list = new List<Entity>();
            for (int i = 0; i <= 0x11; i++)
            {
                Entity item = Entity.GetEntity(i);
                if ((item != null) && item.IsPlayer)
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        public static string GetPlayerTeam(Entity player)
        {
            return player.GetField<string>("sessionteam");
        }

        public static Entity GetPlayerByName(string name)
        {
            foreach (var item in getPlayerList())
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public static Vector3 ParseVector3(string vec3)
        {
            vec3 = vec3.Replace(" ", string.Empty);
            vec3 = vec3.Replace("(", string.Empty);
            vec3 = vec3.Replace(")", string.Empty);
            string[] strArray = vec3.Split(new char[] { ',' });
            return new Vector3(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));
        }

        public static Vector3 rngVec(Vector3 loc, int len)
        {
            int? nullable = new int?(new Random().Next(-len, len));
            return new Vector3(loc.X + ((float)nullable.Value), loc.Y + ((float)nullable.Value), loc.Z + ((float)nullable.Value));
        }
    }
}

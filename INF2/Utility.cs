using System;
using System.Collections.Generic;
using InfinityScript;

namespace INF2
{
    public static class Utility
    {
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
            double? nullable = new double?(new Random().Next(-len, len));
            return new Vector3(loc.X + ((float)nullable.Value), loc.Y + ((float)nullable.Value), loc.Z + ((float)nullable.Value));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF2
{
    public static class Weapons
    {
        private static Random _rng = new Random();

        public static string[] _ar2List = new string[] { "iw5_m16", "iw5_m4" };
        public static string[] _ar3List = new string[] { "iw5_ak47" };
        public static string[] _arList = new string[] { "iw5_fad", "iw5_acr", "iw5_type95", "iw5_mk14", "iw5_scar", "iw5_g36c", "iw5_cm901" };
        public static string[] _autoPistolList = new string[] { "iw5_fmg9", "iw5_skorpion", "iw5_mp9", "iw5_g18" };
        public static string[] _lmgList = new string[] { "iw5_m60", "iw5_mk46", "iw5_pecheneg", "iw5_sa80", "iw5_mg36" };
        public static string[] _otherList = new string[] { "rpg_mp", "iw5_smaw_mp", "xm25_mp", "m320_mp", "riotshield_mp", "javelin_mp", "stinger_mp" };
        public static string[] _pistol2List = new string[] { "iw5_44magnum", "iw5_mp412", "iw5_deserteagle" };
        public static string[] _pistolList = new string[] { "iw5_usp45", "iw5_p99", "iw5_fnfiveseven" };
        public static string[] _shotgunList = new string[] { "iw5_1887", "iw5_striker", "iw5_aa12", "iw5_usas12", "iw5_spas12", "iw5_ksg" };
        public static string[] _smgList = new string[] { "iw5_mp5", "iw5_m9", "iw5_p90", "iw5_pp90m1", "iw5_ump45", "iw5_mp7" };
        public static string[] _sniperList = new string[] { "iw5_dragunov", "iw5_msr", "iw5_barrett", "iw5_rsass", "iw5_as50", "iw5_l96a1" };

        private static string[] GetPistolAttachments = new string[] { "none", "silencer02", "xmags", "akimbo", "tactical" };
        private static string[] GetPistol2Attachments = new string[] { "none", "akimbo", "tactical" };
        private static string[] GetAutoPistolAttachments1 = new string[] { "none", "reflex", "eotech" };
        private static string[] GetAutoPistolAttachments2 = new string[] { "none", "akimbo", "silencer02", "xmags" };
        private static string[] GetSmgAttachments1 = new string[] { "none", "acog", "reflex", "hamrhybrid", "eotech", "thermal" };
        private static string[] GetSmgAttachments2 = new string[] { "none", "silencer", "xmags" };
        private static string[] GetAr1Attachments1 = new string[] { "none", "acog", "reflex", "hybrid", "eotech", "thermal" };
        private static string[] GetAr1Attachments2 = new string[] { "none", "silencer", "m320", "shotgun", "xmags", "heartbeat" };
        private static string[] GetAr2Attachments1 = new string[] { "none", "acog", "reflex", "hybrid", "eotech", "thermal" };
        private static string[] GetAr2Attachments2 = new string[] { "none", "silencer", "gl", "shotgun", "xmags", "heartbeat" };
        private static string[] GetAr3Attachments1 = new string[] { "none", "acog", "reflex", "hybrid", "eotech", "thermal" };
        private static string[] GetAr3Attachments2 = new string[] { "none", "silencer", "gp25", "shotgun", "xmags", "heartbeat" };
        private static string[] GetLmgAttachments1 = new string[] { "none", "acog", "reflex", "eotech", "thermal" };
        private static string[] GetLmgAttachments2 = new string[] { "none", "silencer", "grip", "xmags", "heartbeat" };
        private static string[] GetSnipeAttachments1 = new string[] { "none", "acog", "vzscope", "thermal" };
        private static string[] GetSnipeAttachments2 = new string[] { "none", "silencer03", "xmags", "heartbeat" };
        private static string[] GetShotgunAttachments1 = new string[] { "none", "reflex", "eotech" };
        private static string[] GetShotgunAttachments2 = new string[] { "none", "silencer03", "grip", "xmags" };

        public static string[] _weaponList;

        private static string AddRandomAttachmentToWeapon(string baseWeapon)
        {
            string str = "none";
            string str2 = "none";
            bool flag = false;
            while (!flag)
            {
                if (_pistolList.Contains(baseWeapon))
                {
                    str = GetPistolAttachments[_rng.Next(0, GetPistolAttachments.Length)];
                    str2 = "none";
                    flag = true;
                }
                else
                {
                    if (_pistol2List.Contains(baseWeapon))
                    {
                        str = GetPistol2Attachments[_rng.Next(0, GetPistol2Attachments.Length)];
                        str2 = "none";
                        flag = true;
                        continue;
                    }
                    if (_autoPistolList.Contains(baseWeapon))
                    {
                        str = GetAutoPistolAttachments1[_rng.Next(0, GetAutoPistolAttachments1.Length)];
                        str2 = GetAutoPistolAttachments2[_rng.Next(0, GetAutoPistolAttachments2.Length)];
                        flag = true;
                        continue;
                    }
                    if (_smgList.Contains(baseWeapon))
                    {
                        str = GetSmgAttachments1[_rng.Next(0, GetSmgAttachments1.Length)];
                        str2 = GetSmgAttachments2[_rng.Next(0, GetSmgAttachments2.Length)];
                        flag = true;
                        continue;
                    }
                    if (_arList.Contains(baseWeapon))
                    {
                        str = GetAr1Attachments1[_rng.Next(0, GetAr1Attachments1.Length)];
                        str2 = GetAr1Attachments2[_rng.Next(0, GetAr1Attachments2.Length)];
                        flag = true;
                        continue;
                    }
                    if (_ar2List.Contains(baseWeapon))
                    {
                        str = GetAr2Attachments1[_rng.Next(0, GetAr2Attachments1.Length)];
                        str2 = GetAr2Attachments2[_rng.Next(0, GetAr2Attachments2.Length)];
                        flag = true;
                        continue;
                    }
                    if (_ar3List.Contains(baseWeapon))
                    {
                        str = GetAr3Attachments1[_rng.Next(0, GetAr3Attachments1.Length)];
                        str2 = GetAr3Attachments2[_rng.Next(0, GetAr3Attachments2.Length)];
                        flag = true;
                        continue;
                    }
                    if (_lmgList.Contains(baseWeapon))
                    {
                        str = GetLmgAttachments1[_rng.Next(0, GetLmgAttachments1.Length)];
                        str2 = GetLmgAttachments2[_rng.Next(0, GetLmgAttachments2.Length)];
                        flag = true;
                        continue;
                    }
                    if (_sniperList.Contains(baseWeapon))
                    {
                        str = GetSnipeAttachments1[_rng.Next(0, GetSnipeAttachments1.Length)];
                        str2 = GetSnipeAttachments2[_rng.Next(0, GetSnipeAttachments2.Length)];
                        flag = true;
                        continue;
                    }
                    if (_shotgunList.Contains(baseWeapon))
                    {
                        str = GetShotgunAttachments1[_rng.Next(0, GetShotgunAttachments1.Length)];
                        str2 = GetShotgunAttachments2[_rng.Next(0, GetShotgunAttachments2.Length)];
                        flag = true;
                        continue;
                    }
                    str = "none";
                    str2 = "none";
                    flag = true;
                }
            }
            if (!_otherList.Contains(baseWeapon))
            {
                return Utilities.BuildWeaponName(baseWeapon, str, str2, _rng.Next(0, 14), _rng.Next(0, 7));
            }
            return baseWeapon;
        }

        public static string GetRandomWeapon()
        {
            string item = (from w in _weaponList
                           orderby _rng.Next()
                           select w).FirstOrDefault<string>();
            if (item == null)
            {
                item = _weaponList[_rng.Next(0, _weaponList.Length)];
            }
            return AddRandomAttachmentToWeapon(item);
        }
    }
}

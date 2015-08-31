using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF2
{
    public class Weapon
    {
        private static Random _rng = new Random();

        public string Name { get; private set; }
        public string Text { get; private set; }

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
        private static string[] GetAutoPistolAttachments1 = new string[] { "none", "reflex", "eotech", "akimbo", };
        private static string[] GetAutoPistolAttachments2 = new string[] { "none", "silencer02", "xmags" };
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
            if (_pistolList.Contains(baseWeapon))
            {
                str = GetPistolAttachments[_rng.Next(0, GetPistolAttachments.Length)];
                str2 = "none";
            }
            if (_pistol2List.Contains(baseWeapon))
            {
                str = GetPistol2Attachments[_rng.Next(0, GetPistol2Attachments.Length)];
                str2 = "none";
            }
            if (_autoPistolList.Contains(baseWeapon))
            {
                str = GetAutoPistolAttachments1[_rng.Next(0, GetAutoPistolAttachments1.Length)];
                str2 = GetAutoPistolAttachments2[_rng.Next(0, GetAutoPistolAttachments2.Length)];
            }
            if (_smgList.Contains(baseWeapon))
            {
                str = GetSmgAttachments1[_rng.Next(0, GetSmgAttachments1.Length)];
                str2 = GetSmgAttachments2[_rng.Next(0, GetSmgAttachments2.Length)];
            }
            if (_arList.Contains(baseWeapon))
            {
                str = GetAr1Attachments1[_rng.Next(0, GetAr1Attachments1.Length)];
                str2 = GetAr1Attachments2[_rng.Next(0, GetAr1Attachments2.Length)];
            }
            if (_ar2List.Contains(baseWeapon))
            {
                str = GetAr2Attachments1[_rng.Next(0, GetAr2Attachments1.Length)];
                str2 = GetAr2Attachments2[_rng.Next(0, GetAr2Attachments2.Length)];
            }
            if (_ar3List.Contains(baseWeapon))
            {
                str = GetAr3Attachments1[_rng.Next(0, GetAr3Attachments1.Length)];
                str2 = GetAr3Attachments2[_rng.Next(0, GetAr3Attachments2.Length)];
            }
            if (_lmgList.Contains(baseWeapon))
            {
                str = GetLmgAttachments1[_rng.Next(0, GetLmgAttachments1.Length)];
                str2 = GetLmgAttachments2[_rng.Next(0, GetLmgAttachments2.Length)];
            }
            if (_sniperList.Contains(baseWeapon))
            {
                str = GetSnipeAttachments1[_rng.Next(0, GetSnipeAttachments1.Length)];
                str2 = GetSnipeAttachments2[_rng.Next(0, GetSnipeAttachments2.Length)];
            }
            if (_shotgunList.Contains(baseWeapon))
            {
                str = GetShotgunAttachments1[_rng.Next(0, GetShotgunAttachments1.Length)];
                str2 = GetShotgunAttachments2[_rng.Next(0, GetShotgunAttachments2.Length)];
            }

            if (!_otherList.Contains(baseWeapon))
            {
                return Utilities.BuildWeaponName(baseWeapon, str, str2, _rng.Next(0, 14), _rng.Next(0, 7));
            }
            return baseWeapon;
        }

        private static string GetWeaponName(string text)
        {
            string temp = text.Split(new string[] { "_mp" }, 2, StringSplitOptions.RemoveEmptyEntries)[0];

            switch (temp)
            {
                case "iw5_m16": return "M16A4";
                case "iw5_m4": return "M4A1";
                case "iw5_ak47": return "AK-47";
                case "iw5_fad": return "FAD";
                case "iw5_acr": return "ACR 6.8";
                case "iw5_type95": return "Type 95";
                case "iw5_mk14": return "MK14";
                case "iw5_scar": return "SCAR-L";
                case "iw5_g36c": return "G36C";
                case "iw5_cm901": return "CM901";
                case "iw5_fmg9": return "FMG9";
                case "iw5_skorpion": return "Skorpion";
                case "iw5_mp9": return "MP9";
                case "iw5_g18": return "G18";
                case "iw5_m60": return "M60E4";
                case "iw5_mk46": return "MK46";
                case "iw5_pecheneg": return "PKP Pecheneg";
                case "iw5_sa80": return "L86 LSW";
                case "iw5_mg36": return "MG36";
                case "rpg": return "RPG-7";
                case "iw5_smaw": return "SMAW";
                case "xm25": return "XM25";
                case "m320": return "M320";
                case "riotshield": return "Riotshield";
                case "javelin": return "Javelin";
                case "stinger": return "Stinger";
                case "iw5_44magnum": return ".44 Magnum";
                case "iw5_mp412": return "MP412";
                case "iw5_deserteagle": return "Desert Eagle";
                case "iw5_usp45": return "USP .45";
                case "iw5_p99": return "P99";
                case "iw5_fnfiveseven": return "Fiveseven";
                case "iw5_1887": return "Model 1887";
                case "iw5_striker": return "Striker";
                case "iw5_aa12": return "AA-12";
                case "iw5_usas12": return "USAS-12";
                case "iw5_spas12": return "SPAS-12";
                case "iw5_ksg": return "KSG";
                case "iw5_mp5": return "MP5";
                case "iw5_m9": return "PM-9";
                case "iw5_p90": return "P90";
                case "iw5_pp90m1": return "PP90M1";
                case "iw5_ump45": return "UMP45";
                case "iw5_mp7": return "MP7";
                case "iw5_dragunov": return "Dragunov";
                case "iw5_msr": return "MSR";
                case "iw5_barrett": return "Barrett .50 Cal";
                case "iw5_rsass": return "RSASS";
                case "iw5_as50": return "AS50";
                case "iw5_l96a1": return "L118A";
                default:
                    return string.Empty;
            }
        }

        public static Weapon GetRandomWeapon()
        {
            string item = AddRandomAttachmentToWeapon(_weaponList[_rng.Next(0, _weaponList.Length)]);
            Weapon temp = new Weapon();
            if (item.Contains("hybrid"))
            {
                item = "alt_" + item;
            }
            temp.Name = GetWeaponName(item);
            temp.Text = item;
            return temp;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}

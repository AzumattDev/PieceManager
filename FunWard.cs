using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PieceManager;
//using ServerSync;
using UnityEngine;

namespace WardIsLove
{
    [BepInPlugin(HGUIDLower, ModName, version)]
    public partial class WardIsLovePlugin : BaseUnityPlugin
    {
        public const string version = "1.0.0";
        public const string ModName = "TESTMOD";
        internal const string Author = "Azumatt";
        internal const string HGUID = Author + "." + "FunWard";
        internal const string HGUIDLower = "azumatt.FunWard";
        private const string HarmonyGUID = "Harmony." + Author + "." + ModName;
        private static string ConfigFileName = HGUIDLower + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        public static string ConnectionError = "";
        public static bool IsUpToDate;
        public static bool ValidServer = false;
        public static bool Admin = false;
        public static bool Raidable = false;
        public static bool fInit = false;
        public static int EffectTick = 0;
        public static GameObject Thorward;
        public static GameObject LightningVFX;


        //harmony
        private static Harmony harmony;

        private void Awake()
        {
            BuildPiece buildPiece = new("funward", "funward");
            buildPiece.Name.English("Fun Ward");
            buildPiece.Description.English("Ward For testing the Piece Manager");
            buildPiece.RequiredItems.Add("Iron", 20, 2, false);
            buildPiece.RequiredItems.Add("SwordIronFire", 20, 2, false); // currently can't accept more than one config option. Not binding things correctly. Fix later TODO

            harmony = new Harmony(HarmonyGUID);

            harmony.PatchAll();
        }
    }
}
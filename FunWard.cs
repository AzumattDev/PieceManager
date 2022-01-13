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

namespace FunWard
{
    [BepInPlugin(HGUIDLower, ModName, version)]
    public partial class MOCKPMPlugin : BaseUnityPlugin
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


        //harmony
        private static Harmony harmony;

        private void Awake()
        {
            BuildPiece buildPiece = new("funward", "funward");
            buildPiece.Name.English("Fun Ward");
            buildPiece.Description.English("Ward For testing the Piece Manager");
            buildPiece.RequiredItems.Add("Iron", 20, false);
            buildPiece.RequiredItems.Add("SwordIronFire", 20, false); // currently can't accept more than one config option. Not binding things correctly. Fix later TODO

            harmony = new Harmony(HarmonyGUID);

            harmony.PatchAll();
        }
    }
}
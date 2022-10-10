using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PieceManager;
using UnityEngine;
using UnityEngine.Rendering;

namespace RKPLUGIN
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class RKPLUGINPlugin : BaseUnityPlugin

    {
        internal const string ModName = "RKPLUGIN";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "RK";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        
        private readonly Harmony _harmony = new(ModGUID);
        
        public static readonly ManualLogSource RKPLUGINLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private void Awake()
        {
         
            BuildPiece test = new (PiecePrefabManager.RegisterAssetBundle(""), "");
            test.Category.Add("BJORKNAS","Dungeons");
            GameObject cloudGameObject = new GameObject("Cloud");
            MaterialReplacer.RegisterGameObjectForShaderSwap(cloudGameObject, MaterialReplacer.ShaderType.VegetationShader);
            
            PieceTable pieceTable = new PieceTable("RK_PieceTable");
            pieceTable.Categories = {"Structure", "Dungeons"} ;
            
            
            
            
            _harmony.PatchAll();
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
        }
        
        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                RKPLUGINLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                RKPLUGINLogger.LogError($"There was an issue loading your {ConfigFileName}");
                RKPLUGINLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<bool>? _serverConfigLocked;

        

        private class ConfigurationManagerAttributes
        {
            public bool? Browsable = false;
        }
        
        class AcceptableShortcuts : AcceptableValueBase  // Used for KeyboardShortcut Configs 
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", KeyboardShortcut.AllKeyCodes);
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace PieceManager
{
    [PublicAPI]
    public static class MaterialReplacer
    {
        static MaterialReplacer()
        {
            originalMaterials = new Dictionary<string, Material>();
            ObjectToSwap = new Dictionary<GameObject, bool>();
            Harmony harmony = new("org.bepinex.helpers.PieceManager");
            harmony.Patch(AccessTools.DeclaredMethod(typeof(ZoneSystem), nameof(ZoneSystem.Start)),
                postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(MaterialReplacer),
                    nameof(GetAllMaterials))));
            harmony.Patch(AccessTools.DeclaredMethod(typeof(ZoneSystem), nameof(ZoneSystem.Start)),
                postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(MaterialReplacer),
                    nameof(ReplaceAllMaterialsWithOriginal))));
        }

        private static Dictionary<GameObject, bool> ObjectToSwap;
        internal static Dictionary<string, Material> originalMaterials;

        public static void RegisterGameObjectForMatSwap(GameObject go, bool isJotunnMock = false)
        {
            ObjectToSwap.Add(go, isJotunnMock);
        }
        
        [HarmonyPriority(Priority.VeryHigh)]
        private static void GetAllMaterials()
        {
            var allmats = Resources.FindObjectsOfTypeAll<Material>();
            foreach (var item in allmats)
            {
                originalMaterials[item.name] = item;
            }
        }
        
        [HarmonyPriority(Priority.VeryHigh)]
        private static void ReplaceAllMaterialsWithOriginal()
        {
            if(originalMaterials.Count <= 0) GetAllMaterials();
            foreach (var renderer in ObjectToSwap.SelectMany(gameObject => gameObject.Key.GetComponentsInChildren<Renderer>(true)))
            {
                ObjectToSwap.TryGetValue(renderer.gameObject, out bool jotunnPrefabFlag);
                foreach (var t in renderer.materials)
                {
                    if (jotunnPrefabFlag)
                    {
                        if (!t.name.StartsWith("JVLmock_")) continue;
                        var matName = renderer.material.name.Replace(" (Instance)", string.Empty).Replace("JVLmock_", "");

                        if (originalMaterials!.ContainsKey(matName))
                        {
                            renderer.material = originalMaterials[matName];
                        }
                        else
                        {
                            Debug.LogWarning("No suitable material found to replace: " + matName);
                            // Skip over this material in future
                            originalMaterials[matName] = renderer.material;
                        }
                    }
                    else
                    {
                        if (!t.name.StartsWith("_REPLACE_")) continue;
                        var matName = renderer.material.name.Replace(" (Instance)", string.Empty).Replace("_REPLACE_", "");

                        if (originalMaterials!.ContainsKey(matName))
                        {
                            renderer.material = originalMaterials[matName];
                        }
                        else
                        {
                            Debug.LogWarning("No suitable material found to replace: " + matName);
                            // Skip over this material in future
                            originalMaterials[matName] = renderer.material;
                        }   
                    }
                    
                }
            }
        }
    }
}

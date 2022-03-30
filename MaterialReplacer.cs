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
            _objectToSwap = new Dictionary<GameObject, bool>();
            _objectsForShaderReplace = new List<GameObject>();
            Harmony harmony = new("org.bepinex.helpers.PieceManager");
            harmony.Patch(AccessTools.DeclaredMethod(typeof(ZoneSystem), nameof(ZoneSystem.Start)),
                postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(MaterialReplacer),
                    nameof(GetAllMaterials))));
            harmony.Patch(AccessTools.DeclaredMethod(typeof(ZoneSystem), nameof(ZoneSystem.Start)),
                postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(MaterialReplacer),
                    nameof(ReplaceAllMaterialsWithOriginal))));
            
        }

        private static Dictionary<GameObject, bool> _objectToSwap;
        internal static Dictionary<string, Material> originalMaterials;
        private static List<GameObject> _objectsForShaderReplace;

        public static void RegisterGameObjectForShaderSwap(GameObject go)
        {
            _objectsForShaderReplace?.Add(go);
        }

        public static void RegisterGameObjectForMatSwap(GameObject go, bool isJotunnMock = false)
        {
            _objectToSwap.Add(go, isJotunnMock);
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
            foreach (var renderer in _objectToSwap.SelectMany(gameObject => gameObject.Key.GetComponentsInChildren<Renderer>(true)))
            {
                _objectToSwap.TryGetValue(renderer.gameObject, out bool jotunnPrefabFlag);
                foreach (var t in renderer.materials)
                {
                    if (jotunnPrefabFlag)
                    {
                        if (!t.name.StartsWith("JVLmock_")) continue;
                        var matName = renderer.material.name.Replace(" (Instance)", string.Empty).Replace("JVLmock_", "");

                        if (originalMaterials.ContainsKey(matName))
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

                        if (originalMaterials.ContainsKey(matName))
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
            Shader customPieceShader = ZNetScene.instance.GetPrefab("piece_chest").gameObject.GetComponentInChildren<Renderer>().sharedMaterial.shader;
            foreach (var renderer in _objectsForShaderReplace.SelectMany(gameObject => gameObject.GetComponentsInChildren<Renderer>(true)))
            {
                foreach (var t in renderer.sharedMaterials)
                {
                    t.shader = customPieceShader;
                }
            }
        }
    }
}

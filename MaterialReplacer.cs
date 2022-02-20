using System.Collections.Generic;
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
            Harmony harmony = new("org.bepinex.helpers.PieceManager");
            harmony.Patch(AccessTools.DeclaredMethod(typeof(ObjectDB), nameof(ObjectDB.Awake)),
                postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(MaterialReplacer),
                    nameof(GetAllMaterials))));
            harmony.Patch(AccessTools.DeclaredMethod(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB)),
                postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(MaterialReplacer),
                    nameof(ReplaceAllMaterialsWithOriginal))));
        }

        public static GameObject ObjectToSwap;
        internal static Dictionary<string, Material> originalMaterials;

        public static void RegisterGameObjectForMatSwap(GameObject go)
        {
            ObjectToSwap = go;
        }
        
        [HarmonyPriority(Priority.VeryHigh)]
        public static void GetAllMaterials()
        {
            var allmats = Resources.FindObjectsOfTypeAll<Material>();
            foreach (var item in allmats)
            {
                originalMaterials[item.name] = item;
            }
        }
        
        [HarmonyPriority(Priority.VeryHigh)]
        public static void ReplaceAllMaterialsWithOriginal()
        {
            if (originalMaterials == null) GetAllMaterials();

            foreach (Renderer renderer in ObjectToSwap.GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    if (renderer.materials[i].name.StartsWith("_REPLACE_"))
                    {
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
        }
    }
}

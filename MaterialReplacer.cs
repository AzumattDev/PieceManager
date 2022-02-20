using System.Collections.Generic;
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
        }
        static Dictionary<string, Material> originalMaterials;
        public static void GetAllMaterials()
        {
            var allmats = Resources.FindObjectsOfTypeAll<Material>();
            foreach (var item in allmats)
            {
                originalMaterials[item.name] = item;
            }
        }

        public static void ReplaceAllMaterialsWithOriginal(GameObject go)
        {
            if (originalMaterials == null) GetAllMaterials();

            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>(true))
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

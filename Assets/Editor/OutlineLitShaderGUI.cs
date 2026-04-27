using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CubeMining.Editor
{
    public class OutlineLitShaderGUI : ShaderGUI
    {
        private enum SurfaceType
        {
            Opaque = 0,
            Transparent = 1
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            if (material == null)
            {
                base.OnGUI(materialEditor, properties);
                return;
            }

            MaterialProperty surfaceProp = FindProperty("_Surface", properties, false);
            if (surfaceProp != null)
            {
                EditorGUI.BeginChangeCheck();
                var newSurface = (SurfaceType)EditorGUILayout.EnumPopup("Surface Type", (SurfaceType)surfaceProp.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    surfaceProp.floatValue = (float)newSurface;
                }

                // Keep render state synchronized even after domain reload/reimport.
                ApplySurfaceType(material, (SurfaceType)surfaceProp.floatValue);
            }

            DrawRemainingProperties(materialEditor, properties);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            var surfaceType = material.GetFloat("_Surface") > 0.5f ? SurfaceType.Transparent : SurfaceType.Opaque;
            ApplySurfaceType(material, surfaceType);
        }

        private static void ApplySurfaceType(Material material, SurfaceType surfaceType)
        {
            bool transparent = surfaceType == SurfaceType.Transparent;

            material.SetOverrideTag("RenderType", transparent ? "Transparent" : "Opaque");
            material.SetOverrideTag("Queue", transparent ? "Transparent" : "Geometry");
            material.renderQueue = transparent ? (int)RenderQueue.Transparent : (int)RenderQueue.Geometry;

            material.SetFloat("_SrcBlend", transparent ? (float)BlendMode.SrcAlpha : (float)BlendMode.One);
            material.SetFloat("_DstBlend", transparent ? (float)BlendMode.OneMinusSrcAlpha : (float)BlendMode.Zero);
            material.SetFloat("_ZWrite", transparent ? 0f : 1f);
        }

        private static void DrawRemainingProperties(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                MaterialProperty property = properties[i];
                if (property == null || property.name == "_Surface")
                {
                    continue;
                }

                materialEditor.ShaderProperty(property, property.displayName);
            }
        }
    }
}

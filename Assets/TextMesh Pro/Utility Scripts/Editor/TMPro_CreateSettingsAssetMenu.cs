using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;




namespace TMPro.EditorUtilities
{

    public static class TMPro_CreateSettingsAssetMenu
    {

        [MenuItem("Assets/Create/TextMeshPro - Settings", false, 130)]
        public static void CreateTextMeshProObjectPerform()
        {
            // Get the path to the selected texture.
            string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string filePathWithName = AssetDatabase.GenerateUniqueAssetPath(filePath + "/TMP Settings.asset");

            // Create new Sprite Asset using this texture
            TMP_Settings settings = ScriptableObject.CreateInstance<TMP_Settings>();

            AssetDatabase.CreateAsset(settings, filePathWithName);

            EditorUtility.SetDirty(settings);

            AssetDatabase.SaveAssets();
        }
    }

}
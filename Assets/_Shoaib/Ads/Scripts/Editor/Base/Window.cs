#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor.Base
{
    public class Window : EditorWindow
    {
        static Texture2D infoImage;
        protected virtual void OnGUI()
        {
            if(infoImage==null)
                infoImage = EditorGUIUtility.FindTexture("console.infoicon");
            
            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label(infoImage, new GUIStyle() { fixedWidth = 40, fixedHeight = 60 ,alignment= TextAnchor.MiddleCenter});
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" Developed by Shoaib Riaz", EditorStyles.boldLabel);
            if (GUILayout.Button("Check Update", new GUIStyle(EditorStyles.linkLabel) { alignment = TextAnchor.UpperRight }))
            {
                Application.OpenURL("https://github.com/ShoaibRiaz007/Ads-Package");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(" Email : riazshoaib17@gmail.com");
            EditorGUILayout.LabelField(" It is free to use. For any bug or improvements contact me");
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
#if UNITY_EDITOR
using SH.Ads.Editor.Adons;
using SH.Ads.Editor.Base;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor
{
    public class AdOns : IWindow
    {
        static Vector2 scrollPos = Vector2.zero;

        public override string Name => "Adons";
        public override string ToolTip => "Add Extra Feature for extended functionalities";
        public Adon[] allAdons = new Adon[]
        {
            new FirebaseAnalytics(),
            new RemoteConfig(),
            new GoogleReview(),
        };
        public override void OnEnable(AdSettings settings)
        {
            EditorUtility.SetDirty(settings);
            for (int i = 0; i < allAdons.Length; i++)
            {
                allAdons[i].CheckIfInstalled();
            }
        }
        public override void OnGUI()
        {
            Header();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUIStyle(EditorStyles.helpBox));
            for(int i = 0; i < allAdons.Length; i++)
            {
                allAdons[i].OnGUI();
            }

            EditorGUILayout.EndScrollView();
        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to show all adons that can be installed", MessageType.Info);
            EditorGUILayout.Space();
        }
    }
}
#endif
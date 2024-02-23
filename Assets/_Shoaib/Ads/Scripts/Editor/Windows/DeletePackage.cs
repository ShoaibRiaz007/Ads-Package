#if UNITY_EDITOR
using SH.Ads.Editor.Adons;
using SH.Ads.Editor.Base;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor
{
    public class DeletePackage : IWindow
    {
        static Vector2 scrollPos = Vector2.zero;

        public override string Name => "Delete Packages";
        public override string ToolTip => "Delete Installed Packages. Only Installed by this window";

        InstalledPackages InstalledPackages;

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
            InstalledPackages = InstalledPackages.Load();
        }
        public override void OnGUI()
        {
            Header();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUIStyle(EditorStyles.helpBox));
            for(int i = 0; i < InstalledPackages.Installed.Count; i++)
            {
                InstalledPackages.Installed[i].OnGUI();
            }

            EditorGUILayout.EndScrollView();
        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to delete installed packages", MessageType.Info);
            EditorGUILayout.Space();
        }
    }
}
#endif
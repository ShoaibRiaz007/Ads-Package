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
            new RemoteConfig()
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
                ShowOption(allAdons[i]);
            }

            EditorGUILayout.EndScrollView();
        }

        void ShowOption(Adon adon)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(adon.Name, EditorStyles.largeLabel);
            if (GUILayout.Button(adon.SymbolPresent ? new GUIContent("Deactivate", $"Deactivate adon {adon.Name}") : adon.IsInstalled ? new GUIContent("Activate", $"Activate adon {adon.Name}") : new GUIContent("Install", $"Install {adon.Name} package"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                if (adon.IsInstalled && adon.SymbolPresent)
                {
                    adon.RemoveSymbol();
                } if (adon.IsInstalled && !adon.SymbolPresent)
                {
                    adon.AddSymbol();
                }
                else
                    AdvertiserEditorWindow.ShowPanel<InstallPackage>();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField(adon.Description, EditorStyles.textArea);
            GUILayout.EndVertical();

            GUILayout.Space(20);
        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to show all adons that can be installed", MessageType.Info);
            EditorGUILayout.Space();
        }
    }
}
#endif
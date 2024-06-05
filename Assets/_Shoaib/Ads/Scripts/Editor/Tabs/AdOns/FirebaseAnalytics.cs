#if UNITY_EDITOR
using SH.Ads.Adons;
using System;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor.Adons
{
    public sealed class FirebaseAnalytics : Adon
    {
        const string TYPE_NAME = "SH.Ads.Adons.FirebaseAnalytics, Assembly-CSharp";


        protected override string Symbol => "FirebaseAnalytics";
        protected override string Description => "Firebase Analytics allow us to moniter how user engage with the application. By default it will collect all the advertisers data after enabled";

        public override string Name => "Firebase Analytics";
        public override string PackageName => "FirebaseAnalytics";

        public FirebaseAnalytics()// Default constructor
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        public override void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Name, EditorStyles.largeLabel);
            if (GUILayout.Button(SymbolPresent ? new GUIContent("Deactivate", $"Deactivate adon {Name}") : IsInstalled ? new GUIContent("Activate", $"Activate adon {Name}") : new GUIContent("Install", $"Install {Name} package"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                if (IsInstalled && SymbolPresent)
                {
                    RemoveSymbol();
                }
                if (IsInstalled && !SymbolPresent)
                {
                    AddSymbol();
                }
                else
                    AdvertiserEditorWindow.ShowPanel<InstallPackage>();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField(Description, EditorStyles.textArea);
            GUILayout.EndVertical();
            GUILayout.Space(20);
        }
        public override void CheckIfInstalled()
        {
            try
            {
                Type firebaseAnalyticsType = Type.GetType("Firebase.Analytics.FirebaseAnalytics, Firebase.Analytics");
                if (firebaseAnalyticsType != null)
                {
                    IsInstalled = true;
                }
                else
                {
                    IsInstalled = false;
                }
            }
            catch
            {
                IsInstalled = false;
            }

            SymbolPresent = CheckIfSymboIsPresent(Symbol);
        }
        public override void RemoveSymbol()
        {
            var settings = AdSettings.Load();
            for (int i = 0;i<settings.m_AdOns.Count;i++)
            {
                if (settings.m_AdOns[i].GetType() == Type.GetType(TYPE_NAME))
                {
                    settings.m_AdOns.RemoveAt(i);
                    EditorUtility.SetDirty(settings);
                    break;
                }
            }

            Extensions.RemoveSymbol(Symbol);
        }
        public override void AddSymbol()
        {
            var settings = AdSettings.Load();
            for (int i = 0; i < settings.m_AdOns.Count; i++)
            {
                if (settings.m_AdOns[i].GetType() == Type.GetType(TYPE_NAME))
                {
                    EditorUtility.DisplayDialog("Adon Already Exist", $"The Adon '{Name}' you are trying to add already exists.", "OK");
                    return;
                }
            }
            Extensions.AddSymbol(Symbol);
        }

        void OnAfterAssemblyReload()
        {
#if FirebaseAnalytics
            Type type = Type.GetType(TYPE_NAME, false, true);

            if (type != null)
            {
                var setting = AdSettings.Load();
                setting.m_AdOns.RemoveAll(a => a == null);
                for (int i = 0; i < setting.m_AdOns.Count; i++)
                {

                    if (setting.m_AdOns[i].GetType() == Type.GetType(TYPE_NAME))
                    {
                        return;
                    }
                }
                setting.m_AdOns.Add(Ads.Adons.FirebaseAnalytics.Load());
                EditorUtility.SetDirty(setting);
                EditorUtility.DisplayDialog("Adon Is Active", $"The Adon '{Name}' is now active.", "OK");
                AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            }
            else
            {
                Debug.LogError($"No type found. Finding type {TYPE_NAME}");
            }
#endif
        }


    }
}
#endif
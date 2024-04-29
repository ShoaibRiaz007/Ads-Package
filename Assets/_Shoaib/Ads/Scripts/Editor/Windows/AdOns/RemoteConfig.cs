#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor.Adons
{
    public sealed class RemoteConfig : Adon
    {
        const string TYPE_NAME = "SH.Ads.Adons.RemoteConfig, Assembly-CSharp";


        public override string Name => "Firebase Remote Config";

        protected override string Description => "Firebase Remote Config allows you to update ad IDs and advertiser details dynamically without having to release a new build and submit it to the app store. It provides a convenient way to manage and tweak configurations on the fly, making it easier to experiment, optimize, and adapt your app's behavior without the need for frequent updates through app stores.";

        protected override string Symbol => "RemoteConfig";

        public override string PackageName => "FirebaseRemoteConfig";

#if RemoteConfig
        Ads.Adons.RemoteConfig m_RemoteConfig;
#endif
        bool m_Expand= false;

        public RemoteConfig()// Default constructor
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }
        public override void OnGUI()
        {
#if RemoteConfig
            if (m_RemoteConfig == null)
                m_RemoteConfig = Ads.Adons.RemoteConfig.Load();
#endif
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
#if RemoteConfig
            if (IsInstalled)
            {
                m_Expand = EditorGUILayout.Foldout(m_Expand, "Options");
                if (m_Expand)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Key",EditorStyles.boldLabel); EditorGUILayout.LabelField("Value", EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    m_RemoteConfig.m_AdSettingKey = EditorGUILayout.TextField(m_RemoteConfig.m_AdSettingKey); ; EditorGUILayout.LabelField("Ads Setting", EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    m_RemoteConfig.m_DataKey = EditorGUILayout.TextField(m_RemoteConfig.m_DataKey); m_RemoteConfig.m_RemoteData = EditorGUILayout.ObjectField(m_RemoteConfig.m_RemoteData, typeof(RemoteData), false) as RemoteData;
                    GUILayout.EndHorizontal();
                }
            }
#endif
            GUILayout.EndVertical();
            GUILayout.Space(20);


        }
        public override void CheckIfInstalled()
        {
            try
            {
                Type remoteConfigType = Type.GetType("Firebase.RemoteConfig.FirebaseRemoteConfig, Firebase.RemoteConfig");

                if (remoteConfigType != null)
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
#if RemoteConfig
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
                setting.m_AdOns.Add(m_RemoteConfig);
                EditorUtility.SetDirty(setting);
                EditorUtility.SetDirty(m_RemoteConfig);
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
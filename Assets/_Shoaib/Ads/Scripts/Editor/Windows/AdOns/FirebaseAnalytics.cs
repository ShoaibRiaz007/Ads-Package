#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor.Adons
{
    public sealed class FirebaseAnalytics : Adon
    {
        const string typeName = "SH.Ads.Adons.FirebaseAnalytics, Assembly-CSharp";

        static bool Installed = false,symbol=false;
        public override bool IsInstalled=> Installed;

        public override string Name => "Firebase Analytics";

        public override string Description => "Firebase Analytics allow us to moniter how user engage with the application. By default it will collect all the advertisers data after enabled";

        public override bool SymbolPresent => symbol;

        public override string Symbol => "FirebaseAnalytics";

        public override string PackageName => "FirebaseAnalytics";

        public FirebaseAnalytics()// Default constructor
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        public override void CheckIfInstalled()
        {
            try
            {
                Type firebaseAnalyticsType = Type.GetType("Firebase.Analytics.FirebaseAnalytics, Firebase.Analytics");


                if (firebaseAnalyticsType != null)
                {
                    Installed = true;
                }
                else
                {
                    Installed = false;
                }
            }
            catch
            {
                Installed = false;
            }

            symbol = CheckIfSymboIsPresent(Symbol);
        }
        public override void RemoveSymbol()
        {
            var settings = AdSettings.Load();
            for (int i = 0;i<settings.m_AdOns.Count;i++)
            {
                if (settings.m_AdOns[i].GetType() == Type.GetType(typeName))
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
                if (settings.m_AdOns[i].GetType() == Type.GetType(typeName))
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
            Type type = Type.GetType(typeName, false, true);

            if (type != null)
            {
                var setting = AdSettings.Load();
                for (int i = 0; i < setting.m_AdOns.Count; i++)
                {
                    if (setting.m_AdOns[i].GetType() == Type.GetType(typeName))
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
                Debug.LogError($"No type found. Finding type {typeName}");
            }
#endif
        }
    }
}
#endif
#if UNITY_EDITOR
using DG.Tweening.Core.Easing;
using SH.Ads.Base;
using System;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor.Adons
{
    public sealed class RemoteConfig : Adon
    {
        const string typeName = "SH.Ads.Adons.RemoteConfig, Assembly-CSharp";

        static bool Installed = false,symbol=false;
        public override bool IsInstalled=> Installed;

        public override string Name => "Firebase Remote Config";

        public override string Description => "Firebase Remote Config allows you to update ad IDs and advertiser details dynamically without having to release a new build and submit it to the app store. It provides a convenient way to manage and tweak configurations on the fly, making it easier to experiment, optimize, and adapt your app's behavior without the need for frequent updates through app stores.";

        public override bool SymbolPresent => symbol;

        protected override string Symbol => "RemoteConfig";

        public RemoteConfig()// Default constructor
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        public override void CheckIfInstalled()
        {
            try
            {
                Type remoteConfigType = Type.GetType("Firebase.RemoteConfig.FirebaseRemoteConfig, Firebase.RemoteConfig");

                if (remoteConfigType != null)
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
#if RemoteConfig
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
                setting.m_AdOns.Add(Ads.Adons.RemoteConfig.Load());
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
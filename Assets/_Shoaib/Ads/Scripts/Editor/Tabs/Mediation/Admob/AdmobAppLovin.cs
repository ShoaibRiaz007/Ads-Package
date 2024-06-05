#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor.Mediation
{
    public sealed class AdmobAppLovin : Base.BaseType
    {
        protected override string Symbol => "MediationAdmobAppLovin";
        protected override string Description => "This integration allows advertisers to control and optimize their AppLovin Ads campaigns alongside other mediation partners directly through the AdMob interface";

        public override string Name => "Admob and Applovin";
        public override string PackageName => "GoogleMobileAdsAppLovinMediation";


        public override void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
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
            GUILayout.Space(20);
        }
        public override void CheckIfInstalled()
        {
            try
            {
                string typeName = "GoogleMobileAds.Mediation.AppLovin.Api.AppLovin, Assembly-CSharp";
                Type applovinAdsType = Type.GetType(typeName);
                if (applovinAdsType != null)
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
            ParentName = "Admob";
            IsParentEnabled = CheckIfSymboIsPresent("Admob");
        }
        public override void RemoveSymbol()
        {
            Extensions.RemoveSymbol(Symbol);
        }
        public override void AddSymbol()
        {
            Extensions.AddSymbol(Symbol);
        }
    }
}
#endif
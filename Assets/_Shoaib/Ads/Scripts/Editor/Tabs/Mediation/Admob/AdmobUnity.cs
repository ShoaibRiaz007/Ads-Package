#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor.Mediation
{
    public sealed class AdmobUnity : Base.BaseType
    {
        protected override string Symbol => "MediationAdmobUnity";
        protected override string Description => "This integration allows advertisers to control and optimize their Unity Ads campaigns alongside other mediation partners directly through the AdMob interface";

        public override string Name => "Admob and Unity";
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
                string typeName = "GoogleMobileAds.Mediation.UnityAds.Android.UnityAdsClient, Assembly-CSharp";
                Type unityAdsClientType = Type.GetType(typeName);
                if (unityAdsClientType != null)
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
            ParentName = "Admob";
            SymbolPresent = CheckIfSymboIsPresent(Symbol);
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
#if UNITY_EDITOR
using SH.Ads.Editor.Mediation.Base;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor.Mediation
{
    public class AdmobMediation : Base.MediationUnit
    {
        static Vector2 scrollPos = Vector2.zero;

        internal override string Name => "Admob Medtiation";

        internal override BaseType[] MediationType => new BaseType[]
        {
            new AdmobAppLovin(),
            new AdmobUnity(),
        };
        internal override void OnEnable(AdSettings settings)
        {
            EditorUtility.SetDirty(settings);
            for (int i = 0; i < MediationType.Length; i++)
            {
                MediationType[i].CheckIfInstalled();
            }
        }
        internal override void OnGUI()
        {
            Header();
            
            for (int i = 0; i < MediationType.Length; i++)
            {
                if (!MediationType[i].IsParentEnabled)
                {
                    EditorGUILayout.HelpBox("AdMob mediation requires AdMob to be enabled. Please enable AdMob first before enabling mediation.", MessageType.Error);
                    return;
                }
                MediationType[i].OnGUI();
            }
        }

        void Header()
        {
            EditorGUILayout.LabelField(Name + " : ", new GUIStyle(EditorStyles.boldLabel) { fontSize = 30, fixedHeight = 30 });
            EditorGUILayout.Space(30);
        }
    }
}
#endif